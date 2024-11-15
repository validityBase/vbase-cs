using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using System;

using Nethereum.ABI.EIP712;
using Nethereum.Contracts;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Hex.HexConvertors.Extensions;

public enum RequestType
{
    GET,
    POST
}

public class Eip712Signer
{
    private readonly string privateKey;
    private readonly System.Numerics.BigInteger chainId;
    private readonly string verifyingContract;

    public Eip712Signer(string privateKey, System.Numerics.BigInteger chainId, string verifyingContract)
    {
        this.privateKey = privateKey;
        this.chainId = chainId;
        this.verifyingContract = verifyingContract;
    }

    public string SignForwardRequest(string fromAddress, int nonce, string functionData)
    {
        // Define the Typed Data structure for the ForwardRequest.
        TypedData<Domain> typedData = new TypedData<Domain>
        {
            Domain = new Domain
            {
                // TODO: Initialize this object using the returned domain object.
                // We had signatureData and domain returned from the forwarder
                // and should initialize using the returned domain.
                Name = "CommitmentService",
                Version = "0.0.1",
                ChainId = chainId,
                VerifyingContract = verifyingContract
            },
            Types = new Dictionary<string, MemberDescription[]>
            {
                ["EIP712Domain"] = new[]
                {
                    new MemberDescription { Name = "name", Type = "string" },
                    new MemberDescription { Name = "version", Type = "string" },
                    new MemberDescription { Name = "chainId", Type = "uint256" },
                    new MemberDescription { Name = "verifyingContract", Type = "address" }
                },
                ["ForwardRequest"] = new[]
                {
                    new MemberDescription { Name = "from", Type = "address" },
                    new MemberDescription { Name = "nonce", Type = "uint256" },
                    new MemberDescription { Name = "data", Type = "bytes" }
                }
            },
            PrimaryType = "ForwardRequest",
            Message = new[]
            {
                new MemberValue { TypeName = "address", Value = fromAddress },
                new MemberValue { TypeName = "uint256", Value = nonce },
                new MemberValue { TypeName = "bytes", Value = functionData }
            }
        };

        // Encode the typed data.
        var signer = new Eip712TypedDataSigner();
        byte[] encodedData = signer.EncodeTypedData(typedData);
        byte[] hashedData = Sha3Keccack.Current.CalculateHash(encodedData);

        // Sign the hash with the private key.
        var key = new EthECKey(privateKey);
        EthECDSASignature signature = key.SignAndCalculateV(hashedData);
        string signatureStr = EthECDSASignature.CreateStringSignature(signature);

        return signatureStr;
    }
}

namespace VBase
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ForwarderCommitmentServiceFactory
    {
        // COM visible methods can't be static.
        public ForwarderCommitmentService Create(string forwarderUrl, string apiKey, string privateKey)
        {
            return new ForwarderCommitmentService(forwarderUrl, apiKey, privateKey);
        }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ForwarderCommitmentService
    {
        private readonly string forwarderUrl;
        private readonly string apiKey;
        private readonly string privateKey;
        private readonly string abi;
        private readonly Nethereum.Web3.Accounts.Account account;
        private readonly Web3 web3;
        private readonly Contract commitmentServiceContract;
        private Dictionary<string, object> signatureData;
        private Dictionary<string, object> domain;
        private int? nonce;

        public ForwarderCommitmentService(
            string forwarderUrl,
            string apiKey,
            string privateKey)
        {
            this.forwarderUrl = forwarderUrl;
            this.apiKey = apiKey;
            this.privateKey = privateKey;

            // Read the ABI from the JSON resource.
            abi = JsonLoader.LoadCommitmentServiceJson();

            // Initialize Web3 and get the contract.
            account = new Nethereum.Web3.Accounts.Account(privateKey);
            web3 = new Web3(account);
            // We can init contract with a bogus address since the forwarder
            // never submits transactions to the blockchain directly.
            // We only use the contract for signing messages.
            commitmentServiceContract = web3.Eth.GetContract(abi, "0x1234");
        }

        public static string DictToQueryString(Dictionary<string, string> dict)
        {
            if (dict == null || dict.Count == 0)
                return string.Empty;

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var kvp in dict)
            {
                queryString[kvp.Key] = kvp.Value;
            }

            return queryString.ToString();
        }

        private async Task<Dictionary<string, object>> CallForwarderApiAsync(
            string api,
            RequestType requestType = RequestType.GET,
            Dictionary<string, string> parameters = null,
            Dictionary<string, object> data = null)
        {
            parameters ??= new Dictionary<string, string>();
            data ??= new Dictionary<string, object>();

            // Set the user address.
            parameters["from"] = account.Address;

            // Build the HTTP client request. 
            using var client = new HttpClient();
            // Add the API Key header.
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            // Add the parameters query string.
            var queryString = DictToQueryString(parameters);
            var url = $"{forwarderUrl}{api}";
            url = string.IsNullOrEmpty(queryString) ? url : $"{url}?{queryString}";

            HttpResponseMessage response;
            // TODO: Add proper debug logging
            Console.WriteLine($"ForwarderCommitmentService:CallForwarderApiAsync(): requestType = {requestType}, URL = {url}");
            if (requestType == RequestType.GET)
            {
                response = await client.GetAsync(url);
            }
            else if (requestType == RequestType.POST)
            {
                var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
                response = await client.PostAsync(url, content);
            }
            else
            {
                throw new ArgumentException("Invalid request type");
            }

            // Verify the API call was successful.
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseDict = JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson);
            if (responseDict.TryGetValue("success", out var successElement) && successElement is JsonElement successJsonElement && successJsonElement.ToString().ToLower() == "true")
            {
                // TODO: Add proper debug logging
                Console.WriteLine($"ForwarderCommitmentService:CallForwarderApiAsync(): successJsonElement = {successJsonElement}");
            }
            else
            {
                throw new InvalidOperationException("API call failed");
            }

            // Retrieve the data from the API response.
            if (responseDict.TryGetValue("data", out var dataElement) && dataElement is JsonElement dataJsonElement && dataJsonElement.ValueKind == JsonValueKind.Object)
            {
                // TODO: Add proper debug logging
                Console.WriteLine($"ForwarderCommitmentService:CallForwarderApiAsync(): dataJsonElement = {dataJsonElement}");
            }
            else
            {
                throw new InvalidOperationException($"API returned invalid data = {data}");
            }

            var dataDictionary = new Dictionary<string, object>();
            foreach (var property in dataJsonElement.EnumerateObject())
            {
                // Convert JsonElement values to appropriate .NET types
                dataDictionary[property.Name] = property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString(),
                    JsonValueKind.Number => property.Value.GetInt32(),
                    JsonValueKind.Object => JsonSerializer.Deserialize<Dictionary<string, object>>(property.Value.GetRawText()),
                    JsonValueKind.Array => JsonSerializer.Deserialize<List<object>>(property.Value.GetRawText()),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => property.Value.ToString()
                };
            }

            // TODO: Add proper debug logging
            Console.WriteLine($"ForwarderCommitmentService:CallForwarderApiAsync(): dataDictionary = {JsonSerializer.Serialize(dataDictionary)}");

            return dataDictionary;
        }

        private async Task<string> CallFunctionAsync(string fnName, object[] args)
        {
            // Get signature data and nonce from the API endpoint if necessary.
            if (signatureData == null || nonce == null)
            {
                // Get current signatureData from the API server.
                signatureData = await CallForwarderApiAsync("signature-data");
                if (signatureData == null || !signatureData.ContainsKey("domain"))
                {
                    throw new InvalidOperationException("Unexpected signature_data or missing domain field.");
                }

                // Initialize nonce.
                if (!signatureData.ContainsKey("nonce"))
                {
                    throw new InvalidOperationException("Missing nonce field in signatureData.");
                }
                nonce = Convert.ToInt32(signatureData["nonce"]);

                // Initialize domain.
                domain = (Dictionary<string, object>)signatureData["domain"];
                if (domain == null)
                {
                    throw new InvalidOperationException("Missing domain in signature_data.");
                }
            }

            // TODO: Add proper debug logging
            Console.WriteLine($"ForwarderCommitmentService:CallFunctionAsync(): signatureData = {JsonSerializer.Serialize(signatureData)}");

            // Set up the call.

            // Encode the CommitmentService smart contract call.
            // The following is a Netethereum equivalent to:
            // commitmentServiceContract.encode_abi(fn_name = fnName, args = args);
            var function = commitmentServiceContract.GetFunction(fnName);
            var functionData = function.GetData(args);
            // TODO: Add proper debug logging
            Console.WriteLine($"ForwarderCommitmentService:CallFunctionAsync(): functionData = {JsonSerializer.Serialize(functionData)}");

            // Sign the meta-transaction for the call.
            var eip712Signer = new Eip712Signer(privateKey, new System.Numerics.BigInteger(Convert.ToInt32(domain["chainId"].ToString())), domain["verifyingContract"].ToString());
            string fromAddress = account.Address;
            string signature = eip712Signer.SignForwardRequest(fromAddress, nonce.Value, functionData);
            Console.WriteLine($"Signature: {signature}");

            // Format the ForwardRequest object.
            var forwardRequest = new Dictionary<string, object>
            {
                { "from", fromAddress },
                { "nonce", nonce.Value },
                { "data", functionData }
            };
            var data = new Dictionary<string, object>
            {
                { "forwardRequest", forwardRequest },
                { "signature", signature }
            };

            // TODO: Add exception and error handling.

            // Make the forwarder request.
            var receipt = await CallForwarderApiAsync(
                "execute",
                RequestType.POST,
                null,
                data
            );
            // TODO: Add proper debug logging
            Console.WriteLine($"ForwarderCommitmentService:CallFunctionAsync(): receipt = {JsonSerializer.Serialize(receipt)}");

            // TODO: Check request status.

            // TODO: Process and return the tx receipt.

            // If the tx succeeded, increment the nonce.
            nonce++;

            // TODO: Return the transaction receipt.
            return receipt["transactionHash"].ToString();
        }

        // Synchronous wrapper for VBA compatibility
        public string CallFunction(string functionName, object[] functionInput)
        {
            return CallFunctionAsync(functionName, functionInput).GetAwaiter().GetResult();
        }
    }
}

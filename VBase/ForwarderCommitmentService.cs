using System.Runtime.InteropServices;
using Nethereum.Contracts;
using Nethereum.Web3;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using System;

public enum RequestType
{
    GET,
    POST
}

namespace VBase
{

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
        private object signatureData;
        private int? nonce;
        private int? chainId;

        public ForwarderCommitmentService(
            string forwarderUrl,
            string apiKey,
            string privateKey)
        {
            this.forwarderUrl = forwarderUrl;
            this.apiKey = apiKey;
            this.privateKey = privateKey;
            signatureData = null;
            nonce = null;
            chainId = null;

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
            Dictionary<string, string> data = null)
        {
            parameters ??= new Dictionary<string, string>();
            data ??= new Dictionary<string, string>();

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
            Console.WriteLine($"ForwarderCommitmentService:CallForwarderApiAsync(): dataDictionary = {dataDictionary}");

            return dataDictionary;
        }

        private async Task<string> CallFunctionAsync(string fnName, object[] args)
        {
            // Get signature data and nonce from the API endpoint if necessary.
            if (signatureData == null || nonce == null)
            {
                Dictionary<string, object> signatureData = await CallForwarderApiAsync("signature-data");
                if (signatureData == null || !signatureData.ContainsKey("domain"))
                {
                    throw new InvalidOperationException("Unexpected signature_data or missing domain field.");
                }
                Dictionary<string, object> domain = (Dictionary<string, object>)signatureData["domain"];
                if (domain == null || !domain.ContainsKey("chainId"))
                {
                    throw new InvalidOperationException("Missing chainId field in signature_data[\"domain\"]");
                }
                nonce = Convert.ToInt32(signatureData["nonce"]);

                // Convert chainId to integer to make it compatible with consumer APIs.
                // Technically it is an uint256 and is sent as a string.
                chainId = Convert.ToInt32(signatureData["chainId"].ToString());
            }

            // TODO: Add proper debug logging
            Console.WriteLine($"ForwarderCommitmentService:CallFunctionAsync(): signatureData = {signatureData}");

            // Set up the call.

            // Encode the CommitmentService smart contract call.
            var functionData = commitmentServiceContract.encode_abi(fn_name = fnName, args = args);

            // Return dummy transaction hash for testing.
            return "0x1234";
        }

        // Synchronous wrapper for VBA compatibility
        public string CallFunction(string functionName, object[] functionInput)
        {
            return CallFunctionAsync(functionName, functionInput).GetAwaiter().GetResult();
        }
    }
}

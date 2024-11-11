using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;

namespace VBase
{
    public class JsonLoader
    {
        public static string LoadCommitmentServiceJson()
        {
            // Get the current assembly.
            var assembly = Assembly.GetExecutingAssembly();

            // Define the fully qualified resource name.
            string resourceName = "VBase.abi.CommitmentService.json";

            // Open the resource stream and read its contents into a string
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class CallCommitmentService
    {
        private async Task<string> CallFunctionAsync(string rpcUrl, string contractAddress, string functionName, object[] functionInput, string privateKey)
        {
            // Read the ABI from the JSON resource.
            string abi = JsonLoader.LoadCommitmentServiceJson();

            // Initialize Web3 and get the contract and function.
            var web3 = new Web3(new Nethereum.Web3.Accounts.Account(privateKey), rpcUrl);
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(functionName);

            // Estimate gas and send transaction.
            var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
            var gasEstimate = await function.EstimateGasAsync(functionInput);

            var transactionReceipt = await function.SendTransactionAndWaitForReceiptAsync(
                from: web3.TransactionManager.Account.Address,
                gas: gasEstimate,
                value: new HexBigInteger(0),
                functionInput: functionInput);

            return transactionReceipt.TransactionHash;
        }

        // Synchronous wrapper for VBA compatibility
        public string CallFunction(string rpcUrl, string contractAddress, string functionName, object[] functionInput, string privateKey)
        {
            return CallFunctionAsync(rpcUrl, contractAddress, functionName, functionInput, privateKey).GetAwaiter().GetResult();
        }
    }
}

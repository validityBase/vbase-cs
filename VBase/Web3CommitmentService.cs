using System.Runtime.InteropServices;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using System.Threading.Tasks;
using Nethereum.Contracts;

namespace VBase
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Web3CommitmentServiceFactory
    {
        // COM visible methods can't be static.
        public Web3CommitmentService Create(string rpcUrl,
            string contractAddress,
            string privateKey)
        {
            return new Web3CommitmentService(rpcUrl, contractAddress, privateKey);
        }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Web3CommitmentService
    {
        private readonly string rpcUrl;
        private readonly string contractAddress;
        private readonly string privateKey;
        private readonly Web3 web3;
        private readonly Contract contract;

        public Web3CommitmentService(
            string rpcUrl,
            string contractAddress,
            string privateKey)
        {
            this.rpcUrl = rpcUrl;
            this.contractAddress = contractAddress;
            this.privateKey = privateKey;

            // Read the ABI from the JSON resource.
            string abi = JsonLoader.LoadCommitmentServiceJson();

            // Initialize Web3 and get the contract and function.
            this.web3 = new Web3(new Nethereum.Web3.Accounts.Account(privateKey), rpcUrl);
            this.contract = this.web3.Eth.GetContract(abi, contractAddress);
        }
        
        private async Task<string> CallFunctionAsync(string rpcUrl, string contractAddress, string functionName, object[] functionInput, string privateKey)
        {
            var function = this.contract.GetFunction(functionName);

            // Estimate gas and send transaction.
            var gasPrice = await this.web3.Eth.GasPrice.SendRequestAsync();
            var gasEstimate = await function.EstimateGasAsync(functionInput);

            var transactionReceipt = await function.SendTransactionAndWaitForReceiptAsync(
                from: this.web3.TransactionManager.Account.Address,
                gas: gasEstimate,
                value: new HexBigInteger(0),
                functionInput: functionInput);

            return transactionReceipt.TransactionHash;
        }

        // Synchronous wrapper for VBA compatibility
        public string CallFunction(string functionName, object[] functionInput)
        {
            return CallFunctionAsync(rpcUrl, contractAddress, functionName, functionInput, privateKey).GetAwaiter().GetResult();
        }
    }
}

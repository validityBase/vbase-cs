using System;
using System.Runtime.InteropServices;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;

namespace VBase
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class VBaseClient
    {
        public async Task<string> CallSmartContractFunction(string rpcUrl, string privateKey, string contractAddress, string abi, string functionName, object[] functionInput)
        {
            var web3 = new Web3(new Nethereum.Web3.Accounts.Account(privateKey), rpcUrl);
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(functionName);

            var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
            var gasEstimate = await function.EstimateGasAsync(functionInput);

            var transactionReceipt = await function.SendTransactionAndWaitForReceiptAsync(
                from: web3.TransactionManager.Account.Address,
                gas: gasEstimate,
                value: new HexBigInteger(0),
                functionInput: functionInput);

            return transactionReceipt.TransactionHash;
        }
    }
}

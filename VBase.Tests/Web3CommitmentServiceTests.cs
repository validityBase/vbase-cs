using System;

namespace VBase.Tests
{
    public class Web3CommitmentServiceTests
    {
        // Localhost Ethereum node URL.
        private const string LocalhostRpcUrl = "http://127.0.0.1:8545";
        // The deterministic address where the commitment service is deployed on a localhost test node.
        private const string ContractAddress = "0xe7f1725E7734CE288F8367e1Bb143E90bb3F0512";
        // Use a built-in test address that does not collide with common tests to avoid nonce conflicts:
        // Account #19: 0x8626f6940E2eb28930eFb4CeF49B2d1F2C9C1199
        // Private Key: 0xdf57089febbacf7ba0bc227dafbffa9fc08a93fdc68e1e42411a14efcf23656e
        private const string PrivateKey = "0xdf57089febbacf7ba0bc227dafbffa9fc08a93fdc68e1e42411a14efcf23656e";

        private void CallAddObjectFunction_ReturnsTransactionHash_Worker(Web3CommitmentService client)
        {
            // Test addObject().

            // Generate a random 32-byte objectCid.
            byte[] objectCid = new byte[32];
            new Random().NextBytes(objectCid);

            // Set up function call.
            string functionName = "addObject";
            object[] functionInput = { objectCid };

            string transactionHash = client.CallFunction(
                functionName,
                functionInput
            );

            Assert.False(string.IsNullOrEmpty(transactionHash), "Transaction hash should not be empty.");
            Console.WriteLine($"Transaction Hash: {transactionHash}");
        }

        [Fact]
        public void CallAddObjectFunction_ReturnsTransactionHash()
        {
            Web3CommitmentService client = new Web3CommitmentService(LocalhostRpcUrl, ContractAddress, PrivateKey);
            CallAddObjectFunction_ReturnsTransactionHash_Worker(client);
        }

        [Fact]
        public void CallAddObjectFunctionViaFactory_ReturnsTransactionHash()
        {
            var fact = new Web3CommitmentServiceFactory();
            Web3CommitmentService client = fact.Create(LocalhostRpcUrl, ContractAddress, PrivateKey);
            CallAddObjectFunction_ReturnsTransactionHash_Worker(client);
        }
    }
}

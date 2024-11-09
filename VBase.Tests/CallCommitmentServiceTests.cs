using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xunit;
using Nethereum.Hex.HexTypes;
using VBase;

namespace VBase.Tests
{
    public class CallCommitmentServiceTests
    {
        // Localhost Ethereum node URL.
        private const string LocalhostRpcUrl = "http://127.0.0.1:8545";
        // The deterministic address where the commitment service is deployed on a localhost test node.
        private const string PrivateKey = "0xdf57089febbacf7ba0bc227dafbffa9fc08a93fdc68e1e42411a14efcf23656e";
        // Use a built-in test address that does not collide with common tests to avoid nonce conflicts:
        // Account #19: 0x8626f6940E2eb28930eFb4CeF49B2d1F2C9C1199
        // Private Key: 0xdf57089febbacf7ba0bc227dafbffa9fc08a93fdc68e1e42411a14efcf23656e
        private const string ContractAddress = "0xe7f1725E7734CE288F8367e1Bb143E90bb3F0512";

        [Fact]
        public void CallAddObjectFunction_ReturnsTransactionHash()
        {
            // Test addObject().
            CallCommitmentService client = new CallCommitmentService();
            string functionName = "addObject";

            // Generate a random 32-byte objectCid.
            // byte[] randomBytes = new byte[32];
            // new Random().NextBytes(randomBytes);
            // string hexString = BitConverter.ToString(randomBytes);
            // objectCid = new HexBigInteger(hexString);
            string objectCid = "0x1";

            // Set up function input.
            object[] functionInput = { objectCid };

            string transactionHash = client.CallFunction(
                LocalhostRpcUrl,
                ContractAddress,
                functionName,
                functionInput,
                PrivateKey
            );

            Assert.False(string.IsNullOrEmpty(transactionHash), "Transaction hash should not be empty.");
            Console.WriteLine($"Transaction Hash: {transactionHash}");
        }
    }
}

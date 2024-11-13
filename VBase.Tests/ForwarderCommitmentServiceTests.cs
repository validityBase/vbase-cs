using DotNetEnv;
using System;

namespace VBase.Tests
{
    public class ForwarderCommitmentServiceTests
    {
        // Localhost forwarder settings.
        private const string VBaseCommitmentServiceClass = "ForwarderCommitmentService";
        private const string VBaseForwarderUrl = "http://localhost:3000/forwarder-test/";
        // Load the API Key from the environment variable.
        private readonly string VBaseApiKey;
        // Use a built-in test address that does not collide with common tests to avoid nonce conflicts:
        // Account #19: 0x8626f6940E2eb28930eFb4CeF49B2d1F2C9C1199
        // Private Key: 0xdf57089febbacf7ba0bc227dafbffa9fc08a93fdc68e1e42411a14efcf23656e
        private const string PrivateKey = "0xdf57089febbacf7ba0bc227dafbffa9fc08a93fdc68e1e42411a14efcf23656e";

        public ForwarderCommitmentServiceTests()
        {
            // Load environment variables from .env file.
            // The test is running in the "bin\Debug\net48", so reference the project folder..
            Env.Load("../../../.env");

            // Load the API key from the environment variable.
            VBaseApiKey = Environment.GetEnvironmentVariable("VBASE_API_KEY");

            if (string.IsNullOrEmpty(VBaseApiKey))
            {
                throw new InvalidOperationException("VBASE_API_KEY environment variable is not set.");
            }
        }

        [Fact]
        public void CallAddObjectFunction_ReturnsTransactionHash()
        {
            // Test addObject().
            ForwarderCommitmentService client = new ForwarderCommitmentService(VBaseForwarderUrl, VBaseApiKey, PrivateKey);

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
    }
}

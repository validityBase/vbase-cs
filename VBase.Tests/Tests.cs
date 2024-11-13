using DotNetEnv;
using System;

namespace VBase.Tests
{
    public class Tests
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting tests...");

            // Create a test runner.
            var testAssembly = typeof(Web3CommitmentServiceTests).Assembly;

            // Run tests in the Web3CommitmentServiceTests class.
            var web3Tests = new Web3CommitmentServiceTests();
            RunTest(() => web3Tests.CallAddObjectFunction_ReturnsTransactionHash());

            // Run tests in the ForwarderCommitmentServiceTests class.
            var forwarderTests = new ForwarderCommitmentServiceTests();
            RunTest(() => forwarderTests.CallAddObjectFunction_ReturnsTransactionHash());

            Console.WriteLine("All tests completed.");
        }

        private static void RunTest(Action testMethod)
        {
            try
            {
                testMethod.Invoke();
                Console.WriteLine("Test passed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
            }
        }
    }
}

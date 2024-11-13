using System.IO;
using System.Reflection;

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
}

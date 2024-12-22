# How to build the solution

1. Install the [Microsoft Visual Studio Installer Projects](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2022InstallerProjects) extension for Visual Studio 2022.  
1. Clone or check out the project.  
1. Build the solution using Visual Studio.  
1. Build the setup project using Visual Studio (MSBuild is not supported).

**Or alternatively, you can download the installer from the
[GitHub workflow run](https://github.com/validityBase/vbase-cs/actions) artifacts.**
    
# How to use the COM library

1. Uninstall the library if you have it installed already on your machine.
1. 1. Locate the output files in the setup project's output directory: `vBase.msi` and `setup.exe`.
   - The purpose of the EXE file is to check for prerequisites on the target machine (e.g., .NET Framework).
   - The MSI file is the actual installer.
1. Install the new version using `setup.exe`
1. Open Excel and create a new macro.
1. Add a reference to the library: **Tools** -> **References** -> find **vBase**.
1. If you are using an existing macro with a reference to a previous version of the library, you need to remove the reference and add it again to refresh the TLB.
1. Use the following VBA code to call the library:

```vbnet
Sub BuildAndVerifyDataset()

    
    Dim vBaseBuidler As New vBase.vBaseBuilder
    Dim vBaseClient As vBase.vBaseClient
    Dim vBaseDataset As vBase.vBaseDataset
    Dim verificationResult As vBase.verificationResult

    Dim datasetName As String
    Dim forwarderUrl As String
    Dim apiKey As String
    Dim privateKey As String

    datasetName = "<DATASET NAME>"
    forwarderUrl = "<FORWARDER URL>"
    apiKey = "<API KEY>"
    privateKey = "<PRIVATE KEY>"

    Set vBaseClient = vBaseBuidler.CreateForwarderClient(forwarderUrl, apiKey, privateKey)
    Set vBaseDataset = vBaseBuidler.CreateDataset(vBaseClient, datasetName, vBase.vBaseDatasetRecordTypes_String)

    vBaseDataset.AddRecord ("<Record 1 Data>")
    ' Add more records
    vBaseDataset.AddRecord ("<Record N Data>")

    Set verificationResult = vBaseDataset.VerifyCommitments()

    MsgBox MsgBox "Verification passed: " & verificationResult.VerificationPassed

End Sub
```

# How to Sign an MSI Using a Code Signing Certificate


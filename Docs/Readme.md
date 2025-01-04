# How to build the solution

1. Install the [Microsoft Visual Studio Installer Projects](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2022InstallerProjects) extension for Visual Studio 2022.
1. Clone or check out the project.
1. Build the solution using Visual Studio.
1. Build the setup project using Visual Studio (MSBuild is not supported).

**Or alternatively, you can download the installer from the
[GitHub workflow run](https://github.com/validityBase/vbase-cs/actions) artifacts.**

# Setup Development Environment

1. Install **Visual Studio 2022** or later.
2. Install the [Microsoft Visual Studio Installer Projects](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2022InstallerProjects) extension for Visual Studio 2022.
3. Create a `vBase.Core.Tests\settings.local.yml` file and configure the vBase Forwarder credentials to run the unit tests locally.
   ```yaml
   ApiKey: "FORWARDER_API_KEY"
   PrivateKey: "VBASE_PRIVATE_KEY"
   ```

# How to use the COM library

1. Uninstall the library if you have it installed already on your machine.
1. Locate the output files in the setup project's output directory: `vBase.msi` and `setup.exe`.
   - The purpose of the EXE file is to check for prerequisites on the target machine (e.g., .NET Framework).
   - The MSI file is the actual installer.
1. Install the new version using `setup.exe`
1. Open Excel and create a new macro.
1. Add a reference to the library: **Tools** -> **References** -> find **vBase**.
1. If you are using an existing macro with a reference to a previous version of the library, you need to remove the reference and add it again to refresh the TLB.
1. Use the following VBA code to call the library:

```vbnet
Sub BuildAndVerifyDataset()

    On Error GoTo ErrorHandler

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
    Set vBaseDataset = vBaseBuidler.CreateDataset(vBaseClient, datasetName, vBase.vBaseDatasetRecordTypes.vBaseDatasetRecordTypes_vBaseStringObject)

    vBaseDataset.AddRecord ("<Record 1 Data>")
    ' Add more records
    vBaseDataset.AddRecord ("<Record N Data>")

    Set verificationResult = vBaseDataset.VerifyCommitments()

    MsgBox "Verification passed: " & verificationResult.VerificationPassed

    Exit Sub

ErrorHandler:
    MsgBox "Use [Ctrl+Insert] to copy this message to the clipboard." & vbNewLine & "Error: " & Err.Description, vbCritical

End Sub
```

or

```vbnet
Sub VerifyDataset()

    On Error GoTo ErrorHandler

    Dim vBaseBuidler As New vBase.vBaseBuilder
    Dim vBaseClient As vBase.vBaseClient
    Dim vBaseDataset As vBase.vBaseDataset
    Dim verificationResult As vBase.verificationResult

    Dim forwarderUrl As String
    Dim apiKey As String
    Dim privateKey As String

    datasetName = "<DATASET NAME>"
    forwarderUrl = "<FORWARDER URL>"
    apiKey = "<API KEY>"
    privateKey = "<PRIVATE KEY>"

    Set vBaseClient = vBaseBuidler.CreateForwarderClient(forwarderUrl, apiKey, privateKey)
    Set vBaseDataset = vBaseBuidler.CreateDatasetFromJson(vBaseClient, "<Dataset JSON>")

    Set verificationResult = vBaseDataset.VerifyCommitments()

    MsgBox "Verification passed: " & verificationResult.VerificationPassed
Exit Sub

ErrorHandler:
    MsgBox "Use [Ctrl+Insert] to copy this message to the clipboard." & vbNewLine & "Error: " & Err.Description, vbCritical

End Sub
```

# How to Sign an MSI Using a Code Signing Certificate

1. Install the [Windows SDK](https://developer.microsoft.com/en-us/windows/downloads/windows-sdk/)
1. Make sure that the certificate is installed in the certificate store.
1. Run the command
```cmd
signtool sign /fd SHA256 /n "<CERTIFICATE_NAME>" /t http://timestamp.url.com /v "vBase.msi"
```

# How to Update the Diagrams in the Documentation
- All diagrams are created using [PlantUML](https://plantuml.com).
- The source code for the diagrams is located in the `Docs\Diagrams` folder.

# Known issues
1. The MSI installer installs the SDK library in the `Program Files (x86)` directory instead of `Program Files`.

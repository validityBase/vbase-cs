# How to use the COM library

1. Uninstall the library if you have it installed already on your machine.
1. Install the new version using `setup.exe`
1. Run Microsoft Excel
1. Make sure thta 'Developer' riboon tab is visible. If not, enable it in the Excel options.
    1. right click on the ribbon and select 'Customize the Ribbon'
    1. Check the 'Developer' checkbox in the reabon tabs tree
       ![Customize the Ribbon](images/customize-the-ribbon.png)
1. Got to Developer Ribbon tab and click on 'Visual Basic' button.
1. In the Microsoft Visual basic for Application Add a reference to the library: **Tools** -> **References** -> find **vBase**.
   ![Add Reference](images/add-reference.png)
1. The VBA code bellow demonstrates how to use the library:

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
    Set vBaseDataset = vBaseBuidler.CreateDataset(vBaseClient, datasetName, vBase.ObjectTypes_String)

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

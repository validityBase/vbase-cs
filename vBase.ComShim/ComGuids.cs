using System.Runtime.InteropServices;

namespace vBase
{
  /// <summary>
  /// Contains the GUIDs for the COM interfaces and classes.
  /// </summary>
  [ComVisible(false)]
  internal static class ComGuids
  {
    public const string vBaseBuilderInterface = "09DA00AE-E503-4D60-9B5C-F91FB3EB9CE8";
    public const string vBaseBuilder = "2E1D7C90-2B54-4790-8DDC-25E3E95C1F1C";
    
    public const string vBaseClientInterface = "E8D3B944-1B8F-4EA1-9821-9F8D344958FE";
    public const string vBaseClient = "06B41880-8E79-49B0-A9B4-82AC3BD11CBE";

    public const string vBaseDatasetInterface = "824FC12C-08E4-4CEF-90C8-B53A0DF498E5";
    public const string vBaseDataset = "32D6E01D-412E-4C8F-B751-C2DD7BAD8B03";

    public const string VerificationResultInterface = "2611961F-F796-462C-B676-246216A04FFF";
    public const string VerificationResult = "985DACDF-2FE2-4849-A987-0EFE5F175CE6";
    
    public const string vBaseReceiptInterface = "C6AE138B-A433-4F79-BD2B-BE1A67AFA666";
    public const string vBaseReceipt = "A11F5C6F-ADDB-4D20-B4BD-F32DE518D813";

    public const string vBaseWeb3ReceiptInterface = "C919560A-F536-4E78-BE5E-D436C66A3BC3";
    public const string vBaseWeb3Receipt = "C1CF0EAA-3DB5-4E89-B6D5-CF88C8CFD7CF";
  }
}

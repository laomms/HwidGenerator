using HwidGetCurrentEx;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace gatherosstate
{
    internal class Program
    {

		#region struct
		public enum ALG_ID
		{
			CALG_MD5 = 0x8003,
			CALG_RSA = 0x800C
		}

		[StructLayout(LayoutKind.Sequential)]
		public class RtlOsVersionInfoExW
		{
			public UInt32 dwOSVersionInfoSize;
			public UInt32 dwMajorVersion;
			public UInt32 dwMinorVersion;
			public UInt32 dwBuildNumber;
			public UInt32 dwPlataformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;
			public UInt16 wServicePackMajor;
			public UInt16 wServicePackMinor;
			public UInt16 wSuiteMask;
			public byte bProductType;
			public byte bReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct FILE_TIME
		{
			public uint dwLowDateTime;
			public uint dwHighDateTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SYSTEMTIME
		{
			public Int16 wYear;
			public Int16 wMonth;
			public Int16 wDayOfWeek;
			public Int16 wDay;
			public Int16 wHour;
			public Int16 wMinute;
			public Int16 wSecond;
			public Int16 wMilliseconds;
		}
		#endregion

		#region pinvoke

		[DllImport("Ntdll.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		extern static int RtlGetVersion([In(), Out()] RtlOsVersionInfoExW osversion);

		[DllImport("slc.dll", EntryPoint = "SLGetWindowsInformationDWORD", CharSet = CharSet.Auto)]
		extern static int SLGetWindowsInformationDWORD(string pwszValueName, ref int pdwValue);

		[DllImport("kernel32")]
		static extern void GetSystemTime(ref SYSTEMTIME lpSystemTime);

		[DllImport("kernel32")]
		private extern static void GetSystemTimeAsFileTime(ref FILE_TIME lpSystemTimeAsFileTime);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private extern static bool FileTimeToSystemTime((ref FILE_TIME lpFileTime, out SYSTEMTIME lpSystemTime);

		[DllImport("advapi32.dll")]
		extern static bool CryptCreateHash(IntPtr hProv, ALG_ID Algid, IntPtr hKey, uint dwFlags, ref IntPtr phHash);
		[DllImport("advapi32.dll", SetLastError = true)]
		extern static bool CryptGetHashParam(IntPtr hHash, int dwParam, byte[] pbData,ref int pdwDataLen, int dwFlags);
		[DllImport("Advapi32.dll", SetLastError = true)]
		extern static bool CryptHashData(IntPtr hHash, byte[] pbData, int dwDataLen, int dwFlags);
		[DllImport("advapi32.dll", SetLastError = true)]
		extern static bool CryptDestroyHash(IntPtr hHash);
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		extern static bool CryptAcquireContext(ref IntPtr hProv, string pszContainer, string pszProvider, uint dwProvType, uint dwFlags);


		#endregion

		#region define
		static bool KMS38=false; 
		static Dictionary<string, Tuple<int, int, string, string>> UpdateProductKey = new Dictionary<string, Tuple<int, int, string,string>>()
	{
		{"43TBQ-NH92J-XKTM7-KT3KK-P39PB", new Tuple<int, int, string, string>(125, 17763, "EnterpriseS","Microsoft.Windows.125.X21-83233_8wekyb3d8bbwe")},
		{"NK96Y-D9CD8-W44CQ-R8YTK-DYJWX", new Tuple<int, int, string, string>(125, 14393, "EnterpriseS","Microsoft.Windows.125.X21-05035_8wekyb3d8bbwe")},
		{"FWN7H-PF93Q-4GGP8-M8RF3-MDWWW", new Tuple<int, int, string, string>(125, 10240, "EnterpriseS","Microsoft.Windows.125.X19-99617_8wekyb3d8bbwe")},
		{"QPM6N-7J2WJ-P88HH-P3YRH-YY74H", new Tuple<int, int, string, string>(191, 19044, "IoTEnterpriseS","Microsoft.Windows.191.X21-99682_8wekyb3d8bbwe")},
		{"M33WV-NHY3C-R7FPM-BQGPT-239PG", new Tuple<int, int, string, string>(126, 17763, "EnterpriseSN","Microsoft.Windows.126.X21-83264_8wekyb3d8bbwe")},
		{"2DBW3-N2PJG-MVHW3-G7TDK-9HKR4", new Tuple<int, int, string, string>(126, 14393, "EnterpriseSN","Microsoft.Windows.126.X21-04922_8wekyb3d8bbwe")},
		{"8V8WN-3GXBH-2TCMG-XHRX3-9766K", new Tuple<int, int, string, string>(126, 10240, "EnterpriseSN","Microsoft.Windows.126.X19-98770_8wekyb3d8bbwe")},
		{"XQQYW-NFFMW-XJPBH-K8732-CKFFD", new Tuple<int, int, string, string>(188, 19044, "IoTEnterprise","Microsoft.Windows.188.X21-99378_8wekyb3d8bbwe")},
		{"YTMG3-N6DKC-DKB77-7M9GH-8HVX7", new Tuple<int, int, string, string>(101, 0, "Core","Microsoft.Windows.101.X19-98868_8wekyb3d8bbwe")},
		{"N2434-X9D7W-8PF6X-8DV9T-8TYMD", new Tuple<int, int, string, string>(99, 0, "CoreCountrySpecific","Microsoft.Windows.99.X19-99652_8wekyb3d8bbwe")},
		{"4CPRK-NM3K3-X6XXQ-RXX86-WXCHW", new Tuple<int, int, string, string>(98, 0, "CoreN","Microsoft.Windows.98.X19-98877_8wekyb3d8bbwe")},
		{"BT79Q-G7N6G-PGBYW-4YWX6-6F4BT", new Tuple<int, int, string, string>(100, 0, "CoreSingleLanguage","Microsoft.Windows.100.X19-99661_8wekyb3d8bbwe")},
		{"YNMGQ-8RYV3-4PGQ3-C8XTP-7CFBY", new Tuple<int, int, string, string>(121, 0, "Education","Microsoft.Windows.121.X19-98886_8wekyb3d8bbwe")},
		{"84NGF-MHBT6-FXBX8-QWJK7-DRR8H", new Tuple<int, int, string, string>(122, 0, "EducationN","Microsoft.Windows.122.X19-98892_8wekyb3d8bbwe")},
		{"XGVPP-NMH47-7TTHJ-W3FW7-8HV2C", new Tuple<int, int, string, string>(4, 0, "Enterprise","Microsoft.Windows.4.X19-99683_8wekyb3d8bbwe")},
		{"WGGHN-J84D6-QYCPR-T7PJ7-X766F", new Tuple<int, int, string, string>(27, 0, "EnterpriseN","Microsoft.Windows.27.X19-98747_8wekyb3d8bbwe")},
		{"3V6Q6-NQXCX-V8YXR-9QCYV-QPFCT", new Tuple<int, int, string, string>(171, 0, "EnterpriseG","Microsoft.Windows.27.X19-98746_8wekyb3d8bbwe")},
		{"FW7NV-4T673-HF4VX-9X4MM-B4H4T", new Tuple<int, int, string, string>(172, 0, "EnterpriseGN","Microsoft.Windows.172.X21-24709_8wekyb3d8bbwe")},
		{"VK7JG-NPHTM-C97JM-9MPGT-3V66T", new Tuple<int, int, string, string>(48, 0, "Professional","Microsoft.Windows.48.X19-98841_8wekyb3d8bbwe")},
		{"2B87N-8KFHP-DKV6R-Y2C8J-PKCKT", new Tuple<int, int, string, string>(49, 0, "ProfessionalN","Microsoft.Windows.49.X19-98859_8wekyb3d8bbwe")},
		{"8PTT6-RNW4C-6V7J2-C2D3X-MHBPB", new Tuple<int, int, string, string>(164, 0, "ProfessionalEducation","Microsoft.Windows.164.X21-04955_8wekyb3d8bbwe")},
		{"GJTYN-HDMQY-FRR76-HVGC7-QPF8P", new Tuple<int, int, string, string>(165, 0, "ProfessionalEducationN","Microsoft.Windows.165.X21-04956_8wekyb3d8bbwe")},
		{"DXG7C-N36C4-C4HTG-X4T3X-2YV77", new Tuple<int, int, string, string>(161, 0, "ProfessionalWorkstation","Microsoft.Windows.161.X21-43626_8wekyb3d8bbwe")},
		{"WYPNQ-8C467-V2W6J-TX4WX-WT2RQ", new Tuple<int, int, string, string>(162, 0, "ProfessionalWorkstationN","Microsoft.Windows.162.X21-43644_8wekyb3d8bbwe")}
	};
		static Dictionary<string, Tuple<int, int, string>> KMSProductKey = new Dictionary<string, Tuple<int, int, string>>()
 {
	 {"M7XTQ-FN8P6-TTKYV-9D4CC-J462D", new Tuple<int, int, string>(125, 17763, "EnterpriseS")},
	 {"DCPHK-NFMTC-H88MJ-PFHPY-QJ4BJ", new Tuple<int, int, string>(125, 14393, "EnterpriseS")},
	 {"WNMTR-4C88C-JK8YV-HQ7T2-76DF9", new Tuple<int, int, string>(125, 10240, "EnterpriseS")},
	 {"92NFX-8DJQP-P6BBQ-THF9C-7CG2H", new Tuple<int, int, string>(126, 17763, "EnterpriseSN")},
	 {"QFFDN-GRT3P-VKWWX-X7T3R-8B639", new Tuple<int, int, string>(126, 14393, "EnterpriseSN")},
	 {"2F77B-TNFGY-69QQF-B8YKP-D69TJ", new Tuple<int, int, string>(126, 10240, "EnterpriseSN")},
	 {"TX9XD-98N7V-6WMQ6-BX7FG-H8Q99", new Tuple<int, int, string>(101, 0, "Core")},
	 {"PVMJN-6DFY6-9CCP6-7BKTT-D3WVR", new Tuple<int, int, string>(99, 0, "CoreCountrySpecific")},
	 {"3KHY7-WNT83-DGQKR-F7HPR-844BM", new Tuple<int, int, string>(98, 0, "CoreN")},
	 {"7HNRX-D7KGG-3K4RQ-4WPJ4-YTDFH", new Tuple<int, int, string>(100, 0, "CoreSingleLanguage")},
	 {"NW6C2-QMPVW-D7KKK-3GKT6-VCFB2", new Tuple<int, int, string>(121, 0, "Education")},
	 {"2WH4N-8QGBV-H22JP-CT43Q-MDWWJ", new Tuple<int, int, string>(122, 0, "EducationN")},
	 {"NPPR9-FWDCX-D2C8J-H872K-2YT43", new Tuple<int, int, string>(4, 0, "Enterprise")},
	 {"DPH2V-TTNVB-4X9Q3-TJR4H-KHJW4", new Tuple<int, int, string>(27, 0, "EnterpriseN")},
	 {"YYVX9-NTFWV-6MDM3-9PT4T-4M68B", new Tuple<int, int, string>(171, 0, "EnterpriseG")},
	 {"44RPN-FTY23-9VTTB-MP9BX-T84FV", new Tuple<int, int, string>(172, 0, "EnterpriseGN")},
	 {"W269N-WFGWX-YVC9B-4J6C9-T83GX", new Tuple<int, int, string>(48, 0, "Professional")},
	 {"MH37W-N47XK-V7XM9-C7227-GCQG9", new Tuple<int, int, string>(49, 0, "ProfessionalN")},
	 {"6TP4R-GNPTD-KYYHQ-7B7DP-J447Y", new Tuple<int, int, string>(164, 0, "ProfessionalEducation")},
	 {"YVWGF-BXNMC-HTQYQ-CPQ99-66QFC", new Tuple<int, int, string>(165, 0, "ProfessionalEducationN")},
	 {"NRG8B-VKK3Q-CXVCJ-9G2XF-6Q84J", new Tuple<int, int, string>(161, 0, "ProfessionalWorkstation")},
	 {"9FNHH-K3HBT-3W4TD-6383H-6XYWF", new Tuple<int, int, string>(162, 0, "ProfessionalWorkstationN")},
	 {"VDYBN-27WPP-V4HQT-9VMD4-VMK7H", new Tuple<int, int, string>(7, 20348, "ServerStandard")},
	 {"N69G4-B89J2-4G8F4-WWYCC-J464C", new Tuple<int, int, string>(7, 17763, "ServerStandard")},
	 {"WC2BQ-8NRM3-FDDYY-2BFGV-KHKQY", new Tuple<int, int, string>(7, 0, "ServerStandard")},
	 {"67KN8-4FYJW-2487Q-MQ2J7-4C4RG", new Tuple<int, int, string>(13, 20348, "ServerStandardCore")},
	 {"N2KJX-J94YW-TQVFB-DG9YT-724CC", new Tuple<int, int, string>(13, 17763, "ServerStandardCore")},
	 {"PTXN8-JFHJM-4WC78-MPCBR-9W4KR", new Tuple<int, int, string>(13, 0, "ServerStandardCore")},
	 {"WX4NM-KYWYW-QJJR4-XV3QB-6VM33", new Tuple<int, int, string>(8, 20348, "ServerDatacenter")},
	 {"WMDGN-G9PQG-XVVXX-R3X43-63DFG", new Tuple<int, int, string>(8, 17763, "ServerDatacenter")},
	 {"CB7KF-BWN84-R7R2Y-793K2-8XDDG", new Tuple<int, int, string>(8, 0, "ServerDatacenter")},
	 {"QFND9-D3Y9C-J3KKY-6RPVP-2DPYV", new Tuple<int, int, string>(12, 20348, "ServerDatacenterCore")},
	 {"6NMRW-2C8FM-D24W7-TQWMY-CWH2D", new Tuple<int, int, string>(12, 17763, "ServerDatacenterCore")},
	 {"2HXDN-KRXHB-GPYC7-YCKFJ-7FVDG", new Tuple<int, int, string>(12, 0, "ServerDatacenterCore")},
	 {"6N379-GGTMK-23C6M-XVVTC-CKFRQ", new Tuple<int, int, string>(52, 20348, "ServerSolution")},
	 {"WVDHN-86M7X-466P6-VHXV7-YY726", new Tuple<int, int, string>(52, 17763, "ServerSolution")},
	 {"FDNH6-VW9RW-BXPJ7-4XTYG-239TB", new Tuple<int, int, string>(52, 0, "ServerSolution")},
	 {"TM8T6-9NJWV-PC26Y-RFYXH-YY723", new Tuple<int, int, string>(53, 0, "ServerSolutionCore")}
 };
		static Dictionary<string, string> SkuList = new Dictionary<string, string>()
   {
	   {"1", "Ultimate"},
	   {"2", "HomeBasic"},
	   {"3", "HomePremium"},
	   {"4", "Enterprise"},
	   {"5", "HomeBasicN"},
	   {"6", "Business"},
	   {"7", "ServerStandard"},
	   {"8", "ServerDatacenter"},
	   {"9", "ServerSBSStandard"},
	   {"10", "ServerEnterprise"},
	   {"11", "Starter"},
	   {"12", "ServerDatacenterCore"},
	   {"13", "ServerStandardCore"},
	   {"14", "ServerEnterpriseCore"},
	   {"15", "ServerEnterpriseIA64"},
	   {"16", "BusinessN"},
	   {"17", "ServerWeb"},
	   {"18", "ServerComputeCluster"},
	   {"19", "ServerHomeStandard"},
	   {"20", "ServerStorageExpress"},
	   {"21", "ServerStorageStandard"},
	   {"22", "ServerStorageWorkgroup"},
	   {"23", "ServerStorageEnterprise"},
	   {"24", "ServerWinSB"},
	   {"25", "ServerSBSPremium"},
	   {"26", "HomePremiumN"},
	   {"27", "EnterpriseN"},
	   {"28", "UltimateN"},
	   {"29", "ServerWebCore"},
	   {"30", "ServerMediumBusinessManagement"},
	   {"31", "ServerMediumBusinessSecurity"},
	   {"32", "ServerMediumBusinessMessaging"},
	   {"33", "ServerWinFoundation"},
	   {"34", "ServerHomePremium"},
	   {"35", "ServerWinSBV"},
	   {"36", "ServerStandardV"},
	   {"37", "ServerDatacenterV"},
	   {"38", "ServerEnterpriseV"},
	   {"39", "ServerDatacenterVCore"},
	   {"40", "ServerStandardVCore"},
	   {"41", "ServerEnterpriseVCore"},
	   {"42", "ServerHyperCore"},
	   {"43", "ServerStorageExpressCore"},
	   {"44", "ServerStorageStandardCore"},
	   {"45", "ServerStorageWorkgroupCore"},
	   {"46", "ServerStorageEnterpriseCore"},
	   {"47", "StarterN"},
	   {"48", "Professional"},
	   {"49", "ProfessionalN"},
	   {"50", "ServerSolution"},
	   {"51", "ServerForSBSolutions"},
	   {"52", "ServerSolutionsPremium"},
	   {"53", "ServerSolutionsPremiumCore"},
	   {"54", "ServerSolutionEM"},
	   {"55", "ServerForSBSolutionsEM"},
	   {"56", "ServerEmbeddedSolution"},
	   {"57", "ServerEmbeddedSolutionCore"},
	   {"58", "ProfessionalEmbedded"},
	   {"59", "ServerEssentialManagement"},
	   {"60", "ServerEssentialAdditional"},
	   {"61", "ServerEssentialManagementSvc"},
	   {"62", "ServerEssentialAdditionalSvc"},
	   {"63", "ServerSBSPremiumCore"},
	   {"64", "ServerHPC"},
	   {"65", "Embedded"},
	   {"66", "StarterE"},
	   {"67", "HomeBasicE"},
	   {"68", "HomePremiumE"},
	   {"69", "ProfessionalE"},
	   {"70", "EnterpriseE"},
	   {"71", "UltimateE"},
	   {"72", "EnterpriseEval"},
	   {"74", "Prerelease"},
	   {"76", "ServerMultiPointStandard"},
	   {"77", "ServerMultiPointPremium"},
	   {"79", "ServerStandardEval"},
	   {"80", "ServerDatacenterEval"},
	   {"84", "EnterpriseNEval"},
	   {"85", "EmbeddedAutomotive"},
	   {"86", "EmbeddedIndustryA"},
	   {"87", "ThinPC"},
	   {"88", "EmbeddedA"},
	   {"89", "EmbeddedIndustry"},
	   {"90", "EmbeddedE"},
	   {"91", "EmbeddedIndustryE"},
	   {"92", "EmbeddedIndustryAE"},
	   {"93", "ProfessionalPlus"},
	   {"95", "ServerStorageWorkgroupEval"},
	   {"96", "ServerStorageStandardEval"},
	   {"97", "CoreARM"},
	   {"98", "CoreN"},
	   {"99", "CoreCountrySpecific"},
	   {"100", "CoreSingleLanguage"},
	   {"101", "Core"},
	   {"103", "ProfessionalWMC"},
	   {"105", "EmbeddedIndustryEval"},
	   {"106", "EmbeddedIndustryEEval"},
	   {"107", "EmbeddedEval"},
	   {"108", "EmbeddedEEval"},
	   {"109", "ServerNano"},
	   {"110", "ServerCloudStorage"},
	   {"111", "CoreConnected"},
	   {"112", "ProfessionalStudent"},
	   {"113", "CoreConnectedN"},
	   {"114", "ProfessionalStudentN"},
	   {"115", "CoreConnectedSingleLanguage"},
	   {"116", "CoreConnectedCountrySpecific"},
	   {"117", "ConnectedCar"},
	   {"118", "IndustryHandheld"},
	   {"119", "PPIPro"},
	   {"120", "ServerARM64"},
	   {"121", "Education"},
	   {"122", "EducationN"},
	   {"123", "IoTUAP"},
	   {"124", "ServerCloudHostInfrastructure"},
	   {"125", "EnterpriseS"},
	   {"126", "EnterpriseSN"},
	   {"127", "ProfessionalS"},
	   {"128", "ProfessionalSN"},
	   {"129", "EnterpriseSEval"},
	   {"130", "EnterpriseSNEval"},
	   {"131", "IoTUAPCommercial"},
	   {"133", "MobileEnterprise"},
	   {"135", "Holographic"},
	   {"136", "HolographicBusiness"},
	   {"138", "ProfessionalSingleLanguage"},
	   {"139", "ProfessionalCountrySpecific"},
	   {"140", "EnterpriseSubscription"},
	   {"141", "EnterpriseSubscriptionN"},
	   {"143", "ServerDatacenterNano"},
	   {"144", "ServerStandardNano"},
	   {"145", "ServerDatacenterACor"},
	   {"146", "ServerStandardACor"},
	   {"147", "ServerDatacenterWSCor"},
	   {"148", "ServerStandardWSCor"},
	   {"149", "UtilityVM"},
	   {"159", "ServerDatacenterEvalCor"},
	   {"160", "ServerStandardEvalCor"},
	   {"161", "ProfessionalWorkstation"},
	   {"162", "ProfessionalWorkstationN"},
	   {"164", "ProfessionalEducation"},
	   {"165", "ProfessionalEducationN"},
	   {"168", "ServerAzureCor"},
	   {"169", "ServerAzureNano"},
	   {"171", "EnterpriseG"},
	   {"172", "EnterpriseGN"},
	   {"175", "ServerRdsh"},
	   {"178", "Cloud"},
	   {"179", "CloudN"},
	   {"180", "HubOS"},
	   {"182", "OneCoreUpdateOS"},
	   {"183", "CloudE"},
	   {"184", "Andromeda"},
	   {"185", "IoTOS"},
	   {"186", "CloudEN"},
	   {"187", "IoTEdgeOS"},
	   {"188", "IoTEnterprise"},
	   {"189", "Lite"},
	   {"191", "IoTEnterpriseS"},
	   {"202", "CloudEditionN"},
	   {"203", "CloudEdition"},
	   {"406", "ServerAzureStackHCICor"},
	   {"407", "ServerTurbine"},
	   {"408", "ServerTurbineCor"}
   };
		#endregion
		static void Main(string[] args)
        {
			string VersionString = GetOsVersion();
			if (NativeOsVersion().ToString().Contains("10.0") == false && NativeOsVersion().ToString().Contains("11") == false)
			{
				return;
			}

			byte[] DST = HWID.HwidGetCurrentEx();
			string Base64String1 = HWID.HwidCreateBlock(DST, DST[0]);
			string SessionId = VersionString + ";" + "Hwid=" + Base64String1 + ";";
			
			int EditionID = 0;
			int szRes = SLGetWindowsInformationDWORD("Kernel-ProductInfo",ref EditionID);
			if (szRes != 0)
            {
				RegistryKey regkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)).OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", true);
				if (regkey != null)
				{
					string EditionName = regkey.GetValue("EditionID").ToString();
					EditionID =int.Parse(SkuList.Where((x) => x.Value.ToString() == EditionName).Select((y) => y.Key.ToString()).ToList()[0]);
				}
			}

			if (KMS38)
            {
				string ProductKeys = KMSProductKey.Where((x) => x.Value.Item1 == EditionID && NativeOsVersion().Build >= x.Value.Item2).Select((y) => y.Key).ToList()[0];
				if (string.IsNullOrEmpty(ProductKeys)) return;
				SessionId = SessionId + "GVLKExp=" + FileTime2SystemTime() + ";DownlevelGenuineState=1;";
			}
			else
            {
				string ProductKeys = UpdateProductKey.Where((x) => x.Value.Item1 == EditionID && NativeOsVersion().Build >= x.Value.Item2).Select((y) => y.Key).ToList()[0];
				if (string.IsNullOrEmpty(ProductKeys)) return;
				string pfn = UpdateProductKey[ProductKeys].Item4;
				SessionId = SessionId + "Pfn=" + pfn + ";DownlevelGenuineState=1;";
			}
			
			byte[] bytesessionid = System.Text.Encoding.Unicode.GetBytes(SessionId);
			bytesessionid = bytesessionid.Concat(new byte[] { 0, 0 }).ToArray();
			string base64string2 = Convert.ToBase64String(bytesessionid);
			SessionId = base64string2 + ";" + UtcTimeToIso8601();
			byte[] hashArray = ComputeHashEx("SessionId=" + SessionId);
			string base64string3= VRSAVaultSignPKCS(hashArray);
			if (!string.IsNullOrEmpty(base64string3))
				SaveData(SessionId, base64string3, System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\DigitalLicense.xml");
				//SaveData(SessionId, base64string3, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Microsoft\\Windows\\ClipSVC\\GenuineTicket\\DigitalLicense.xml");

		}

		#region function
		static void SaveData(string SessionId, string Base64String3, string szPath)
		{
			if (string.IsNullOrEmpty(Base64String3) || string.IsNullOrEmpty(SessionId))
			{
				return;
			}
			string pubkey = "BgIAAACkAABSU0ExAAgAAAEAAQARq+V11k+dvHMCaLWVCaSbeQNlOdWTLkkl0hdMh5V3YhLU2R4h0Jd+7k7qfZ4aIo4ussduwGgmyDRikj5L2R77GG2ciHk4i8siK8qg7frOU0KT5rEks3qVj38C3dS1wS6D67shBFrxPlOEP8+JlelgP7Gxmwdao7NF4LXZ3+KdbJ//9jkmN8iAOP0N2XzW0/cJp9P1q6hE7eeqc/3Qn3zMr0q1Dx7vstN98oV17hNYCwumOxxS1rH+3n7ap2JKRSelo8Jvi214jZLBL+hOtYaGpxs7zIL3ofpoaYy5g7pc/DaTvyfpJho5634jK7dXVFMpzJZMn9w0F/3rkquk0Amm";
			using (XmlTextWriter writer = new XmlTextWriter(szPath, System.Text.Encoding.UTF8))
			{
				writer.WriteStartDocument();
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 4;
				writer.WriteStartElement("genuineAuthorization", "http://www.microsoft.com/DRM/SL/GenuineAuthorization/1.0");
				writer.WriteStartElement("version");
				writer.WriteString("1.0");
				writer.WriteEndElement();
				writer.WriteStartElement("genuineProperties");
				writer.WriteAttributeString("origin", "sppclient");
				writer.WriteStartElement("properties");
				writer.WriteString("SessionId=" + SessionId);
				writer.WriteEndElement();
				writer.WriteStartElement("signatures");
				writer.WriteStartElement("signature");
				writer.WriteAttributeString("name", "downlevelGTkey");
				writer.WriteAttributeString("method", "rsa-sha256");
				writer.WriteAttributeString("key", pubkey);
				writer.WriteString(Base64String3);
				writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}

		}
		static string GetOsVersion() 
		{
			RtlOsVersionInfoExW osVersionInfo = new RtlOsVersionInfoExW();
			osVersionInfo.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osVersionInfo);
			int status = RtlGetVersion(osVersionInfo);
			if (status != 0)
			{
				return ""; 
			}			
			return "OSMajorVersion=" + osVersionInfo.dwMajorVersion + ";OSMinorVersion=" + osVersionInfo.dwMinorVersion + ";OSPlatformId=" + osVersionInfo.dwPlataformId + ";PP=0";
		}
		static Version NativeOsVersion()
		{
			RtlOsVersionInfoExW osVersionInfo = new RtlOsVersionInfoExW();
			osVersionInfo.dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(RtlOsVersionInfoExW));
			int status = RtlGetVersion(osVersionInfo);
			if (status != 0)
			{
				return Environment.OSVersion.Version;
			}
			return new Version((int)osVersionInfo.dwMajorVersion, (int)osVersionInfo.dwMinorVersion, (int)osVersionInfo.dwBuildNumber);
		}

		static string FileTime2SystemTime()
		{
			FILE_TIME SystemTimeAsFileTime = new FILE_TIME();
			GetSystemTimeAsFileTime(ref SystemTimeAsFileTime);
			SYSTEMTIME SysTime = new SYSTEMTIME();
			FILE_TIME filetime = new FILE_TIME();
			filetime.dwLowDateTime = SystemTimeAsFileTime.dwLowDateTime;
			if (Convert.ToBoolean(FileTimeToSystemTime(ref filetime,out SysTime)) == true)
			{
				var timestampclient = SysTime.wYear.ToString("0000") + "-" + SysTime.wMonth.ToString("00") + "-" + SysTime.wDay.ToString("00") + "T" + SysTime.wHour.ToString("00") + ":" + SysTime.wMinute.ToString("00") + ":" + SysTime.wSecond.ToString("00") + "Z";
				return timestampclient.Replace(timestampclient.Substring(0, 10), "2038-01-19");
			}
			return "";
		}

		private static string UtcTimeToIso8601()
		{
			string timestampclient = "";
			SYSTEMTIME SysTime = new SYSTEMTIME();
			GetSystemTime(ref SysTime);
			timestampclient = SysTime.wYear.ToString("0000") + "-" + SysTime.wMonth.ToString("00") + "-" + SysTime.wDay.ToString("00") + "T" + SysTime.wHour.ToString("00") + ":" + SysTime.wMinute.ToString("00") + ":" + SysTime.wSecond.ToString("00") + "Z";
			if (string.IsNullOrEmpty(timestampclient))
			{
				return string.Empty; 
			}
			return "TimeStampClient=" + timestampclient;			
		}
		private static byte[] ComputeHashEx(string szSessionId)
		{
			var cryptographic = "Microsoft Enhanced RSA and AES Cryptographic Provider";
			IntPtr phProv = new IntPtr();
			uint CRYPt_VERIFYCONTEXT = 0xF0000020U;
			uint PROV_RsA_FULL = 0x18;
			bool res = CryptAcquireContext(ref phProv, null, cryptographic, PROV_RsA_FULL, CRYPt_VERIFYCONTEXT);
			byte[] pbDatas = new byte[4];
			byte[] pNewData = ComputeHash(phProv, szSessionId, pbDatas);
			pbDatas = new byte[pNewData[0]];			
			byte[] pbNewData = ComputeHash(phProv, szSessionId, pbDatas);
			return pbNewData;
		}
		private static byte[] ComputeHash(IntPtr phProv, string SessionId, byte[] pbDatas)
		{
			byte[] ReturnBytes = null;
			IntPtr hHash = new IntPtr();			
			byte[] pbData = new byte[4];
			if (CryptCreateHash(phProv, ALG_ID.CALG_RSA, IntPtr.Zero, 0, ref hHash))
			{
				int pdwDataLen = 4;
				if (CryptGetHashParam(hHash, 4, pbData,ref pdwDataLen, 0))
				{
					if (!(pbDatas.Length == 4))
					{
						var pdDataLen = BitConverter.ToInt32(pbData, 0);
						byte[] pbBuffer = System.Text.Encoding.UTF8.GetBytes(SessionId);
						if (CryptHashData(hHash, pbBuffer, pbBuffer.Length, 0))
						{
							pdwDataLen = pbDatas.Length;
							if (CryptGetHashParam(hHash, 2, pbDatas, ref pdwDataLen, 0))
							{								
								ReturnBytes = pbDatas;
							}
						}
					}
					else
					{
						ReturnBytes = pbData;
					}
				}
			}
			CryptDestroyHash(hHash);
			return ReturnBytes;
		}
		static string VRSAVaultSignPKCS(byte[] hashArray)
        {
			IntPtr retValue = Marshal.AllocHGlobal(256);
			byte[] DST = new byte[256];
			DST[254] = 1;
			hashArray.Reverse().ToArray().CopyTo(DST, 0);
			byte[] sign = new byte[] { 0x20, 0x04, 0x00, 0x05, 0x01, 0x02, 0x04, 0x03, 0x65, 0x01, 0x48, 0x86, 0x60, 0x09, 0x06, 0x0D, 0x30, 0x31, 0x30 };//0x13
			sign.CopyTo(DST, hashArray.Length);
			Marshal.Copy(DST, 0, retValue, 256);
			byte[] pData = VbnRsaVault_ModExpPriv_clear.compute.ModExpPriv_clear(hashArray);            
            if (pData != null)
            {
                return System.Convert.ToBase64String(pData);
            }
            return string.Empty;
        }

		#endregion

	}
}

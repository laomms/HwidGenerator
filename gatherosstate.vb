DIA反汇编代码:
text:00B497D8 push    offset aOsmajorversion          ; 'OSMajorVersion=%d;OSMinorVersion=%d;OSPlatformId=%d'
.text:00B497DD push    eax
.text:00B497DE call    GatherOsInformation
.text:00B497E3 mov     esi, eax
.text:00B497E5 add     esp, 14h
.text:00B497E8 test    esi, esi
.text:00B497EA js      loc_B49BC0
.text:00B497F0 mov     eax, large fs:30h
.text:00B497F6 movzx   eax, byte ptr [eax+3]
.text:00B497FA shr     eax, 1
.text:00B497FC and     eax, 1
.text:00B497FF push    eax
.text:00B49800 lea     eax, [esp+204h+var_1D4]
.text:00B49804 push    offset aPpD                     ; "PP=%d;"
.text:00B49809 push    eax
.text:00B4980A call    GatherOsInformation

vb.net 实现:
    <StructLayout(LayoutKind.Sequential)>
    Public Class RtlOsVersionInfoExW
        Public dwOSVersionInfoSize As UInt32
        Public dwMajorVersion As UInt32
        Public dwMinorVersion As UInt32
        Public dwBuildNumber As UInt32
        Public dwPlataformId As UInt32
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)> Public szCSDVersion As String
        Public wServicePackMajor As UInt16
        Public wServicePackMinor As UInt16
        Public wSuiteMask As UInt16
        Public bProductType As Byte
        Public bReserved As Byte
    End Class
     <DllImport("Ntdll.dll", CharSet:=CharSet.Unicode, ExactSpelling:=True)>
    Friend Function RtlGetVersion(<[In](), Out()> ByVal osversion As RtlOsVersionInfoExW) As Integer
    End Function
    Public Function NativeOsVersion() As Version      
        Dim osVersionInfo As New RtlOsVersionInfoExW()
        osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(GetType(RtlOsVersionInfoExW))
        Dim status As Integer = RtlGetVersion(osVersionInfo)
        If status <> 0 Then
            Return Environment.OSVersion.Version
        End If
        Return New Version(osVersionInfo.dwMajorVersion, osVersionInfo.dwMinorVersion, osVersionInfo.bProductType)
    End Function
    '拼接后字符串:OSMajorVersion=10;OSMinorVersion=0;OSPlatformId=2;PP=0;

.text:00B49831 push    ecx                             ; unsigned int *
.text:00B49832 push    ecx                             ; struct _HWID **
.text:00B49833 lea     eax, [esp+208h+var_198]
.text:00B49837 push    eax                             ; unsigned int
.text:00B49838 lea     eax, [esp+20Ch+var_1D8]
.text:00B4983C push    eax                             ; unsigned __int8 *
.text:00B4983D call    HwidGetCurrentEx                ;F7跟入
.text:00B49842 mov     esi, eax
.text:00B49844 test    esi, esi
void *__stdcall HwidGetCurrentEx(unsigned __int8 *a1, unsigned int a2, struct _HWID **a3, unsigned int *a4, int **a5, unsigned int *a6)
************
.text:00B8594B push    118h                            ; Size
.text:00B85950 push    ebx                             ; Val
.text:00B85951 push    esi                             ; Dst
.text:00B85952 call    _memset
.text:00B85957 ; 63:   v6[1] = 0;
.text:00B85957 xor     eax, eax
.text:00B85959 add     esp, 0Ch
.text:00B8595C mov     [esi+2], ax
.text:00B85960 ; 64:   v12 = CreateSppNamedParams(v11, &v38, v30, v31);
.text:00B85960 lea     eax, [ebp+var_20]
.text:00B85963 push    eax                             ; struct _GUID *
.text:00B85964 call    CreateSppNamedParams
long __cdecl CreateSppNamedParams(struct _GUID const & __ptr64,struct IUnknown * __ptr64,void * __ptr64 * __ptr64)

.text:00B85980 and     dword ptr [ebp+var_20.Data2], 0
.text:00B85984 ; 72:     v13 = 0;
.text:00B85984 xor     eax, eax
.text:00B85986 ; 73:     v38.Data1 = 0;
.text:00B85986 mov     [ebp+var_20.Data1], eax
.text:00B85989 ; 75:     v9 = (void *)CreateInstance(*(int *)((char *)&dword_AECDC0 + v13), &v37);
public: static long __cdecl CHardwareComponentManagerT<0>::CreateInstance(enum _HWIDCLASS,struct ISppHwidCollector * __ptr64 * __ptr64)

.text:00B859AA mov     dword ptr [ebp+var_20.Data4], eax
.text:00B859AD lea     eax, [ebp+var_24]
.text:00B859B0 push    eax
.text:00B859B1 push    [ebp+var_30]
.text:00B859B4 call    Insert
public: long __cdecl CArray<struct CSppParam,unsigned short const * __ptr64,class CAdaptorDefault,class CPoliciesDefault>::Insert(int,struct CSppParam const & __ptr64) __ptr64

text:00B859C6 mov     ecx, dword ptr [ebp+var_20.Data4]
.text:00B859C9 push    edi
.text:00B859CA push    ecx
.text:00B859CB mov     eax, [ecx]
.text:00B859CD mov     ebx, [eax+0Ch]
.text:00B859D0 mov     ecx, ebx
.text:00B859D2 call    ds:___guard_check_icall_fptr
.text:00B859D8 call    ebx                             ; 遍历GUID结构  F7跟入
***********
.text:00B8998E jnz     loc_B89A18
.text:00B89994 push    6                               ; Flags
.text:00B89996 xor     eax, eax
.text:00B89998 push    eax                             ; hwndParent
.text:00B89999 push    eax                             ; Enumerator
.text:00B8999A push    eax                             ; ClassGuid
.text:00B8999B call    ds:__imp__SetupDiGetClassDevsW@16 ; load__SetupDiGetClassDevsW(x,x,x,x) ...
...
.text:00B89B7A mov     [esp+68h+var_48], edx
.text:00B89B7E push    eax                             ; DeviceInfoData
.text:00B89B7F push    edx                             ; MemberIndex
.text:00B89B80 push    [esp+70h+DeviceInfoSet]         ; DeviceInfoSet
.text:00B89B84
.text:00B89B84 loc_B89B84:                             ; CODE XREF: CollectInternal+11B↑j
.text:00B89B84 mov     [esp+74h+DeviceInfoData.cbSize], 1Ch
.text:00B89B8C call    ds:__imp__SetupDiEnumDeviceInfo@12 ; load__SetupDiEnumDeviceInfo(x,x,x) ...
        MyGuid=GUID_DEVCLASS_CDROM,GUID_DEVCLASS_..
        Dim hdevDisplayInfoSet As IntPtr = SetupDiGetClassDevsW(MyGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT)
        If (hdevDisplayInfoSet <> IntPtr.Zero) Then
            Dim devInfoData = New SP_DEVINFO_DATA
            devInfoData.cbSize = Marshal.SizeOf(devInfoData)
            Dim drvInfoData = New SP_DRVINFO_DATA_V2_W
            drvInfoData.cbSize = Marshal.SizeOf(drvInfoData)
            Dim drvInfoDetailData = New SP_DRVINFO_DETAIL_DATA_W
            drvInfoDetailData.cbSize = Marshal.SizeOf(drvInfoDetailData)
            Dim index = 0
            Do While SetupDiEnumDeviceInfo(hdevDisplayInfoSet, index, devInfoData)
                If SetupDiBuildDriverInfoList(hdevDisplayInfoSet, devInfoData, SPDIT_COMPATDRIVER) = False Then
                    SetupDiDestroyDeviceInfoList(hdevDisplayInfoSet)
                    SetupDiDestroyDriverInfoList(hdevDisplayInfoSet, devInfoData, SPDIT_COMPATDRIVER)
                    Exit Do
                End If
                ...
                index = (index + 1)
            Loop

.text:00B49842 mov     esi, eax
.text:00B49844 test    esi, esi
.text:00B49846 js      loc_B49BC0
.text:00B4984C lea     eax, [esp+200h+var_1B0]
.text:00B49850 push    eax                             ; struct _HWID *
.text:00B49851 lea     eax, [esp+204h+hMem]
.text:00B49855 push    eax                             ; unsigned int
.text:00B49856 push    [esp+208h+var_1D8]              ; struct _HWID_TIMEWEIGHT *
.text:00B4985A lea     edx, [esp+20Ch+var_28]
.text:00B49861 push    ecx                             ; unsigned __int16
.text:00B49862 call    HwidCreateBlock
<_HWID_BLOCK::CreateInstance(ushort,_HWID_TIMEWEIGHT * const,ulong,_HWID *,_HWID_BLOCK * *,ulong *)>
得到转换后数数组 69 00 00 00 13 00 3E 00 00 00 00 00 01 00 02 00 02 00 01 00 04 00 00 00 06 00 01 00 01 00 68 BE

.text:00B4986B js      loc_B49BC0
.text:00B49871 mov     edx, [esp+200h+var_1B0]
.text:00B49875 lea     eax, [esp+200h+var_1B4]
.text:00B49879 push    eax
.text:00B4987A push    ecx
.text:00B4987B mov     ecx, [esp+208h+hMem]
.text:00B4987F call    Base64Encod
base64加密:aQAAABMAPgAAAAAAAQACAAIAAQAEAAAABgABAAEAaL78GYIZzC+CMpoYdisY1rBaF40Ojl/xpA7eLlxQmlykYNbxEO8MAAIAAQEA

text:00B49896 push    offset aHwidS                   ; "Hwid=%s;"
.text:00B4989B push    eax
.text:00B4989C call    GatherOsInformation
拼接字符串:OSMajorVersion=10;OSMinorVersion=0;OSPlatformId=2;PP=0;Hwid=aQAAABMAPgAAAAAAAQACAAIAAQAEAAAABgABAAEA(取40位)

.text:00B498A1 mov     esi, eax
.text:00B498A3 add     esp, 0Ch
.text:00B498A6 test    esi, esi
.text:00B498A8 js      loc_B49BC0
.text:00B498AE lea     ecx, [esp+200h+var_170]
.text:00B498B5 call    _GetActiveWindowsSkuStatus@4    ; GetActiveWindowsSkuStatus(x)  跟进用了三个sls.dll api
.text:00B498BA mov     esi, eax
.text:00B498BC test    esi, esi
.text:00B498BE js      loc_B49BC0
.text:00B498C4 mov     ecx, [esp+200h+var_1F4]
.text:00B498C8 lea     eax, [esp+200h+var_1AC]
.text:00B498CC push    eax
.text:00B498CD push    offset aVolumeGvlk_0            ; "Volume:GVLK"
.text:00B498D2 lea     edx, [esp+208h+var_170]
.text:00B498D9 call    CompareSkuChannel               ;对比SKU版本是否位Retail

    Public Enum SLDATATYPE
        SL_DATA_NONE
        SL_DATA_SZ
        SL_DATA_DWORD
        SL_DATA_BINARY
        SL_DATA_MULTI_SZ
        SL_DATA_SUM
    End Enum
      <DllImport("slc.dll", EntryPoint:="SLGetWindowsInformation", CharSet:=CharSet.Auto)>
    Shared Function SLGetWindowsInformation(ByVal ValueName As String, ByRef DataType As SLDATATYPE, ByRef cbValue As UInteger, ByRef Value As IntPtr) As UInteger
    Dim peDataType As New SLDATATYPE()
    Dim pcbValue As UInteger = Nothing
    Dim ppbValue As IntPtr = Nothing
    Dim hInf As Integer = SLGetWindowsInformation("Kernel-EditionName", peDataType, pcbValue, ppbValue)
    Dim szEdition As String = Marshal.PtrToStringUni(ppbValue)
    
    <DllImport("slc.dll", EntryPoint:="SLGetProductSkuInformation", ExactSpelling:=False, CharSet:=CharSet.Unicode)>
    Shared Function SLGetProductSkuInformation(ByVal hSLC As IntPtr, ByVal pProductSkuId() As Byte, ByVal pwszValueName As String, ByRef peDataType As SLDATATYPE, ByRef pcbValue As UInteger, ByRef ppbValue As IntPtr) As Integer
    End Function
    Dim pcbValue As UInteger = Nothing
    Dim ppbValue As IntPtr = Nothing
    Dim datatype As New SLDATATYPE()
    Dim hSkuInf As Integer = SLGetProductSkuInformation(hSLC, bSkuId, "Channel", datatype, pcbValue, ppbValue)
    dim szChannel = Marshal.PtrToStringUni(ppbValue)
                        
    <DllImport("slc.dll", EntryPoint:="SLGetSLIDList", ExactSpelling:=False, CharSet:=CharSet.Unicode)>
    Shared Function SLGetSLIDList(ByVal hSLC As IntPtr, ByVal eQueryIdType As SLIDTYPE, ByVal pQueryId() As Byte, ByVal eReturnIdType As SLIDTYPE, ByRef pnReturnIds As UInteger, ByRef ppReturnIds As IntPtr) As Integer
    End Function
     SLGetSLIDList(hSLC, SLIDTYPE.SL_ID_PRODUCT_SKU, bSkuId, SLIDTYPE.SL_ID_APPLICATION, pnStatusCount, ppReturnId)
     
     
.text:00B498FE push    [esp+204h+var_15C]
.text:00B49905 lea     edx, [esp+208h+FileTime]
.text:00B4990C lea     ecx, [esp+208h+SystemTimeAsFileTime]
.text:00B49910 call    AddTimeDelta
.text:00B49915 mov     esi, eax
.text:00B49917 test    esi, esi
.text:00B49919 js      loc_B49BC0
.text:00B4991F lea     eax, [esp+200h+SystemTime]
.text:00B49926 push    eax                             ; lpSystemTime
.text:00B49927 lea     eax, [esp+204h+FileTime]
.text:00B4992B push    eax                             ; lpFileTime
.text:00B4992C call    ds:__imp__FileTimeToSystemTime@8 ; FileTimeToSystemTime(x,x)
.text:00B49932 test    eax, eax
.text:00B49934 jz      loc_B497AB
.text:00B4993A lea     edx, [esp+200h+var_1C4]
.text:00B4993E lea     ecx, [esp+200h+SystemTime]
.text:00B49945 call    UtcTimeToIso8601
拼接时间:

.text:00B49A01 mov     ecx, [esp+208h+var_1D4]
.text:00B49A05 lea     edx, ds:2[eax*2]
.text:00B49A0C call    Base64Encod
取刚才拼接字符串的前38位base64加密:OSMajorVersion=10;OSMinorVersion=0;OSP
得到base64字符串:TwBTAE0AYQBqAG8AcgBWAGUAcgBzAGkAbwBuAD0AMQAwADsATwBTAE0AaQBuAG8AcgBWAGUAcgBzAGkAbwBuAD0AMAA7AE8AUwBQ

.text:00B49A1F lea     eax, [esp+204h+pwszValueName]
.text:00B49A23 push    offset aSlGetGenuineAu          ; "SL_GET_GENUINE_AUTHZ:%s"
.text:00B49A28 push    eax                             ; unsigned __int16 **
.text:00B49A29 call    STRAPI_Format
.text:00B499BD lea     eax, [esp+200h+pdwValue]
.text:00B499C1 push    eax                             ; pdwValue
.text:00B499C2 push    offset pwszValueName            ; "Security-SPP-GenuineLocalStatus"
.text:00B499C7 call    ds:__imp__SLGetWindowsInformationDWORD@8 ; load__SLGetWindowsInformationDWORD(x,x) ...
判断系统激活状态
hInf = SLGetWindowsInformationDWORD("Security-SPP-GenuineLocalStatus", LocalStatus)
    

.text:00B49A98 lea     eax, [esp+200h+lpMem]
.text:00B49A9C push    eax
.text:00B49A9D push    ecx
.text:00B49A9E push    ecx
.text:00B49A9F push    [esp+20Ch+var_1A8]
.text:00B49AA3 push    ecx
.text:00B49AA4 call    _SkuGetUpgradeProductKeyEx@28   ; SkuGetUpgradeProductKeyEx(x,x,x,x,x,x,x)  取密钥并加密，跟进
............
 loc_B8D0CB:                             ; CODE XREF: SkuGetUpgradeProductKeyEx(x,x,x,x,x,x,x)+1E↑j
.text:00B8D0CB lea     eax, [ebp+pdwValue]
.text:00B8D0CE push    eax                             ; pdwValue
.text:00B8D0CF push    offset aKernelProducti          ; "Kernel-ProductInfo"
.text:00B8D0D4 call    ds:__imp__SLGetWindowsInformationDWORD@8 ; load__SLGetWindowsInformationDWORD(x,x) ...
.text:00B8D0DA mov     ebx, eax
.text:00B8D0DC test    ebx, ebx
.text:00B8D0DE js      loc_B8D1CF
.text:00B8D0E4 call    ds:__imp__GetSystemDefaultUILanguage@0 ; GetSystemDefaultUILanguage()
.text:00B8D0EA movzx   eax, ax
.text:00B8D0ED mov     dword ptr [ebp+var_18], eax
.text:00B8D0F0 test    ax, ax
.text:00B8D0F3 jnz     short loc_B8D0FF
.text:00B8D0F5 mov     ebx, 80070490h
.text:00B8D0FA jmp     loc_B8D1CF
.text:00B8D0FF ; ---------------------------------------------------------------------------
.text:00B8D0FF
.text:00B8D0FF loc_B8D0FF:                             ; CODE XREF: SkuGetUpgradeProductKeyEx(x,x,x,x,x,x,x)+52↑j
.text:00B8D0FF lea     eax, [ebp+var_10]
.text:00B8D102 push    eax                             ; int
.text:00B8D103 lea     eax, [ebp+hMem]
.text:00B8D106 push    eax                             ; int
.text:00B8D107 push    ecx                             ; int
.text:00B8D108 call    ?GetValue@?$CRegUtilT@PAXUCRegType@@$0A@$00@@SGJPAUHKEY__@@PBG1PAPAXPAK@Z ; CRegUtilT<void *,CRegType,0,1>::GetValue(HKEY__ *,ushort const *,ushort const *,void * *,ulong *)
.text:00B8D10D test    eax, eax
.text:00B8D10F js      short loc_B8D149
.text:00B8D111 cmp     dword ptr [ebp+var_10], 4F8h
.text:00B8D118 jnz     short loc_B8D149
.text:00B8D11A mov     ecx, [ebp+hMem]
.text:00B8D11D lea     edx, [ebp+var_4]
.text:00B8D120 lea     ecx, [ecx+378h]
.text:00B8D126 call    ?GetActualPkpn@@YGJPBGPAPAG@Z   ; GetActualPkpn(ushort const *,ushort * *)
.text:00B8D12B mov     ebx, eax
.text:00B8D12D test    ebx, ebx
.text:00B8D12F jns     short loc_B8D140
.text:00B8D131 mov     ecx, ebx
.text:00B8D133 call    CheckToBreakOnFailure
.text:00B8D138 mov     edi, [ebp+var_4]
.text:00B8D13B jmp     loc_B8D1D6
.text:00B8D140 ; ---------------------------------------------------------------------------
.text:00B8D140
.text:00B8D140 loc_B8D140:                             ; CODE XREF: SkuGetUpgradeProductKeyEx(x,x,x,x,x,x,x)+8E↑j
.text:00B8D140 mov     edi, [ebp+var_4]
.text:00B8D143 mov     edx, edi
.text:00B8D145 test    edi, edi
.text:00B8D147 jnz     short loc_B8D14B
.text:00B8D149
.text:00B8D149 loc_B8D149:                             ; CODE XREF: SkuGetUpgradeProductKeyEx(x,x,x,x,x,x,x)+6E↑j
.text:00B8D149                                         ; SkuGetUpgradeProductKeyEx(x,x,x,x,x,x,x)+77↑j
.text:00B8D149 mov     edx, esi
.text:00B8D14B
.text:00B8D14B loc_B8D14B:                             ; CODE XREF: SkuGetUpgradeProductKeyEx(x,x,x,x,x,x,x)+A6↑j
.text:00B8D14B mov     ecx, [ebp+pdwValue]
.text:00B8D14E lea     eax, [ebp+var_10]
.text:00B8D151 push    eax                             ; unsigned __int16 *
.text:00B8D152 push    dword ptr [ebp+var_18]          ; __int16
.text:00B8D155 call    ?GetTargetEdition@@YGJKPBGGPAK@Z ; GetTargetEdition(ulong,ushort const *,ushort,ulong *)
.text:00B8D15A mov     ebx, eax
.text:00B8D15C test    ebx, ebx
.text:00B8D15E js      short loc_B8D1CF
.text:00B8D160 lea     edx, [ebp+var_4]
.text:00B8D163 call    ?GetChannelEnum@@YGJPBGPAW4_CHANNEL_ENUM@@@Z ; GetChannelEnum(ushort const *,_CHANNEL_ENUM *)
.text:00B8D168 mov     ebx, eax
.text:00B8D16A test    ebx, ebx
.text:00B8D16C js      short loc_B8D1CF
.text:00B8D16E mov     ecx, [ebp+arg_4]
.text:00B8D171 lea     edx, [ebp+var_18]
.text:00B8D174 call    ?GetPartnerEnum@@YGJPBGPAW4_PARTNER_ENUM@@@Z ; GetPartnerEnum(ushort const *,_PARTNER_ENUM *)
.text:00B8D179 mov     ebx, eax
.text:00B8D17B test    ebx, ebx
.text:00B8D17D js      short loc_B8D1CF
.text:00B8D17F mov     edx, [ebp+var_4]
.text:00B8D182 lea     eax, [ebp+var_4]
.text:00B8D185 mov     ecx, dword ptr [ebp+var_10]
.text:00B8D188 push    eax
.text:00B8D189 call    ?GetTargetChannel@@YGJKW4_CHANNEL_ENUM@@PAW41@@Z ; GetTargetChannel(ulong,_CHANNEL_ENUM,_CHANNEL_ENUM *)
.text:00B8D18E mov     ebx, eax
.text:00B8D190 test    ebx, ebx
.text:00B8D192 js      short loc_B8D1CF
.text:00B8D194 mov     edx, [ebp+var_4]
.text:00B8D197 lea     eax, [ebp+var_8]
.text:00B8D19A mov     ecx, dword ptr [ebp+var_10]
.text:00B8D19D push    eax
.text:00B8D19E lea     eax, [ebp+var_14]
.text:00B8D1A1 push    eax
.text:00B8D1A2 push    dword ptr [ebp+var_18]
.text:00B8D1A5 call    ?GetProductKeyForEdition@@YGJKW4_CHANNEL_ENUM@@W4_PARTNER_ENUM@@PAPAG2@Z ; GetProductKeyForEdition(ulong,_CHANNEL_ENUM,_PARTNER_ENUM,ushort * *,ushort * *)
.text:00B8D1AA mov     ebx, eax
.text:00B8D1AC test    ebx, ebx










根据gatherosstate原理用.net写的数字权利工具

![image](https://github.com/laomms/gatherosstate/blob/master/test.png)   

# 逆向分析:  

# gatherosstate 1803（17134）

Tool to collect the genuine state  
Gather Downlevel OS Activation State

命令行运行: 
```c
GatherOsState.exe /p <Partner name>  <output directory>
```
或
```c
GatherOsState.exe /c
```    
或直接运行.
    
```c
.text:00B497D8 push    offset aOsmajorversion          ; 'OSMajorVersion=%d;OSMinorVersion=%d;OSPlatformId=%d'
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
```
拼接后字符串:OSMajorVersion=10;OSMinorVersion=0;OSPlatformId=2;PP=0;

```c
.text:00B49831 push    ecx                             ; unsigned int *
.text:00B49832 push    ecx                             ; struct _HWID **
.text:00B49833 lea     eax, [esp+208h+var_198]
.text:00B49837 push    eax                             ; unsigned int
.text:00B49838 lea     eax, [esp+20Ch+var_1D8]
.text:00B4983C push    eax                             ; unsigned __int8 *
.text:00B4983D call    HwidGetCurrentEx                ;F7跟入
.text:00B49842 mov     esi, eax
.text:00B49844 test    esi, esi
void *__stdcall HwidGetCurrentEx(unsigned __int8 *key, int pid2, struct _HWID **struct_HWID, unsigned int *byte, int **a5, unsigned int *a6)

这个函数在系统自带的LicensingWinRT.dll中就有,但是是内部函数,调用方法我已经写在另一篇文章里.
比如我这里获取的总共64位的硬件数组是:
3E 00 00 00 00 00 01 00 02 00 02 00 01 00 04 00
00 00 06 00 01 00 01 00 68 BE FC 19 82 19 CC 2F
82 32 9A 18 76 2B 18 D6 B0 5A 17 8D 0E 8E 5F F1
A4 0E DE 2E 5C 50 9A 5C A4 60 D6 F1 10 EF 00 00

具体算法看另一项目：
https://github.com/laomms/HwidGetCurrentExApi

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
算法过程是：
取获取到的硬件数组,就是HwidGetCurrentEx返回的数组.
将数组大小加6,再加25,新建一个这么大的数组,我这里64位系统等于69
偏移3位插入高位13变成:69 00 00 00 13 00
再连接获取到的HwidGetCurrentEx数组,结尾加0C.
取之前创建的数组的30位连接到后面.
取这个全部组合后的数组的base64加密.

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
     
     
.text:00B4A2A0 lea     edx, [ebp+var_158]
.text:00B4A2A6 mov     ecx, offset aTimestampclien     ; "TimeStampClient"
.text:00B4A2AB call    Create
.text:00B4A2B0 mov     ebx, eax
.text:00B4A2B2 test    ebx, ebx
.text:00B4A2B4 js      loc_B4A13E
.text:00B4A2BA lea     edx, [ebp+var_154]
.text:00B4A2C0 lea     ecx, [ebp+SystemTime]
.text:00B4A2C6 call    UtcTimeToIso8601
拼接时间:20xx-07-25T14:56:25Z

.text:00B49A01 mov     ecx, [esp+208h+var_1D4]
.text:00B49A05 lea     edx, ds:2[eax*2]
.text:00B49A0C call    Base64Encod
取刚才拼接数组base64加密
得到base64字符串:TwBTAE0AYQBqAG8AcgBWAGUAcgBzAGkAbwBuAD0AMQAwADsATwBTAE0AaQBuAG8AcgBWAGUAcgBzAGkAbwBuAD0AMAA7AE8AUwBQ

.text:00B49A1F lea     eax, [esp+204h+pwszValueName]
.text:00B49A23 push    offset aSlGetGenuineAu          ; "SL_GET_GENUINE_AUTHZ:%s"
.text:00B49A28 push    eax                             ; unsigned __int16 **
.text:00B49A29 call    STRAPI_Format
.text:00B499BD lea     eax, [esp+200h+pdwValue]
.text:00B499C1 push    eax                             ; pdwValue
.text:00B499C2 push    offset pwszValueName            ; "Security-SPP-GenuineLocalStatus"
.text:00B499C7 call    ds:__imp__SLGetWindowsInformationDWORD@8 ; load__SLGetWindowsInformationDWORD(x,x) ...
拼接字符串:SL_GET_GENUINE_AUTHZ:TwBTAE0AYQBqAG8AcgBWAGUAcgBzAGkAbwBuAD0AMQAwADsATwBTAE0AaQBuAG8AcgBWAGUAcgBzAGk(取80位)
判断系统激活状态
hInf = SLGetWindowsInformationDWORD("Security-SPP-GenuineLocalStatus", LocalStatus)
    
.text:00B4A241 mov     ecx, offset aSessionid          ; "SessionId"
.text:00B4A246 call    Create
拼接SessionId：SessionId=TwBTAE0AYQBqAG8AcgBWAGUAcgBzAGkAbwBuAD0AMQAwADsATwBTAE0AaQBuAG8AcgBWAGUAcgBzAGkAbwBuAD0AMA          
                        
.text:0008ACBD push    edi                             ; lpUsedDefaultChar
.text:0008ACBE push    edi                             ; lpDefaultChar
.text:0008ACBF push    edi                             ; cbMultiByte
.text:0008ACC0 push    edi                             ; lpMultiByteStr
.text:0008ACC1 push    0FFFFFFFFh                      ; cchWideChar
.text:0008ACC3 push    eax                             ; lpWideCharStr
.text:0008ACC4 push    edi                             ; dwFlags
.text:0008ACC5 push    0FDE9h                          ; CodePage
.text:0008ACCA mov     [ebp+lpWideCharStr], eax
.text:0008ACCD mov     esi, edi
.text:0008ACCF call    ds:__imp__WideCharToMultiByte@32 ; WideCharToMultiByte(x,x,x,x,x,x,x,x)
将unicode宽字符串转成单字节字符串SessionId=TwBTAE0AYQBqAG8AcgBWAGUAcgBzAGkAbwBuAD0AMQAwADsATwBTAE0AaQBuAG8AcgBWAGUAcgBzAGkAbwBuAD0AMAA7AE8AUwBQAGwAYQB0AGYAbwByAG0ASQBkAD0AMgA7AFAAUAA9ADAAOwBIAHcAaQBkAD0AYQBRAEEAQQBBAEIATQBBAFAAZwBBAEEAQQBBAEEAQQBBAFEAQQBDAEEAQQ             

.text:0008A36C push    0F0000020h                      ; dwFlags
.text:0008A371 push    18h                             ; dwProvType
.text:0008A373 push    offset szProvider               ; "Microsoft Enhanced RSA and AES Cryptogr"...
.text:0008A378 push    ebx                             ; szContainer
.text:0008A379 lea     eax, [ebp+phProv]
.text:0008A37F push    eax                             ; phProv
.text:0008A380 call    ds:__imp__CryptAcquireContextW@20 ; CryptAcquireContextW(x,x,x,x,x)
调用ADVAPI32.dll中的CryptAcquireContextW函数

              
                        
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
.....
.text:00B49ADC push    [esp+200h+var_1C0]              ;根据密钥版本及SKU查询列表中的windows密钥版本 "Microsoft.Windows.48.X19-98841_8wekyb3d8bbwe"   
.text:00B49AE0 push    offset aPfnS                    ; "Pfn=%s;"

text:00B4A36C push    0F0000020h                      ; dwFlags
.text:00B4A371 push    18h                             ; dwProvType
.text:00B4A373 push    offset szProvider               ; "Microsoft Enhanced RSA and AES Cryptogr"...
.text:00B4A378 push    ebx                             ; szContainer
.text:00B4A379 lea     eax, [ebp+phProv]
.text:00B4A37F push    eax                             ; phProv
.text:00B4A380 call    ds:__imp__CryptAcquireContextW@20 ; CryptAcquireContextW(x,x,x,x,x)
 取字固定符串:microsoft enhanced rsa and aes cryptographic provider

text:00B4A3B7 push    ecx                             ; int
.text:00B4A3B8 push    eax                             ; int
.text:00B4A3B9 push    0                               ; BYTE *
.text:00B4A3BB push    [ebp+dwDataLen]                 ; dwDataLen
.text:00B4A3C1 lea     ecx, [ebp+phProv]
.text:00B4A3C7 push    esi                             ; BYTE *
.text:00B4A3C8 call    ?ComputeHash@?$CCryptoApiHelperT@VCMSRSAAESCryptoApiHelper@@@@QAEJPBXKPAXPAKI@Z ; CCryptoApiHelperT<CMSRSAAESCryptoApiHelper>::ComputeHash(void const *,ulong,void *,ulong *,uint)
.text:00B4A3CD mov     ebx, eax
加密字符串:SessionId=TwBTAE0AYQBqAG8AcgBWAGUAcgBzAGkAbwBuAD0AMQAwADsATwBTAE0AaQBuAG8AcgBWAGUAcgBzAGkAbwBuAD0AMAA7AE8AUwBQAGwAYQB0AGYAbwByAG0ASQBkAD0AMgA7AFAAUAA9ADAAOwBIAHcAaQBkAD0AYQBRAEEAQQBBAEIATQBBAFAAZwBBAEEAQQBBAEEAQQBBAFEAQQBDAEEAQQBJAEEAQQB
                        
.text:00B4A42B push    eax                             ; unsigned int
.text:00B4A42C lea     eax, [ebp+var_108]
.text:00B4A432 push    eax                             ; unsigned __int8 *
.text:00B4A433 push    [ebp+dwBytes]                   ; unsigned int
.text:00B4A439 mov     eax, [ebp+lpMem]
.text:00B4A43F push    eax                             ; int (__stdcall *)(void *, const void *)
.text:00B4A440 push    ecx                             ; const void *(__stdcall *)(unsigned int *, unsigned int *)
.text:00B4A441 call    ?VRSAVaultSignPKCS@@YGJP6GPBXPAK0@ZP6GHPAXPBX@ZIPBEKPAE0@Z ; VRSAVaultSignPKCS(void const * (*)(ulong *,ulong *),int (*)(void *,void const *),uint,uchar const *,ulong,uchar *,ulong *)
.text:00B4A446 mov     ebx, eax
RSA签名数组:5e 14 d6 fd 4e 02 23 fd d0 da 34 30 14 96 cd 69 c5 fb 7e f5 be ee 89 8a 86 13 92 d6 57 96 6c 02 11 b0 2f b5 15 14 cc e5 25 ec 34 be bd ea fd c6 c7 c5 3c 30 ab eb 62 5f 66 ac b5 d0 b6 31 63 cc 2f 88 ac e4 2a cd 6e 21 07 b9 67
                        
.text:00B4A45E lea     ecx, [ebp+var_108]
.text:00B4A464 call    Base64Encod  
BASE64加密: XhTW/U4CI/3Q2jQwFJbNacX7fvW+7omKhhOS1leWbAIRsC+1FRTM5SXsNL696v3Gx8U8MKvrYl9mrLXQtjFjzC+IrOQqzW4hB7ln
                        
.text:00B4A473 lea     edx, [ebp+var_1B8]
.text:00B4A479 mov     ecx, offset aDownlevelgtkey     ; "downlevelGTkey"
.text:00B4A47E call    Create
.text:00B4A483 mov     ebx, eax
.text:00B4A485 test    ebx, ebx
.text:00B4A487 js      loc_B4A13E
.text:00B4A48D lea     edx, [ebp+var_1B0]
.text:00B4A493 mov     ecx, offset aBgiaaackaabsu0     ; BgIAAACkAABSU0ExAAgAAAEAAQARq+V11k+dvHMCaLWVCaSbeQNlOdWTLkkl0hdMh5V3YhLU2R4h0Jd+7k7qfZ4aIo4ussduwGgm
固定公钥:.text:00029E80 aBgiaaackaabsu0:                        ; DATA XREF: CreateGenuineTicketClient+531↓o
                 text "UTF-16LE", 'BgIAAACkAABSU0ExAAgAAAEAAQARq+V11k+dvHMCaLWVCaSbeQN'
                 text "UTF-16LE", 'lOdWTLkkl0hdMh5V3YhLU2R4h0Jd+7k7qfZ4aIo4ussduwGgmyD'
                 text "UTF-16LE", 'Rikj5L2R77GG2ciHk4i8siK8qg7frOU0KT5rEks3qVj38C3dS1w'
                 text "UTF-16LE", 'S6D67shBFrxPlOEP8+JlelgP7Gxmwdao7NF4LXZ3+KdbJ//9jkm'
                 text "UTF-16LE", 'N8iAOP0N2XzW0/cJp9P1q6hE7eeqc/3Qn3zMr0q1Dx7vstN98oV'
                 text "UTF-16LE", '17hNYCwumOxxS1rH+3n7ap2JKRSelo8Jvi214jZLBL+hOtYaGpx'
                 text "UTF-16LE", 's7zIL3ofpoaYy5g7pc/DaTvyfpJho5634jK7dXVFMpzJZMn9w0F'
                 text "UTF-16LE", '/3rkquk0Amm',0
                        
.text:00B4A498 call    Create
.text:00B4A49D mov     ebx, eax
.text:00B4A49F test    ebx, ebx
.text:00B4A4A1 js      loc_B4A13E
.text:00B4A4A7 lea     eax, [ebp+var_1B8]
.text:00B4A4AD push    eax
.text:00B4A4AE push    [ebp+var_190]
.text:00B4A4B4 lea     ecx, [ebp+var_194]
.text:00B4A4BA call    ?Insert@?$CArray@UCGenuineSignature@@U1@VCAdaptorDefault@@VCPoliciesDefault@@@@QAEJHABUCGenuineSignature@@@Z ; CArray<CGenuineSignature,CGenuineSignature,CAdaptorDefault,CPoliciesDefault>::Insert(int,CGenuineSignature const &)
.text:00B4A4BF mov     ebx, eax
.text:00B4A4C1 test    ebx, ebx
.text:00B4A4C3 js      loc_B4A13E
.text:00B4A4C9 lea     eax, [ebp+var_19C]
.text:00B4A4CF push    eax
.text:00B4A4D0 push    0
.text:00B4A4D2 lea     ecx, [ebp+var_1A8]
.text:00B4A4D8 call    ?Insert@?$CArray@U?$CSignedGenuinePropertiesT@VCEmptyType@@@@U1@VCAdaptorDefault@@VCPoliciesDefault@@@@QAEJHABU?$CSignedGenuinePropertiesT@VCEmptyType@@@@@Z ; CArray<CSignedGenuinePropertiesT<CEmptyType>,CSignedGenuinePropertiesT<CEmptyType>,CAdaptorDefault,CPoliciesDefault>::Insert(int,CSignedGenuinePropertiesT<CEmptyType> const &)
.text:00B4A4DD mov     ebx, eax
.text:00B4A4DF test    ebx, ebx
.text:00B4A4E1 js      loc_B4A13E
.text:00B4A4E7 lea     eax, [ebp+var_160]
.text:00B4A4ED push    eax                             ; unsigned int *
.text:00B4A4EE lea     eax, [ebp+var_164]
.text:00B4A4F4 push    eax                             ; unsigned __int8 **
.text:00B4A4F5 lea     ecx, [ebp+var_1A8]              ; this
.text:00B4A4FB call    ?Serialize@CGenuineAuthorizationEnvelope@@QAEJPAPAEPAK@Z ; CGenuineAuthorizationEnvelope::Serialize(uchar * *,ulong *)
...
.text:00B4A691 mov     esi, [ebp+var_4]
.text:00B4A694 lea     eax, [ebp+var_14]
.text:00B4A697 push    esi
.text:00B4A698 push    offset aPropertiesSPro          ; "<properties>%s</properties>"
.text:00B4A69D push    eax
.text:00B4A69E call    GatherOsInformation
.text:00B4A6A3 mov     edi, eax
.text:00B4A6A5 add     esp, 0Ch
.text:00B4A6A8 test    edi, edi
.text:00B4A6AA js      loc_B4A7DB
.text:00B4A6B0 push    offset aSignatures_0            ; "<signatures>"
.text:00B4A6B5 lea     ecx, [ebp+var_14]
.text:00B4A6B8 call    Append
.text:00B4A6BD mov     edi, eax
.text:00B4A6BF test    edi, edi
.text:00B4A6C1 js      loc_B4A7DB
.text:00B4A6C7 xor     eax, eax
.text:00B4A6C9 mov     [ebp+var_8], eax
.text:00B4A6CC cmp     [ebx+0Ch], eax
.text:00B4A6CF jle     loc_B4A7A4
.text:00B4A6D5 mov     [ebp+var_4], eax
.text:00B4A6D8
.text:00B4A6D8 loc_B4A6D8:                             ; CODE XREF: Serialize+195↓j
.text:00B4A6D8 push    offset aSignature               ; "<signature"
.text:00B4A6DD lea     ecx, [ebp+var_14]
.text:00B4A6E0 call    Append
.text:00B4A6E5 mov     edi, eax
.text:00B4A6E7 test    edi, edi
.text:00B4A6E9 js      loc_B4A7DB
.text:00B4A6EF mov     eax, [ebx+10h]
.text:00B4A6F2 mov     ecx, [ebp+var_4]
.text:00B4A6F5 push    dword ptr [ecx+eax]
.text:00B4A6F8 lea     eax, [ebp+var_14]
.text:00B4A6FB push    offset aNameS                   ; " name=\"%s\""
.text:00B4A700 push    eax
.text:00B4A701 call    GatherOsInformation
.text:00B4A706 mov     edi, eax
.text:00B4A708 add     esp, 0Ch
.text:00B4A70B test    edi, edi
.text:00B4A70D js      loc_B4A7DB
.text:00B4A713 mov     ecx, [ebx+10h]
.text:00B4A716 mov     eax, [ebp+var_4]
.text:00B4A719 cmp     dword ptr [eax+ecx+4], 0
.text:00B4A71E jz      short loc_B4A726
.text:00B4A720 mov     eax, [eax+ecx+4]
.text:00B4A724 jmp     short loc_B4A72B
.text:00B4A726 ; ---------------------------------------------------------------------------
.text:00B4A726
.text:00B4A726 loc_B4A726:                             ; CODE XREF: Serialize+115↑j
.text:00B4A726 mov     eax, offset aRsaSha256          ; "rsa-sha256"
.text:00B4A72B
.text:00B4A72B loc_B4A72B:                             ; CODE XREF: Serialize+11B↑j
.text:00B4A72B push    eax
.text:00B4A72C lea     eax, [ebp+var_14]
.text:00B4A72F push    offset aMethodS                 ; " method=\"%s\""
.text:00B4A734 push    eax
.text:00B4A735 call    GatherOsInformation
.text:00B4A73A mov     edi, eax
.text:00B4A73C add     esp, 0Ch
.text:00B4A73F test    edi, edi
.text:00B4A741 js      loc_B4A7DB
.text:00B4A747 mov     ecx, [ebx+10h]
.text:00B4A74A mov     eax, [ebp+var_4]
.text:00B4A74D cmp     dword ptr [eax+ecx+8], 0
.text:00B4A752 jz      short loc_B4A76F
.text:00B4A754 push    dword ptr [eax+ecx+8]
.text:00B4A758 lea     eax, [ebp+var_14]
.text:00B4A75B push    offset aKeyS                    ; " key=\"%s\""
.text:00B4A760 push    eax
.text:00B4A761 call    GatherOsInformation
.text:00B4A766 mov     edi, eax
.text:00B4A768 add     esp, 0Ch
.text:00B4A76B test    edi, edi
.text:00B4A76D js      short loc_B4A7DB
.text:00B4A76F
.text:00B4A76F loc_B4A76F:                             ; CODE XREF: Serialize+149↑j
.text:00B4A76F mov     eax, [ebx+10h]
.text:00B4A772 mov     ecx, [ebp+var_4]
.text:00B4A775 push    dword ptr [ecx+eax+0Ch]
.text:00B4A779 lea     eax, [ebp+var_14]
.text:00B4A77C push    offset aSSignature              ; ">%s</signature>"
.text:00B4A781 push    eax
.text:00B4A782 call    GatherOsInformation
.text:00B4A787 mov     edi, eax
.text:00B4A789 add     esp, 0Ch
.text:00B4A78C test    edi, edi
.text:00B4A78E js      short loc_B4A7DB
.text:00B4A790 mov     eax, [ebp+var_8]
.text:00B4A793 add     [ebp+var_4], 10h
.text:00B4A797 inc     eax
.text:00B4A798 mov     [ebp+var_8], eax
.text:00B4A79B cmp     eax, [ebx+0Ch]
.text:00B4A79E jl      loc_B4A6D8
.text:00B4A7A4
.text:00B4A7A4 loc_B4A7A4:                             ; CODE XREF: Serialize+C6↑j
.text:00B4A7A4 push    offset aSignatures              ; "</signatures>"
.text:00B4A7A9 lea     ecx, [ebp+var_14]
.text:00B4A7AC call    Append
.text:00B4A7B1 mov     edi, eax
.text:00B4A7B3 test    edi, edi
.text:00B4A7B5 js      short loc_B4A7DB
.text:00B4A7B7 push    offset aGenuinepropert          ; "</genuineProperties>"
.text:00B4A7BC lea     ecx, [ebp+var_14]
.text:00B4A7BF call    Append
拼接门票文档过程
                        
.text:00B49B8A mov     ebx, offset aGenuineticketX     ; "GenuineTicket.xml"
.text:00B49B8F
.text:00B49B8F loc_B49B8F:                             ; CODE XREF: GatherOsInformation(ushort const *,ushort const *,ushort const *)+4E8↑j
.text:00B49B8F lea     eax, [esp+200h+lpFileName]
.text:00B49B93 mov     edx, ebx                        ; Src
.text:00B49B95 push    eax                             ; int
.text:00B49B96 push    ecx                             ; int
.text:00B49B97 mov     ecx, [esp+208h+Src]             ; Src
.text:00B49B9B call    CombinePath
.text:00B49BA0 mov     esi, eax
.text:00B49BA2 test    esi, esi
.text:00B49BA4 js      short loc_B49BC0
.text:00B49BA6 mov     edx, [esp+200h+pcbValue]
.text:00B49BAA push    [esp+200h+ppbValue]             ; lpBuffer
.text:00B49BAE mov     ecx, [esp+204h+lpFileName]      ; lpFileName
.text:00B49BB2 lea     edx, [edx-1]                    ; nNumberOfBytesToWrite
.text:00B49BB5 call    SaveBinaryAsFile
```
总结下流程：
```php
# GetVersionExW 获取系统版本信息 转成格式 OSMajorVersion=%d;OSMinorVersion=%d;OSPlatformId=%d
# HwidGetCurrentEx 获取系统硬件信息数组  
# HwidCreateBlock 通过算法和合并转成新的数组
# Base64Encode 将数组BASE64加密,取加密的结果连接成Hwid=%s格式,与刚才取到的系统信息拼接成拼接字符串:OSMajorVersion=%d;OSMinorVersion=%d;OSPlatformId=%d;PP=%d;Hwid=%s
# GetActiveWindowsSkuStatus 获取系统激活状态,这里可以patch掉让其返回0即可
# CompareSkuChannel 判断默认SKU是Retail还是GVLK
# SkuGetUpgradeProductKeyEx 获取默认PFN(package family name),组成Pfn=%s格式,并根据该PFN查表得到默认的密钥及ID(DPID3)值
# SLGetServiceInformation 是否为BiosProductKey
# SLGetWindowsInformationDWORD 判断当前系统的激活状态.字符串后面加"DownlevelGenuineState=1;")
# Base64Encode 将组合后的字符串用base64加密,将得到的加密结果连接字符串拼成格式:SL_GET_GENUINE_AUTHZ:%s
# SLGetGenuineInformation 根据SL_GET_GENUINE_AUTHZ:%s获取数据
# CreateGenuineTicketClient 如果不是有效数据执行创建数字文件操作
-GetSystemTime(&SystemTime)获取系统时间
-UtcTimeToIso8601组成TimeStampClient=系统时间格式
-拼接sessionid,sessionid=上面得到的即SL_GET_GENUINE_AUTHZ字符串中的的base64值
-WideCharToMultiByte 将sessionid=%s转成单字节字符串
-ComputeHash 利用CryptCreateHash、CryptHashData、CryptGetHashParam组成的RSA加密算法,生成32位数组
-VRSAVaultSignPKCS RSA签名数组算法,包括四个子函数，VbnRsaVault_GetPubParams、ApplyPKCS1SigningFormat、VbnRsaVault_ModExpPriv_clear、vMemrev. VbnRsaVault_GetPubParams是申请一个256大小的数组空间，ApplyPKCS1SigningFormat是逆序RSA数组然后连接一个固定数组,VbnRsaVault_ModExpPriv_clear里的VBN_MainLoop算法很变态,利用CFG调用600多个函数,将这个256字节的数组进行加密算法.最后生成256位数组.这几个算法跟clipUp.exe中的算法是一样的,Clipup会解析这个XML证书到安装目录并安装ClipSVC目录的token里.
-Base64Encode 加密签名数组成base64字符串
-将字符串"downlevelGTkey"和固定公钥key=BgIAAACkAABSU0ExAAgAAAEAAQARq+V11k+dvHMCaLWVCaSbeQNlOdWTLkkl0hdMh5V3YhLU2R4h0Jd+7k7qfZ4aIo4ussduwGgmyDRikj5L2R77GG2ciHk4i8siK8qg7frOU0KT5rEks3qVj38C3dS1wS6D67shBFrxPlOEP8+JlelgP7Gxmwdao7NF4LXZ3+KdbJ//9jkmN8iAOP0N2XzW0/cJp9P1q6hE7eeqc/3Qn3zMr0q1Dx7vstN98oV17hNYCwumOxxS1rH+3n7ap2JKRSelo8Jvi214jZLBL+hOtYaGpxs7zIL3ofpoaYy5g7pc/DaTvyfpJho5634jK7dXVFMpzJZMn9w0F/3rkquk0Amm"以及签名数组的base64字符串拼接成完整的签名字符串.
-将sppclient与<genuineproperties origin="%s">拼接成<genuineProperties origin="sppclient">
-把剩下的XML标签补全,<properties>%s</properties> <signatures>%s</signatures> <signature name="downlevelGTkey" method="rsa-sha256" key="%s" </signature>...
-CombinePath SaveBinaryAsFile 创建数字证书,供clipc安装到数字token

```

创建数字证书
```php
<?xml version="1.0" encoding="utf-8"?>
<genuineAuthorization
    xmlns="http://www.microsoft.com/DRM/SL/GenuineAuthorization/1.0">
    <version>1.0</version>
    <genuineProperties origin="sppclient">
        <properties>SessionId=TwBTAE0AYQBqAG8AcgBWAGUAcgBzAGkAbwBuAD0AMQAwADsATwBTAE0AaQBuAG8AcgBWAGUAcgBzAGkAbwBuAD0AMAA7AE8AUwBQAGwAYQB0AGYAbwByAG0ASQBkAD0AMgA7AFAAUAA9ADAAOwBIAHcAaQBkAD0AYQBRAEEAQQBBAEIATQBBAFAAZwBBAEEAQQBBAEEAQQBBAFEAQQBDAEEAQQBJAEEAQQBRAEEARQBBAEEAQQBBAEIAZwBBAEIAQQBBAEUAQQBhAEwANwA4AEcAWQBJAFoAegBDACsAQwBNAHAAbwBZAGQAaQBzAFkAMQByAEIAYQBGADQAMABPAGoAbAAvAHgAcABBADcAZQBMAGwAeABRAG0AbAB5AGsAWQBOAGIAeABFAE8AOABNAEEAQQBJAEEAQQBRAEUAQQBBAGcAVQBBAEEAdwBFAEEAQgBBAEkAQQBCAGcARQBBAEMAQQBjAEEAQwBRAE0AQQBDAGcARQBBAEQAQQBjAEEAQQBBAEEAQQBBAEEAQQBBADsAUABmAG4APQBkAGMAYQAxAC0AOAA1AGEANwAtADcANQA1ADkANwA0ADkAMQBhADEAOABiADsARABvAHcAbgBsAGUAdgBlAGwARwBlAG4AdQBpAG4AZQBTAHQAYQB0AGUAPQAxADsAAAA=;TimeStampClient=2019-07-25T14:56:25Z</properties>
        <signatures>
            <signature name="downlevelGTkey" method="rsa-sha256" key="BgIAAACkAABSU0ExAAgAAAEAAQARq+V11k+dvHMCaLWVCaSbeQNlOdWTLkkl0hdMh5V3YhLU2R4h0Jd+7k7qfZ4aIo4ussduwGgmyDRikj5L2R77GG2ciHk4i8siK8qg7frOU0KT5rEks3qVj38C3dS1wS6D67shBFrxPlOEP8+JlelgP7Gxmwdao7NF4LXZ3+KdbJ//9jkmN8iAOP0N2XzW0/cJp9P1q6hE7eeqc/3Qn3zMr0q1Dx7vstN98oV17hNYCwumOxxS1rH+3n7ap2JKRSelo8Jvi214jZLBL+hOtYaGpxs7zIL3ofpoaYy5g7pc/DaTvyfpJho5634jK7dXVFMpzJZMn9w0F/3rkquk0Amm">nZDtNodj1g7NtmZrZeW/sKuL+lMw4ujn5gg2FOnrwO0Q8esItL/uWSWuDpqsL8l2wyiA92VUVNqqt9wcUegncRWRvN/yiUgN9IgipnOKuKnF7ku2677ZpzO9rn2XSCb8gGR7CXLZhEPSNvsrD231H3OMfhTuKSs9D9+fCMWayzbF3fqdZUNsDSSWWoPGPDochGN2p/2UXcY14hXeuuzIjEMslsT4YobmQEEWa13IYbu1NyJh9fGg9hY9V5dmPI3Iy4VYn13vZPFBcLy0SWSFz7DPBxzARl0VjlDewiQbTmsvC6abIBNYMfoL41a4f8gMNcw/rCgy+cB0KTJIcQiZ4A==</signature>
        </signatures>
    </genuineProperties>
</genuineAuthorization>
```

include windows.inc

IFNDEF      _RTL_CRITICAL_SECTION
_RTL_CRITICAL_SECTION struc ; (sizeof=0x18, align=0x4, copyof_15)
                                        ; XREF: .data:g_encryptionLock/r
DebugInfo       dd ?                    ; offset
LockCount       dd ?
RecursionCount  dd ?
OwningThread    dd ?                    ; offset
LockSemaphore   dd ?                    ; offset
SpinCount       dd ?
_RTL_CRITICAL_SECTION EndS
ENDIF

externdef ___guard_check_icall_fptr:dword
externdef off_408A5C:dword
externdef word_4B2380:word
externdef dword_4B2384:dword
externdef dword_4B2388:dword
externdef dword_4B238C:dword
externdef dword_4B2390:dword
externdef dword_4B2394:dword
externdef dword_4B2398:dword
externdef word_4B23B0:word
externdef word_4B23A4:word
externdef dword_4B23A8:dword
externdef dword_4B23AC:dword
externdef word_4B237C:word
externdef word_4B2378:word
externdef word_4B2374:word
externdef dword_4B239C:dword
externdef dword_4B23A0:dword
externdef dword_4B22E8:dword
externdef dword_4B22A4:dword
externdef dword_4B2298:dword
externdef dword_4B229C:dword
externdef dword_4B22A8:dword
externdef dword_4B22AC:dword
externdef dword_40BBFC:dword
externdef aBt:byte
externdef dword_40BC50:dword
externdef dword_40A550:dword
externdef dword_40A500:dword
externdef off_40BB5C:dword
externdef dword_40BC90:dword
externdef dword_468B2C:dword
externdef dword_468B14:dword
externdef a43c111821dfa8d:byte
externdef ___defaultmatherr:dword
externdef ___security_cookie:dword
externdef ___security_cookie_complement:dword
externdef funcs_4A3C99:dword
externdef hInst:HINSTANCE
externdef dword_40BC5C:dword
externdef dword_40BC30:dword
externdef dword_40BC20:dword
externdef dword_40A98C:dword
externdef dword_40A980:dword
externdef dword_40A9B0:dword
externdef dword_40BCA8:dword
externdef dword_40BCC0:dword
externdef dword_40BC08:dword
externdef dword_40A510:dword
externdef dword_40BC40:dword
externdef dword_40A540:dword
externdef dword_40A9A0:dword
public dword_40A568
public dword_40A580
public dword_40A590
public dword_40A59C
public dword_40A5A8
public dword_40A5B4
public dword_40A5C0
public dword_40A5D0
public dword_40A5E0
public dword_40A5E8
public dword_40A600
public dword_40A610
public dword_40A61C
public dword_40A628
public dword_40A640
public dword_40A650
public dword_40A660
public dword_40A670
public dword_40A680
public dword_40A690
public dword_40A6A0
public dword_40A6B0
public dword_40A6C0
public dword_40A6D0
public dword_40A6E0
public dword_40A6EC
public dword_40A700
public dword_40A710
public dword_40A71C
public dword_40C0D0
public dword_40C0E0
public dword_40C0F0
public dword_40C0FC
public dword_40C110
public dword_40C120
public dword_40C130
public dword_40C140
public dword_40C14C
public dword_40C158
public dword_40C164
public dword_40C170
public dword_40C17C
public dword_40C188
public dword_40C194
public dword_40C1A0
public dword_40C1B0
public dword_40C1C0
public dword_40C1D0
public dword_40C1E0
public dword_40C1F0
public dword_40C200
public dword_40C20C
public dword_40C220
public dword_40C230
public dword_40C23C
public dword_40C250
public dword_40C260
public dword_40C270
public dword_40C280
public dword_40C290
public dword_40C2A0
public dword_40C2B0
public dword_40C2C0
public dword_40C2D0
public dword_40C2E0
public dword_40C2F0
public dword_40C300
public dword_40C310
public dword_40C320
public dword_40C32C
public dword_40C338
public dword_40C344
public dword_40C350
public dword_40C360
public dword_40C370
public dword_40C380
public dword_40C38C
public dword_40C398
public dword_40C3A4
public dword_40C3AC
public dword_40C3B8
public dword_40C3C4
public dword_40C3D0
public dword_40C3E0
public dword_40C3F0
public dword_40C400
public dword_40C410
public dword_40C41C
public dword_40C430
public dword_40C440
public dword_40C44C
public dword_40C460
public dword_40C470
public dword_40C480
public dword_40C490
public dword_40C4A0
public dword_40C4B0
public dword_40C4BC
public dword_40C4D0
public dword_40C4E0
public dword_40C4F0
public dword_40C500
public dword_40C510
public dword_40C520
public dword_40C528
public dword_40C540
public dword_40C550
public dword_40C560
public dword_40C570
public dword_40C580
public dword_40C590
public dword_40C5A0
public dword_40C5B0
public dword_40C5C0
public dword_40C5D0
public dword_40C5E0
public dword_40C5F0
public dword_40C600
public dword_40C610
public dword_40C620
public dword_40C630
public dword_40C640
public dword_40C64C
public dword_40C660
public dword_40C670
public dword_40C680
public dword_40C68C
public dword_40C698
public dword_40C6B0
public dword_40C6C0
public dword_40C6D0
public dword_40C6E0
public dword_40C6F0
public dword_40C700
public dword_40C710
public dword_40C720
public dword_40C730
public dword_40C73C
public dword_40C750
public dword_40C760
public dword_40C770
public dword_40C780
public dword_40C78C
public dword_40C7A0
public dword_40C7B0
public dword_40C7BC
public dword_40C7D0
public dword_40C7E0
public dword_40C7EC
public dword_40C7F8
public dword_40C804
public dword_40C810
public dword_40C820
public dword_40C830
public dword_40C840
public dword_40C850
public dword_40C85C
public dword_40C870
public dword_40C880
public dword_40C890
public dword_40C8A0
public dword_40C8B0
public dword_40C8C0
public dword_40A2C0
public dword_40A2D0
public dword_40A2E0
public dword_40A2EC
public dword_40A2F8
public dword_40A310
public dword_40A320
public dword_40A32C
public dword_40A340
public dword_40A360
public dword_40A370
public dword_40A380
public dword_40A390
public dword_40A39C
public dword_40A3B0
public dword_40A3C0
public dword_40A3CC
public dword_40A3E0
public dword_40A3F0
public dword_40A400
public dword_40A410
public dword_40A420
public dword_40A430
public dword_40A43C
public dword_40A448
public dword_40A450
public dword_40A460
public dword_40A470
public dword_40A480
public dword_40A490
public dword_40A49C
public dword_40A4B0
public dword_40A4C0
public dword_40A4D0
public dword_40A4E0
public dword_40C8E0
public dword_40C900
public dword_40C910
public dword_40C920
public dword_40C930
public dword_40C940
public dword_40C950
public dword_40C960
public dword_40C96C
public dword_40C974
public dword_40C980
public dword_40C98C
public dword_40C9A0
public dword_40C9B0
public dword_40C9C0
public dword_40C9D0
public dword_40C9E0
public dword_40C9E8
public dword_40C9F4
public dword_40CA00
public dword_40CA10
public dword_40CA20
public dword_40CA30
public dword_40CA40
public dword_40A9E0
public dword_40A9E8
public dword_40A9F4
public dword_40AA00
public dword_40AA10
public dword_40AA20
public dword_40AA30
public dword_40AA40
public dword_40AA50
public dword_40AA5C
public dword_40AA68
public dword_40AA80
public dword_40AA90
public dword_40AAA0
public dword_40AAB0
public dword_40AAC0
public dword_40AAD0
public dword_40AAE0
public dword_40AAE8
public dword_40AAF0
public dword_40AB00
public dword_40AB08
public dword_40AB20
public dword_40AB30
public dword_40AB3C
public dword_40AB50
public dword_40AB60
public dword_40AB70
public dword_40AB80
public dword_40AB90
public dword_40ABA0
public dword_40ABB0
public dword_40ABC0
public dword_40ABD0
public dword_40ABE4
public dword_40ABF0
public dword_40AC00
public dword_40AC10
public dword_40AC1C
public dword_40AC28
public dword_40AC40
public dword_40AC50
public dword_40AC60
public dword_40AC70
public dword_40AC80
public dword_40AC90
public dword_40AC9C
public dword_40ACB0
public dword_40ACC0
public dword_40ACD0
public dword_40ACDC
public dword_40ACE8
public dword_40ACFC
public dword_40AD00
public dword_40AD10
public dword_40AD20
public dword_40AD30
public dword_40AD40
public dword_40AD48
public dword_40AD54
public dword_40AD60
public dword_40AD6C
public dword_40AD80
public dword_40AD90
public dword_40ADA0
public dword_40ADB0
public dword_40ADC0
public dword_40ADD0
public dword_40B9F0
public dword_40B9FC
public dword_40BA08
public dword_40BA10
public dword_40BA1C
public dword_40BA30
public dword_40BA40
public dword_40BA4C
public dword_40BA60
public dword_40BA70
public dword_40BA80
public dword_40BA90
public dword_40BAA0
public dword_40BAB0
public dword_40BAC0
public dword_40BACC
public dword_40BAD8
public dword_40BAE4
public dword_40BAF0
public dword_40BAFC
public dword_40BB10
public dword_40BB20
public dword_40BB30
public dword_40BB40
public dword_40BB50
public dword_40BB70
public dword_40BB80
public dword_40BB8C
public dword_40BBA0
public dword_40BBB0
public dword_40BBC0
public dword_40BBD0
public dword_40BBE0
public dword_40A7B0
public dword_40A7C0
public dword_40A7D0
public dword_40A7E0
public dword_40A7EC
public dword_40A800
public dword_40A810
public dword_40A81C
public dword_40A830
public dword_40A840
public dword_40A850
public dword_40A860
public dword_40A86C
public dword_40A878
public dword_40A884
public dword_40A88C
public dword_40A898
public dword_40A8A4
public dword_40A8B0
public dword_40A8C0
public dword_40A8D0
public dword_40A8E0
public dword_40A8F0
public dword_40A8FC
public dword_40A910
public dword_40A920
public dword_40A92C
public dword_40A940
public dword_40A950
public dword_40A958
public dword_40A960
public dword_40BD80
public dword_40A740
public dword_40A74C
public dword_40A758
public dword_40A770
public dword_40A780
public dword_40A78C
public dword_40BCDC
public dword_40BCE8
public dword_40BCF4
public dword_40BD00
public dword_40BD0C
public dword_40BD20
public dword_40BD30
public dword_40BD3C
public dword_40BD50
public dword_40BD60
public dword_40BD70
public dword_40BD90
public dword_40BDA0
public dword_40BDAC
public dword_40BDB8
public dword_40BDD0
public dword_40BDE0
public dword_40BDEC
public dword_40BDF4
public dword_40BE00
public dword_40BE0C
public dword_40BE14
public dword_40BE20
public dword_40BE30
public dword_40BE3C
public dword_40BE50
public dword_40BE60
public dword_40BE6C
public dword_40BE80
public dword_40BE90
public dword_40BE9C
public dword_40BEB0
public dword_40BEC0
public dword_40BECC
public dword_40BED8
public dword_40BEE4
public dword_40BEF0
public dword_40BF00
public dword_40BF10
public dword_40BF20
public dword_40BF30
public dword_40BF40
public dword_40BF50
public dword_40BF60
public dword_40BF70
public dword_40BF84
public dword_40BF90
public dword_40BFA0
public dword_40BFB0
public dword_40BFC0
public dword_40BFCC
public dword_40BFD8
public dword_40BFF0
public dword_40C000
public dword_40C010
public dword_40C018
public dword_40C030
public dword_40C040
public dword_40C050
public dword_40C060
public dword_40C070
public dword_40C07C
public dword_40C088
public dword_40C094
public dword_40C0A0
public dword_40C0AC
public dword_40A530
public dword_40A970
public dword_40A520
___raise_securityfailure Proto NEAR
___report_gsfailure Proto NEAR
__alloca_probe_16 Proto NEAR
__aullshr Proto NEAR
__chkstk Proto NEAR
_VbnRsaVault_GetPubParams Proto NEAR
_VbnRsaVault_ModExpPriv_clear Proto NEAR
ApplyPKCS1SigningFormat Proto NEAR
DllEntryPoint Proto hInstance:HINSTANCE, dwReason:DWord, lpvReserved:LPVOID
guard_check_icall_nop Proto NEAR
security_check_cookie Proto NEAR
sub_40A4F0 Proto FAR
sub_40BBF0 Proto NEAR
sub_40C0B4 Proto FAR
VBN_MainLoop Proto NEAR
VBN_TransformContext Proto NEAR
VBNCB_unnamed_80001ef8 Proto NEAR
VBNCB_unnamed_80001f27 Proto NEAR
VBNCB_unnamed_80001f31 Proto NEAR
VBNCB_unnamed_80001f46 Proto NEAR
VBNCB_unnamed_80001f5d Proto NEAR
VBNCB_unnamed_80001f76 Proto NEAR
VBNCB_unnamed_80001f87 Proto NEAR
VBNCB_unnamed_80001f9b Proto NEAR
VBNCB_unnamed_80001fa1 Proto NEAR
VBNCB_unnamed_80001fc9 Proto NEAR
VBNCB_unnamed_80001fd7 Proto NEAR
VBNCB_unnamed_80002004 Proto NEAR
VBNCB_unnamed_8000201d Proto NEAR
VBNCB_unnamed_800020a0 Proto NEAR
VBNCB_unnamed_800020b1 Proto NEAR
VBNCB_unnamed_800020b7 Proto NEAR
VBNCB_unnamed_800020dc Proto NEAR
VBNCB_unnamed_8000210e Proto NEAR
VBNCB_unnamed_80002114 Proto NEAR
VBNCB_unnamed_8000214f Proto NEAR
VBNCB_unnamed_80002177 Proto NEAR
VBNCB_unnamed_8000218e Proto NEAR
VBNCB_unnamed_800021a1 Proto NEAR
VBNCB_unnamed_800021dd Proto NEAR
VBNCB_unnamed_80002220 Proto NEAR
VBNCB_unnamed_80002237 Proto NEAR
VBNCB_unnamed_80002250 Proto NEAR
VBNCB_unnamed_80002263 Proto NEAR
VBNCB_unnamed_80002269 Proto NEAR
VBNCB_unnamed_80002296 Proto NEAR
VBNCB_unnamed_800022c1 Proto NEAR
VBNCB_unnamed_800022cf Proto NEAR
VBNCB_unnamed_800022e9 Proto NEAR
VBNCB_unnamed_80002319 Proto NEAR
VBNCB_unnamed_80002327 Proto NEAR
VBNCB_unnamed_8000233e Proto NEAR
VBNCB_unnamed_80002344 Proto NEAR
VBNCB_unnamed_8000235d Proto NEAR
VBNCB_unnamed_8000236f Proto NEAR
VBNCB_unnamed_8000237c Proto NEAR
VBNCB_unnamed_80002393 Proto NEAR
VBNCB_unnamed_800023ac Proto NEAR
VBNCB_unnamed_800023bf Proto NEAR
VBNCB_unnamed_800023f8 Proto NEAR
VBNCB_unnamed_800023fa Proto NEAR
VBNCB_unnamed_80002408 Proto NEAR
VBNCB_unnamed_8000240a Proto NEAR
VBNCB_unnamed_8000241d Proto NEAR
VBNCB_unnamed_80002a6f Proto NEAR
vBng___10_stk Proto NEAR
vBng___100 Proto NEAR
vBng___100_stk Proto NEAR
vBng___101_stk Proto NEAR
vBng___102 Proto NEAR
vBng___103_stk Proto NEAR
vBng___104 Proto NEAR
vBng___105 Proto NEAR
vBng___105_stk Proto NEAR
vBng___108_stk Proto NEAR
vBng___112_stk Proto NEAR
vBng___113 Proto NEAR
vBng___113_stk Proto NEAR
vBng___119_stk Proto NEAR
vBng___12_stk Proto NEAR
vBng___120 Proto NEAR
vBng___123 Proto NEAR
vBng___126_stk Proto NEAR
vBng___127 Proto NEAR
vBng___128 Proto NEAR
vBng___13 Proto NEAR
vBng___133 Proto NEAR
vBng___133_stk Proto NEAR
vBng___136_stk Proto NEAR
vBng___137 Proto NEAR
vBng___138 Proto NEAR
vBng___139_stk Proto NEAR
vBng___140_stk Proto NEAR
vBng___142 Proto NEAR
vBng___143 Proto NEAR
vBng___148 Proto NEAR
vBng___15 Proto NEAR
vBng___15_stk Proto NEAR
vBng___150_stk Proto NEAR
vBng___151_stk Proto NEAR
vBng___153 Proto NEAR
vBng___154 Proto NEAR
vBng___154_stk Proto NEAR
vBng___156_stk Proto NEAR
vBng___157 Proto NEAR
vBng___158_stk Proto NEAR
vBng___16 Proto NEAR
vBng___16_stk Proto NEAR
vBng___160 Proto NEAR
vBng___160_stk Proto NEAR
vBng___161 Proto NEAR
vBng___161_stk Proto NEAR
vBng___164 Proto NEAR
vBng___164_stk Proto NEAR
vBng___166 Proto NEAR
vBng___167 Proto NEAR
vBng___169 Proto NEAR
vBng___169_stk Proto NEAR
vBng___170 Proto NEAR
vBng___171_stk Proto NEAR
vBng___173 Proto NEAR
vBng___177_stk Proto NEAR
vBng___179 Proto NEAR
vBng___18 Proto NEAR
vBng___180_stk Proto NEAR
vBng___181_stk Proto NEAR
vBng___182 Proto NEAR
vBng___183_stk Proto NEAR
vBng___185_stk Proto NEAR
vBng___187 Proto NEAR
vBng___188_stk Proto NEAR
vBng___189_stk Proto NEAR
vBng___19_stk Proto NEAR
vBng___192 Proto NEAR
vBng___194_stk Proto NEAR
vBng___197_stk Proto NEAR
vBng___198 Proto NEAR
vBng___2_stk Proto NEAR
vBng___20_stk Proto NEAR
vBng___201_stk Proto NEAR
vBng___203 Proto NEAR
vBng___27_stk Proto NEAR
vBng___28 Proto NEAR
vBng___28_stk Proto NEAR
vBng___29 Proto NEAR
vBng___31 Proto NEAR
vBng___31_stk Proto NEAR
vBng___33 Proto NEAR
vBng___35 Proto NEAR
vBng___36_stk Proto NEAR
vBng___37 Proto NEAR
vBng___4 Proto NEAR
vBng___40 Proto NEAR
vBng___42_stk Proto NEAR
vBng___43 Proto NEAR
vBng___43_stk Proto NEAR
vBng___45 Proto NEAR
vBng___46_stk Proto NEAR
vBng___48_stk Proto NEAR
vBng___49 Proto NEAR
vBng___51_stk Proto NEAR
vBng___52_stk Proto NEAR
vBng___54 Proto NEAR
vBng___54_stk Proto NEAR
vBng___56 Proto NEAR
vBng___57 Proto NEAR
vBng___57_stk Proto NEAR
vBng___59 Proto NEAR
vBng___6 Proto NEAR
vBng___6_stk Proto NEAR
vBng___61_stk Proto NEAR
vBng___62 Proto NEAR
vBng___63 Proto NEAR
vBng___63_stk Proto NEAR
vBng___65 Proto NEAR
vBng___66 Proto NEAR
vBng___68 Proto NEAR
vBng___68_stk Proto NEAR
vBng___69_stk Proto NEAR
vBng___72 Proto NEAR
vBng___73 Proto NEAR
vBng___73_stk Proto NEAR
vBng___74_stk Proto NEAR
vBng___75 Proto NEAR
vBng___75_stk Proto NEAR
vBng___77_stk Proto NEAR
vBng___80 Proto NEAR
vBng___84 Proto NEAR
vBng___86 Proto NEAR
vBng___87_stk Proto NEAR
vBng___88 Proto NEAR
vBng___89_stk Proto NEAR
vBng___90 Proto NEAR
vBng___93 Proto NEAR
vBng___95 Proto NEAR
vBng___97 Proto NEAR
vBng___98 Proto NEAR
vBng___98_stk Proto NEAR
vBng___stk Proto NEAR
vBng_1__1_stk Proto NEAR
vBng_1__2 Proto NEAR
vBng_1__5 Proto NEAR
vBng_1__6 Proto NEAR
vBng_2 Proto NEAR
vBng_h__13 Proto NEAR
vBng_h__14 Proto NEAR
vBng_h__18 Proto NEAR
vBng_h__2 Proto NEAR
vBng_h__21 Proto NEAR
vBng_h__23 Proto NEAR
vBng_h__3_stk Proto NEAR
vBng_h__30 Proto NEAR
vBng_h__34 Proto NEAR
vBng_h__35 Proto NEAR
vBng_h__36 Proto NEAR
vBng_h__37 Proto NEAR
vBng_h__38 Proto NEAR
vBng_h__5 Proto NEAR
vBng_h__8 Proto NEAR
vBng_h__9 Proto NEAR
vBng_stk Proto NEAR
vMemrev Proto NEAR
VRSAVaultSignPKCS Proto a1:DWord, a2:DWord, arg_4:DWord, arg_8:DWord, Dsts:DWord, a6:DWord, a7:Byte, a8:_RTL_CRITICAL_SECTION
__allmul Proto NEAR
sub_40A730 Proto NEAR
sub_40A9D0 Proto NEAR
sub_40BC70 Proto FAR
sub_40BC9C Proto NEAR
VBNCB_unnamed_8000106b Proto NEAR
VBNCB_unnamed_80001ec8 Proto NEAR
VBNCB_unnamed_80001ee5 Proto NEAR
VBNCB_unnamed_80001efe Proto NEAR
VBNCB_unnamed_80001f21 Proto NEAR
VBNCB_unnamed_80001f37 Proto NEAR
VBNCB_unnamed_80001f4c Proto NEAR
VBNCB_unnamed_80001f70 Proto NEAR
VBNCB_unnamed_80001fdd Proto NEAR
VBNCB_unnamed_80001ff1 Proto NEAR
VBNCB_unnamed_8000200a Proto NEAR
VBNCB_unnamed_80002023 Proto NEAR
VBNCB_unnamed_80002030 Proto NEAR
VBNCB_unnamed_80002036 Proto NEAR
VBNCB_unnamed_80002047 Proto NEAR
VBNCB_unnamed_80002060 Proto NEAR
VBNCB_unnamed_8000207a Proto NEAR
VBNCB_unnamed_80002087 Proto NEAR
VBNCB_unnamed_8000208d Proto NEAR
VBNCB_unnamed_800020e2 Proto NEAR
VBNCB_unnamed_800020f5 Proto NEAR
VBNCB_unnamed_800020fb Proto NEAR
VBNCB_unnamed_80002122 Proto NEAR
VBNCB_unnamed_80002128 Proto NEAR
VBNCB_unnamed_80002155 Proto NEAR
VBNCB_unnamed_80002169 Proto NEAR
VBNCB_unnamed_800021a7 Proto NEAR
VBNCB_unnamed_800021bb Proto NEAR
VBNCB_unnamed_800021cf Proto NEAR
VBNCB_unnamed_800021f4 Proto NEAR
VBNCB_unnamed_800021fa Proto NEAR
VBNCB_unnamed_80002213 Proto NEAR
VBNCB_unnamed_80002226 Proto NEAR
VBNCB_unnamed_8000223d Proto NEAR
VBNCB_unnamed_8000227c Proto NEAR
VBNCB_unnamed_80002282 Proto NEAR
VBNCB_unnamed_80002290 Proto NEAR
VBNCB_unnamed_800022a7 Proto NEAR
VBNCB_unnamed_800022ad Proto NEAR
VBNCB_unnamed_800022bb Proto NEAR
VBNCB_unnamed_800022d5 Proto NEAR
VBNCB_unnamed_800022e3 Proto NEAR
VBNCB_unnamed_8000232d Proto NEAR
VBNCB_unnamed_80002357 Proto NEAR
VBNCB_unnamed_80002369 Proto NEAR
VBNCB_unnamed_80002382 Proto NEAR
VBNCB_unnamed_80002399 Proto NEAR
VBNCB_unnamed_800023bd Proto NEAR
VBNCB_unnamed_800023d0 Proto NEAR
VBNCB_unnamed_800023d2 Proto NEAR
VBNCB_unnamed_8000241f Proto NEAR
vBng__ Proto NEAR
vBng___1 Proto NEAR
vBng___101 Proto NEAR
vBng___102_stk Proto NEAR
vBng___104_stk Proto NEAR
vBng___106 Proto NEAR
vBng___106_stk Proto NEAR
vBng___107 Proto NEAR
vBng___107_stk Proto NEAR
vBng___109_stk Proto NEAR
vBng___11 Proto NEAR
vBng___11_stk Proto NEAR
vBng___110 Proto NEAR
vBng___110_stk Proto NEAR
vBng___111 Proto NEAR
vBng___114 Proto NEAR
vBng___115_stk Proto NEAR
vBng___116 Proto NEAR
vBng___118_stk Proto NEAR
vBng___12 Proto NEAR
vBng___120_stk Proto NEAR
vBng___121_stk Proto NEAR
vBng___122 Proto NEAR
vBng___122_stk Proto NEAR
vBng___123_stk Proto NEAR
vBng___124_stk Proto NEAR
vBng___125 Proto NEAR
vBng___125_stk Proto NEAR
vBng___127_stk Proto NEAR
vBng___129 Proto NEAR
vBng___13_stk Proto NEAR
vBng___132_stk Proto NEAR
vBng___134 Proto NEAR
vBng___134_stk Proto NEAR
vBng___137_stk Proto NEAR
vBng___138_stk Proto NEAR
vBng___139 Proto NEAR
vBng___14 Proto NEAR
vBng___14_stk Proto NEAR
vBng___141 Proto NEAR
vBng___141_stk Proto NEAR
vBng___142_stk Proto NEAR
vBng___144_stk Proto NEAR
vBng___145_stk Proto NEAR
vBng___146_stk Proto NEAR
vBng___147_stk Proto NEAR
vBng___148_stk Proto NEAR
vBng___149 Proto NEAR
vBng___150 Proto NEAR
vBng___152_stk Proto NEAR
vBng___157_stk Proto NEAR
vBng___158 Proto NEAR
vBng___159 Proto NEAR
vBng___159_stk Proto NEAR
vBng___163 Proto NEAR
vBng___163_stk Proto NEAR
vBng___165 Proto NEAR
vBng___166_stk Proto NEAR
vBng___167_stk Proto NEAR
vBng___168 Proto NEAR
vBng___17 Proto NEAR
vBng___170_stk Proto NEAR
vBng___173_stk Proto NEAR
vBng___175 Proto NEAR
vBng___175_stk Proto NEAR
vBng___176 Proto NEAR
vBng___178_stk Proto NEAR
vBng___179_stk Proto NEAR
vBng___181 Proto NEAR
vBng___185 Proto NEAR
vBng___186_stk Proto NEAR
vBng___189 Proto NEAR
vBng___19 Proto NEAR
vBng___190 Proto NEAR
vBng___191 Proto NEAR
vBng___191_stk Proto NEAR
vBng___192_stk Proto NEAR
vBng___193 Proto NEAR
vBng___193_stk Proto NEAR
vBng___194 Proto NEAR
vBng___195_stk Proto NEAR
vBng___196_stk Proto NEAR
vBng___198_stk Proto NEAR
vBng___199 Proto NEAR
vBng___199_stk Proto NEAR
vBng___200_stk Proto NEAR
vBng___201 Proto NEAR
vBng___204 Proto NEAR
vBng___21 Proto NEAR
vBng___21_stk Proto NEAR
vBng___22 Proto NEAR
vBng___23 Proto NEAR
vBng___24 Proto NEAR
vBng___24_stk Proto NEAR
vBng___25 Proto NEAR
vBng___25_stk Proto NEAR
vBng___26 Proto NEAR
vBng___3_stk Proto NEAR
vBng___30_stk Proto NEAR
vBng___32 Proto NEAR
vBng___32_stk Proto NEAR
vBng___33_stk Proto NEAR
vBng___34_stk Proto NEAR
vBng___37_stk Proto NEAR
vBng___39 Proto NEAR
vBng___40_stk Proto NEAR
vBng___41 Proto NEAR
vBng___42 Proto NEAR
vBng___44 Proto NEAR
vBng___44_stk Proto NEAR
vBng___45_stk Proto NEAR
vBng___46 Proto NEAR
vBng___47 Proto NEAR
vBng___48 Proto NEAR
vBng___5 Proto NEAR
vBng___5_stk Proto NEAR
vBng___50 Proto NEAR
vBng___50_stk Proto NEAR
vBng___52 Proto NEAR
vBng___53 Proto NEAR
vBng___55 Proto NEAR
vBng___56_stk Proto NEAR
vBng___58 Proto NEAR
vBng___58_stk Proto NEAR
vBng___60 Proto NEAR
vBng___60_stk Proto NEAR
vBng___62_stk Proto NEAR
vBng___64 Proto NEAR
vBng___65_stk Proto NEAR
vBng___67 Proto NEAR
vBng___69 Proto NEAR
vBng___7 Proto NEAR
vBng___70 Proto NEAR
vBng___70_stk Proto NEAR
vBng___71 Proto NEAR
vBng___74 Proto NEAR
vBng___76 Proto NEAR
vBng___76_stk Proto NEAR
vBng___77 Proto NEAR
vBng___78 Proto NEAR
vBng___79 Proto NEAR
vBng___79_stk Proto NEAR
vBng___8 Proto NEAR
vBng___8_stk Proto NEAR
vBng___84_stk Proto NEAR
vBng___85_stk Proto NEAR
vBng___86_stk Proto NEAR
vBng___90_stk Proto NEAR
vBng___91_stk Proto NEAR
vBng___92 Proto NEAR
vBng___93_stk Proto NEAR
vBng___94 Proto NEAR
vBng___94_stk Proto NEAR
vBng___95_stk Proto NEAR
vBng___96_stk Proto NEAR
vBng_0 Proto NEAR
vBng_1 Proto NEAR
vBng_1_ Proto NEAR
vBng_1__1 Proto NEAR
vBng_1__4 Proto NEAR
vBng_1__7 Proto NEAR
vBng_1_stk Proto NEAR
vBng_h__10 Proto NEAR
vBng_h__17 Proto NEAR
vBng_h__19 Proto NEAR
vBng_h__2_stk Proto NEAR
vBng_h__20 Proto NEAR
vBng_h__22 Proto NEAR
vBng_h__24 Proto NEAR
vBng_h__25 Proto NEAR
vBng_h__26 Proto NEAR
vBng_h__28 Proto NEAR
vBng_h__29 Proto NEAR
vBng_h__3 Proto NEAR
vBng_h__32 Proto NEAR
vBng_h__39 Proto NEAR
vBng_h__7 Proto NEAR

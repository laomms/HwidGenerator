.text:00AECE80 ; GUID GUID_BTHPORT_DEVICE_INTERFACE
.text:00AECE80 _GUID_BTHPORT_DEVICE_INTERFACE dd 850302Ah             ; Data1
.text:00AECE80                                         ; DATA XREF: CHwidPnPClassEnumeratorT<CEmptyType>::EnumInterfaces<CHwidBluetoothDataCollector<0>>(_GUID const &,ulong,ulong,CHwidBluetoothDataCollector<0> &)+26↓o
.text:00AECE80                                         ; CHwidPnPClassEnumeratorT<CEmptyType>::EnumInterfaces<CHwidBluetoothDataCollector<0>>(_GUID const &,ulong,ulong,CHwidBluetoothDataCollector<0> &)+69↓o
.text:00AECE80                 dw 0B344h               ; Data2
.text:00AECE80                 dw 4FDAh                ; Data3
.text:00AECE80                 db 9Bh, 0E9h, 90h, 57h, 6Bh, 8Dh, 46h, 0F0h; Data4
.text:00AECE90 dword_AECE90    dd 0                    ; DATA XREF: CSmBiosInformation::CSmbiosData::Next(void)+61↓o
.text:00AECE94 ; GUID GUID_NDIS_LAN_CLASS
.text:00AECE94 _GUID_NDIS_LAN_CLASS dd 0AD498944h           ; Data1
.text:00AECE94                                         ; DATA XREF: CHwidPnPClassEnumeratorT<CEmptyType>::EnumInterfaces<CHwidNetworkDataCollector<0>>(_GUID const &,ulong,ulong,CHwidNetworkDataCollector<0> &)+26↓o
.text:00AECE94                                         ; CHwidPnPClassEnumeratorT<CEmptyType>::EnumInterfaces<CHwidNetworkDataCollector<0>>(_GUID const &,ulong,ulong,CHwidNetworkDataCollector<0> &)+69↓o
.text:00AECE94                 dw 762Fh                ; Data2
.text:00AECE94                 dw 11D0h                ; Data3
.text:00AECE94                 db 8Dh, 0CBh, 0, 0C0h, 4Fh, 0C3h, 35h, 8Ch; Data4
.text:00AECEA4 ; GUID GUID_DEVCLASS_SCSIADAPTER
.text:00AECEA4 _GUID_DEVCLASS_SCSIADAPTER dd 4D36E97Bh            ; Data1
.text:00AECEA4                                         ; DATA XREF: CHwidPnPDataCollector<4,0>::Collect(ISppNamedParamsReadWrite *)+8↓o
.text:00AECEA4                 dw 0E325h               ; Data2
.text:00AECEA4                 dw 11CEh                ; Data3
.text:00AECEA4                 db 0BFh, 0C1h, 8, 0, 2Bh, 0E1h, 3, 18h; Data4
.text:00AECEB4 ; GUID GUID_DEVCLASS_PCMCIA
.text:00AECEB4 _GUID_DEVCLASS_PCMCIA dd 4D36E977h            ; Data1
.text:00AECEB4                                         ; DATA XREF: CHwidPnPDataCollector<5,0>::Collect(ISppNamedParamsReadWrite *)+8↓o
.text:00AECEB4                 dw 0E325h               ; Data2
.text:00AECEB4                 dw 11CEh                ; Data3
.text:00AECEB4                 db 0BFh, 0C1h, 8, 0, 2Bh, 0E1h, 3, 18h; Data4
.text:00AECEC4 ; GUID GUID_DEVCLASS_MEDIA
.text:00AECEC4 _GUID_DEVCLASS_MEDIA dd 4D36E96Ch            ; Data1
.text:00AECEC4                                         ; DATA XREF: CHwidAudioAdaptorCollector<0>::CollectInternal<0>(ISppNamedParamsReadWrite *)+102↓o
.text:00AECEC4                 dw 0E325h               ; Data2
.text:00AECEC4                 dw 11CEh                ; Data3
.text:00AECEC4                 db 0BFh, 0C1h, 8, 0, 2Bh, 0E1h, 3, 18h; Data4
.text:00AECED4 ; GUID GUID_DEVCLASS_HDC
.text:00AECED4 _GUID_DEVCLASS_HDC dd 4D36E96Ah            ; Data1
.text:00AECED4                                         ; DATA XREF: CHwidPnPDataCollector<1,0>::Collect(ISppNamedParamsReadWrite *)+8↓o
.text:00AECED4                 dw 0E325h               ; Data2
.text:00AECED4                 dw 11CEh                ; Data3
.text:00AECED4                 db 0BFh, 0C1h, 8, 0, 2Bh, 0E1h, 3, 18h; Data4
.text:00AECEE4 ; GUID GUID_DEVCLASS_DISPLAY
.text:00AECEE4 _GUID_DEVCLASS_DISPLAY dd 4D36E968h            ; Data1
.text:00AECEE4                                         ; DATA XREF: CHwidPnPDataCollector<3,0>::Collect(ISppNamedParamsReadWrite *)+8↓o
.text:00AECEE4                 dw 0E325h               ; Data2
.text:00AECEE4                 dw 11CEh                ; Data3
.text:00AECEE4                 db 0BFh, 0C1h, 8, 0, 2Bh, 0E1h, 3, 18h; Data4
.text:00AECEF4 ; GUID GUID_DEVCLASS_CDROM
.text:00AECEF4 _GUID_DEVCLASS_CDROM dd 4D36E965h, 11CEE325h, 8C1BFh, 1803E12Bh
.text:00AECEF4                                         ; DATA XREF: CHwidPnPDataCollector<0,0>::Collect(ISppNamedParamsReadWrite *)+8↓o
.text:00AECF04 ; GUID _GUID_eb89a21b_1f9c_4093_9a4d_05d4002543f6
.text:00AECF04 __GUID_eb89a21b_1f9c_4093_9a4d_05d4002543f6 dd 0EB89A21Bh           ; Data1
.text:00AECF04                                         ; DATA XREF: CHwidDataCollectorBase::_InternalQueryInterface(_GUID const &,void * *)+2C↓o
.text:00AECF04                                         ; CHwidDataCollectorBase::CreateInstance<CHwidPnPDataCollector<0,0>>(_GUID const &,void * *):loc_B879C1↓o ...
.text:00AECF04                 dw 1F9Ch                ; Data2
.text:00AECF04                 dw 4093h                ; Data3
.text:00AECF04                 db 9Ah, 4Dh, 5, 0D4h, 0, 25h, 43h, 0F6h; Data4




Public Shared ReadOnly GUID_BTHPORT_DEVICE_INTERFACE As New Guid("{0x0850302A,0xB344,0x4fda,0x9B,0xE9,0x90,0x57,0x6B,0x8D,0x46,0xF0}}")
Public Shared ReadOnly GUID_NDIS_LAN_CLASS As New Guid("{0xad498944, 0x762f, 0x11d0, 0x8d, 0xcb, 0x00, 0xc0, 0x4f, 0xc3, 0x35, 0x8c}}")
Public Shared ReadOnly GUID_DEVCLASS_SCSIADAPTER As New Guid("{0x4d36e97b, 0xe325, 0x11ce, {0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18}}")
Public Shared ReadOnly GUID_DEVCLASS_PCMCIA As New Guid("{0x4d36e977, 0xe325, 0x11ce, {0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18}}")
Public Shared ReadOnly GUID_DEVCLASS_MEDIA As New Guid("{0x4d36e96c, 0xe325, 0x11ce, {0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18}}")
Public Shared ReadOnly GUID_DEVCLASS_HDC As New Guid("{0x4d36e96a, 0xe325, 0x11ce, {0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18}}")
Public Shared ReadOnly GUID_DEVCLASS_DISPLAY As New Guid("{0x4d36e968, 0xe325, 0x11ce, {0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18}}")
Public Shared ReadOnly GUID_DEVCLASS_CDROM As New Guid("{0x4d36e965, 0xe325, 0x11ce, {0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18}}")
Public Shared ReadOnly GUID As New Guid("{eb89a21b_1f9c_4093_9a4d_05d4002543f6}")

using System;

namespace HwidGetCurrentEx
{
    /// <summary>
    /// The device class and device interface GUIDs the collectors name.
    ///
    /// Two different namespaces live here and they are not interchangeable: GUID_DEVCLASS_* is
    /// what SP_DEVINFO_DATA.ClassGuid holds (and what the PnP collectors filter on), while
    /// GUID_DEVINTERFACE_* identifies an interface class. Putting an interface GUID where a
    /// class GUID belongs silently matches nothing.
    /// </summary>
    internal static class GUID_DEVINTERFACE
    {
        // --- setup classes (SP_DEVINFO_DATA.ClassGuid) ---------------------------------
        public static readonly Guid GUID_DEVCLASS_CDROM       = new Guid("4d36e965-e325-11ce-bfc1-08002be10318");
        public static readonly Guid GUID_DEVCLASS_DISPLAY     = new Guid("4d36e968-e325-11ce-bfc1-08002be10318");
        public static readonly Guid GUID_DEVCLASS_HDC         = new Guid("4d36e96a-e325-11ce-bfc1-08002be10318");
        public static readonly Guid GUID_DEVCLASS_MEDIA       = new Guid("4d36e96c-e325-11ce-bfc1-08002be10318");
        public static readonly Guid GUID_DEVCLASS_PCMCIA      = new Guid("4d36e977-e325-11ce-bfc1-08002be10318");
        public static readonly Guid GUID_DEVCLASS_SCSIADAPTER = new Guid("4d36e97b-e325-11ce-bfc1-08002be10318");

        // --- device interface classes ---------------------------------------------------
        public static readonly Guid GUID_DEVINTERFACE_DISK        = new Guid("53f56307-b6bf-11d0-94f2-00a0c91efb8b");
        public static readonly Guid GUID_KSCATEGORY_AUDIO         = new Guid("6994ad04-93ef-11d0-a3cc-00a0c9223196");
        public static readonly Guid GUID_NDIS_LAN_CLASS           = new Guid("ad498944-762f-11d0-8dcb-00c04fc3358c");
        public static readonly Guid GUID_BTHPORT_DEVICE_INTERFACE = new Guid("0850302a-b344-4fda-9be9-90576b8d46f0");
    }
}

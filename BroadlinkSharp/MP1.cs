using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x4EB5,"MP1")]
    [BroadlinkDevice(0x4EF7, "Honyar oem mp1")]
    public class MP1 : BroadlinkDevice
    {
        public MP1(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
            DeviceTypeDescription = "MP1";
        }
    }
}

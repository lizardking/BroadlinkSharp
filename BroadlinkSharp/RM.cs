using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x2712, "RM2")]
    [BroadlinkDevice(0x2737, "RM Mini")]
    [BroadlinkDevice(0x273d, "RM Pro Phicomm")]
    [BroadlinkDevice(0x2783, "RM2 Home Plus")]
    [BroadlinkDevice(0x277c, "RM2 Home Plus GDT")]
    [BroadlinkDevice(0x272a, "RM2 Pro Plus")]
    [BroadlinkDevice(0x2787, "RM2 Pro Plus2")]
    [BroadlinkDevice(0x279d, "RM2 Pro Plus3")]
    [BroadlinkDevice(0x27a9, "RM2 Pro Plus_300")]
    [BroadlinkDevice(0x278b, "RM2 Pro Plus BL")]
    [BroadlinkDevice(0x2797, "RM2 Pro Plus HYC")]
    [BroadlinkDevice(0x27a1, "RM2 Pro Plus R1")]
    [BroadlinkDevice(0x27a6, "RM2 Pro PP")]
    [BroadlinkDevice(0x278f, "RM Mini Shate")]
    public class RM : BroadlinkDevice
    {
        public RM(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
            
        }
    }
}

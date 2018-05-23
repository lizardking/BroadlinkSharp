using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x2711, "SP2")]
    [BroadlinkDevice(0x2719, "Honeywell SP2")]
    [BroadlinkDevice(0x7919, "Honeywell SP2")]
    [BroadlinkDevice(0x271a, "Honeywell SP2")]
    [BroadlinkDevice(0x791a, "Honeywell SP2")]
    [BroadlinkDevice(0x2720, "SPMini")]
    [BroadlinkDevice(0x753e, "SP3")]
    [BroadlinkDevice(0x7D00, "OEM branded SP3")]
    [BroadlinkDevice(0x947a, "SP3S")]
    [BroadlinkDevice(0x9479, "SP3S")]
    [BroadlinkDevice(0x2728, "SPMini2")]
    [BroadlinkDevice(0x2733, "OEM branded SPMini")]
    [BroadlinkDevice(0x273e, "OEM branded SPMini")]
    [BroadlinkDevice(0x7530, "OEM branded SPMini2")]
    [BroadlinkDevice(0x7918, "OEM branded SPMini2")]
    [BroadlinkDevice(0x2736, "SPMiniPlus")]
    public class SP2 : BroadlinkDevice
    {
        public SP2(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
           
        }
    }
}

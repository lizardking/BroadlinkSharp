using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x2722, "S1 (SmartOne Alarm Kit)")]
    public class S1C : BroadlinkDevice
    {
        public S1C(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
            DeviceTypeDescription = "S1C";
        }
    }
}

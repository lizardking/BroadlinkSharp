using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0, "SP1")]
    public class SP1 : BroadlinkDevice
    {
       
        public SP1(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
            DeviceTypeDescription = "SP1";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x2714, "A1")]
    public class A1 : BroadlinkDevice
    {
        public A1(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
            
        }
    }
}

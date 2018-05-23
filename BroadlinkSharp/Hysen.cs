using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x4EAD, "Hysen controller")]
    public class Hysen: BroadlinkDevice
    {
        public Hysen(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
           
        }
    }
}

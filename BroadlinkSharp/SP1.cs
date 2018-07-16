//Since I dont own this device the code in this class is untested.
//Code is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;


namespace BroadlinkSharp
{
    [BroadlinkDevice(0, "SP1")]
    public class SP1 : BroadlinkDevice
    {
        #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
        //class sp1(device):
        //  def __init__ (self, host, mac, devtype):
        //    device.__init__(self, host, mac, devtype)
        //    self.type = "SP1"
        //
        //  def set_power(self, state):
        //    packet = bytearray(4)
        //    packet[0] = state
        //	  self.send_packet(0x66, packet)    
        #endregion


        public SP1(IPEndPoint host, PhysicalAddress mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
           
        }

        public void SetPower(byte State)
        {
            byte[] packet = new byte[4];
            packet[0] = State;
            SendPacket(0x66, packet);
        }

    }
}

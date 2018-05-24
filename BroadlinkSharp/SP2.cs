//Since I dont own this device the code in this class is untested.
//Code is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

using System;
using System.Collections.Generic;
using System.Linq;
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

        #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
        //class sp2(device):
        //  def __init__ (self, host, mac, devtype):
        //    device.__init__(self, host, mac, devtype)
        //    self.type = "SP2"
        //
        //  def set_power(self, state):
        //    """Sets the power state of the smart plug."""
        //    packet = bytearray(16)
        //    packet[0] = 2
        //    if self.check_nightlight():
        //      packet[4] = 3 if state else 2
        //    else:
        //      packet[4] = 1 if state else 0
        //    self.send_packet(0x6a, packet)
        //
        //  def set_nightlight(self, state):
        //    """Sets the night light state of the smart plug"""
        //    packet = bytearray(16)
        //    packet[0] = 2
        //    if self.check_power():
        //      packet[4] = 3 if state else 1
        //    else:
        //      packet[4] = 2 if state else 0
        //    self.send_packet(0x6a, packet)
        //
        //  def check_power(self):
        //    """Returns the power state of the smart plug."""
        //    packet = bytearray(16)
        //    packet[0] = 1
        //    response = self.send_packet(0x6a, packet)
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err == 0:
        //      payload = self.decrypt(bytes(response[0x38:]))
        //      if type(payload[0x4]) == int:
        //        if payload[0x4] == 1 or payload[0x4] == 3:
        //          state = True
        //        else:
        //          state = False
        //      else:
        //        if ord(payload[0x4]) == 1 or ord(payload[0x4]) == 3:
        //          state = True
        //        else:
        //          state = False
        //      return state
        //
        //  def check_nightlight(self):
        //    """Returns the power state of the smart plug."""
        //    packet = bytearray(16)
        //    packet[0] = 1
        //    response = self.send_packet(0x6a, packet)
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err == 0:
        //      payload = self.decrypt(bytes(response[0x38:]))
        //      if type(payload[0x4]) == int:
        //        if payload[0x4] == 2 or payload[0x4] == 3:
        //          state = True
        //        else:
        //          state = False
        //      else:
        //        if ord(payload[0x4]) == 2 or ord(payload[0x4]) == 3:
        //          state = True
        //        else:
        //          state = False
        //      return state
        //
        //  def get_energy(self):
        //    packet = bytearray([8, 0, 254, 1, 5, 1, 0, 0, 0, 45])
        //    response = self.send_packet(0x6a, packet)
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err == 0:
        //      payload = self.decrypt(bytes(response[0x38:]))
        //      if type(payload[0x07]) == int:
        //        energy = int(hex(payload[0x07] * 256 + payload[0x06])[2:]) + int(hex(payload[0x05])[2:])/100.0
        //      else:
        //        energy = int(hex(ord(payload[0x07]) * 256 + ord(payload[0x06]))[2:]) + int(hex(ord(payload[0x05]))[2:])/100.0
        //      return energy 
        #endregion

        public SP2(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {

        }

        public void SetPower(bool State)
        {
            //    """Sets the power state of the smart plug."""
            byte[] packet = new byte[16];
            packet[0] = 2;
            if (CheckNightLight())
            {
                packet[4] = (byte)(State ? 3 : 2);
            }
            else
            {
                packet[4] = (byte)(State ? 1 : 0);
            }
            SendPacket(0x6a, packet);
        }


        public void SetNightLight(bool State)
        {
            //    """Sets the power state of the smart plug."""
            byte[] packet = new byte[16];
            packet[0] = 2;
            if (CheckPower())
            {
                packet[4] = (byte)(State ? 3 : 1);
            }
            else
            {
                packet[4] = (byte)(State ? 2 : 0);
            }
            SendPacket(0x6a, packet);
        }


        public bool CheckPower()
        {
            //    """Returns the power state of the smart plug."""
            byte[] packet = new byte[16];
            packet[0] = 1;
            byte[] response = SendPacket(0x6a, packet);
            int err = response[0x22] | (response[0x23] << 8);
            if (err == 0)
            {
                byte[] payload = Decrypt(response.Skip(0x38));
                return (payload[0x04] == 1 || payload[0x04] == 3);
            }
            else
            {
                throw new Exception($"Unexpected answer. Error code: {err}");
            } ;
            
        }

        public bool CheckNightLight()
        {
            //    """Returns the power state of the smart plug."""
            byte[] packet = new byte[16];
            packet[0] = 1;
            byte[] response = SendPacket(0x6a, packet);
            int err = response[0x22] | (response[0x23] << 8);
            if (err == 0)
            {
                byte[] payload = Decrypt(response.Skip(0x38));
                return (payload[0x04] == 2 || payload[0x04] == 3);
            }
            else
            {
                throw new Exception($"Unexpected answer. Error code: {err}");
            };

        }

        public double GetEnergy()
        {
            byte[] packet = new byte[] { 8, 0, 254, 1, 5, 1, 0, 0, 0, 45 };
            byte[] response = SendPacket(0x6a, packet);
            int err = response[0x22] | (response[0x23] << 8);
            if (err == 0)
            {
                byte[] payload = Decrypt(response.Skip(0x38));
                //Not sure about this codeline (my python really sucks), the org looks like this:
                //energy = int(hex(payload[0x07] * 256 + payload[0x06])[2:]) + int(hex(payload[0x05])[2:])/100.0
                return double.Parse(((int)(payload[0x07] * 256 + payload[0x06])).ToString("X"), System.Globalization.NumberStyles.HexNumber) + double.Parse(((int)(payload[0x05])).ToString("X"), System.Globalization.NumberStyles.HexNumber) / 100;
            }
            else
            {
                throw new Exception($"Unexpected answer. Error code: {err}");
            };
        }


    }
}

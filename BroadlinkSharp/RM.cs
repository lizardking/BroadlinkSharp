//Since I dont own this device the code in this class is untested.
//Code is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
        #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
        //class rm(device):
        //  def __init__ (self, host, mac, devtype):
        //    device.__init__(self, host, mac, devtype)
        //    self.type = "RM2"
        //
        //  def check_data(self):
        //    packet = bytearray(16)
        //    packet[0] = 4
        //    response = self.send_packet(0x6a, packet)
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err == 0:
        //      payload = self.decrypt(bytes(response[0x38:]))
        //      return payload[0x04:]
        //
        //  def send_data(self, data):
        //    packet = bytearray([0x02, 0x00, 0x00, 0x00])
        //    packet += data
        //    self.send_packet(0x6a, packet)
        //
        //  def enter_learning(self):
        //    packet = bytearray(16)
        //    packet[0] = 3
        //    self.send_packet(0x6a, packet)
        //
        //  def check_temperature(self):
        //    packet = bytearray(16)
        //    packet[0] = 1
        //    response = self.send_packet(0x6a, packet)
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err == 0:
        //      payload = self.decrypt(bytes(response[0x38:]))
        //      if type(payload[0x4]) == int:
        //        temp = (payload[0x4] * 10 + payload[0x5]) / 10.0
        //      else:
        //        temp = (ord(payload[0x4]) * 10 + ord(payload[0x5])) / 10.0
        //return temp 
        #endregion


        public RM(IPEndPoint host, PhysicalAddress mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {

        }

        public byte[] CheckData()
        {
            byte[] packet = new byte[16];
            packet[0] = 4;
            byte[] response = SendPacket(0x6a, packet);

            int err = response[0x22] | (response[0x23] << 8);
            if (err == 0)
            {
                byte[] payload = Decrypt(response.Skip(0x38));
                return payload.Skip(0x04).ToArray();
            }
            else
            {
                throw new Exception($"Unexpected answer. Error code: {err}");
            };
        }

        public void SendData(byte[] data)
        {
            byte[] packet = new byte[4 + data.Length];
            packet[0x00] = 0x02;
            Array.Copy(data, 0, packet, 0, data.Length);
            SendPacket(0x6a, packet);
        }

        public void EnterLearning()
        {
            byte[] packet = new byte[16];
            packet[0] = 3;
            SendPacket(0x6a, packet);
        }

        public double CheckTemperature()
        {
            byte[] packet = new byte[16];
            packet[0] = 1;
            SendPacket(0x6a, packet);
            byte[] response = SendPacket(0x6a, packet);

            int err = response[0x22] | (response[0x23] << 8);
            if (err == 0)
            {
                byte[] payload = Decrypt(response.Skip(0x38));

                return (payload[0x4] * 10 + payload[0x5]) / 10.0;
            }
            else
            {
                throw new Exception($"Unexpected answer. Error code: {err}");
            };
        }
    }
}

//This is the only Broadlink device I own and therfore this is the only device class that has been tested.
//Code is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x4E4D,"Dooya DT360E")]
    public class Dooya : BroadlinkDevice
    {

        #region #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
        //class dooya(device):
        //  def __init__ (self, host, mac, devtype):
        //    device.__init__(self, host, mac, devtype)
        //    self.type = "Dooya DT360E"
        //
        //  def _send(self, magic1, magic2):
        //    packet = bytearray(16)
        //    packet[0] = 0x09
        //    packet[2] = 0xbb
        //    packet[3] = magic1
        //    packet[4] = magic2
        //    packet[9] = 0xfa
        //    packet[10] = 0x44
        //    response = self.send_packet(0x6a, packet)
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err == 0:
        //      payload = self.decrypt(bytes(response[0x38:]))
        //      return ord(payload[4])
        //
        //  def open(self):
        //    return self._send(0x01, 0x00)
        //
        //  def close(self):
        //    return self._send(0x02, 0x00)
        //
        //  def stop(self):
        //    return self._send(0x03, 0x00)
        //
        //  def get_percentage(self):
        //    return self._send(0x06, 0x5d)
        //
        //  def set_percentage_and_wait(self, new_percentage):
        //    current = self.get_percentage()
        //    if current > new_percentage:
        //      self.close()
        //      while current is not None and current > new_percentage:
        //        time.sleep(0.2)
        //        current = self.get_percentage()
        //
        //    elif current < new_percentage:
        //      self.open()
        //      while current is not None and current < new_percentage:
        //        time.sleep(0.2)
        //        current = self.get_percentage()
        //self.stop() 
        #endregion

        public Dooya(IPEndPoint host, byte[] mac, int devtype, int timeout = 10):base(host,mac,devtype,timeout)
        {
           
        }

        private int Send(byte magic1, byte magic2)
        {


            byte[] packet = new byte[16];
            packet[0] = 0x09;
            packet[2] = 0xbb;
            packet[3] = magic1;
            packet[4] = magic2;
            packet[9] = 0xfa;
            packet[10] = 0x44;
            
            byte[] response = SendPacket(0x6a, packet);

            int err = response[0x22] | (response[0x23] << 8);
            if(err==0)
            {
                byte[] payload = Decrypt(response.Skip(0x38));
                return payload[4];
            } else
            {
                return -1;
            }
        }

        public int Close()
        {
            return Send(0x02, 0x00);
        }


        public int Open()
        {
            return Send(0x01, 0x00);
        }

        public int Stop()
        {
            return Send(0x03, 0x00);
        }


        public int GetPercentage()
        {
            return Send(0x06, 0x5d);
        }





    }


}

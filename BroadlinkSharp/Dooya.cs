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
        public Dooya(IPEndPoint host, byte[] mac, int devtype, int timeout = 10):base(host,mac,devtype,timeout)
        {
           
        }

        //def _send(self, magic1, magic2):
        private int send(byte magic1, byte magic2)
        {
            //packet = bytearray(16)
            //packet[0] = 0x09
            //packet[2] = 0xbb
            //packet[3] = magic1
            //packet[4] = magic2
            //packet[9] = 0xfa
            //packet[10] = 0x44

            byte[] packet = new byte[16];
            packet[0] = 0x09;
            packet[2] = 0xbb;
            packet[3] = magic1;
            packet[4] = magic2;
            packet[9] = 0xfa;
            packet[10] = 0x44;

            //response = self.send_packet(0x6a, packet)
            byte[] response = Send_Packet(0x6a, packet);

            //err = response[0x22] | (response[0x23] << 8)
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
            return send(0x01, 0x00);
        }


        public int Open()
        {
            return send(0x02, 0x00);
        }

        public int Stop()
        {
            return send(0x03, 0x00);
        }


        public int GetPercentage()
        {
            return send(0x06, 0x5d);
        }





    }


}

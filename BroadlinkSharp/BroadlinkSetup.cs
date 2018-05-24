//Setup a new Broadlink device via AP Mode. Review the README to see how to enter AP Mode.
//Class is untested, since I have naver managed to get the only Broadlink device I own into AP mode.
//Code is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BroadlinkSharp
{


    public static class BroadlinkSetup
    {
        #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
        //# Setup a new Broadlink device via AP Mode. Review the README to see how to enter AP Mode.
        //# Only tested with Broadlink RM3 Mini (Blackbean)
        //def setup(ssid, password, security_mode):
        //  # Security mode options are (0 - none, 1 = WEP, 2 = WPA1, 3 = WPA2, 4 = WPA1/2)
        //  payload = bytearray(0x88)
        //  payload[0x26] = 0x14  # This seems to always be set to 14
        //  # Add the SSID to the payload
        //  ssid_start = 68
        //  ssid_length = 0
        //  for letter in ssid:
        //    payload[(ssid_start + ssid_length)] = ord(letter)
        //    ssid_length += 1
        //  # Add the WiFi password to the payload
        //  pass_start = 100
        //  pass_length = 0
        //  for letter in password:
        //    payload[(pass_start + pass_length)] = ord(letter)
        //    pass_length += 1
        //
        //  payload[0x84] = ssid_length  # Character length of SSID
        //  payload[0x85] = pass_length  # Character length of password
        //  payload[0x86] = security_mode  # Type of encryption (00 - none, 01 = WEP, 02 = WPA1, 03 = WPA2, 04 = WPA1/2)
        //
        //  checksum = 0xbeaf
        //  for i in range(len(payload)):
        //    checksum += payload[i]
        //    checksum = checksum & 0xffff
        //
        //  payload[0x20] = checksum & 0xff  # Checksum 1 position
        //  payload[0x21] = checksum >> 8  # Checksum 2 position
        //
        //  sock = socket.socket(socket.AF_INET,  # Internet
        //                       socket.SOCK_DGRAM)  # UDP
        //  sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        //  sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
        //sock.sendto(payload, ('255.255.255.255', 80)) 
        #endregion

        public static void Setup(string SSID, string Password, BroadLinkSecurityModeEnum SecurityMode = BroadLinkSecurityModeEnum.None)
        {
            byte[] payload = new byte[0x88];
            payload[0x26] = 0x14;  // This seems to always be set to 14

            //Add the SSID to the payload
            const int SSIDStart = 68;
            for (int i = 0; i < SSID.Length - 1; i++)
            {
                payload[SSIDStart + i] = (byte)SSID[i];
            }
            payload[0x84] = (byte)SSID.Length; // Character length of SSID

            //Add the WiFi password to the payload
            const int PasswordStart = 100;
            for (int i = 0; i < Password.Length - 1; i++)
            {
                payload[PasswordStart + i] = (byte)Password[i];
            }
            payload[0x85] = (byte)Password.Length; // Character length of password
            payload[0x86] = (byte)SecurityMode;

            int checksum = 0xbeaf;
            foreach (byte b in payload)
            {
                checksum = (checksum + b) & 0xffff;
            }
            payload[0x20] = (byte)(checksum & 0xff); //Checksum 1 position
            payload[0x21] = (byte)((checksum >> 8) & 0xff); //Checksum 2 position

            Socket socket = new Socket(SocketType.Dgram, ProtocolType.IP);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            socket.SendTo(payload, new IPEndPoint(new IPAddress(new byte[] { 255, 255, 255, 255 }), 80));

        }

    }

    public enum BroadLinkSecurityModeEnum
    {
        None = 0,
        WEP = 1,
        WPA1 = 2,
        WPA2 = 3,
        WPA1_2 = 4
    }

}

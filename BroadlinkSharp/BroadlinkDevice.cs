using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace BroadlinkSharp
{
    public class BroadlinkDevice
    {
        Random Rnd = new Random();

        public BroadlinkDevice(IPEndPoint host, byte[] mac, int deviceTypeCode, int timeout = 10)
        {
            Init(host, mac, deviceTypeCode, timeout);
        }


        private void Init(IPEndPoint host, byte[] mac, int deviceTypeCode, int timeout = 10)
        {
            //self.host = host
            //self.mac = mac
            //self.devtype = devtype
            //self.timeout = timeout
            //self.count = random.randrange(0xffff)
            this.host = host;
            this.mac = mac;
            this.DeviceTypeCode = deviceTypeCode;



            this.timeout = timeout;
            this.count = Rnd.Next(0xffff);

            //Socket init - still need to figure this out
            ////self.cs = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            ////self.cs.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            ////self.cs.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
            ////self.cs.bind(('', 0))

            cs = new Socket(SocketType.Dgram, ProtocolType.IP);
            cs.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            cs.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            cs.SendTimeout = 1000;
            cs.ReceiveTimeout = 1000;

            //Socket binding still missing


            //Encryption setup
            //if 'pyaes' in globals() :
            //     self.encrypt = self.encrypt_pyaes
            //     self.decrypt = self.decrypt_pyaes
            // else:
            //     self.encrypt = self.encrypt_pycrypto
            //     self.decrypt = self.decrypt_pycrypto

        }

        private IPEndPoint host;
        private byte[] mac;
        public int DeviceTypeCode { get; private set; }
        private int timeout = 10;
        private int count = 0;

        private Socket cs;

        //self.key = bytearray([0x09, 0x76, 0x28, 0x34, 0x3f, 0xe9, 0x9e, 0x23, 0x76, 0x5c, 0x15, 0x13, 0xac, 0xcf, 0x8b, 0x02])
        //self.iv = bytearray([0x56, 0x2e, 0x17, 0x99, 0x6d, 0x09, 0x3d, 0x28, 0xdd, 0xb3, 0xba, 0x69, 0x5a, 0x2e, 0x6f, 0x58])
        //self.id = bytearray([0, 0, 0, 0])
        private byte[] key = { 0x09, 0x76, 0x28, 0x34, 0x3f, 0xe9, 0x9e, 0x23, 0x76, 0x5c, 0x15, 0x13, 0xac, 0xcf, 0x8b, 0x02 };
        private byte[] iv = { 0x56, 0x2e, 0x17, 0x99, 0x6d, 0x09, 0x3d, 0x28, 0xdd, 0xb3, 0xba, 0x69, 0x5a, 0x2e, 0x6f, 0x58 };
        private byte[] id = { 0, 0, 0, 0 };

        //self.type = "Unknown"
        private string _DeviceTypeDescription = null;
        public string DeviceTypeDescription
        {
            get
            {
                if (_DeviceTypeDescription == null)
                {
                    BroadlinkDeviceAttribute broadlinkDeviceAttribute = this.GetType().GetCustomAttributes(typeof(BroadlinkDeviceAttribute), true).Select(a => a as BroadlinkDeviceAttribute).FirstOrDefault(ba => ba.DeviceTypeCode == this.DeviceTypeCode);
                    if (broadlinkDeviceAttribute != null)
                    {
                        _DeviceTypeDescription = broadlinkDeviceAttribute.DeviceTypeDescription;

                    }
                    if (string.IsNullOrWhiteSpace(_DeviceTypeDescription))
                    {
                        _DeviceTypeDescription = $"Unknown {this.GetType().Name} (DeviceTypeCode : {DeviceTypeCode:X})";
                    }
                }
                return _DeviceTypeDescription;
            }
        }

        //self.lock = threading.Lock()
        private readonly object locker = new object();


        //ENcryption routines
        //def encrypt_pyaes(self, payload):
        //  aes = pyaes.AESModeOfOperationCBC(self.key, iv = bytes(self.iv))
        //  return b"".join([aes.encrypt(bytes(payload[i: i + 16])) for i in range(0, len(payload), 16)])

        //def decrypt_pyaes(self, payload):
        //  aes = pyaes.AESModeOfOperationCBC(self.key, iv = bytes(self.iv))
        //  return b"".join([aes.decrypt(bytes(payload[i: i + 16])) for i in range(0, len(payload), 16)])

        //def encrypt_pycrypto(self, payload):
        //  aes = AES.new(bytes(self.key), AES.MODE_CBC, bytes(self.iv))
        //  return aes.encrypt(bytes(payload))

        //def decrypt_pycrypto(self, payload):
        //  aes = AES.new(bytes(self.key), AES.MODE_CBC, bytes(self.iv))
        //  return aes.decrypt(bytes(payload))

        private byte[] Encrypt(byte[] data)
        {
            try
            {
                using (var rijndaelManaged = new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC, BlockSize = 128, Padding = PaddingMode.None })
                {
                    using (var memoryStream = new MemoryStream())
                    {

                        using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data, 0, data.Length);
                            cryptoStream.FlushFinalBlock();
                            return memoryStream.ToArray();

                        }
                    }
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }

        }

        protected byte[] Decrypt(IEnumerable<byte> data)
        {
            // throw new NotImplementedException();

            try
            {
                using (var rijndaelManaged = new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC, BlockSize = 128, Padding = PaddingMode.None })
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        //memoryStream.Write(data.ToArray(), 0, data.Count());
                        //memoryStream.Position = 0;
                        using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data.ToArray(), 0, data.Count());
                            cryptoStream.FlushFinalBlock();
                            return memoryStream.ToArray();// new BinaryReader(cryptoStream).ReadBytes((int)cryptoStream.Length);
                        }
                    }
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        public bool Authorize()
        {
            byte[] payload = new byte[0x50];
            payload[0x04] = 0x31;
            payload[0x05] = 0x31;
            payload[0x06] = 0x31;
            payload[0x07] = 0x31;
            payload[0x08] = 0x31;
            payload[0x09] = 0x31;
            payload[0x0a] = 0x31;
            payload[0x0b] = 0x31;
            payload[0x0c] = 0x31;
            payload[0x0d] = 0x31;
            payload[0x0e] = 0x31;
            payload[0x0f] = 0x31;
            payload[0x10] = 0x31;
            payload[0x11] = 0x31;
            payload[0x12] = 0x31;
            payload[0x1e] = 0x01;
            payload[0x2d] = 0x01;
            payload[0x30] = (byte)'T';
            payload[0x31] = (byte)'e';
            payload[0x32] = (byte)'s';
            payload[0x33] = (byte)'t';
            payload[0x34] = (byte)' ';
            payload[0x35] = (byte)' ';
            payload[0x36] = (byte)'1';


            //Send message, receive answer
            byte[] response = Send_Packet(0x65, payload);

            //Decode answer
            payload = Decrypt(response.Skip(0x38).ToArray());


            //        if not payload:
            //            return False
            //key = payload[0x04:0x14]
            //      if len(key) % 16 != 0:
            // return False
            //self.id = payload[0x00:0x04]
            //      self.key = key


            //return True

            if (payload == null)
            {
                return false;
            }
            else
            {
                if (payload.Length < 20)
                {
                    return false;
                };
                Array.ConstrainedCopy(payload, 4, key, 0, 16);
                Array.ConstrainedCopy(payload, 0, id, 0, 4);
                return true;
            }
        }




        protected byte[] Send_Packet(byte command, byte[] payload)
        {
            //self.count = (self.count + 1) & 0xffff
            count = (count + 1) % 0xffff;

            byte[] packet = new byte[0x38];
            packet[0x00] = 0x5a;
            packet[0x01] = 0xa5;
            packet[0x02] = 0xaa;
            packet[0x03] = 0x55;
            packet[0x04] = 0x5a;
            packet[0x05] = 0xa5;
            packet[0x06] = 0xaa;
            packet[0x07] = 0x55;
            packet[0x24] = 0x2a;
            packet[0x25] = 0x27;
            packet[0x26] = command;
            packet[0x28] = (byte)(count & 0xff);
            packet[0x29] = (byte)((count >> 8) & 0xff);
            packet[0x2a] = mac[0];
            packet[0x2b] = mac[1];
            packet[0x2c] = mac[2];
            packet[0x2d] = mac[3];
            packet[0x2e] = mac[4];
            packet[0x2f] = mac[5];
            packet[0x30] = id[0];
            packet[0x31] = id[1];
            packet[0x32] = id[2];
            packet[0x33] = id[3];

            //# pad the payload for AES encryption
            //if len(payload) > 0:
            //    numpad = (len(payload)//16+1)*16
            //    payload = payload.ljust(numpad, b"\x00")
            byte[] pl;
            if ((payload?.Length ?? 0) > 0)
            {
                int numpad = ((payload.Length / 16) + 1) * 16;
                pl = new byte[numpad];
                Array.Copy(payload, pl, payload.Length);
            }
            else
            {
                pl = new byte[0];
            }

            //checksum = 0xbeaf
            //for i in range(len(payload)) :
            //    checksum += payload[i]
            //    checksum = checksum & 0xffff

            int checksum = 0xbeaf;
            foreach (byte b in pl)
            {
                checksum = (checksum + b) & 0xffff;
            }

            pl = Encrypt(pl);

            packet[0x34] = (byte)(checksum & 0xff);
            packet[0x35] = (byte)((checksum >> 8) & 0xff);

            //for i in range(len(payload)) :
            //    packet.append(payload[i])
            Array.Resize(ref packet, packet.Length + pl.Length);
            Array.ConstrainedCopy(pl, 0, packet, packet.Length - pl.Length, pl.Length);

            //checksum = 0xbeaf
            //for i in range(len(packet)) :
            //    checksum += packet[i]
            //    checksum = checksum & 0xffff
            //packet[0x20] = checksum & 0xff
            //packet[0x21] = checksum >> 8
            checksum = 0xbeaf;
            foreach (byte b in packet)
            {
                checksum = (checksum + b) & 0xffff;
            }
            packet[0x20] = (byte)(checksum & 0xff);
            packet[0x21] = (byte)((checksum >> 8) & 0xff);

            //starttime = time.time()
            //with self.lock:
            //while True:
            //    try:
            //        self.cs.sendto(packet, self.host)
            //        self.cs.settimeout(1)
            //        response = self.cs.recvfrom(2048)
            //        break
            //    except socket.timeout:
            //        if (time.time() - starttime) > self.timeout:
            //        raise
            //return bytearray(response[0])

            byte[] response = new byte[2048];
            int bytesReceived = 0;
            DateTime starttime = DateTime.Now;
            lock (locker)
            {
                while (true)
                {
                    try
                    {
                        cs.SendTo(packet, host);
                        //Timeout set in init method    
                        EndPoint receivedFrom = new IPEndPoint(0, 0);
                        bytesReceived = cs.ReceiveFrom(response, ref receivedFrom);

                        break;
                    }
                    catch (Exception E)
                    {
                        if ((DateTime.Now - starttime).TotalSeconds > timeout)
                        {
                            throw new TimeoutException($"Sending and/or receiving the packet has failed and/or timedout");
                        }
                    }
                }
            };
            return response.Take(bytesReceived).ToArray();


        }





    }
}

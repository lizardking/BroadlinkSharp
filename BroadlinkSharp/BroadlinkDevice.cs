//Code is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace BroadlinkSharp
{
    /// <summary>
    /// Generic Broadlink device class.
    /// This is the base for all other Broadlink devices classes (they must inherit from this class to be recognized as Broadlink device classes).
    /// If the discover process cant determine the type of device (e.g. if no specific device calss has been implemented) a instance of this generic device class will be returned.
    /// </summary>
    public class BroadlinkDevice
    {

        #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
        //class device:
        //  def __init__(self, host, mac, devtype, timeout=10):
        //    self.host = host
        //    self.mac = mac
        //    self.devtype = devtype
        //    self.timeout = timeout
        //    self.count = random.randrange(0xffff)
        //    self.key = bytearray([0x09, 0x76, 0x28, 0x34, 0x3f, 0xe9, 0x9e, 0x23, 0x76, 0x5c, 0x15, 0x13, 0xac, 0xcf, 0x8b, 0x02])
        //    self.iv = bytearray([0x56, 0x2e, 0x17, 0x99, 0x6d, 0x09, 0x3d, 0x28, 0xdd, 0xb3, 0xba, 0x69, 0x5a, 0x2e, 0x6f, 0x58])
        //    self.id = bytearray([0, 0, 0, 0])
        //    self.cs = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        //    self.cs.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        //    self.cs.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
        //    self.cs.bind(('',0))
        //    self.type = "Unknown"
        //    self.lock = threading.Lock()
        //
        //    if 'pyaes' in globals():
        //        self.encrypt = self.encrypt_pyaes
        //        self.decrypt = self.decrypt_pyaes
        //    else:
        //        self.encrypt = self.encrypt_pycrypto
        //        self.decrypt = self.decrypt_pycrypto
        //
        //  def encrypt_pyaes(self, payload):
        //    aes = pyaes.AESModeOfOperationCBC(self.key, iv = bytes(self.iv))
        //    return b"".join([aes.encrypt(bytes(payload[i:i+16])) for i in range(0, len(payload), 16)])
        //
        //  def decrypt_pyaes(self, payload):
        //    aes = pyaes.AESModeOfOperationCBC(self.key, iv = bytes(self.iv))
        //    return b"".join([aes.decrypt(bytes(payload[i:i+16])) for i in range(0, len(payload), 16)])
        //
        //  def encrypt_pycrypto(self, payload):
        //    aes = AES.new(bytes(self.key), AES.MODE_CBC, bytes(self.iv))
        //    return aes.encrypt(bytes(payload))
        //
        //  def decrypt_pycrypto(self, payload):
        //    aes = AES.new(bytes(self.key), AES.MODE_CBC, bytes(self.iv))
        //    return aes.decrypt(bytes(payload))
        //
        //  def auth(self):
        //    payload = bytearray(0x50)
        //    payload[0x04] = 0x31
        //    payload[0x05] = 0x31
        //    payload[0x06] = 0x31
        //    payload[0x07] = 0x31
        //    payload[0x08] = 0x31
        //    payload[0x09] = 0x31
        //    payload[0x0a] = 0x31
        //    payload[0x0b] = 0x31
        //    payload[0x0c] = 0x31
        //    payload[0x0d] = 0x31
        //    payload[0x0e] = 0x31
        //    payload[0x0f] = 0x31
        //    payload[0x10] = 0x31
        //    payload[0x11] = 0x31
        //    payload[0x12] = 0x31
        //    payload[0x1e] = 0x01
        //    payload[0x2d] = 0x01
        //    payload[0x30] = ord('T')
        //    payload[0x31] = ord('e')
        //    payload[0x32] = ord('s')
        //    payload[0x33] = ord('t')
        //    payload[0x34] = ord(' ')
        //    payload[0x35] = ord(' ')
        //    payload[0x36] = ord('1')
        //
        //    response = self.send_packet(0x65, payload)
        //
        //    payload = self.decrypt(response[0x38:])
        //
        //    if not payload:
        //     return False
        //
        //    key = payload[0x04:0x14]
        //    if len(key) % 16 != 0:
        //     return False
        //
        //    self.id = payload[0x00:0x04]
        //    self.key = key
        //
        //    return True
        //
        //  def get_type(self):
        //    return self.type
        //
        //  def send_packet(self, command, payload):
        //    self.count = (self.count + 1) & 0xffff
        //    packet = bytearray(0x38)
        //    packet[0x00] = 0x5a
        //    packet[0x01] = 0xa5
        //    packet[0x02] = 0xaa
        //    packet[0x03] = 0x55
        //    packet[0x04] = 0x5a
        //    packet[0x05] = 0xa5
        //    packet[0x06] = 0xaa
        //    packet[0x07] = 0x55
        //    packet[0x24] = 0x2a
        //    packet[0x25] = 0x27
        //    packet[0x26] = command
        //    packet[0x28] = self.count & 0xff
        //    packet[0x29] = self.count >> 8
        //    packet[0x2a] = self.mac[0]
        //    packet[0x2b] = self.mac[1]
        //    packet[0x2c] = self.mac[2]
        //    packet[0x2d] = self.mac[3]
        //    packet[0x2e] = self.mac[4]
        //    packet[0x2f] = self.mac[5]
        //    packet[0x30] = self.id[0]
        //    packet[0x31] = self.id[1]
        //    packet[0x32] = self.id[2]
        //    packet[0x33] = self.id[3]
        //
        //    # pad the payload for AES encryption
        //    if len(payload)>0:
        //      numpad=(len(payload)//16+1)*16
        //      payload=payload.ljust(numpad, b"\x00")
        //
        //    checksum = 0xbeaf
        //    for i in range(len(payload)):
        //      checksum += payload[i]
        //      checksum = checksum & 0xffff
        //
        //    payload = self.encrypt(payload)
        //
        //    packet[0x34] = checksum & 0xff
        //    packet[0x35] = checksum >> 8
        //
        //    for i in range(len(payload)):
        //      packet.append(payload[i])
        //
        //    checksum = 0xbeaf
        //    for i in range(len(packet)):
        //      checksum += packet[i]
        //      checksum = checksum & 0xffff
        //    packet[0x20] = checksum & 0xff
        //    packet[0x21] = checksum >> 8
        //
        //    starttime = time.time()
        //    with self.lock:
        //      while True:
        //        try:
        //          self.cs.sendto(packet, self.host)
        //          self.cs.settimeout(1)
        //          response = self.cs.recvfrom(2048)
        //          break
        //        except socket.timeout:
        //          if (time.time() - starttime) > self.timeout:
        //            raise
        //    return bytearray(response[0]) 
        #endregion



        Random Rnd = new Random();

        public BroadlinkDevice(IPEndPoint host, PhysicalAddress MacAddress, int deviceTypeCode, int timeout = 10)
        {

            Init(host, MacAddress, deviceTypeCode, timeout);
        }


        private void Init(IPEndPoint host, PhysicalAddress MacAddress, int deviceTypeCode, int timeout = 10)
        {
            this.host = host;
            this.MacAddress = MacAddress;
            this.DeviceTypeCode = deviceTypeCode;



            this.timeout = timeout;
            this.count = Rnd.Next(0xffff);


            cs = new Socket(SocketType.Dgram, ProtocolType.IP);
            cs.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            cs.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            cs.SendTimeout = 1000;
            cs.ReceiveTimeout = 1000;
            //Org Python class has the flowwing line after socket initialization:
            //self.cs.bind(('',0))
            //This line is missing here, but the code still work.
        }

        private IPEndPoint host;
        /// <summary>
        /// Gets the mac Address.
        /// </summary>
        /// <value>
        /// The mac Address.
        /// </value>
        public PhysicalAddress MacAddress { get; private set; } = null;

        private int timeout = 10;
        private int count = 0;

        private Socket cs;


        private byte[] key = { 0x09, 0x76, 0x28, 0x34, 0x3f, 0xe9, 0x9e, 0x23, 0x76, 0x5c, 0x15, 0x13, 0xac, 0xcf, 0x8b, 0x02 };
        private byte[] iv = { 0x56, 0x2e, 0x17, 0x99, 0x6d, 0x09, 0x3d, 0x28, 0xdd, 0xb3, 0xba, 0x69, 0x5a, 0x2e, 0x6f, 0x58 };
        private byte[] id = { 0, 0, 0, 0 };

        /// <summary>
        /// Gets the device type code.
        /// </summary>
        /// <value>
        /// The device type code.
        /// </value>
        public int DeviceTypeCode { get; private set; }



        private string _DeviceTypeDescription = null;
        /// <summary>
        /// Gets the device type description.
        /// </summary>
        /// <value>
        /// The device type description.
        /// </value>
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

        private readonly object locker = new object();



        /// <summary>
        /// Encrypts the specified data.
        /// Use this method to encrypt the payload for commands (if needed).
        /// </summary>
        /// <param name="data">The unencypted data.</param>
        /// <returns>Encrypted data</returns>
        protected byte[] Encrypt(byte[] data)
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

        /// <summary>
        /// Decrypts the specified data.
        /// Use this method to decrypt the data returned from a Broadlink device (if needed).
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <returns>Unencrypted data</returns>
        protected byte[] Decrypt(IEnumerable<byte> data)
        {


            try
            {
                using (var rijndaelManaged = new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC, BlockSize = 128, Padding = PaddingMode.None })
                {
                    using (var memoryStream = new MemoryStream())
                    {

                        using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data.ToArray(), 0, data.Count());
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

        /// <summary>
        /// Authorizes connection to the Broadlink devices.
        /// </summary>
        /// <returns><c>true</c> if authorization was successful, otherwise <c>false</c></returns>
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



            byte[] response = SendPacket(0x65, payload);


            payload = Decrypt(response.Skip(0x38).ToArray());

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




        /// <summary>
        /// Sends a packet of data to the Broadlink devices and receives the answer (if provided)
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="payload">The payload/data packet.</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        protected byte[] SendPacket(byte command, byte[] payload)
        {

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
            byte[] m = MacAddress.GetAddressBytes();
            packet[0x2a] = m[0];
            packet[0x2b] = m[1];
            packet[0x2c] = m[2];
            packet[0x2d] = m[3];
            packet[0x2e] = m[4];
            packet[0x2f] = m[5];
            packet[0x30] = id[0];
            packet[0x31] = id[1];
            packet[0x32] = id[2];
            packet[0x33] = id[3];

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

            int checksum = 0xbeaf;
            foreach (byte b in pl)
            {
                checksum = (checksum + b) & 0xffff;
            }

            pl = Encrypt(pl);

            packet[0x34] = (byte)(checksum & 0xff);
            packet[0x35] = (byte)((checksum >> 8) & 0xff);

            Array.Resize(ref packet, packet.Length + pl.Length);
            Array.ConstrainedCopy(pl, 0, packet, packet.Length - pl.Length, pl.Length);

            checksum = 0xbeaf;
            foreach (byte b in packet)
            {
                checksum = (checksum + b) & 0xffff;
            }
            packet[0x20] = (byte)(checksum & 0xff);
            packet[0x21] = (byte)((checksum >> 8) & 0xff);

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
                            throw new TimeoutException($"Sending and/or receiving the packet has failed and/or timedout", E);
                        }
                    }
                }
            };
            return response.Take(bytesReceived).ToArray();


        }





    }
}

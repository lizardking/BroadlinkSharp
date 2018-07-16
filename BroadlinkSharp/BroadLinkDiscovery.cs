//The static BroadLinkDiscovery class is used to dicover the Broadlink devices on the network. 
//Code is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
//The gendevice logic has been changed to used attributes on the device classes instead of using a fixed dictionary like in the org code. 

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace BroadlinkSharp
{
    /// <summary>
    /// Used to find the Broadlink devices on the network.
    /// </summary>
    public static class BroadLinkDiscovery
    {

        #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

        //def gendevice(devtype, host, mac):
        //  devices = {
        //          sp1: [0],
        //          sp2: [0x2711,                          # SP2
        //                0x2719, 0x7919, 0x271a, 0x791a,  # Honeywell SP2
        //                0x2720,                          # SPMini
        //                0x753e,                          # SP3
        //                0x7D00,                          # OEM branded SP3
        //                0x947a, 0x9479,                  # SP3S
        //                0x2728,                          # SPMini2
        //                0x2733, 0x273e,                  # OEM branded SPMini
        //                0x7530, 0x7918,                  # OEM branded SPMini2
        //                0x2736                           # SPMiniPlus
        //                ],
        //          rm: [0x2712,  # RM2
        //               0x2737,  # RM Mini
        //               0x273d,  # RM Pro Phicomm
        //               0x2783,  # RM2 Home Plus
        //               0x277c,  # RM2 Home Plus GDT
        //               0x272a,  # RM2 Pro Plus
        //               0x2787,  # RM2 Pro Plus2
        //               0x279d,  # RM2 Pro Plus3
        //               0x27a9,  # RM2 Pro Plus_300
        //               0x278b,  # RM2 Pro Plus BL
        //               0x2797,  # RM2 Pro Plus HYC
        //               0x27a1,  # RM2 Pro Plus R1
        //               0x27a6,  # RM2 Pro PP
        //               0x278f   # RM Mini Shate
        //               ],
        //          a1: [0x2714],  # A1
        //          mp1: [0x4EB5,  # MP1
        //                0x4EF7   # Honyar oem mp1
        //                ],
        //          hysen: [0x4EAD],  # Hysen controller
        //          S1C: [0x2722],  # S1 (SmartOne Alarm Kit)
        //          dooya: [0x4E4D]  # Dooya DT360E (DOOYA_CURTAIN_V2)
        //          }
        //
        //  # Look for the class associated to devtype in devices
        //  [deviceClass] = [dev for dev in devices if devtype in devices[dev]] or [None]
        //  if deviceClass is None:
        //    return device(host=host, mac=mac, devtype=devtype)
        //  return deviceClass(host=host, mac=mac, devtype=devtype)
        //
        //def discover(timeout=None, local_ip_address=None):
        //  if local_ip_address is None:
        //      s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        //      s.connect(('8.8.8.8', 53))  # connecting to a UDP address doesn't send packets
        //      local_ip_address = s.getsockname()[0]
        //  address = local_ip_address.split('.')
        //  cs = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        //  cs.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        //  cs.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
        //  cs.bind((local_ip_address,0))
        //  port = cs.getsockname()[1]
        //  starttime = time.time()
        //
        //  devices = []
        //
        //  timezone = int(time.timezone/-3600)
        //  packet = bytearray(0x30)
        //
        //  year = datetime.now().year
        //
        //  if timezone < 0:
        //    packet[0x08] = 0xff + timezone - 1
        //    packet[0x09] = 0xff
        //    packet[0x0a] = 0xff
        //    packet[0x0b] = 0xff
        //  else:
        //    packet[0x08] = timezone
        //    packet[0x09] = 0
        //    packet[0x0a] = 0
        //    packet[0x0b] = 0
        //  packet[0x0c] = year & 0xff
        //  packet[0x0d] = year >> 8
        //  packet[0x0e] = datetime.now().minute
        //  packet[0x0f] = datetime.now().hour
        //  subyear = str(year)[2:]
        //  packet[0x10] = int(subyear)
        //  packet[0x11] = datetime.now().isoweekday()
        //  packet[0x12] = datetime.now().day
        //  packet[0x13] = datetime.now().month
        //  packet[0x18] = int(address[0])
        //  packet[0x19] = int(address[1])
        //  packet[0x1a] = int(address[2])
        //  packet[0x1b] = int(address[3])
        //  packet[0x1c] = port & 0xff
        //  packet[0x1d] = port >> 8
        //  packet[0x26] = 6
        //  checksum = 0xbeaf
        //
        //  for i in range(len(packet)):
        //      checksum += packet[i]
        //  checksum = checksum & 0xffff
        //  packet[0x20] = checksum & 0xff
        //  packet[0x21] = checksum >> 8
        //
        //  cs.sendto(packet, ('255.255.255.255', 80))
        //  if timeout is None:
        //    response = cs.recvfrom(1024)
        //    responsepacket = bytearray(response[0])
        //    host = response[1]
        //    mac = responsepacket[0x3a:0x40]
        //    devtype = responsepacket[0x34] | responsepacket[0x35] << 8
        //
        //
        //    return gendevice(devtype, host, mac)
        //  else:
        //    while (time.time() - starttime) < timeout:
        //      cs.settimeout(timeout - (time.time() - starttime))
        //      try:
        //        response = cs.recvfrom(1024)
        //      except socket.timeout:
        //        return devices
        //      responsepacket = bytearray(response[0])
        //      host = response[1]
        //      devtype = responsepacket[0x34] | responsepacket[0x35] << 8
        //      mac = responsepacket[0x3a:0x40]
        //      dev = gendevice(devtype, host, mac)
        //      devices.append(dev)
        //return devices 
        #endregion

        private static Dictionary<int, Type> deviceTypesDictionary = null;

        private static Dictionary<int, Type> GetDeviceTypesDictionary()
        {
            if (deviceTypesDictionary == null)
            {
                deviceTypesDictionary = new Dictionary<int, Type>();

                foreach (Type broadlinkDeviceType in typeof(BroadlinkDevice).Assembly.GetTypes().Where(t => t != typeof(BroadlinkDevice) && typeof(BroadlinkDevice).IsAssignableFrom(t)))
                {
                    foreach (BroadlinkDeviceAttribute broadlinkDeviceAttribute in broadlinkDeviceType.GetCustomAttributes(typeof(BroadlinkDeviceAttribute), true).Select(a => a as BroadlinkDeviceAttribute))
                    {
                        if (deviceTypesDictionary.ContainsKey(broadlinkDeviceAttribute.DeviceTypeCode))
                        {
                            throw new Exception($"The {nameof(BroadlinkDeviceAttribute.DeviceTypeCode)} {broadlinkDeviceAttribute.DeviceTypeCode} of the {nameof(BroadlinkDeviceAttribute)} has been defined more than once. It has been defined for the type {deviceTypesDictionary[broadlinkDeviceAttribute.DeviceTypeCode].Name} and also for type {broadlinkDeviceType.Name}");
                        }
                        deviceTypesDictionary.Add(broadlinkDeviceAttribute.DeviceTypeCode, broadlinkDeviceType);
                    }
                }
            }
            return deviceTypesDictionary;
        }



        private static BroadlinkDevice Gendevice(int deviceTypeCode, IPEndPoint host, PhysicalAddress mac)
        {

            if (GetDeviceTypesDictionary().TryGetValue(deviceTypeCode, out Type deviceType))
            {
                return (BroadlinkDevice)Activator.CreateInstance(deviceType, new object[] { host, mac, deviceTypeCode, 10 });
            }
            else
            {
                return new BroadlinkDevice(host, mac, deviceTypeCode, 10);
            }


        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }

            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        /// <summary>
        /// Discovers a specific Broadlink device
        /// </summary>
        /// <param name="TimeoutMs">Timeout in milliseconds. 0 will only discover 1 device, values &gt;0 might discover several devices. If one of the devices found matches the MacAddress, this device will be return, otherwise null will be returned</param>
        /// <param name="MacAddress">The mac address of the device to be found.</param>
        /// <param name="LocalIpAddress">The local ip address. If para is null or blank, the IP of the computer running the code is used. If your computer has several IP addresses you might need to specify the correct one (Untested)(</param>
        /// <returns>A broadlink device with the specified MacAddress. Otherwise null.</returns>
        public static BroadlinkDevice DiscoverSpecificDevice(int TimeoutMs, string MacAddress = null, string LocalIpAddress = null)
        {
            string MacToFind = null;
            if (!string.IsNullOrWhiteSpace(MacAddress))
            {
                MacToFind = PhysicalAddress.Parse(MacAddress.Replace(":", "-").ToUpper()).ToString();
            }

            return Discover(TimeoutMs, MacAddress, LocalIpAddress).FirstOrDefault(d => d.MacAddress.ToString() == MacAddress);
        }

        /// <summary>
        /// Discovers the Broadlink devices on the network
        /// </summary>
        /// <param name="TimeoutMs">Timeout in milliseconds. 0 will only discover 1 device, values &gt;0 might discover several devices (discovery of several devices is not tested since I only own 1 Broadlink devices). Timeout specifies the timeout in milliseconds afdter the last device has been discovered.</param>
        /// <param name="AbortOnMacAddress">The discover operation will stop, once a device with the given mac address has been found.</param>
        /// <param name="LocalIpAddress">The local ip address. If para is null or blank, the IP of the computer running the code is used. If your computer has several IP addresses you might need to specify the correct one (Untested)(</param>
        /// <returns>
        /// List of BroadlinkDevices classes
        /// </returns>
        /// <exception cref="Exception"></exception>
        public static List<BroadlinkDevice> Discover(int TimeoutMs, string AbortOnMacAddress = null, string LocalIpAddress = null)
        {

            if (string.IsNullOrWhiteSpace(LocalIpAddress))
            {
                LocalIpAddress = GetLocalIPAddress();
            }

            string MacToFind = null;
            if (!string.IsNullOrWhiteSpace(AbortOnMacAddress))
            {
                MacToFind = PhysicalAddress.Parse(AbortOnMacAddress.Replace(":", "-").ToUpper()).ToString();
            }


            Socket cs = new Socket(SocketType.Dgram, ProtocolType.IP);
            cs.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            cs.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            IPAddress.TryParse(LocalIpAddress, out IPAddress ipAddress);
            cs.Bind(new IPEndPoint(ipAddress, 0));



            List<BroadlinkDevice> devices = new List<BroadlinkDevice>();

            byte[] packet = new byte[0x30];

            DateTime currentTime = DateTime.Now;

            int timezone = (int)(System.TimeZoneInfo.Local.GetUtcOffset(currentTime).TotalSeconds / -3600);


            if (timezone < 0)
            {
                packet[0x08] = (byte)(0xff + timezone - 1);
                packet[0x09] = 0xff;
                packet[0x0a] = 0xff;
                packet[0x0b] = 0xff;
            }
            else
            {
                packet[0x08] = (byte)timezone;
                packet[0x09] = 0;
                packet[0x0a] = 0;
                packet[0x0b] = 0;
            }
            packet[0x0c] = (byte)(currentTime.Year & 0xff);
            packet[0x0d] = (byte)((currentTime.Year >> 8) & 0xff);
            packet[0x0e] = (byte)currentTime.Minute;
            packet[0x0f] = (byte)currentTime.Hour;
            packet[0x10] = (byte)(currentTime.Year % 100);
            packet[0x11] = (byte)((((byte)currentTime.DayOfWeek) + 1) % 7); //Monday is 1
            packet[0x12] = (byte)currentTime.Day;
            packet[0x13] = (byte)currentTime.Month;
            byte[] addressBytes = ((IPEndPoint)cs.LocalEndPoint).Address.GetAddressBytes();
            packet[0x18] = addressBytes[12];
            packet[0x19] = addressBytes[13];
            packet[0x1a] = addressBytes[14];
            packet[0x1b] = addressBytes[15];
            int port = ((IPEndPoint)cs.LocalEndPoint).Port;
            packet[0x1c] = (byte)(port & 0xff);
            packet[0x1d] = (byte)((port >> 8) & 0xff);
            packet[0x26] = 6;

            int checksum = 0xbeaf;
            foreach (byte b in packet)
            {
                checksum += b;
            }
            checksum = checksum & 0xffff;
            packet[0x20] = (byte)(checksum & 0xff);
            packet[0x21] = (byte)((checksum >> 8) & 0xff);

            cs.ReceiveTimeout = (TimeoutMs <= 0 ? 500 : TimeoutMs);
            cs.SendTimeout = cs.ReceiveTimeout;

            cs.SendTo(packet, new IPEndPoint(new IPAddress(new byte[] { 255, 255, 255, 255 }), 80));


            if (TimeoutMs <= 0)
            {


                byte[] response = new byte[1024];
                EndPoint host = new IPEndPoint(0, 0);
                cs.ReceiveFrom(response, ref host);

                byte[] macbytes = new byte[6];
                Array.ConstrainedCopy(response, 0x3a, macbytes, 0, 6);
                PhysicalAddress macAddress = new PhysicalAddress(macbytes);

                int devtype = response[0x34] | response[0x35] << 8;

                devices.Add(Gendevice(devtype, (IPEndPoint)host, macAddress));

            }
            else
            {
                while (true)
                {
                    try
                    {


                        byte[] response = new byte[1024];
                        EndPoint host = new IPEndPoint(0, 0);
                        cs.ReceiveFrom(response, ref host);

                        byte[] macbytes = new byte[6];
                        Array.ConstrainedCopy(response, 0x3a, macbytes, 0, 6);
                        PhysicalAddress macAddress = new PhysicalAddress(macbytes);

                        int devtype = response[0x34] | response[0x35] << 8;



                        BroadlinkDevice newDevice = Gendevice(devtype, (IPEndPoint)host, macAddress);
                        devices.Add(newDevice);
                        if (MacToFind != null && newDevice.MacAddress.ToString() == MacToFind)
                        {
                            break;
                        };
                    }
                    catch (SocketException E)
                    {
                        if (E.SocketErrorCode == SocketError.TimedOut)
                        {
                            //This is expected if no further Broadlink devices can be found
                            break;
                        }
                        else
                        {
                            throw new Exception($"A unhandled SocketExcpetion occured. {E.Message}", E);
                        }
                    }
                }

            }
            return devices;
        }







    }
}

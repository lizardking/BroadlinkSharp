using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace BroadlinkSharp
{
    public class BroadLinkDiscovery
    {
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



        private static BroadlinkDevice Gendevice(int deviceTypeCode, IPEndPoint host, byte[] mac)
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

        public static string GetLocalIPAddress()
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


        //def discover(timeout= None, local_ip_address= None):
        public static List<BroadlinkDevice> Discover(int timeout, string local_ip_address = null)
        {


            //if local_ip_address is None:
            //    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            //    s.connect(('8.8.8.8', 53))  # connecting to a UDP address doesn't send packets
            //    local_ip_address = s.getsockname()[0]
            if (string.IsNullOrWhiteSpace(local_ip_address))
            {
                local_ip_address = GetLocalIPAddress();
            }

            //address = local_ip_address.split('.') - Dont know yet what this is used for
            //cs = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            //cs.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            //cs.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
            //cs.bind((local_ip_address, 0))

            Socket cs = new Socket(SocketType.Dgram, ProtocolType.IP);
            cs.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            cs.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            cs.ReceiveTimeout = timeout * 1000;
            cs.SendTimeout = timeout * 1000;
            IPAddress.TryParse(local_ip_address, out IPAddress ipAddress);
            cs.Bind(new IPEndPoint(ipAddress, 0));

            //port = cs.getsockname()[1] - Dont know yet what this is used for

            //starttime = time.time()
            DateTime starttime = DateTime.Now;

            //devices = [];
            List<BroadlinkDevice> devices = new List<BroadlinkDevice>();

            //timezone = int(time.timezone / -3600) - Dont know yet what this is used for

            //packet = bytearray(0x30)
            byte[] packet = new byte[0x30];

            //timezone = int(time.timezone / -3600)
            DateTime currentTime = DateTime.Now;

            int timezone = (int)(System.TimeZoneInfo.Local.GetUtcOffset(currentTime).TotalSeconds / -3600);

            //if timezone < 0:
            //    packet[0x08] = 0xff + timezone - 1
            //    packet[0x09] = 0xff
            //    packet[0x0a] = 0xff
            //    packet[0x0b] = 0xff
            //else:
            //    packet[0x08] = timezone
            //    packet[0x09] = 0
            //    packet[0x0a] = 0
            //    packet[0x0b] = 0
            //packet[0x0c] = year & 0xff
            //packet[0x0d] = year >> 8
            //packet[0x0e] = datetime.now().minute
            //packet[0x0f] = datetime.now().hour
            //subyear = str(year)[2:]
            //packet[0x10] = int(subyear)
            //packet[0x11] = datetime.now().isoweekday()
            //packet[0x12] = datetime.now().day
            //packet[0x13] = datetime.now().month
            //packet[0x18] = int(address[0])
            //packet[0x19] = int(address[1])
            //packet[0x1a] = int(address[2])
            //packet[0x1b] = int(address[3])
            //packet[0x1c] = port & 0xff
            //packet[0x1d] = port >> 8
            //packet[0x26] = 6

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

            //checksum = 0xbeaf
            //for i in range(len(packet)):
            //    checksum += packet[i]
            //checksum = checksum & 0xffff
            //packet[0x20] = checksum & 0xff
            //packet[0x21] = checksum >> 8

            int checksum = 0xbeaf;
            foreach (byte b in packet)
            {
                checksum += b;
            }
            checksum = checksum & 0xffff;
            packet[0x20] = (byte)(checksum & 0xff);
            packet[0x21] = (byte)((checksum >> 8) & 0xff);

            //cs.sendto(packet, ('255.255.255.255', 80))
            cs.SendTo(packet, new IPEndPoint(new IPAddress(new byte[] { 255, 255, 255, 255 }), 80));

            // if timeout is None:
            if (timeout <= 0)
            {
                //response = cs.recvfrom(1024)
                //responsepacket = bytearray(response[0])
                //host = response[1]
                //mac = responsepacket[0x3a:0x40]
                //devtype = responsepacket[0x34] | responsepacket[0x35] << 8
                byte[] response = new byte[1024];
                EndPoint host = new IPEndPoint(0, 0);
                cs.ReceiveFrom(response, ref host);

                byte[] mac = new byte[6];
                Array.ConstrainedCopy(response, 0x3a, mac, 0, 6);
                int devtype = response[0x34] | response[0x35] << 8;

                devices.Add(Gendevice(devtype, (IPEndPoint)host, mac));

            }
            else
            {


                //while (time.time() - starttime) < timeout:
                //  cs.settimeout(timeout - (time.time() - starttime)) - Will skip this
                //  try:
                //    response = cs.recvfrom(1024)
                //  except socket.timeout:
                //    return devices
                //  responsepacket = bytearray(response[0])
                //  host = response[1]
                //  devtype = responsepacket[0x34] | responsepacket[0x35] << 8
                //  mac = responsepacket[0x3a:0x40]
                //  dev = gendevice(devtype, host, mac)
                //  devices.append(dev)
                //return devices

                while ((DateTime.Now - starttime).TotalSeconds < timeout)
                {
                    try
                    {
                        byte[] response = new byte[1024];
                        EndPoint host = new IPEndPoint(0, 0);
                        cs.ReceiveFrom(response, ref host);

                        byte[] mac = new byte[6];
                        Array.ConstrainedCopy(response, 0x3a, mac, 0, 6);
                        int devtype = response[0x34] | response[0x35] << 8;

                        devices.Add(Gendevice(devtype, (IPEndPoint)host, mac));
                    }
                    catch (Exception E)
                    {
                        break;

                    }
                }

            }
            return devices;
        }







    }
}

//Since I dont own this device the code in this class is untested.
//Code is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x2714, "A1")]
    public class A1 : BroadlinkDevice
    {
        #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
        //class a1(device):
        //  def __init__ (self, host, mac, devtype):
        //    device.__init__(self, host, mac, devtype)
        //    self.type = "A1"
        //
        //  def check_sensors(self):
        //    packet = bytearray(16)
        //    packet[0] = 1
        //    response = self.send_packet(0x6a, packet)
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err == 0:
        //      data = {}
        //      payload = self.decrypt(bytes(response[0x38:]))
        //      if type(payload[0x4]) == int:
        //        data['temperature'] = (payload[0x4] * 10 + payload[0x5]) / 10.0
        //        data['humidity'] = (payload[0x6] * 10 + payload[0x7]) / 10.0
        //        light = payload[0x8]
        //        air_quality = payload[0x0a]
        //        noise = payload[0xc]
        //      else:
        //        data['temperature'] = (ord(payload[0x4]) * 10 + ord(payload[0x5])) / 10.0
        //        data['humidity'] = (ord(payload[0x6]) * 10 + ord(payload[0x7])) / 10.0
        //        light = ord(payload[0x8])
        //        air_quality = ord(payload[0x0a])
        //        noise = ord(payload[0xc])
        //      if light == 0:
        //        data['light'] = 'dark'
        //      elif light == 1:
        //        data['light'] = 'dim'
        //      elif light == 2:
        //        data['light'] = 'normal'
        //      elif light == 3:
        //        data['light'] = 'bright'
        //      else:
        //        data['light'] = 'unknown'
        //      if air_quality == 0:
        //        data['air_quality'] = 'excellent'
        //      elif air_quality == 1:
        //        data['air_quality'] = 'good'
        //      elif air_quality == 2:
        //        data['air_quality'] = 'normal'
        //      elif air_quality == 3:
        //        data['air_quality'] = 'bad'
        //      else:
        //        data['air_quality'] = 'unknown'
        //      if noise == 0:
        //        data['noise'] = 'quiet'
        //      elif noise == 1:
        //        data['noise'] = 'normal'
        //      elif noise == 2:
        //        data['noise'] = 'noisy'
        //      else:
        //        data['noise'] = 'unknown'
        //      return data
        //
        //  def check_sensors_raw(self):
        //    packet = bytearray(16)
        //    packet[0] = 1
        //    response = self.send_packet(0x6a, packet)
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err == 0:
        //      data = {}
        //      payload = self.decrypt(bytes(response[0x38:]))
        //      if type(payload[0x4]) == int:
        //        data['temperature'] = (payload[0x4] * 10 + payload[0x5]) / 10.0
        //        data['humidity'] = (payload[0x6] * 10 + payload[0x7]) / 10.0
        //        data['light'] = payload[0x8]
        //        data['air_quality'] = payload[0x0a]
        //        data['noise'] = payload[0xc]
        //      else:
        //        data['temperature'] = (ord(payload[0x4]) * 10 + ord(payload[0x5])) / 10.0
        //        data['humidity'] = (ord(payload[0x6]) * 10 + ord(payload[0x7])) / 10.0
        //        data['light'] = ord(payload[0x8])
        //        data['air_quality'] = ord(payload[0x0a])
        //        data['noise'] = ord(payload[0xc])
        //      return data
        // 
        #endregion


        public A1(IPEndPoint host, byte[] mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
            
        }


        public Dictionary<string,double> CheckSensorsRaw()
        {
            byte[] packet = new byte[16];
            packet[0] = 1;
            byte[] response = SendPacket(0x6a, packet);
            int err = response[0x22] | (response[0x23] << 8);
            if (err == 0)
            {
                byte[] payload = Decrypt(response.Skip(0x38));

                return new Dictionary<string, double>()
                {
                    ["Temperature"] = (payload[0x4] * 10 + payload[0x5]) / 10.0,
                    ["Humidity"] = (payload[0x6] * 10 + payload[0x7]) / 10.0,
                    ["Light"] = payload[0x8],
                    ["AirQuality"] = payload[0x0a],
                    ["NoiseLevel"] = payload[0xc]
                };
            }
            else
            {
                throw new Exception($"Unexpected answer. Error code: {err}");
            };

        }

        public A1Data CheckSensors()
        {
            Dictionary<string, double> RawData = CheckSensorsRaw();

            return new A1Data()
            {
                Temperature = RawData["Temperature"],
                Humidity = RawData["Humidity"],
                Light=(!Enum.IsDefined(typeof(A1LightEnum), RawData["Light"])?A1LightEnum.Unknown:(A1LightEnum)RawData["Light"]),
                AirQuality = (!Enum.IsDefined(typeof(A1AirQualityEnum), RawData["AirQuality"]) ? A1AirQualityEnum.Unknown : (A1AirQualityEnum)RawData["AirQuality"]),
                NoiseLevel = (!Enum.IsDefined(typeof(A1NoiseLevelEnum), RawData["NoiseLevel"]) ? A1NoiseLevelEnum.Unknown : (A1NoiseLevelEnum)RawData["NoiseLevel"]),
            };
        }



    }
}

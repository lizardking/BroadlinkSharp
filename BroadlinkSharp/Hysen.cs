//Since I dont own this device the code in this class is not (yet) implemented.
//Org Python code below is based on https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace BroadlinkSharp
{
    [BroadlinkDevice(0x4EAD, "Hysen controller")]
    public class Hysen: BroadlinkDevice
    {

        #region Org Code from https://github.com/mjg59/python-broadlink/blob/master/broadlink/__init__.py
        //class hysen(device):
        //  def __init__ (self, host, mac, devtype):
        //    device.__init__(self, host, mac, devtype)
        //    self.type = "Hysen heating controller"
        //
        //  # Send a request
        //  # input_payload should be a bytearray, usually 6 bytes, e.g. bytearray([0x01,0x06,0x00,0x02,0x10,0x00]) 
        //  # Returns decrypted payload
        //  # New behaviour: raises a ValueError if the device response indicates an error or CRC check fails
        //  # The function prepends length (2 bytes) and appends CRC
        //  def send_request(self,input_payload):
        //    
        //    from PyCRC.CRC16 import CRC16
        //    crc = CRC16(modbus_flag=True).calculate(bytes(input_payload))
        //
        //    # first byte is length, +2 for CRC16
        //    request_payload = bytearray([len(input_payload) + 2,0x00])
        //    request_payload.extend(input_payload)
        //    
        //    # append CRC
        //    request_payload.append(crc & 0xFF)
        //    request_payload.append((crc >> 8) & 0xFF)
        //
        //    # send to device
        //    response = self.send_packet(0x6a, request_payload)
        //
        //    # check for error
        //    err = response[0x22] | (response[0x23] << 8)
        //    if err: 
        //      raise ValueError('broadlink_response_error',err)
        //    
        //    response_payload = bytearray(self.decrypt(bytes(response[0x38:])))
        //
        //    # experimental check on CRC in response (first 2 bytes are len, and trailing bytes are crc)
        //    response_payload_len = response_payload[0]
        //    if response_payload_len + 2 > len(response_payload):
        //      raise ValueError('hysen_response_error','first byte of response is not length')
        //    crc = CRC16(modbus_flag=True).calculate(bytes(response_payload[2:response_payload_len]))
        //    if (response_payload[response_payload_len] == crc & 0xFF) and (response_payload[response_payload_len+1] == (crc >> 8) & 0xFF):
        //      return response_payload[2:response_payload_len]
        //    else: 
        //      raise ValueError('hysen_response_error','CRC check on response failed')
        //      
        //
        //  # Get current room temperature in degrees celsius
        //  def get_temp(self):
        //    payload = self.send_request(bytearray([0x01,0x03,0x00,0x00,0x00,0x08]))
        //    return payload[0x05] / 2.0
        //
        //  # Get current external temperature in degrees celsius
        //  def get_external_temp(self):
        //    payload = self.send_request(bytearray([0x01,0x03,0x00,0x00,0x00,0x08]))
        //    return payload[18] / 2.0
        //
        //  # Get full status (including timer schedule)
        //  def get_full_status(self):
        //    payload = self.send_request(bytearray([0x01,0x03,0x00,0x00,0x00,0x16]))    
        //    data = {}
        //    data['remote_lock'] =  payload[3] & 1
        //    data['power'] =  payload[4] & 1
        //    data['active'] =  (payload[4] >> 4) & 1
        //    data['temp_manual'] =  (payload[4] >> 6) & 1
        //    data['room_temp'] =  (payload[5] & 255)/2.0
        //    data['thermostat_temp'] =  (payload[6] & 255)/2.0
        //    data['auto_mode'] =  payload[7] & 15
        //    data['loop_mode'] =  (payload[7] >> 4) & 15
        //    data['sensor'] = payload[8]
        //    data['osv'] = payload[9]
        //    data['dif'] = payload[10]
        //    data['svh'] = payload[11]
        //    data['svl'] = payload[12]
        //    data['room_temp_adj'] = ((payload[13] << 8) + payload[14])/2.0
        //    if data['room_temp_adj'] > 32767:
        //      data['room_temp_adj'] = 32767 - data['room_temp_adj']
        //    data['fre'] = payload[15]
        //    data['poweron'] = payload[16]
        //    data['unknown'] = payload[17]
        //    data['external_temp'] = (payload[18] & 255)/2.0
        //    data['hour'] =  payload[19]
        //    data['min'] =  payload[20]
        //    data['sec'] =  payload[21]
        //    data['dayofweek'] =  payload[22]
        //    
        //    weekday = []
        //    for i in range(0, 6):
        //      weekday.append({'start_hour':payload[2*i + 23], 'start_minute':payload[2*i + 24],'temp':payload[i + 39]/2.0})
        //    
        //    data['weekday'] = weekday
        //    weekend = []
        //    for i in range(6, 8):
        //      weekend.append({'start_hour':payload[2*i + 23], 'start_minute':payload[2*i + 24],'temp':payload[i + 39]/2.0})
        //
        //    data['weekend'] = weekend
        //    return data
        //
        //  # Change controller mode
        //  # auto_mode = 1 for auto (scheduled/timed) mode, 0 for manual mode.
        //  # Manual mode will activate last used temperature.  In typical usage call set_temp to activate manual control and set temp.
        //  # loop_mode refers to index in [ "12345,67", "123456,7", "1234567" ]
        //  # E.g. loop_mode = 0 ("12345,67") means Saturday and Sunday follow the "weekend" schedule
        //  # loop_mode = 2 ("1234567") means every day (including Saturday and Sunday) follows the "weekday" schedule
        //  # The sensor command is currently experimental
        //  def set_mode(self, auto_mode, loop_mode,sensor=0):
        //    mode_byte = ( (loop_mode + 1) << 4) + auto_mode
        //    # print 'Mode byte: 0x'+ format(mode_byte, '02x')
        //    self.send_request(bytearray([0x01,0x06,0x00,0x02,mode_byte,sensor]))
        //
        //  # Advanced settings
        //  # Sensor mode (SEN) sensor = 0 for internal sensor, 1 for external sensor, 2 for internal control temperature, external limit temperature. Factory default: 0.
        //  # Set temperature range for external sensor (OSV) osv = 5..99. Factory default: 42C
        //  # Deadzone for floor temprature (dIF) dif = 1..9. Factory default: 2C
        //  # Upper temperature limit for internal sensor (SVH) svh = 5..99. Factory default: 35C
        //  # Lower temperature limit for internal sensor (SVL) svl = 5..99. Factory default: 5C
        //  # Actual temperature calibration (AdJ) adj = -0.5. Prescision 0.1C
        //  # Anti-freezing function (FrE) fre = 0 for anti-freezing function shut down, 1 for anti-freezing function open. Factory default: 0
        //  # Power on memory (POn) poweron = 0 for power on memory off, 1 for power on memory on. Factory default: 0
        //  def set_advanced(self, loop_mode, sensor, osv, dif, svh, svl, adj, fre, poweron):
        //    input_payload = bytearray([0x01,0x10,0x00,0x02,0x00,0x05,0x0a, loop_mode, sensor, osv, dif, svh, svl, (int(adj*2)>>8 & 0xff), (int(adj*2) & 0xff), fre, poweron])
        //    self.send_request(input_payload)
        //
        //  # For backwards compatibility only.  Prefer calling set_mode directly.  Note this function invokes loop_mode=0 and sensor=0.
        //  def switch_to_auto(self):
        //    self.set_mode(auto_mode=1, loop_mode=0)
        //  
        //  def switch_to_manual(self):
        //    self.set_mode(auto_mode=0, loop_mode=0)
        //
        //  # Set temperature for manual mode (also activates manual mode if currently in automatic)
        //  def set_temp(self, temp):
        //    self.send_request(bytearray([0x01,0x06,0x00,0x01,0x00,int(temp * 2)]) )
        //
        //  # Set device on(1) or off(0), does not deactivate Wifi connectivity.  Remote lock disables control by buttons on thermostat.
        //  def set_power(self, power=1, remote_lock=0):
        //    self.send_request(bytearray([0x01,0x06,0x00,0x00,remote_lock,power]) )
        //
        //  # set time on device
        //  # n.b. day=1 is Monday, ..., day=7 is Sunday
        //  def set_time(self, hour, minute, second, day):
        //    self.send_request(bytearray([0x01,0x10,0x00,0x08,0x00,0x02,0x04, hour, minute, second, day ]))
        //
        //  # Set timer schedule
        //  # Format is the same as you get from get_full_status.
        //  # weekday is a list (ordered) of 6 dicts like:
        //  # {'start_hour':17, 'start_minute':30, 'temp': 22 }
        //  # Each one specifies the thermostat temp that will become effective at start_hour:start_minute
        //  # weekend is similar but only has 2 (e.g. switch on in morning and off in afternoon)
        //  def set_schedule(self,weekday,weekend):
        //    # Begin with some magic values ...
        //    input_payload = bytearray([0x01,0x10,0x00,0x0a,0x00,0x0c,0x18])
        //
        //    # Now simply append times/temps
        //    # weekday times
        //    for i in range(0, 6):
        //      input_payload.append( weekday[i]['start_hour'] )
        //      input_payload.append( weekday[i]['start_minute'] )
        //
        //    # weekend times
        //    for i in range(0, 2):
        //      input_payload.append( weekend[i]['start_hour'] )
        //      input_payload.append( weekend[i]['start_minute'] )
        //
        //    # weekday temperatures
        //    for i in range(0, 6):
        //      input_payload.append( int(weekday[i]['temp'] * 2) )
        //
        //    # weekend temperatures
        //    for i in range(0, 2):
        //      input_payload.append( int(weekend[i]['temp'] * 2) )
        //
        //    self.send_request(input_payload)
        //
        //
        //S1C_SENSORS_TYPES = {
        //    0x31: 'Door Sensor',  # 49 as hex
        //    0x91: 'Key Fob',  # 145 as hex, as serial on fob corpse
        //    0x21: 'Motion Sensor'  # 33 as hex
        //} 
        #endregion


        public Hysen(IPEndPoint host, PhysicalAddress mac, int devtype, int timeout = 10) : base(host, mac, devtype, timeout)
        {
            throw new NotImplementedException($"The logic for the {nameof(Hysen)} class has not (yet) been implemented");
        }


    
   

    }
}

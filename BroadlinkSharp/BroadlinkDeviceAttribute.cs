using System;
using System.Collections.Generic;
using System.Text;

namespace BroadlinkSharp
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=true,Inherited=true)]
    public class BroadlinkDeviceAttribute : Attribute
    {
        public int DeviceTypeCode { get; set; }
        public string DeviceTypeDescription { get; set; }

        public BroadlinkDeviceAttribute(int DeviceTypeCode, string DeviceTypeDescription)
        {
            this.DeviceTypeCode = DeviceTypeCode;
            this.DeviceTypeDescription = DeviceTypeDescription;
        }

    }
}

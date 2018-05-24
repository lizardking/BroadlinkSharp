using System;
using System.Collections.Generic;
using System.Text;

namespace BroadlinkSharp
{
    /// <summary>
    /// This attribute is used to assign device type codes and descriptions to the device classes.
    /// </summary>
    /// <seealso cref="System.Attribute" />
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

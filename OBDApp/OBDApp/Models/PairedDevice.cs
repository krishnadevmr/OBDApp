using System;
using System.Collections.Generic;
using System.Text;

namespace OBDApp.Models
{
    public class PairedDevice
    {
        public string DeviceName { get; set; }
        public string DeviceAddress { get; set; }

        public PairedDevice()
        {

        }

        public PairedDevice(string name, string address) {
            DeviceName = name;
            DeviceAddress = address;
        }
    }
}

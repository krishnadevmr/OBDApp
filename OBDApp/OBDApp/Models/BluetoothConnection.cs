using System;
using System.Collections.Generic;
using System.Text;

using Android.Bluetooth;

namespace OBDApp.Models
{
    public class BluetoothConnection
    {
        public BluetoothConnection() { }

        public string deviceName { get; set; }
        public BluetoothSocket bluetoothSocket { get; set; }
    }
}

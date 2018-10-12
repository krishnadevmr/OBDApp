using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using OBDApp.Models;

using Android.Bluetooth;
using Xamarin.Forms;



namespace OBDApp.ViewModels
{
    public class BluetoothConnectionHandler
    {
        public BluetoothConnectionHandler()
        {

        }

        public static BluetoothConnection bluetoothConnection;

        public Command getBluetoothConnection {
            get
            {
                return new Command(async () =>{
                    Debug.WriteLine("************ ENTERS CH FUNCTION *************");
                    bluetoothConnection = ConnectToBluetoothDevice();
                    CommunicationHandler ch = new CommunicationHandler();
                    await ch.SetupOBD(bluetoothConnection.bluetoothSocket);
                    Debug.WriteLine("OBD setup successfull");
                });
            }
        }

        public BluetoothConnection ConnectToBluetoothDevice()
        {
            bluetoothConnection = new BluetoothConnection();
            bluetoothConnection.deviceName = "SESTO-L1808016";

            try
            {
  
                BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                BluetoothDevice bluetoothDevice = (from device in bluetoothAdapter.BondedDevices
                                                   where device.Name == bluetoothConnection.deviceName
                                                   select device).FirstOrDefault();

                BluetoothSocket bluetoothSocket = bluetoothDevice.CreateInsecureRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                bluetoothSocket.Connect();
                bluetoothConnection.bluetoothSocket = bluetoothSocket;

                return bluetoothConnection;
            }
            catch(Exception) {
                throw;
            }
        }
    }
}

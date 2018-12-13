using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using OBDApp.Models;

using Android.Bluetooth;
using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OBDApp.ViewModels
{
    public class BluetoothConnectionHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static string SelectedBluetoothDevice;

        public List<PairedDevice> devList { get; set; }
        public PairedDevice _selectedDevName; //{ get; set; }

        protected virtual void onPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine("************************* ON PROPERTY CHANGE "+ propertyName);
        }

        public BluetoothConnectionHandler()
        {
            devList = PopulateDevicesList();
        }

        public string _selectedDevice;

        public string selectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                onPropertyChanged();
            }
        }
       

        public PairedDevice selectedDevName
        {
            get { return _selectedDevName; }
            set
            {
                if (_selectedDevName != value)
                {
                    _selectedDevName = value;
                    HandleSelectedItem();
                    
                }
            }
        }

        private void HandleSelectedItem()
        {
            Debug.WriteLine(selectedDevName.DeviceName);
            Debug.WriteLine(selectedDevName.DeviceAddress);
            SelectedBluetoothDevice = selectedDevName.DeviceName;
            MessagingCenter.Send(this,"dev");
            selectedDevice = selectedDevName.DeviceName;

        }

        public List<PairedDevice> PopulateDevicesList()
        {
            try
            {

                BluetoothAdapter btAdapter = BluetoothAdapter.DefaultAdapter;
                var btDevicesList = btAdapter.BondedDevices;
                //List<PairedDevice> BondedDeviceNames = new List<PairedDevice>();
                List<PairedDevice> BondedDeviceNames = new List<PairedDevice>();

                foreach (BluetoothDevice device in btDevicesList)
                {
                    BondedDeviceNames.Add(new PairedDevice(device.Name, device.Address));
                    Debug.WriteLine(device.Name);
                }

                Debug.WriteLine("List fill succeded...........................................");
                return BondedDeviceNames;
            }
            catch (Exception)
            {
                Debug.WriteLine("List fill failed...........................................");
                return null;
            }

        }

        public static BluetoothConnection bluetoothConnection;

        public Command getBluetoothConnectionCommand {
            get
            {
                return new Command(async () =>{
                    Debug.WriteLine("************ ENTERS CH FUNCTION *************");
                    bluetoothConnection = ConnectToBluetoothDevice();
                    if (bluetoothConnection != null)
                    {
                        CommunicationHandler ch = new CommunicationHandler();

                        await ch.SetupOBD(bluetoothConnection.bluetoothSocket);
                        Debug.WriteLine("OBD setup successfull");
                        //DataHandler dh = new DataHandler();
                        //dh.MessageListerener();
                        MessagingCenter.Send(this, Constants.OBD_SETUP_SUCCESSFULL);

                        
                    }

                });
            }
        }


        public BluetoothConnection ConnectToBluetoothDevice()
        {
            bluetoothConnection = new BluetoothConnection();
            //bluetoothConnection.deviceName = "SESTO-L1808016";
            if (SelectedBluetoothDevice == null) {
                Debug.WriteLine("******** NO device selected ***********");
                MessagingCenter.Send(this, Constants.MESSAGE_DEVICE_SELECTED);
                return null;
                    }
            bluetoothConnection.deviceName = SelectedBluetoothDevice;
            
            try
            {
                Debug.WriteLine("CONNECTION 1");
                BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                Debug.WriteLine("CONNECTION 2");
                BluetoothDevice bluetoothDevice = (from device in bluetoothAdapter.BondedDevices
                                                   where device.Name == bluetoothConnection.deviceName
                                                   select device).FirstOrDefault();
                Debug.WriteLine("CONNECTION 3");
                BluetoothSocket bluetoothSocket = bluetoothDevice.CreateInsecureRfcommSocketToServiceRecord(Java.Util.UUID.FromString(Constants.UUID));
                Debug.WriteLine("CONNECTION 4");
                bluetoothSocket.Connect();
                Debug.WriteLine("CONNECTION 5");
                bluetoothConnection.bluetoothSocket = bluetoothSocket;

                return bluetoothConnection;
            }
            catch(Exception) {
                throw;
            } 
        }
    }
}

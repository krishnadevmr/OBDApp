using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Strados.Obd;
using OBDApp.Models;

using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OBDApp.ViewModels
{
    public class DataHandler : INotifyPropertyChanged
    {
        public DataHandler() {
            MessageListerener();
        }



        private ObdParameters _obdParameters;

        public static bool isRunning = false;

        public ObdParameters obdParameters
        {
            get { return _obdParameters; }
            set
            {
                _obdParameters = value;
                onPropertyChanged();
            }
        }

        public static Dictionary<string,string> pidDictionary = Models.ObdParameters.createPIDdictionary();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void onPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /*         public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void onPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/

        public Command getAllObdDataCommand
        {
            get
            {
                return new Command(async () => {
                    isRunning = true;
                    WebServicesHandler wsh = new WebServicesHandler();
                    while (isRunning) {

                        try
                        {
                            obdParameters = await getAllCurrentDataAsync();
                        }
                        catch (Exception ex)
                        {
                            if (ex is Java.IO.IOException || ex is System.ArgumentException)
                            {
                                Debug.WriteLine("Exception in command data is: " + ex.Message);
                                DataHandler.isRunning = false;
                            }
                            throw;
                        }

                        wsh.SaveRecordsUsingRest(obdParameters);
                    }
                    wsh.GetRecordsRest();
                });
            }
        }



        public void MessageListerener()
        {
            MessagingCenter.Subscribe<BluetoothConnectionHandler>(this, Constants.OBD_SETUP_SUCCESSFULL, async (s) => {
                isRunning = true;
                Debug.WriteLine("*************** WORKS WITH MESSAGE *************': " );
                WebServicesHandler wsh = new WebServicesHandler();
                while (isRunning)
                {

                    try
                    {
                        obdParameters = await getAllCurrentDataAsync();
                    }
                    catch (Exception ex)
                    {
                        if (ex is Java.IO.IOException || ex is System.ArgumentException)
                        {
                            Debug.WriteLine("Exception in command data is: " + ex.Message);
                            DataHandler.isRunning = false;
                        }
                        throw;
                    }

                    //wsh.SaveRecordsUsingRest(obdParameters);
                }
                wsh.GetRecordsRest();
            });
        }


        public Command StopExtractinDataCommand
        {
            get
            {
                return new Command(() => {
                    stopextractingData();
                });
            }
        }

        public void stopextractingData()
        {
            isRunning = false;

        }

        public async Task<ObdParameters> getAllCurrentDataAsync()
        {

            ObdParameters obdParameters = new ObdParameters();
            CommunicationHandler ch = new CommunicationHandler();
            StringBuilder sb = new StringBuilder();

            try { 
            string pidCode = pidDictionary["RPM"];
            obdParameters.Rpm = await ch.getData(BluetoothConnectionHandler.bluetoothConnection.bluetoothSocket, pidCode);
            sb.Append("RPM: " + obdParameters.Rpm);


            pidCode = pidDictionary["Speed"];
            obdParameters.Speed = await ch.getData(BluetoothConnectionHandler.bluetoothConnection.bluetoothSocket, pidCode);
            sb.Append("Speed: " + obdParameters.Speed);


            pidCode = pidDictionary["EngineTemperature"];
            obdParameters.engineCoolantTemperature = await ch.getData(BluetoothConnectionHandler.bluetoothConnection.bluetoothSocket, pidCode);
            sb.Append("EngineTemperature: " + obdParameters.engineCoolantTemperature);


            pidCode = pidDictionary["throttlePosition"];
            obdParameters.throttlePosition = await ch.getData(BluetoothConnectionHandler.bluetoothConnection.bluetoothSocket, pidCode);
            sb.Append("Throttle Position: " + obdParameters.throttlePosition);


            pidCode = pidDictionary["massAirflow"];
            obdParameters.massAirflow = await ch.getData(BluetoothConnectionHandler.bluetoothConnection.bluetoothSocket, pidCode);
            sb.Append("Mass Airflow: " + obdParameters.massAirflow);
            Debug.WriteLine(sb.ToString());

             //pidCode = pidDictionary["VIN"];
             //obdParameters.VIN = await ch.getData(BluetoothConnectionHandler.bluetoothConnection.bluetoothSocket, pidCode);
             //sb.Append("VIN: " + obdParameters.VIN);
             //Debug.WriteLine(sb.ToString());

                return obdParameters;
            }
            catch (Exception ex)
            {
                if (ex is Java.IO.IOException || ex is System.ArgumentException) { 
                Debug.WriteLine("Exception in get all data is: " + ex.Message);
                DataHandler.isRunning = false;
                }
                throw;
            }


        }


        public string testStrados(string testdata)
        {
            try
            {
                Debug.WriteLine("Strados string input:" + testdata);
                string ParsableTestdata = TestDeviceOutput(testdata);
                
                if (!string.IsNullOrEmpty(ParsableTestdata)) {
                    Debug.WriteLine("Parsable string input:" + ParsableTestdata);
                    ObdResult obdResult = ObdParser.Parse(ParsableTestdata);
                    Debug.WriteLine(obdResult.Mode);
                    Debug.WriteLine(obdResult.Command);
                    Debug.WriteLine(obdResult.Name);
                    Debug.WriteLine(obdResult.Value);
                    return obdResult.Value.ToString();
                }
                else {
                    Debug.WriteLine("***** UNParseable data *****"+testdata);
                    return "UnParseable data";
                }

            }
            catch (Exception ex)
            {
                if (ex is System.ArgumentException || ex is Strados.Obd.Exceptions.ObdBadCommandException) {
                    Debug.WriteLine("Strados Exception: " + ex.Message);
                }
                
                throw;
            }
        }

        public string TestDeviceOutput(string StreamData)
        {
            string  ParseableString = String.Empty;
            bool startReading = false;
            foreach(char c in StreamData)
            {
                if(c == '>') {
                    Debug.WriteLine("Encountered Ending sign '>' in OBD output: "+ StreamData);
                }
                else if (c == '4' && !startReading)
                {
                    ParseableString += c;
                    startReading = true;
                }
                else if (startReading)
                { ParseableString += c; }
            }

            return ParseableString;
        }


    }

    
}

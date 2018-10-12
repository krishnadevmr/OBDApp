using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Strados.Obd;
using OBDApp.Models;

using Xamarin.Forms;



namespace OBDApp.ViewModels
{
    public class DataHandler
    {
        public DataHandler() { }

        public static Dictionary<string,string> pidDictionary = Models.ObdParameters.createPIDdictionary();

        public Command getAllObdData
        {
            get
            {
                return new Command(async () => {
                    Debug.WriteLine("************ ENTERS FUNCTION *************");
                    await getAllCurrentDataAsync();
                });
            }
        }

        public async Task<bool> getAllCurrentDataAsync()
        {

            ObdParameters obdParameters = new ObdParameters();
            CommunicationHandler ch = new CommunicationHandler();
            StringBuilder sb = new StringBuilder();

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
            //updateDisplayData(obdParameters);
            //bool wasSaved = await SaveItemAsync(obdParameters, true);
            //return wasSaved;
            return true;

        }


        public string testStrados(string testdata)
        {
            try
            {
                Debug.WriteLine("Strados string input:" + testdata);
                ObdResult obdResult = ObdParser.Parse(testdata);
                Debug.WriteLine(obdResult.Mode);
                Debug.WriteLine(obdResult.Command);
                Debug.WriteLine(obdResult.Name);
                Debug.WriteLine(obdResult.Value);
                return obdResult.Value.ToString();
            }
            catch (System.ArgumentException ex)
            {
                throw;
            }
        }


    }

    
}

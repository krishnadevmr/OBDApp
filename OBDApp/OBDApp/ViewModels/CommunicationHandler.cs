using System;
using System.Collections.Generic;
using System.Text;
using Android.Bluetooth;
using System.Diagnostics;

using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace OBDApp.ViewModels
{
    public class CommunicationHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<bool> SetupOBD(BluetoothSocket bluetoothSocket)
        {
            // Create Commands for setting up the OBD
            //Thread.Sleep(150);
            //byte[] initCheck = await readResponse(bluetoothSocket);
            //Debug.WriteLine("Flush Response is: " + Encoding.ASCII.GetString(initCheck));

            // Reset
            byte[] Command = Encoding.ASCII.GetBytes("ATZ\r");
            await SendSetupCommands(bluetoothSocket, Command, "Reset");

            /*
            await sendCommand(bluetoothSocket, Command);
            byte[] c1response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Reset Response is: " + Encoding.ASCII.GetString(c1response));
            Thread.Sleep(100);*/

            // Set Defaults
            byte[] Command2 = Encoding.ASCII.GetBytes("ATD\r");
            await SendSetupCommands(bluetoothSocket, Command2, "Default");
            /*
            await sendCommand(bluetoothSocket, Command2);
            byte[] c2response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Default Response is: " + Encoding.ASCII.GetString(c2response));
            Thread.Sleep(100); */

            // Disable Echo
            byte[] Command3 = Encoding.ASCII.GetBytes("AT E0\r");
            await SendSetupCommands(bluetoothSocket, Command3, "Echo");
            /*
            await sendCommand(bluetoothSocket, Command3);
            byte[] c3response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Echo Response is: " + Encoding.ASCII.GetString(c3response));
            Thread.Sleep(100); */

            // Disable Linefeed
            byte[] Command4 = Encoding.ASCII.GetBytes("AT L0\r");
            await SendSetupCommands(bluetoothSocket, Command4, "Linefeed");
            /*
            await sendCommand(bluetoothSocket, Command4);
            byte[] c4response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Linefeed Response is: " + Encoding.ASCII.GetString(c4response));
            Thread.Sleep(100); */

            // Disable spaces
            byte[] Command5 = Encoding.ASCII.GetBytes("AT S0\r");
            await SendSetupCommands(bluetoothSocket, Command5, "Spaces");
            /*
            await sendCommand(bluetoothSocket, Command5);
            byte[] c5response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Spaces Response is: " + Encoding.ASCII.GetString(c5response)); */

            // Flush any remaining output
            //Thread.Sleep(150);
            //byte[] flushAwait = await readResponse(bluetoothSocket);
            //Debug.WriteLine("Flush Response is: " + Encoding.ASCII.GetString(flushAwait));

            return true;
        }

        // Used to send set up commands to the OBD.
        // Takes ASCII input for the command
        public async Task<bool> SendSetupCommands(BluetoothSocket bluetoothSocket, byte[] setupCommand, string commandType)
        {
            await sendCommand(bluetoothSocket, setupCommand);
            byte[] response = await readResponse(bluetoothSocket);
            Debug.WriteLine(commandType+" Response is: " + Encoding.ASCII.GetString(response));
            Thread.Sleep(150);
            return true;
        }


        // Opens up an outputstream to a connected socket.
        // Writes the ASCII encoded data into the stream.
        public async Task<bool> sendCommand(BluetoothSocket bluetoothSocket, byte[] command)
        {
            try
            {
                await bluetoothSocket.OutputStream.WriteAsync(command, 0, command.Length);
                bluetoothSocket.OutputStream.Flush();
                return true;
            }
            catch (Java.IO.IOException e1)
            {
                Debug.WriteLine("Exception in send command is: " + e1.Message);
                DataHandler.isRunning = false;
                throw;
            }
        }

        // Opens an inputstream to a connected socket.
        // Asynchronously reads data from the stream.
        public async Task<byte[]> readResponse(BluetoothSocket bluetoothSocket)
        {
            try
            {

                byte[] responseBuffer = new byte[50];
                await bluetoothSocket.InputStream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                return responseBuffer;
            }
            catch (Java.IO.IOException e1)
            {
                Debug.WriteLine("Exception in readResponse is: " + e1.Message);
                DataHandler.isRunning = false;
                throw;
            }
        }

        // Sends and recieves response from a connected socket.
        public async Task<string> getData(BluetoothSocket bluetoothSocket, string pidCode)
        {
            Debug.WriteLine("Entering get for " + pidCode);

            try { 
            byte[] speedPID = Encoding.ASCII.GetBytes(pidCode);
            bool isCommand1 = await sendCommand(bluetoothSocket, speedPID);
            Thread.Sleep(500);
            byte[] responseCode = await readResponse(bluetoothSocket);
            string response = Encoding.ASCII.GetString(responseCode);
            Debug.WriteLine("The response is: " + response);
            DataHandler dh = new DataHandler();
            string val = dh.testStrados(response);
            return val;
            }
            catch(Java.IO.IOException e1)
            {
                Debug.WriteLine("Exception in get data is: "+e1.Message);
                DataHandler.isRunning = false;
                throw;
            }
            catch (System.ArgumentException e1)
            {
                Debug.WriteLine("Exception in get data is: " + e1.Message);
                DataHandler.isRunning = false;
                throw;
            }
        }
    }
}

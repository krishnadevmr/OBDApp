using System;
using System.Collections.Generic;
using System.Text;
using Android.Bluetooth;
using System.Diagnostics;

using System.Threading;
using System.Threading.Tasks;

namespace OBDApp.ViewModels
{
    public class CommunicationHandler
    {

        public async Task<bool> SetupOBD(BluetoothSocket bluetoothSocket)
        {
            byte[] Command = Encoding.ASCII.GetBytes("ATZ\r");
            byte[] Command2 = Encoding.ASCII.GetBytes("ATD\r");
            byte[] Command3 = Encoding.ASCII.GetBytes("AT E0\r");
            byte[] Command4 = Encoding.ASCII.GetBytes("AT L0\r");
            byte[] Command5 = Encoding.ASCII.GetBytes("AT S0\r");


            bool isCommand1 = await sendCommand(bluetoothSocket, Command);
            byte[] c1response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Response1 is: " + Encoding.ASCII.GetString(c1response));
            Thread.Sleep(100);

            bool isCommand2 = await sendCommand(bluetoothSocket, Command2);
            byte[] c2response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Response2 is: " + Encoding.ASCII.GetString(c2response));
            Thread.Sleep(100);

            bool isCommand3 = await sendCommand(bluetoothSocket, Command3);
            byte[] c3response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Response3 is: " + Encoding.ASCII.GetString(c3response));
            Thread.Sleep(100);

            bool isCommand4 = await sendCommand(bluetoothSocket, Command4);
            byte[] c4response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Response4 is: " + Encoding.ASCII.GetString(c4response));
            Thread.Sleep(100);

            bool isCommand5 = await sendCommand(bluetoothSocket, Command5);
            byte[] c5response = await readResponse(bluetoothSocket);
            Debug.WriteLine("Response5 is: " + Encoding.ASCII.GetString(c5response));

            Thread.Sleep(100);
            byte[] flushAwait = await readResponse(bluetoothSocket);
            Debug.WriteLine("Flush Wait is: " + Encoding.ASCII.GetString(flushAwait));

            return true;
        }

        public async Task<bool> sendCommand(BluetoothSocket bluetoothSocket, byte[] command)
        {
            try
            {
                await bluetoothSocket.OutputStream.WriteAsync(command, 0, command.Length);
                bluetoothSocket.OutputStream.Flush();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> readResponse(BluetoothSocket bluetoothSocket)
        {
            try
            {

                byte[] responseBuffer = new byte[50];
                await bluetoothSocket.InputStream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                return responseBuffer;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> getData(BluetoothSocket bluetoothSocket, string pidCode)
        {
            Debug.WriteLine("Entering get for " + pidCode);


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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{

    class DMFPlatformUDP : IDMFPlatform
    {
        private UDP udpClient;

        public DMFPlatformUDP()
        {
            udpClient = new UDP();
        }

        /*
         * This method is colled at the end of every tick.
         * The method should return only when the DMF platform has applied all the previus changes. 
         * Thus, applying backpressure on the VM execution.
         */
        public void WaitForDMFPlatform()
        {
            System.Threading.Thread.Sleep(500); // Simulates backpressure from board for debug purposes.
            return;
        }

        /*
         * This method receives two lists of integers precifiing the ID of the electrodes to be set and cleared.
         * It is called at the end of every tick.
         */
        public void UpdateElectrodes(List<int> electrodesToClear, List<int> electrodesToSet)
        {
            // For now this just prints the action. 
            if (electrodesToClear.Count != 0)
            {
                Console.Write("Electrodes to clear: ");
                foreach (int electrode in electrodesToClear)
                {
                    Console.Write(electrode + " ");
                    string command = "clrel " + electrode.ToString();
                    udpClient.UDPSend(command);
                }
                Console.Write("\n");
            }

            if (electrodesToSet.Count != 0)
            {
                Console.Write("Electrodes to set: ");
                foreach (int electrode in electrodesToSet)
                {
                    Console.Write(electrode + " ");
                    string command = "setel " + electrode.ToString();
                    udpClient.UDPSend(command);
                }
                Console.Write("\n");
            }
        }

        /*
         * This method receives a dictionary the type (deviceAddress, writeValue).
         * The deviceAddress and the writeValue are the one passed in the BioAssembly instruction.
         * It is called at the end of every tick.
         */
        public void WriteDevices(Dictionary<int, int> devicesToWrite)
        {
           // For now this just prints the action. 
           if (devicesToWrite.Count != 0)
           {
               Console.Write("Write to device: ");
               foreach (KeyValuePair<int,int> device in devicesToWrite)
               {
                   int deviceAddress = device.Key;
                   int writeData = device.Value;
                   Console.Write("(A:" + deviceAddress + ",D:" + writeData + ") ");
               }
               Console.Write("\n");
           }
       }

        /*
         * This method receives a dictionary deviceToRead of the type (deviceAddress, readValue) by reference and 
         * a dictionary deviceToWrite of the type (deviceAddress, writeValue) by value.
         * For the deviceToRead dictionary, the deviceAddress is the one passed in the BioAssembly instruction, readValue is 0.
         * The function should read from device and populate the values of the dictionary.
         * For the deviceToWrite, the deviceAddress and the writeValue are the one passed in the BioAssembly instruction.
         * The method is called immediatly during code execution.
         * It must be blocking and should return only when the write/read operation is complete.
         * Implmentation should be READ before WRITE.
         */
        public void AsynchronousReadWriteDevices(ref Dictionary<int, int> devicesToAsyncRead, Dictionary<int, int> devicesToAsyncWrite)
        {
            // For now this just prints the action. 
            if (devicesToAsyncRead.Count != 0)
            {
                Console.Write("Asynchronous read to device: ");
                foreach (KeyValuePair<int, int> device in devicesToAsyncRead)
                {
                    int deviceAddress = device.Key;
                    int readData = 100; // Must be read from device
                    devicesToAsyncRead[deviceAddress] = readData;
                    Console.Write("(A:" + deviceAddress + ") ");
                }
                Console.Write("\n");
            }
                // For now this just prints the action. 
                if (devicesToAsyncWrite.Count != 0)
            {
                Console.Write("Asynchronous write to device: ");
                foreach (KeyValuePair<int, int> device in devicesToAsyncWrite)
                {
                    int deviceAddress = device.Key;
                    int writeData = device.Value;
                    Console.Write("(A:" + deviceAddress + ",D:" + writeData + ") ");
                }
                Console.Write("\n");
            }
        }

        public void ClearAllElectrodes()
        {
            throw new NotImplementedException();
        }
    }
}

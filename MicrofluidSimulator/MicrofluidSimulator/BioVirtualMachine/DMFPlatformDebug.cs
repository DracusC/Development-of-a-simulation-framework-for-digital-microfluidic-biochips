using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    class DMFPlatformDebug : IDMFPlatform
    {
        ILogger logger;

        public DMFPlatformDebug(ILogger logger)
        {
            this.logger = logger;
        }

        public void WaitForDMFPlatform()
        {
            logger.Debug("Waiting for DMF Platform.");
            System.Threading.Thread.Sleep(500); // Simulates backpressure from board for debug purposes.
        }


        public void UpdateElectrodes(List<int> electrodesToClear, List<int> electrodesToSet)
        {
            if (electrodesToClear.Count != 0)
            {
                string answer = "";
                answer += "Electrodes to clear: ";
                foreach (int electrode in electrodesToClear)
                {
                    answer += '\n' + electrode.ToString();
                }
                logger.Debug(answer);
            }
            if (electrodesToSet.Count != 0)
            {
                string answer = "";
                answer += "Electrodes to set: "; 
                foreach (int electrode in electrodesToSet)
                {
                    answer += '\n' + electrode.ToString();
                }
                logger.Debug(answer);
            }
        }

        public void ClearAllElectrodes()
        {
            logger.Debug("Clearing all electrodes.");
        }

        public void WriteDevices(Dictionary<int, int> devicesToWrite)
        {
            if (devicesToWrite.Count != 0)
            {
                string answer = "";
                answer += "Synchronous write to device: ";
                foreach (KeyValuePair<int, int> device in devicesToWrite)
                {
                    int deviceAddress = device.Key;
                    int writeData = device.Value;
                    answer += '\n' + String.Format("Device address: {0}, Write data: {1}", deviceAddress, writeData);
                }
                logger.Debug(answer);
            }
        }


        public void AsynchronousReadWriteDevices(ref Dictionary<int, int> devicesToAsyncRead, Dictionary<int, int> devicesToAsyncWrite)
        {
            if (devicesToAsyncRead.Count != 0)
            {
                string answer = "";
                answer += "Asynchronous read to device: ";
                foreach (KeyValuePair<int, int> device in devicesToAsyncRead)
                {
                    int deviceAddress = device.Key;
                    int readData = 100; // Must be read from device
                    devicesToAsyncRead[deviceAddress] = readData;
                    answer += '\n' + String.Format("Device address: {0}, Read data: N/A", deviceAddress);
                }
                logger.Debug(answer);
            }
            // For now this just prints the action. 
            if (devicesToAsyncWrite.Count != 0)
            {
                string answer = "";
                answer += "Asynchronous write to device: ";
                foreach (KeyValuePair<int, int> device in devicesToAsyncWrite)
                {
                    int deviceAddress = device.Key;
                    int writeData = device.Value;
                    answer += '\n' + String.Format("Device address: {0}, Write data: {1}", deviceAddress, writeData);
                }
                logger.Debug(answer);
            }
        }

        public void SetTickSize(decimal tickSize)
        {
            throw new NotImplementedException();
        }
    }
}

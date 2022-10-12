using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioVirtualMachine
{
    interface IDMFPlatform
    {
        /// <summary>
        /// This method is colled at the end of every tick.
        /// The method should return only when the DMF platform has applied all the previus changes.
        /// Thus, applying backpressure on the VM execution.
        /// </summary>
        void WaitForDMFPlatform();


        /// <summary>
        /// This method receives two lists of integers precifiing the ID of the electrodes to be set and cleared.
        /// It is called at the end of every tick.
        /// </summary>
        /// <param name="electrodesToClear"></param>
        /// <param name="electrodesToSet"></param>
        void UpdateElectrodes(List<int> electrodesToClear, List<int> electrodesToSet);
        

        /// <summary>
        /// This method clears all the electrodes of the DMF platform
        /// </summary>
        void ClearAllElectrodes();

        /// <summary>
        /// This method receives a dictionary the type (deviceAddress, writeValue).
        /// The deviceAddress and the writeValue are the one passed in the BioAssembly instruction.
        /// It is called at the end of every tick.
        /// </summary>
        /// <param name="devicesToWrite"></param>
        void WriteDevices(Dictionary<int, int> devicesToWrite);


         /// <summary>
         /// This method receives a dictionary deviceToRead of the type (deviceAddress, readValue) by reference and 
         /// a dictionary deviceToWrite of the type(deviceAddress, writeValue) by value.
         /// For the deviceToRead dictionary, the deviceAddress is the one passed in the BioAssembly instruction, readValue is 0.
         /// The function should read from device and populate the values of the dictionary.
         /// For the deviceToWrite, the deviceAddress and the writeValue are the one passed in the BioAssembly instruction.
         /// The method is called immediatly during code execution.
         /// It must be blocking and should return only when the write/read operation is complete.
         /// Implmentation should be READ before WRITE.
         /// </summary>
         /// <param name="devicesToAsyncRead"></param>
         /// <param name="devicesToAsyncWrite"></param>
         void AsynchronousReadWriteDevices(ref Dictionary<int, int> devicesToAsyncRead, Dictionary<int, int> devicesToAsyncWrite);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using MicrofluidSimulator.SimulatorCode.Simulator;

namespace BioVirtualMachine
{
    class DMFPlatform : IDMFPlatform
    {

        decimal time;
        Simulator simulator;
        public DMFPlatform(Simulator simulator)
        {
            this.time = 1;
            this.simulator = simulator;
        }

        public void WaitForDMFPlatform()
        {
            //Console.WriteLine("Waiting for DMF Platform.");
            //Console.WriteLine(this.time);
        }

        public void SetTickSize(decimal timeUpdate)
        {
            this.time = timeUpdate/1000;
        }
        public void UpdateElectrodes(List<int> electrodesToClear, List<int> electrodesToSet)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            // If they are not global ints, they have to be translated
            if (electrodesToClear.Count != 0)
            {
                string answer = "";
                answer += "Electrodes to clear: ";
                foreach (int electrode in electrodesToClear)
                {
                    actionQueueInstructions.Enqueue(ActionQueueGenerator.generateAction(electrode, 0, this.time));
                    
                    answer += '\n' + electrode.ToString();
                }
                //Console.WriteLine(answer);
            }
            if (electrodesToSet.Count != 0)
            {
                string answer = "";
                answer += "Electrodes to set: ";
                foreach (int electrode in electrodesToSet)
                {
                    actionQueueInstructions.Enqueue(ActionQueueGenerator.generateAction(electrode, 1, this.time));

                    answer += '\n' + electrode.ToString();
                }
                //Console.WriteLine(answer);
            }
            this.pushActionsAtTime(simulator.container.currentTime, actionQueueInstructions);
        }

        public void ClearAllElectrodes()
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            for (int i = 1; i < 725; i++)
            {
                actionQueueInstructions.Enqueue(ActionQueueGenerator.generateAction(i, 0, this.time));
            }

            Console.WriteLine("Clearing all electrodes.");
            this.pushActionsAtTime(simulator.container.currentTime, actionQueueInstructions);
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
                Console.WriteLine(answer);
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
                Console.WriteLine(answer);
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
                Console.WriteLine(answer);
            }
        }

        private void pushActionsAtTime(decimal time, Queue<ActionQueueItem> queueToPush)
        {
            if (simulator.container.currentTime == time)
            {

                Queue<ActionQueueItem> copyOfQueueToPush = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(queueToPush);


                foreach (ActionQueueItem item in copyOfQueueToPush)
                {
                    item.time += simulator.container.currentTime;
                    item.time = (decimal)Math.Round((Decimal)item.time, 3, MidpointRounding.AwayFromZero);
                }
                simulator.actionQueue = ActionQueueModels.pushActionQueueToOriginalActionQueue(simulator.actionQueue, copyOfQueueToPush);
            }
        }
    }
}

/*
 * This is a dumb version of the virtual machine, it is used for testing purposes,
 * since we do not have access to the actual virtual machine.
 */
using MicrofluidSimulator.SimulatorCode.Models;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Simulator;

namespace MicrofluidSimulator.DumbVM
{
    public class DumbVM
    {
        private Simulator simulator;
        private Queue<ActionQueueItem> queueToPush;

        public DumbVM(Simulator simulator)
        {
            this.simulator = simulator;
            this.queueToPush = generateTestQueue(simulator.container.currentTime);

        }   

        // Main loop
        public void doApiCall()
        {

            this.turnOnHeaterAtTime(10, 100, 727);
            Console.WriteLine("COLOR 1 " + this.readRGBValueOfColorSensorAtTime(simulator.container.currentTime, 725)[0]);
            if (Enumerable.SequenceEqual(this.readRGBValueOfColorSensorAtTime(simulator.container.currentTime, 725), new int[] {255, 66, 0 })){
                Console.WriteLine("READ COLOR " + 255 + ", " + 66 + ", " + 0);
                this.pushActionsAtTime(simulator.container.currentTime, this.queueToPush);
            }
            




        }

        private void pushActionsAtTime(float time, Queue<ActionQueueItem> queueToPush)
        {
            if (simulator.container.currentTime == time)
            {

                Queue<ActionQueueItem> copyOfQueueToPush = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(queueToPush);
                Queue<ActionQueueItem> copyOfQueueToPushForTimeCalculation = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(queueToPush);
                
                foreach(ActionQueueItem item in copyOfQueueToPush)
                {
                    item.time += simulator.container.currentTime;
                }
                pushActionQueueToSimulatorActionQueue(copyOfQueueToPush, copyOfQueueToPushForTimeCalculation);
            }
        }

        private void pushActionQueueToSimulatorActionQueue(Queue<ActionQueueItem> copyOfQueueToPush, Queue<ActionQueueItem> copyOfQueueToPushForTimeCalculation)
        {
            
            float accumulatedTime = 0;

            float min = Int32.MaxValue;
            float max = Int32.MinValue;
            for (int i = 0; i < copyOfQueueToPushForTimeCalculation.Count; i++)
            {

                float tempMin = copyOfQueueToPushForTimeCalculation.Dequeue().time;
                float tempMax = tempMin;


                if (tempMin < min)
                {
                    min = tempMin;
                }
                if (tempMax > max)
                {
                    max = tempMax;
                }

                accumulatedTime = max - min;
            }

            

            simulator.actionQueue = ActionQueueModels.pushActionQueueToStartOfOriginalActionQueue(simulator.actionQueue, copyOfQueueToPush, accumulatedTime);
            

        }

        private void turnOnHeaterAtTime(float time, float desiredTemperature, int heaterID)
        {
            float currentSimulatorTime = simulator.container.currentTime;
            if(currentSimulatorTime == time)
            {
                
                simulator.setActuatorTargetTemperature(heaterID, desiredTemperature);
            }
        }

        private int[] readRGBValueOfColorSensorAtTime(float time, int sensorID)
        {
            float currentSimulatorTime = simulator.container.currentTime;
            if(currentSimulatorTime == time)
            {
                return simulator.getColorOfSensorWithID(sensorID);
            }
            return new int[] { -1, -1, -1 };
        }
        private static Queue<ActionQueueItem> generateTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 100, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 0.1F+ currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 101, 1);
            ActionQueueItem item2 = new ActionQueueItem(action2, 0.1F+ currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 34, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 0.3F+ currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 2, 1);
            ActionQueueItem item4 = new ActionQueueItem(action4, 0.4F+ currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 2, 0);
            ActionQueueItem item5 = new ActionQueueItem(action5, 0.5F+ currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 100, 1);
            ActionQueueItem item6 = new ActionQueueItem(action6, 0.6F+ currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 102, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 0.7F+ currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 103, 1);
            ActionQueueItem item8 = new ActionQueueItem(action8, 0.8F+ currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 104, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 0.9F+ currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 101, 1);
            ActionQueueItem item10 = new ActionQueueItem(action10, 0.10F+ currentTime);
            actionQueueInstructions.Enqueue(item10);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 0.9F+ currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 132, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 100, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 131, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);

            action9 = new SimulatorAction("electrode", 99, 0);
            item9 = new ActionQueueItem(action9, 0.9F + currentTime);
            actionQueueInstructions.Enqueue(item9);






            return actionQueueInstructions;
        }

        
    }
}

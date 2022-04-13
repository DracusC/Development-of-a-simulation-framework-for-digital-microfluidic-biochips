/*
 * This is a dumb version of the virtual machine, it is used for testing purposes,
 * since we do not have access to the actual virtual machine.
 */
using System.Drawing;
using MicrofluidSimulator.SimulatorCode.Models;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Simulator;

namespace MicrofluidSimulator.DumbVM
{
    public class DumbVM
    {
        private Simulator simulator;
        private Queue<ActionQueueItem> redQueuePush;
        private Queue<ActionQueueItem> cyanQueuePush;
        
        private bool rgbSensorCalled;
        private bool heaterCalled;
        public DumbVM(Simulator simulator)
        {
            this.simulator = simulator;
            this.redQueuePush = generateRedTestQueue(simulator.container.currentTime);
            this.cyanQueuePush = generateBlueTestQueue(simulator.container.currentTime);
            
            this.heaterCalled = false;
            this.rgbSensorCalled = false;
        }

        
        
        // Main loop
        public void doApiCall()
        {
            Color red = ColorTranslator.FromHtml("#FF4200");
            Color cyan = ColorTranslator.FromHtml("#00FFFF");
            int[] redArray = new int[] { red.R, red.G, red.B };
            int[] cyanArray = new int[] { cyan.R, cyan.G, cyan.B };
            this.turnOnHeaterAtTime(10, 100, 727);

            int[] colorRead = this.readRGBValueOfColorSensorAtTime(simulator.container.currentTime, 725);
            if (Enumerable.SequenceEqual(colorRead, redArray)){
                
                this.pushActionsAtTime(simulator.container.currentTime, this.redQueuePush);
            }
            
            if (Enumerable.SequenceEqual(colorRead, cyanArray))
            {
                
                this.pushActionsAtTime(simulator.container.currentTime, this.cyanQueuePush);
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
            if(currentSimulatorTime >= time && heaterCalled == false)
            {
                heaterCalled = true;
                simulator.setActuatorTargetTemperature(heaterID, desiredTemperature);
            }else if(currentSimulatorTime < time)
            {
                
                heaterCalled = false;
            }
        }

        private int[] readRGBValueOfColorSensorAtTime(float time, int sensorID)
        {
            float currentSimulatorTime = simulator.container.currentTime;
            if(currentSimulatorTime == time && rgbSensorCalled == false)
            {
                int[] colorArray = simulator.getColorOfSensorWithID(sensorID);
                if(!Enumerable.SequenceEqual(colorArray, new int[] { -1, -1, -1 }) && !Enumerable.SequenceEqual(colorArray, new int[] { 0, 0, 0 }))
                {
                    rgbSensorCalled = true;
                }
                return colorArray;
            }
            rgbSensorCalled = false;
            return new int[] { -1, -1, -1 };
        }
        private static Queue<ActionQueueItem> generateRedTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 266, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1+ currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 298, 0);
            ActionQueueItem item2 = new ActionQueueItem(action2, 2+ currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 234, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3+ currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 266, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4+ currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 202, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5+ currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 234, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6+ currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 170, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7+ currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 202, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8+ currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 138, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9+ currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 170, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10+ currentTime);
            actionQueueInstructions.Enqueue(item10);

            return actionQueueInstructions;
        }

        private static Queue<ActionQueueItem> generateBlueTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 330, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1 + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 298, 0);
            ActionQueueItem item2 = new ActionQueueItem(action2, 2 + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 362, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3 + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 330, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4 + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 394, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5 + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 362, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6 + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 426, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7 + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 394, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8 + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 458, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9 + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 426, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10 + currentTime);
            actionQueueInstructions.Enqueue(item10);

            return actionQueueInstructions;
        }
    }
}

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
        private Queue<ActionQueueItem> temperatureQueuePush;

        private bool rgbSensorCalled;
        private bool temperatureSensorCalled;
        private bool heater1Called;
        private bool heater2Called;
        public DumbVM(Simulator simulator)
        {
            this.simulator = simulator;
            this.redQueuePush = generateRedTestQueue(simulator.container.currentTime);
            this.cyanQueuePush = generateBlueTestQueue(simulator.container.currentTime);
            this.temperatureQueuePush = generateTemperatureTestQueue(simulator.container.currentTime);

            this.heater1Called = false;
            this.heater2Called = false;
            this.rgbSensorCalled = false;
            this.temperatureSensorCalled = false;
        }

       



        // Main loop
        public void doApiCall()
        {
            Color red = ColorTranslator.FromHtml("#FF4200");
            Color cyan = ColorTranslator.FromHtml("#00FFFF");
            int[] redArray = new int[] { red.R, red.G, red.B };
            int[] cyanArray = new int[] { cyan.R, cyan.G, cyan.B };
            this.turnOnHeaterAtTime(10, 100, 727, heater1Called);
            this.turnOnHeaterAtTime(1, 100, 729, heater2Called);
            int[] colorRead = this.readRGBValueOfColorSensorAtTime(simulator.container.currentTime, 725);
            if (Enumerable.SequenceEqual(colorRead, redArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.redQueuePush);
                foreach (ActionQueueItem action in simulator.actionQueue)
                {
                    Console.WriteLine("ACTION " + action.action.actionName + ", " + action.action.actionOnID + ", " + action.time);
                }
            }

            if (Enumerable.SequenceEqual(colorRead, cyanArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.cyanQueuePush);
                foreach (ActionQueueItem action in simulator.actionQueue)
                {
                    Console.WriteLine("ACTION " + action.action.actionName + ", " + action.action.actionOnID + ", " + action.time);
                }
            }
            float temperature = this.readTemperatureOfTemperatureSensorAtTime(simulator.container.currentTime, 726);

            if(temperature >= 60){
                this.pushActionsAtTime(simulator.container.currentTime, this.temperatureQueuePush);
                foreach(ActionQueueItem action in simulator.actionQueue)
                {
                    Console.WriteLine("ACTION " + action.action.actionName + ", " + action.action.actionOnID + ", " + action.time);
                }
            }

        }

        private void pushActionsAtTime(float time, Queue<ActionQueueItem> queueToPush)
        {
            if (simulator.container.currentTime == time)
            {

                Queue<ActionQueueItem> copyOfQueueToPush = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(queueToPush);
                Queue<ActionQueueItem> copyOfQueueToPushForTimeCalculation = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(queueToPush);

                foreach (ActionQueueItem item in copyOfQueueToPush)
                {
                    item.time += simulator.container.currentTime;
                }
                pushActionQueueToSimulatorActionQueue(copyOfQueueToPush, copyOfQueueToPushForTimeCalculation);
            }
        }

        private void pushActionQueueToSimulatorActionQueue(Queue<ActionQueueItem> copyOfQueueToPush, Queue<ActionQueueItem> copyOfQueueToPushForTimeCalculation)
        {
            Console.WriteLine("COPY OF QUEUE ");
            foreach (ActionQueueItem action in copyOfQueueToPush)
            {
                Console.WriteLine("ACTION " + action.action.actionName + ", " + action.action.actionOnID + ", " + action.time);
            }
            Console.WriteLine("COPY OF QUEUE  FOR TIME ");
            foreach (ActionQueueItem action in copyOfQueueToPushForTimeCalculation)
            {
                Console.WriteLine("ACTION " + action.action.actionName + ", " + action.action.actionOnID + ", " + action.time);
            }
            float accumulatedTime = 0;


            float min = Int32.MaxValue;
            float max = Int32.MinValue;
            for (int i = 0; i < copyOfQueueToPushForTimeCalculation.Count; i++)
            {

                float tempMin = copyOfQueueToPushForTimeCalculation.ElementAt(i).time;
                float tempMax = tempMin;
                Console.WriteLine("MIN AND MAX" + tempMin);

                if (tempMin < min)
                {
                    min = tempMin;
                    Console.WriteLine("MIN TIME " + min);
                }
                if (tempMax > max)
                {
                    max = tempMax;
                    Console.WriteLine("MAX TIME " + max);
                }

                accumulatedTime = max - min;
            }
            Console.WriteLine("ACCUMULATED TIME " + accumulatedTime);


            simulator.actionQueue = ActionQueueModels.pushActionQueueToStartOfOriginalActionQueue(simulator.actionQueue, copyOfQueueToPush, accumulatedTime);


        }

        private void turnOnHeaterAtTime(float time, float desiredTemperature, int heaterID, bool heaterCalled)
        {
            float currentSimulatorTime = simulator.container.currentTime;
            if (currentSimulatorTime >= time && heaterCalled == false)
            {
                heaterCalled = true;
                simulator.setActuatorTargetTemperature(heaterID, desiredTemperature);
            }
            else if (currentSimulatorTime < time)
            {

                heaterCalled = false;
            }
        }

        private int[] readRGBValueOfColorSensorAtTime(float time, int sensorID)
        {
            float currentSimulatorTime = simulator.container.currentTime;
            if (currentSimulatorTime == time && rgbSensorCalled == false)
            {
                int[] colorArray = simulator.getColorOfSensorWithID(sensorID);
                if (!Enumerable.SequenceEqual(colorArray, new int[] { -1, -1, -1 }) && !Enumerable.SequenceEqual(colorArray, new int[] { 0, 0, 0 }))
                {
                    rgbSensorCalled = true;
                }
                return colorArray;
            }
            rgbSensorCalled = false;
            return new int[] { -1, -1, -1 };
        }

        private float readTemperatureOfTemperatureSensorAtTime(float time, int sensorID)
        {
            float currentSimulatorTime = simulator.container.currentTime;
            if (currentSimulatorTime == time && rgbSensorCalled == false)
            {
                float temperature = simulator.getTemperatureOfSensorWithID(sensorID);
                if (temperature != -1 && temperature != 20)
                {
                    Console.WriteLine("temperature == " + temperature);
                    temperatureSensorCalled = true;
                }
                Console.WriteLine("temperature -1  == -1");
                return temperature;
            }
            temperatureSensorCalled = false;
            return -1;
        }
        private static Queue<ActionQueueItem> generateRedTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 266, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1 + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 298, 0);
            ActionQueueItem item2 = new ActionQueueItem(action2, 2 + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 234, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3 + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 266, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4 + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 202, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5 + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 234, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6 + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 170, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7 + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 202, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8 + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 138, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9 + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 170, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10 + currentTime);
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
        private static Queue<ActionQueueItem> generateTemperatureTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 273, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1 + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 305, 0);
            ActionQueueItem item2 = new ActionQueueItem(action2, 2 + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 241, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3 + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 273, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4 + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 242, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5 + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 241, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6 + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 243, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7 + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 242, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8 + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 275, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9 + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 243, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10 + currentTime);
            actionQueueInstructions.Enqueue(item10);

            SimulatorAction action11 = new SimulatorAction("electrode", 307, 1);
            ActionQueueItem item11 = new ActionQueueItem(action11, 11 + currentTime);
            actionQueueInstructions.Enqueue(item11);

            SimulatorAction action12 = new SimulatorAction("electrode", 275, 0);
            ActionQueueItem item12 = new ActionQueueItem(action12, 12 + currentTime);
            actionQueueInstructions.Enqueue(item12);

            SimulatorAction action13 = new SimulatorAction("electrode", 306, 1);
            ActionQueueItem item13 = new ActionQueueItem(action13, 13 + currentTime);
            actionQueueInstructions.Enqueue(item13);

            SimulatorAction action14 = new SimulatorAction("electrode", 307, 0);
            ActionQueueItem item14 = new ActionQueueItem(action14, 14 + currentTime);
            actionQueueInstructions.Enqueue(item14);

            SimulatorAction action15 = new SimulatorAction("electrode", 305, 1);
            ActionQueueItem item15 = new ActionQueueItem(action15, 15 + currentTime);
            actionQueueInstructions.Enqueue(item15);

            SimulatorAction action16 = new SimulatorAction("electrode", 306, 0);
            ActionQueueItem item16 = new ActionQueueItem(action16, 16 + currentTime);
            actionQueueInstructions.Enqueue(item16);

            return actionQueueInstructions;
        }
    }
}
/*
 * This is a dumb version of the virtual machine, it is used for testing purposes,
 * since we do not have access to the actual virtual machine.
 */
using System.Drawing;
using MicrofluidSimulator.SimulatorCode.Models;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Simulator;

namespace MicrofluidSimulator.SimpleVM
{
    public class SimpleVM
    {
        private Simulator simulator;
        private Queue<ActionQueueItem> redQueuePush;
        private Queue<ActionQueueItem> cyanQueuePush;
        private Queue<ActionQueueItem> yellowQueuePush;
        private Queue<ActionQueueItem> orangeQueuePush;
        private Queue<ActionQueueItem> hotTemperatureQueuePush;
        private Queue<ActionQueueItem> coldTemperatureQueuePush;

        private bool rgbSensorCalled;
        private bool temperatureSensorCalled;
        private bool heater727Called;
        private bool heater729Called;
        public SimpleVM(Simulator simulator)
        {
            this.simulator = simulator;
            this.redQueuePush = generateRedTestQueue(simulator.container.currentTime);
            this.cyanQueuePush = generateCyanTestQueue(simulator.container.currentTime);
            this.yellowQueuePush = generateYellowTestQueue(simulator.container.currentTime);
            this.orangeQueuePush = generateOrangeTestQueue(simulator.container.currentTime);
            this.hotTemperatureQueuePush = generateHotTemperatureTestQueue(simulator.container.currentTime);
            this.coldTemperatureQueuePush = generateColdTemperatureTestQueue(simulator.container.currentTime);

            this.heater727Called = false;
            this.heater729Called = false;
            this.rgbSensorCalled = false;
            this.temperatureSensorCalled = false;
        }


        // Main loop
        public void doApiCall()
        {
            Color red = ColorTranslator.FromHtml("#FF4200");
            Color cyan = ColorTranslator.FromHtml("#00FFFF");
            Color yellow = ColorTranslator.FromHtml("#FFFF00");
            Color orange = ColorTranslator.FromHtml("#FFA500");
            int[] redArray = new int[] { red.R, red.G, red.B };
            int[] cyanArray = new int[] { cyan.R, cyan.G, cyan.B };
            int[] yellowArray = new int[] { yellow.R, yellow.G, yellow.B };
            int[] orangeArray = new int[] { orange.R, orange.G, orange.B };
            this.turnOnHeaterAtTime(10, 70, 727, heater727Called);
            this.turnOnHeaterAtTime(1, 70, 729, heater729Called);
            int[] colorRead = this.readRGBValueOfColorSensorAtTime(simulator.container.currentTime, 725);
            if (Enumerable.SequenceEqual(colorRead, redArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.redQueuePush);
                
            }
            if (Enumerable.SequenceEqual(colorRead, cyanArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.cyanQueuePush);

            }
            if (Enumerable.SequenceEqual(colorRead, yellowArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.yellowQueuePush);

            }

            if (Enumerable.SequenceEqual(colorRead, orangeArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.orangeQueuePush);

            }

            float temperatureHot = this.readTemperatureOfTemperatureSensorAtTime(simulator.container.currentTime, 726);

            if(temperatureHot >= 60){
                this.pushActionsAtTime(simulator.container.currentTime, this.hotTemperatureQueuePush);
                
            }

            float temperatureCold = this.readTemperatureOfTemperatureSensorAtTime(simulator.container.currentTime, 727);

            if (temperatureCold <= 30 && temperatureCold >= 20)
            {
                this.pushActionsAtTime(simulator.container.currentTime, this.coldTemperatureQueuePush);
                
            }

        }

        private void pushActionsAtTime(float time, Queue<ActionQueueItem> queueToPush)
        {
            if (simulator.container.currentTime == time)
            {

                Queue<ActionQueueItem> copyOfQueueToPush = HelpfullRetreiveFunctions.createDeepCopyOfActionQueue(queueToPush);
                

                foreach (ActionQueueItem item in copyOfQueueToPush)
                {
                    item.time += simulator.container.currentTime;
                    item.time = (float)Math.Round((Decimal)item.time, 3, MidpointRounding.AwayFromZero);
                }
                simulator.actionQueue = ActionQueueModels.pushActionQueueToOriginalActionQueue(simulator.actionQueue, copyOfQueueToPush);
            }
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
            if (currentSimulatorTime == time && temperatureSensorCalled == false)
            {
                float temperature = simulator.getTemperatureOfSensorWithID(sensorID);
                if (temperature != -1 && temperature != 20)
                {
                    
                    temperatureSensorCalled = true;
                }
               
                return temperature;
            }
            temperatureSensorCalled = false;
            return -1;
        }
        private static Queue<ActionQueueItem> generateRedTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new();
            SimulatorAction action1 = new("electrode", 266, 1);
            ActionQueueItem item1 = new(action1, 0.2f + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new("electrode", 298, 0);
            ActionQueueItem item2 = new(action2, 0.4f + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new("electrode", 234, 1);
            ActionQueueItem item3 = new(action3, 0.6f + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 266, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 0.8f + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 202, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 1f + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 234, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 1.2f + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 170, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 1.4f + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 202, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 1.6f + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 138, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 1.8f + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 170, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 2f + currentTime);
            actionQueueInstructions.Enqueue(item10);

            SimulatorAction action11 = new SimulatorAction("electrode", 139, 1);
            ActionQueueItem item11 = new ActionQueueItem(action11, 2.2f + currentTime);
            actionQueueInstructions.Enqueue(item11);

            SimulatorAction action12 = new SimulatorAction("electrode", 138, 0);
            ActionQueueItem item12 = new ActionQueueItem(action12, 2.4f + currentTime);
            actionQueueInstructions.Enqueue(item12);

            SimulatorAction action13 = new SimulatorAction("electrode", 140, 1);
            ActionQueueItem item13 = new ActionQueueItem(action13, 2.6f + currentTime);
            actionQueueInstructions.Enqueue(item13);

            SimulatorAction action14 = new SimulatorAction("electrode", 139, 0);
            ActionQueueItem item14 = new ActionQueueItem(action14, 2.8f + currentTime);
            actionQueueInstructions.Enqueue(item14);

            return actionQueueInstructions;
        }

        private static Queue<ActionQueueItem> generateCyanTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 330, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 0.2f + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 298, 0);
            ActionQueueItem item2 = new ActionQueueItem(action2, 0.4f + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 362, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 0.6f + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 330, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 0.8f + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 394, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 1f + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 362, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 1.2f + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 426, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 1.4f + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 394, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 1.6f + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 458, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 1.8f + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 426, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 2f + currentTime);
            actionQueueInstructions.Enqueue(item10);

            SimulatorAction action11 = new SimulatorAction("electrode", 459, 1);
            ActionQueueItem item11 = new ActionQueueItem(action11, 2.2f + currentTime);
            actionQueueInstructions.Enqueue(item11);

            SimulatorAction action12 = new SimulatorAction("electrode", 458, 0);
            ActionQueueItem item12 = new ActionQueueItem(action12, 2.4f + currentTime);
            actionQueueInstructions.Enqueue(item12);

            SimulatorAction action13 = new SimulatorAction("electrode", 460, 1);
            ActionQueueItem item13 = new ActionQueueItem(action13, 2.6f + currentTime);
            actionQueueInstructions.Enqueue(item13);

            SimulatorAction action14 = new SimulatorAction("electrode", 459, 0);
            ActionQueueItem item14 = new ActionQueueItem(action14, 2.8f + currentTime);
            actionQueueInstructions.Enqueue(item14);

            return actionQueueInstructions;
        }

        private Queue<ActionQueueItem> generateOrangeTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 330, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 0.2f + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 298, 0);
            ActionQueueItem item2 = new ActionQueueItem(action2, 0.4f + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 362, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 0.6f + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 330, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 0.8f + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 394, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 1f + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 362, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 1.2f + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 426, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 1.4f + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 394, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 1.6f + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 458, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 1.8f + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 426, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 2f + currentTime);
            actionQueueInstructions.Enqueue(item10);

            SimulatorAction action11 = new SimulatorAction("electrode", 457, 1);
            ActionQueueItem item11 = new ActionQueueItem(action11, 2.2f + currentTime);
            actionQueueInstructions.Enqueue(item11);

            SimulatorAction action12 = new SimulatorAction("electrode", 458, 0);
            ActionQueueItem item12 = new ActionQueueItem(action12, 2.4f + currentTime);
            actionQueueInstructions.Enqueue(item12);

            SimulatorAction action13 = new SimulatorAction("electrode", 456, 1);
            ActionQueueItem item13 = new ActionQueueItem(action13, 2.6f + currentTime);
            actionQueueInstructions.Enqueue(item13);

            SimulatorAction action14 = new SimulatorAction("electrode", 457, 0);
            ActionQueueItem item14 = new ActionQueueItem(action14, 2.8f + currentTime);
            actionQueueInstructions.Enqueue(item14);

            return actionQueueInstructions;
        }

        private Queue<ActionQueueItem> generateYellowTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new();
            SimulatorAction action1 = new("electrode", 266, 1);
            ActionQueueItem item1 = new(action1, 0.2f + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new("electrode", 298, 0);
            ActionQueueItem item2 = new(action2, 0.4f + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new("electrode", 234, 1);
            ActionQueueItem item3 = new(action3, 0.6f + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 266, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 0.8f + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 202, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 1f + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 234, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 1.2f + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 170, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 1.4f + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 202, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 1.6f + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 138, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 1.8f + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 170, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 2f + currentTime);
            actionQueueInstructions.Enqueue(item10);

            SimulatorAction action11 = new SimulatorAction("electrode", 137, 1);
            ActionQueueItem item11 = new ActionQueueItem(action11, 2.2f + currentTime);
            actionQueueInstructions.Enqueue(item11);

            SimulatorAction action12 = new SimulatorAction("electrode", 138, 0);
            ActionQueueItem item12 = new ActionQueueItem(action12, 2.4f + currentTime);
            actionQueueInstructions.Enqueue(item12);

            SimulatorAction action13 = new SimulatorAction("electrode", 136, 1);
            ActionQueueItem item13 = new ActionQueueItem(action13, 2.6f + currentTime);
            actionQueueInstructions.Enqueue(item13);

            SimulatorAction action14 = new SimulatorAction("electrode", 137, 0);
            ActionQueueItem item14 = new ActionQueueItem(action14, 2.8f + currentTime);
            actionQueueInstructions.Enqueue(item14);

            return actionQueueInstructions;
        }

        private static Queue<ActionQueueItem> generateHotTemperatureTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 306, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1 + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 305, 0);
            ActionQueueItem item2 = new ActionQueueItem(action2, 2 + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 307, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3 + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 306, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4 + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 308, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5 + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 307, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6 + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 309, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7 + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 308, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8 + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 310, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9 + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 309, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10 + currentTime);
            actionQueueInstructions.Enqueue(item10);




            return actionQueueInstructions;
        }
        private static Queue<ActionQueueItem> generateColdTemperatureTestQueue(float currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new Queue<ActionQueueItem>();
            SimulatorAction action1 = new SimulatorAction("electrode", 309, 1);
            ActionQueueItem item1 = new ActionQueueItem(action1, 1 + currentTime);
            actionQueueInstructions.Enqueue(item1);

            SimulatorAction action2 = new SimulatorAction("electrode", 310, 0);
            ActionQueueItem item2 = new ActionQueueItem(action2, 2 + currentTime);
            actionQueueInstructions.Enqueue(item2);

            SimulatorAction action3 = new SimulatorAction("electrode", 308, 1);
            ActionQueueItem item3 = new ActionQueueItem(action3, 3 + currentTime);
            actionQueueInstructions.Enqueue(item3);

            SimulatorAction action4 = new SimulatorAction("electrode", 309, 0);
            ActionQueueItem item4 = new ActionQueueItem(action4, 4 + currentTime);
            actionQueueInstructions.Enqueue(item4);

            SimulatorAction action5 = new SimulatorAction("electrode", 307, 1);
            ActionQueueItem item5 = new ActionQueueItem(action5, 5 + currentTime);
            actionQueueInstructions.Enqueue(item5);

            SimulatorAction action6 = new SimulatorAction("electrode", 308, 0);
            ActionQueueItem item6 = new ActionQueueItem(action6, 6 + currentTime);
            actionQueueInstructions.Enqueue(item6);

            SimulatorAction action7 = new SimulatorAction("electrode", 306, 1);
            ActionQueueItem item7 = new ActionQueueItem(action7, 7 + currentTime);
            actionQueueInstructions.Enqueue(item7);

            SimulatorAction action8 = new SimulatorAction("electrode", 307, 0);
            ActionQueueItem item8 = new ActionQueueItem(action8, 8 + currentTime);
            actionQueueInstructions.Enqueue(item8);

            SimulatorAction action9 = new SimulatorAction("electrode", 305, 1);
            ActionQueueItem item9 = new ActionQueueItem(action9, 9 + currentTime);
            actionQueueInstructions.Enqueue(item9);

            SimulatorAction action10 = new SimulatorAction("electrode", 306, 0);
            ActionQueueItem item10 = new ActionQueueItem(action10, 10 + currentTime);
            actionQueueInstructions.Enqueue(item10);

            return actionQueueInstructions;
        }
    }
}
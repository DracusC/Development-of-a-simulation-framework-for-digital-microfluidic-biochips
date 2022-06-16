/*
 * This is a simple version of the virtual machine, it is used for testing purposes,
 * since we do not have access to the actual virtual machine.
 */
using System.Drawing;
using MicrofluidSimulator.SimulatorCode.Models;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Simulator;
using MicrofluidSimulator.SimulatorCode;

namespace MicrofluidSimulator.SimpleVM
{
    public class SimpleVM
    {
        private Simulator simulator;
        private Queue<ActionQueueItem> redQueuePush;
        private Queue<ActionQueueItem> blueQueuePush;
        private Queue<ActionQueueItem> greenQueuePush;
        private Queue<ActionQueueItem> blackQueuePush;
        private Queue<ActionQueueItem> hotTemperatureQueuePush;
        private Queue<ActionQueueItem> coldTemperatureQueuePush;
        private Queue<string> colorQueue;
        Queue<string> copyOfColorQueue;

        private bool rgbSensorCalled;
        private bool temperatureSensorCalled;
        private bool heater727Called;
        private bool heater729Called;
        /// <summary>
        /// Constructor for the simpleVM taking the simulator as parameter
        /// </summary>
        /// <param name="simulator"></param>
        public SimpleVM(Simulator simulator)
        {
            this.simulator = simulator;
            this.redQueuePush = generateRedTestQueue(simulator.container.currentTime);
            this.blueQueuePush = generateBlueTestQueue(simulator.container.currentTime);
            this.greenQueuePush = generateGreenTestQueue(simulator.container.currentTime);
            this.blackQueuePush = generateBlackTestQueue(simulator.container.currentTime);
            this.hotTemperatureQueuePush = generateHotTemperatureTestQueue(simulator.container.currentTime);
            this.coldTemperatureQueuePush = generateColdTemperatureTestQueue(simulator.container.currentTime);

            this.heater727Called = false;
            this.heater729Called = false;
            this.rgbSensorCalled = false;
            this.temperatureSensorCalled = false;

            this.colorQueue = new Queue<string>();
            this.copyOfColorQueue = new Queue<string>();

            colorQueue.Enqueue("#000000");
            colorQueue.Enqueue("#FF4200");
            colorQueue.Enqueue("#00FF00");
            colorQueue.Enqueue("#0000FF");
            colorQueue.Enqueue("#FF4200");
            colorQueue.Enqueue("#0000FF");
            colorQueue.Enqueue("#000000");
            colorQueue.Enqueue("#000000");
            colorQueue.Enqueue("#0000FF");
            colorQueue.Enqueue("#00FF00");
            colorQueue.Enqueue("#00FF00");
            colorQueue.Enqueue("#FF4200");
            colorQueue.Enqueue("#FF4200");
            colorQueue.Enqueue("#00FF00");
            colorQueue.Enqueue("#0000FF");
            colorQueue.Enqueue("#000000");
            colorQueue.Enqueue("#000000");
            colorQueue.Enqueue("#FF4200");
            colorQueue.Enqueue("#00FF00");
            colorQueue.Enqueue("#0000FF");



        }


        /// <summary>
        /// Main function handling the input from the simulator and updating the action queue dependingly
        /// </summary>
        public void doApiCall()
        {
            Color red = ColorTranslator.FromHtml("#FF4200");
            Color blue = ColorTranslator.FromHtml("#0000FF");
            Color green = ColorTranslator.FromHtml("#00FF00");
            Color black = ColorTranslator.FromHtml("#000000");
            int[] redArray = new int[] { red.R, red.G, red.B };
            int[] blueArray = new int[] { blue.R, blue.G, blue.B };
            int[] greenArray = new int[] { green.R, green.G, green.B };
            int[] blackArray = new int[] { black.R, black.G, black.B };
            this.turnOnHeaterAtTime(10, 70, 727, heater727Called);
            this.turnOnHeaterAtTime(1, 70, 729, heater729Called);
            int[] colorRead = this.readRGBValueOfColorSensorAtTime(simulator.container.currentTime, 725);
            
            
            if (simulator.container.currentTime == 0.16m)
            {  
                copyOfColorQueue = HelpfullRetreiveFunctions.createDeepCopyOfColorQueue(this.colorQueue);
            }
           
            if ((simulator.container.currentTime*100) % 496 == 0 && simulator.container.currentTime != 0 && copyOfColorQueue.Count > 0)
            {
                Droplets newDroplet = new Droplets("test droplet", (int)simulator.container.currentTime, "h20", 700, 190, 22, 22, copyOfColorQueue.Dequeue(),
                20, 380, 318, (int)simulator.container.currentTime, 0);

                simulator.container.droplets.Add(newDroplet);
                simulator.container.subscribedDroplets.Add(newDroplet.ID);
                SubscriptionModels.dropletSubscriptions(simulator.container, newDroplet);
            }
            

            if (Enumerable.SequenceEqual(colorRead, redArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.redQueuePush);
                
            }
            if (Enumerable.SequenceEqual(colorRead, blueArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.blueQueuePush);

            }
            if (Enumerable.SequenceEqual(colorRead, greenArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.greenQueuePush);

            }

            if (Enumerable.SequenceEqual(colorRead, blackArray))
            {

                this.pushActionsAtTime(simulator.container.currentTime, this.blackQueuePush);

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
        /// <summary>
        /// Intertwine the given action queue at the time stamp with the action queue in the simulator
        /// </summary>
        /// <param name="time"></param>
        /// <param name="queueToPush"></param>
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

        

    
        /// <summary>
        /// Turn on heater at time with the decired temperature
        /// </summary>
        /// <param name="time"></param>
        /// <param name="desiredTemperature"></param>
        /// <param name="heaterID"></param>
        /// <param name="heaterCalled"></param>
        private void turnOnHeaterAtTime(decimal time, float desiredTemperature, int heaterID, bool heaterCalled)
        {
            decimal currentSimulatorTime = simulator.container.currentTime;
            if (currentSimulatorTime >= time && heaterCalled == false)
            {
                heaterCalled = true;
                SimpleVMUtilityFunctions.setActuatorTargetTemperature(heaterID, desiredTemperature, simulator.container);
            }
            else if (currentSimulatorTime < time)
            {

                heaterCalled = false;
            }
        }
        /// <summary>
        /// Read the RGB value of the color sensor at the given time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        private int[] readRGBValueOfColorSensorAtTime(decimal time, int sensorID)
        {
            decimal currentSimulatorTime = simulator.container.currentTime;
            if (currentSimulatorTime == time && rgbSensorCalled == false)
            {
                int[] colorArray = SimpleVMUtilityFunctions.getColorOfSensorWithID(sensorID, simulator.container);
                if (!Enumerable.SequenceEqual(colorArray, new int[] { -1, -1, -1 }) && !Enumerable.SequenceEqual(colorArray, new int[] { 0, 0, 0 }))
                {
                    rgbSensorCalled = true;
                }
                return colorArray;
            }
            rgbSensorCalled = false;
            return new int[] { -1, -1, -1 };
        }
        /// <summary>
        /// Reads the temperature of the temperature sensor at the given time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        private float readTemperatureOfTemperatureSensorAtTime(decimal time, int sensorID)
        {
            decimal currentSimulatorTime = simulator.container.currentTime;
            if (currentSimulatorTime == time && temperatureSensorCalled == false)
            {
                float temperature = SimpleVMUtilityFunctions.getTemperatureOfSensorWithID(sensorID, simulator.container);
                if (temperature != -1 && temperature != 20)
                {
                    
                    temperatureSensorCalled = true;
                }
               
                return temperature;
            }
            temperatureSensorCalled = false;
            return -1;
        }

        /// <summary>
        /// All the generate "_" queues are the hard coded action queues used for the droplets of different colour or temperature
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        private static Queue<ActionQueueItem> generateRedTestQueue(decimal currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new();
            int[] electrodeOrder = new int[] { 297, 298, 296, 297, 295, 296, 294, 295, 293, 294, 261, 293, 229, 261, 197, 229,
            165, 197, 133, 165, 101, 133};

            for (int i = 0; i < 22; i++)
            {
                
                if (i % 2 == 0)
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 1, i * 0.16m));
                }
                else
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 0, i * 0.16m));
                }

            }
            
          

            return actionQueueInstructions;
        }

        private static Queue<ActionQueueItem> generateBlueTestQueue(decimal currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new();
            int[] electrodeOrder = new int[] {297, 298, 296, 297, 295, 296, 294, 295, 293, 294, 325, 293, 357, 325, 389, 357, 421, 389, 453,
            421, 485, 453};

            for (int i = 0; i < 22; i++)
            {

                if (i % 2 == 0)
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 1, i * 0.16m));
                }
                else
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 0, i * 0.16m));
                }

            }

            

            return actionQueueInstructions;
        }

        private Queue<ActionQueueItem> generateBlackTestQueue(decimal currentTime)
        {
            

            Queue<ActionQueueItem> actionQueueInstructions = new();
            int[] electrodeOrder = new int[] {330, 298, 362, 330, 394, 362, 426, 394, 458, 426, 490, 458, 491, 490, 492, 491, 493, 492,
            494, 493, 495, 494};

            for (int i = 0; i < 22; i++)
            {

                if (i % 2 == 0)
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 1, i * 0.16m));
                }
                else
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 0, i * 0.16m));
                }

            }



            return actionQueueInstructions;
        }

        private Queue<ActionQueueItem> generateGreenTestQueue(decimal currentTime)
        {
            Queue<ActionQueueItem> actionQueueInstructions = new();
            int[] electrodeOrder = new int[] {266, 298, 234, 266, 202, 234, 170, 202, 138, 170, 106, 138, 107, 106, 108, 107, 109,
                108, 110, 109, 111, 110};

            for (int i = 0; i < 22; i++)
            {

                if (i % 2 == 0)
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 1, i * 0.16m));
                }
                else
                {
                    actionQueueInstructions.Enqueue(generateAction(electrodeOrder[i], 0, i * 0.16m));
                }

            }
            


            return actionQueueInstructions;
        }

        private static Queue<ActionQueueItem> generateHotTemperatureTestQueue(decimal currentTime)
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

        private static Queue<ActionQueueItem> generateColdTemperatureTestQueue(decimal currentTime)
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
        /// <summary>
        /// Generate action is a helper function to easily generate an action given an id, actionchange and time of execution
        /// </summary>
        /// <param name="actionOnID"></param>
        /// <param name="actionChange"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static ActionQueueItem generateAction(int actionOnID, int actionChange, decimal time)
        {
            SimulatorAction action = new SimulatorAction("electrode", actionOnID, actionChange);
            ActionQueueItem item = new ActionQueueItem(action, time);

            return item;

        }
        
    }
}
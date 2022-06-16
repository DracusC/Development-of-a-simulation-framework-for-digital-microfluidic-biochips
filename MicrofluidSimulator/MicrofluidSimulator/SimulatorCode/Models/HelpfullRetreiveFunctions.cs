using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HelpfullRetreiveFunctions
    {


        /// <summary>
        /// Function that is used when a id of an electrode is known and the index 
        /// in the electrode array i needed
        /// It first tries the id as index, if this dosent work it does a binary search
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static int getIndexOfDropletByID(int ID, Container container)
        {
            List<Droplets> droplets = container.droplets;
            int i = 0;
            foreach(Droplets droplet in droplets)
            {
               if(droplet.ID == ID)
               {
                    return i;
               }
               i++;
            }
            return -1;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static float getMassOfDropletGivenSubstance(Droplets caller)
        {
            switch (caller.substance_name)
            {
              
                case "h20":
                    float waterDensity = 0.997F;
                    return (caller.volume/1000) * waterDensity;
            }
            return -1;
        }
        /// <summary>
        /// Makes a complete copy of the action queue
        /// </summary>
        /// <param name="actionQueue"></param>
        /// <returns></returns>
        public static Queue<ActionQueueItem>? createDeepCopyOfActionQueue(Queue<ActionQueueItem> actionQueue)
        {
            Queue<ActionQueueItem> copyOfQueue = new Queue<ActionQueueItem>();

            foreach(ActionQueueItem item in actionQueue)
            {
                SimulatorAction newAction = new SimulatorAction(item.action.actionName, item.action.actionOnID, item.action.actionChange);
                ActionQueueItem newItem = new ActionQueueItem(newAction, item.time);
                copyOfQueue.Enqueue(newItem);
            }
            return copyOfQueue;
        }

        public static float getAreaOfDroplet(Droplets caller)
        {
            return (float)(Math.PI * (Math.Pow(caller.sizeX / 2, 2)));

        }
        public static float getThermalConductivityOfDroplet(Droplets caller)
        {
            switch (caller.substance_name)
            {

                case "h20":

                    return 0.598F;
            }
            return -1;



        }
        public static float getHeatCapacityOfDropletGivenSubstance(Droplets caller)
        {
            switch (caller.substance_name)
            {

                case "h20":
                    int waterHeatCapacity = 4182;
                    return waterHeatCapacity;
            }
            return -1;
        }
        public static bool hasNeighbouringDroplet(Container container, int posX, int posY)
        {
            foreach (Droplets droplet in container.droplets)
            {
                int vecX = posX - droplet.positionX;
                int vecY = posY - droplet.positionY;
                double vecLength = Math.Sqrt(Math.Pow(vecX, 2) + Math.Pow(vecY, 2));
                if (vecLength <= droplet.sizeX / 2)
                {
                    return true;
                }
            }
            return false;

        }
        /// <summary>
        /// Get a a droplet located on a sensor returns the droplet or NULL if there is no droplet
        /// </summary>
        /// <param name="container"></param>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public static Droplets getDropletOnSensor(Container container, Sensors sensor)
        {
            foreach (Droplets droplet in container.droplets)
            {
                
                double vecX = droplet.positionX - (sensor.positionX+(sensor.sizeX/2));
                double vecY = droplet.positionY - (sensor.positionY+(sensor.sizeY/2));

                double vecLength = Math.Sqrt(Math.Pow(vecX, 2) + Math.Pow(vecY, 2));
                

                if(vecLength < GlobalVariables.RECTANGULARELECTRODESIZE)
                {
                    foreach (Droplets otherDroplet in container.droplets)
                    {
                        if(droplet.ID != otherDroplet.ID)
                        {
                            double vecUX = otherDroplet.positionX - droplet.positionX;
                            double vecUY = otherDroplet.positionY - droplet.positionY;

                            double dropletVecLength = Math.Sqrt(Math.Pow(vecUX, 2) + Math.Pow(vecUY, 2));
                            if (dropletVecLength <= GlobalVariables.RECTANGULARELECTRODESIZE)
                            {
                                
                                return null;
                            }
                        }
                        

                    }
                    return droplet;
                }
                
            }
            return null;
        }
        /// <summary>
        /// Creates a complete copy of the color queue
        /// </summary>
        /// <param name="colorQueue"></param>
        /// <returns></returns>
        public static Queue<string> createDeepCopyOfColorQueue(Queue<string> colorQueue)
        {
            Queue<string> copyOfQueue = new Queue<string>();

            foreach (string item in colorQueue)
            {
                
                copyOfQueue.Enqueue(item);
            }
            return copyOfQueue;
        }

        //bubble retreive functions
        /// <summary>
        /// Get the diameter of the bubbles
        /// </summary>
        /// <param name="volumeFromDroplet"></param>
        /// <returns></returns>
        public static double getDiameterOfBubble(double volumeFromDroplet)
        {
            double airDensity = 0.0012F;
            double volume = volumeFromDroplet / airDensity;
            double radius = Math.Pow((3 * volume / (4 * Math.PI)), (1.0 / 3.0));
            return radius*2;
        }
        /// <summary>
        /// Get a bubbles in the bubble list based on the bubbles ID
        /// </summary>
        /// <param name="container"></param>
        /// <param name="bubbleID"></param>
        /// <returns></returns>
        public static Bubbles getBubbleByID(Container container, int bubbleID)
        {
            foreach (Bubbles bubble in container.bubbles)
            {
                if (bubble.ID == bubbleID)
                {
                    return bubble;
                }
            }
            return null;

        }
        /// <summary>
        /// Creates a copy of the subscribed bubbles arraylist
        /// </summary>
        /// <param name="subscribedBubbles"></param>
        /// <returns></returns>
        public static ArrayList copyOfSubscribedBubbles(ArrayList subscribedBubbles)
        {
            ArrayList copyOfSubscribedBubbles = new ArrayList();
            foreach (int bubbleID in subscribedBubbles)
            {
                copyOfSubscribedBubbles.Add(bubbleID);
            }
            return copyOfSubscribedBubbles;
        }

        //actuator retreive functions

        /// <summary>
        /// Get the actuator on a droplets position
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <param name="actuatorType"></param>
        /// <returns></returns>
        public static Actuators getActuatorOnDroplet(Container container, Droplets caller, string actuatorType)
        {
            
            foreach (Actuators actuator in container.actuators)
            {
                if (actuator.type.Equals(actuatorType))
                {

                    
                    
                    int minMarginX = actuator.positionX;
                    int maxMarginX = actuator.positionX + (actuator).sizeX;
                    int minMarginY = actuator.positionY;
                    int maxMarginY = actuator.positionY + (actuator).sizeY;

                    if (caller.positionX >= minMarginX && caller.positionX <= maxMarginX && caller.positionY >= minMarginY && caller.positionY <= maxMarginY)
                    {
                        
                        return actuator;
                    }
                }

            }
            return null;
        }
        /// <summary>
        /// Get the index of an actuator in the actuator array based on its ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static int getIndexOfActuatorByID(int ID, Container container)
        {
            //tries to check the with the id as index since this often is the case else does a binary search
            Actuators[] actuators = container.actuators;

            if (actuators[ID].ID == ID)
            {
                return ID;
            }

            return binarySearchActuators(ID, container);
        }
        /// <summary>
        /// Get an actuar based on ID, return the actuator or NULL if it does not exist
        /// </summary>
        /// <param name="container"></param>
        /// <param name="actuatorID"></param>
        /// <returns></returns>
        public static Actuators getActuatorByID(Container container, int actuatorID)
        {
            foreach (Actuators actuator in container.actuators)
            {
                if (actuator.ID == actuatorID)
                {
                    return actuator;
                }
            }
            return null;
        }
        /// <summary>
        /// This is used for getIndexOfActuatorByID
        /// Get the index of an actuator in the actuator array based on its ID -1 if it does not exist
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static int binarySearchActuators(int ID, Container container)
        {
            Actuators[] actuators = container.actuators;
            int min = 0;
            int max = actuators.Length - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (ID == actuators[mid].ID)
                {
                    return mid;
                }
                else if (ID < actuators[mid].ID)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            return -1;
        }

        //electrode retreive functions
        /// <summary>
        /// Gets the index of an electrode in the electrod list based on ID
        /// This is used for translating the boardconfiguration json into our data representation removing the need for driver id
        /// </summary>
        /// <param name="electrodeId"></param>
        /// <param name="driverId"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static int getIdOfElectrodByElectrodID(int electrodeId, int driverId, Container container)
        {
            Electrode[] electrodes = container.electrodes;

            foreach(Electrode electrode in electrodes)
            {
                if(electrode.electrodeID == electrodeId && electrode.driverID == driverId)
                {
                    return electrode.ID;
                }
            }
            return -1;
        }
        /// <summary>
        /// Gets the index of a electrode in the electrode array based on the index, returns -1 if it does not exist
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static int getIndexOfElectrodeByID(int ID, Container container)
        {
            // tries to use the id as index if it is not the correct does binary search
            Electrode[] electrodes = container.electrodes;
            
                if (electrodes[ID].ID == ID)
                {
                    return ID;
                }
                
            return binarySearchElectrode(ID, container);
        }
        /// <summary>
        /// Binary searches for the index of a electrode based on ID used for getIndexOfElectrodeByID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static int binarySearchElectrode(int ID, Container container)
        {
            Electrode[] electrodes = container.electrodes;
            int min = 0;
            int max = electrodes.Length - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (ID == electrodes[mid].ID)
                {
                    return mid;
                }
                else if (ID < electrodes[mid].ID)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            return -1;
        }
        /// <summary>
        /// Function to find the electrode based on a bord postion
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="electrodeBoard"></param>
        /// <returns></returns>
        public static int getIDofElectrodeByPosition(int positionX, int positionY, Electrode[] electrodeBoard)
        {

            for (int i = 0; i < electrodeBoard.Length; i++)
            {
                Electrode electrode = electrodeBoard[i];
                if (electrode.positionX <= positionX && electrode.positionX + electrode.sizeX >= positionX && electrode.positionY <= positionY && electrode.positionY + electrode.sizeY >= electrode.positionY)
                {
                    return electrodeBoard[i].ID;
                }
            }
            return -1;
        }
        /// <summary>
        /// function to copy the initial container, used for restarting the simulator
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static Container createCopyAndResetContainer(Container container)
        {
            
            Electrode[] electrodes = new Electrode[container.electrodes.Length];
            List<Droplets> droplets = new List<Droplets>();
            List<Bubbles> bubbles = new List<Bubbles>();
            Actuators[] actuators  = new Actuators[container.actuators.Length];
            Sensors[] sensors = new Sensors[container.sensors.Length];
            Information information = container.information;
            decimal currentTime = 0;
            
            for (int i = 0; i < electrodes.Length; i++)
            {
                
                electrodes[i] = new Electrode(container.electrodes[i].name, container.electrodes[i].ID, container.electrodes[i].electrodeID, container.electrodes[i].driverID, container.electrodes[i].shape,
                container.electrodes[i].positionX, container.electrodes[i].positionY, container.electrodes[i].sizeX, container.electrodes[i].sizeY, container.electrodes[i].status, container.electrodes[i].corners);

                electrodes[i].neighbours = container.electrodes[i].neighbours;
            }



            int j = 0;
            foreach (Droplets droplet in container.droplets)
            {
                droplets.Add(new Droplets(droplet.name, droplet.ID, droplet.substance_name, droplet.positionX, droplet.positionY, droplet.sizeX, droplet.sizeY, droplet.color, droplet.temperature, droplet.volume, droplet.electrodeID, droplet.group, 0));
                j++;
            }

            
            


            for (int i = 0; i < container.actuators.Length; i++)
            {
                switch (container.actuators[i].type)
                {
                    case "heater":



                        actuators[i] = (new Heater(container.actuators[i].name, container.actuators[i].ID, container.actuators[i].actuatorID, container.actuators[i].type, container.actuators[i].positionX,
                            container.actuators[i].positionY, container.actuators[i].sizeX, container.actuators[i].sizeY, container.actuators[i].valueActualTemperature, container.actuators[i].valueDesiredTemperature,
                            container.actuators[i].valuePowerStatus));

                        break;

                }

            }
            for (int i = 0; i < sensors.Length; i++)
            {
                sensors[i] = container.sensors[i];
            }

            for (int i = 0; i < container.sensors.Length; i++)
            {
                switch (sensors[i].type)
                {
                    case "RGB_color":



                        sensors[i] = new ColorSensor(container.sensors[i].name, container.sensors[i].ID, container.sensors[i].sensorID, container.sensors[i].type, container.sensors[i].positionX, container.sensors[i].positionY,
                            container.sensors[i].sizeX, container.sensors[i].sizeY, container.sensors[i].valueRed, container.sensors[i].valueGreen, container.sensors[i].valueBlue, HelpfullRetreiveFunctions.getIDofElectrodeByPosition(container.sensors[i].positionX, container.sensors[i].positionY,electrodes));

                        break;
                    case "temperature":

                        sensors[i] = new TemperatureSensor(container.sensors[i].name, container.sensors[i].ID, container.sensors[i].sensorID, container.sensors[i].type, container.sensors[i].positionX, container.sensors[i].positionY,
                            container.sensors[i].sizeX, container.sensors[i].sizeY, container.sensors[i].valueTemperature, HelpfullRetreiveFunctions.getIDofElectrodeByPosition(container.sensors[i].positionX, container.sensors[i].positionY, electrodes));

                        break;

                }
                

            }
            foreach (Bubbles bubble in container.bubbles)
            {
                bubbles.Add(new Bubbles(bubble.name, bubble.ID, bubble.positionX, bubble.positionY, bubble.sizeX, bubble.sizeY));
                j++;
            }
            Container newContainer = new Container(electrodes, droplets, actuators, sensors, information, bubbles, currentTime);
            foreach(Droplets droplet in newContainer.droplets)
            {
                newContainer.subscribedDroplets.Add(droplet.ID);
            }
            foreach(Bubbles bubble in newContainer.bubbles){
                newContainer.subscribedBubbles.Add(bubble.ID);
            }
            foreach(Actuators actuator in newContainer.actuators)
            {
                newContainer.subscribedActuators.Add(actuator.ID);
            }
            

            return newContainer;
        }

        //sensor retreive functions
        /// <summary>
        /// Gets a sensor by ID return NULL if there is no sensor with the requested ID
        /// </summary>
        /// <param name="container"></param>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        public static Sensors getSensorByID(Container container, int sensorID)
        {
            foreach (Sensors sensor in container.sensors)
            {
                if (sensor.ID == sensorID)
                {
                    return sensor;
                }
            }
            return null;
        }




    }

}

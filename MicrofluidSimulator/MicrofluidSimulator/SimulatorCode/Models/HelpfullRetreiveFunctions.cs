using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HelpfullRetreiveFunctions
    {
        /* Function that is used when a id of an electrode is known and the index 
         * in the electrode array i needed
         * It first tries the id as index, if this dosent work it does a binary search*/

        //droplet retreive functions
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
        public static float getMassOfDropletGivenSubstance(Droplets caller)
        {
            switch (caller.substance_name)
            {
              
                case "h20":
                    float waterDensity = 0.997F;
                    return (caller.volume/1000) * waterDensity;
                    break;
            }
            return -1;
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
                    break;
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
                    break;
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
        public static Droplets getDropletOnSensor(Container container, Sensors sensor)
        {
            foreach (Droplets droplet in container.droplets)
            {

                int minMarginX = droplet.positionX;
                int maxMarginX = droplet.positionX + (droplet).sizeX;
                int minMarginY = (droplet).positionY;
                int maxMarginY = (droplet).positionY + (droplet).sizeY;

                if (sensor.positionX <= minMarginX && sensor.positionX >= maxMarginX && sensor.positionY <= minMarginY && sensor.positionY >= maxMarginY)
                {
                    return droplet;
                }
            }
            return null;
        }

        //bubble retreive functions
        public static double getDiameterOfBubble(double volumeFromDroplet)
        {
            double airDensity = 0.0012F;
            double volume = volumeFromDroplet / airDensity;
            double radius = Math.Pow((3 * volume / (4 * Math.PI)), (1.0 / 3.0));
            return radius*2;
        }
        public static Bubbles getBubbleById(Container container, int bubbleID)
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
        public static Heater getHeaterOnDroplet(Container container, Droplets caller)
        {
            foreach (Actuators actuator in container.actuators)
            {
                if (actuator.type.Equals("heater"))
                {
                    
                    int minMarginX = ((Heater)actuator).positionX;
                    int maxMarginX = ((Heater)actuator).positionX + ((Heater)actuator).sizeX;
                    int minMarginY = ((Heater)actuator).positionY;
                    int maxMarginY = ((Heater)actuator).positionY + ((Heater)actuator).sizeY;

                    if (caller.positionX >= minMarginX && caller.positionX <= maxMarginX && caller.positionY >= minMarginY && caller.positionY <= maxMarginY)
                    {
                        
                        return (Heater)actuator;
                    }
                }
            }
            return null;
        }
        public static int getIndexOfActuatorByID(int ID, Container container)
        {

            Actuators[] actuators = container.actuators;

            if (actuators[ID].ID == ID)
            {
                return ID;
            }

            return binarySearchActuators(ID, container);
        }
        public static Actuators getActuatorById(Container container, int actuatorID)
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

        public static int binarySearchActuators(int ID, Container container)
        {
            Actuators[] actuators = container.actuators;
            int min = 0;
            int max = actuators.Count() - 1;

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
        public static int getIdOfElectrodByElectrodId(int electrodeId, int driverId, Container container)
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

        public static int getIndexOfElectrodeByID(int ID, Container container)
        {

            Electrode[] electrodes = container.electrodes;
            //if (ID < electrodes.Count())
            //{
                if (electrodes[ID].ID == ID)
                {
                    return ID;
                }
                //for (int i = 0; i < electrodes.Count(); i++)
                //{
                //    if (electrodes[i].ID1 == ID)
                //    {
                //        return i;
                //    }
                //}


            //}
            //Console.WriteLine("WFT!" + ID);
            //return 0; 
            return binarySearchElectrode(ID, container);
        }
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
        public static int binarySearchElectrode(int ID, Container container)
        {
            Electrode[] electrodes = container.electrodes;
            int min = 0;
            int max = electrodes.Count() - 1;

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

        //function to copy the initial container, used for restarting the simulator
        public static Container createCopyAndResetContainer(Container container)
        {
            
            Electrode[] electrodes = new Electrode[container.electrodes.Length];
            List<Droplets> droplets = new List<Droplets>();
            List<Bubbles> bubbles = new List<Bubbles>();
            Actuators[] actuators  = new Actuators[container.actuators.Length];
            Sensors[] sensors = new Sensors[container.sensors.Length];
            Information information = container.information;
            float currentTime = 0;
            
            for (int i = 0; i < electrodes.Length; i++)
            {
                
                electrodes[i] = new Electrode(container.electrodes[i].name, container.electrodes[i].ID, container.electrodes[i].electrodeID, container.electrodes[i].driverID, container.electrodes[i].shape,
                container.electrodes[i].positionX, container.electrodes[i].positionY, container.electrodes[i].sizeX, container.electrodes[i].sizeY, container.electrodes[i].status, container.electrodes[i].corners);

                electrodes[i].neighbours = container.electrodes[i].neighbours;
            }



            Console.WriteLine("CONTAINER DROPLETS COUNT " + container.droplets.Count);
            int j = 0;
            foreach (Droplets droplet in container.droplets)
            {
                droplets.Add(new Droplets(droplet.name, droplet.ID, droplet.substance_name, droplet.positionX, droplet.positionY, droplet.sizeX, droplet.sizeY, droplet.color, droplet.temperature, droplet.volume, droplet.electrodeID, droplet.group, 0));
                j++;
            }
            Console.WriteLine("THERE IS THIS MANY DROPLETS AFTER COPY" + j);

            
            


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

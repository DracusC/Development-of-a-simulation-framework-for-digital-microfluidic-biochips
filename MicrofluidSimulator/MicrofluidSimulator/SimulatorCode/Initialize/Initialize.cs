using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;


namespace MicrofluidSimulator.SimulatorCode.Initialize
{

    public class Initialize
    {

        public Initialize()
        {

        }
        /// <summary>
        /// Initialization function for intializing the data in the container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="electrodesWithNeighbours"></param>
        /// <returns></returns>
        public Container initialize(Container container, ElectrodesWithNeighbours[] electrodesWithNeighbours)
        {


            Electrode[] electrodeBoard = initializeBoard(container.electrodes, electrodesWithNeighbours);
            List<Droplets> droplets = initializeDroplets(container.droplets);
            List<Bubbles> bubbles = initializeBubbles(container.bubbles);
            Actuators[] actuatorsInitial = initializeActuators(container.actuators);
            Sensors[] sensorsInitial = initializeSensors(container.sensors, electrodeBoard);
            Information information = initializeInformation(container.information);
            Container initialContainer = new Container(electrodeBoard, droplets, actuatorsInitial, sensorsInitial, information, bubbles, 0);
            foreach(Droplets droplet in container.droplets)
            {
                initialContainer.subscribedDroplets.Add(droplet.ID);
            }
            foreach (Actuators actuators in actuatorsInitial)
            {
                initialContainer.subscribedActuators.Add(actuators.ID);
            }
            foreach(Bubbles bubble in bubbles)
            {
                initialContainer.subscribedBubbles.Add(bubble.ID);
            }
            if (electrodesWithNeighbours == null)
            {
                NeighbourFinder neighbourFinder = new NeighbourFinder();
                NeighbourFinder.findNeighbours(initialContainer);
            }

            foreach (Droplets droplet in container.droplets)
            {
                DropletUtillityFunctions.updateGroupNumber(initialContainer, droplet, droplet.group);
            }

            initializeSubscriptions(initialContainer);
            return initialContainer;
        }
        /// <summary>
        /// Function to initialize the bubbles
        /// </summary>
        /// <param name="bubbles"></param>
        /// <returns></returns>
        private List<Bubbles> initializeBubbles(List<Bubbles> bubbles)
        {
            List<Bubbles> initialBubbles = new List<Bubbles>();
            foreach(Bubbles bubble in bubbles)
            {
                initialBubbles.Add(new Bubbles(bubble.name, bubble.ID, bubble.positionX, bubble.positionY, bubble.sizeX, bubble.sizeY));
            }
            return initialBubbles;
           
        }
        /// <summary>
        /// Function to initializing the information
        /// </summary>
        /// <param name="informationInput"></param>
        /// <returns></returns>
        private Information initializeInformation(Information informationInput)
        {
            Information information = new Information(informationInput.platform_name, informationInput.platform_type, informationInput.platform_ID, informationInput.sizeX, informationInput.sizeY);
            return information;
        }
        /// <summary>
        /// Funciton to initialize the sensor
        /// </summary>
        /// <param name="sensors"></param>
        /// <param name="electrodeBoard"></param>
        /// <returns></returns>
        private Sensors[] initializeSensors(Sensors[] sensors, Electrode[] electrodeBoard)
        {
            Sensors[] sensorsInitial = new DataTypes.Sensors[sensors.Length];

            for (int i = 0; i < sensors.Length; i++)
            {
                switch (sensors[i].type)
                {
                    case "RGB_color":



                        sensorsInitial[i] = new ColorSensor(sensors[i].name, sensors[i].ID, sensors[i].sensorID, sensors[i].type, sensors[i].positionX, sensors[i].positionY,
                            sensors[i].sizeX, sensors[i].sizeY, sensors[i].valueRed, sensors[i].valueGreen, sensors[i].valueBlue, HelpfullRetreiveFunctions.getIDofElectrodeByPosition(sensors[i].positionX, sensors[i].positionY, electrodeBoard));

                        break;
                    case "temperature":

                        sensorsInitial[i] = new TemperatureSensor(sensors[i].name, sensors[i].ID, sensors[i].sensorID, sensors[i].type, sensors[i].positionX, sensors[i].positionY,
                            sensors[i].sizeX, sensors[i].sizeY, sensors[i].valueTemperature, HelpfullRetreiveFunctions.getIDofElectrodeByPosition(sensors[i].positionX, sensors[i].positionY, electrodeBoard));

                        break;

                }

            }
            return sensorsInitial;
        }
        /// <summary>
        /// Function to initialize the actuators
        /// </summary>
        /// <param name="actuators"></param>
        /// <returns></returns>
        private DataTypes.Actuators[] initializeActuators(Actuators[] actuators)
        {
            
            DataTypes.Actuators[] actuatorsInitial = new DataTypes.Actuators[actuators.Length];
            
            for(int i = 0; i < actuators.Length; i++)
            {
                switch (actuators[i].type)
                {
                    case "heater":
                        
                        

                        actuatorsInitial[i]= (new Heater(actuators[i].name, actuators[i].ID, actuators[i].actuatorID, actuators[i].type, actuators[i].positionX,
                            actuators[i].positionY, actuators[i].sizeX, actuators[i].sizeY, actuators[i].valueActualTemperature, actuators[i].valueDesiredTemperature,
                            actuators[i].valuePowerStatus));

                        break;
                    
                }
                
            }

            return actuatorsInitial;

        }
        /// <summary>
        /// Function to initialize the electrode board
        /// </summary>
        /// <param name="electrodes"></param>
        /// <param name="electrodesWithNeighbours"></param>
        /// <returns></returns>
        private Electrode[] initializeBoard(Electrode[] electrodes, ElectrodesWithNeighbours[] electrodesWithNeighbours)
        {


            //Electrode[] electrodeBoard = new Electrode[electrodes.Count];
            for (int i = 0; i < electrodes.Length; i++)
            {
                
                if (electrodesWithNeighbours != null)
                {
                    
                    for(int j = 0; j < electrodesWithNeighbours[i].Neighbours.Count; j++)
                    {
                        electrodes[i].neighbours.Add(electrodesWithNeighbours[i].Neighbours[j]);
                    }
                    
                }

            }

            return electrodes;


        }
        /// <summary>
        /// Funtion for initializing the droplets
        /// </summary>
        /// <param name="droplets"></param>
        /// <returns></returns>
        private List<Droplets> initializeDroplets(List<Droplets> droplets)
        {
            List<Droplets> dropletsArray = new List<Droplets>();
            int i = 0;
            foreach (Droplets droplet in droplets)
            {
                dropletsArray.Add(new Droplets(droplet.name, droplet.ID, droplet.substance_name, droplet.positionX, droplet.positionY, droplet.sizeX, droplet.sizeY, droplet.color, droplet.temperature, DropletUtillityFunctions.getVolumeOfDroplet(droplet.sizeX), droplet.electrodeID, i, 0));
                i++;
            }
            
            
            return dropletsArray;
        }

        
        /// <summary>
        /// Function for intializing all subscribtions of all droplets
        /// </summary>
        /// <param name="container"></param>
        private void initializeSubscriptions(Container container)
        {
            List<Droplets> droplets = container.droplets;
            foreach (Droplets droplet in droplets)
            {
                SubscriptionModels.dropletSubscriptions(container, droplet);

                
            }


        }
    }
}

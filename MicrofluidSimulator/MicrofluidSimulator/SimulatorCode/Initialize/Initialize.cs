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

        public Container initialize(Container container, ElectrodesWithNeighbours[] electrodesWithNeighbours)
        {

            //Electrodes[] electrodes = new Electrodes[jsonContainer.electrodes.Count];

            //Electrode[] electrodeBoard = initializeBoard(jsonContainer.electrodes, electrodesWithNeighbours);

            Electrode[] electrodeBoard = initializeBoard(container.electrodes, electrodesWithNeighbours);

            //List<Droplets> droplets = initializeDroplets(jsonContainer.droplets);
            List<Droplets> droplets = initializeDroplets(container.droplets);
            Console.WriteLine("actuatorname: " + container.actuators[0].name);
            DataTypes.Actuators[] actuatorsInitial = initializeActuators(container.actuators);
            DataTypes.Sensors[] sensorsInitial = initializeSensors(container.sensors, electrodeBoard);
            Information information = initializeInformation(container.information);
            Container initialContainer = new Container(electrodeBoard, droplets, actuatorsInitial, sensorsInitial, information, 0);
            foreach(Droplets droplet in container.droplets)
            {
                initialContainer.subscribedDroplets.Add(droplet.ID);
            }
            foreach (DataTypes.Actuators actuators in actuatorsInitial)
            {
                initialContainer.subscribedActuators.Add(actuators.ID);
            }
            if (electrodesWithNeighbours == null)
            {
                NeighbourFinder neighbourFinder = new NeighbourFinder();
                NeighbourFinder.findNeighbours(initialContainer);
            }

            initializeSubscriptions(initialContainer);


            return initialContainer;
        }

        private Information initializeInformation(Information informationInput)
        {
            Information information = new Information(informationInput.platform_name, informationInput.platform_type, informationInput.platform_ID, informationInput.sizeX, informationInput.sizeY);
            return information;
        }

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

        private DataTypes.Actuators[] initializeActuators(Actuators[] actuators)
        {
            //List<MicrofluidSimulator.SimulatorCode.DataTypes.Actuators> actuatorsInitial = new List<MicrofluidSimulator.SimulatorCode.DataTypes.Actuators>();
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

        public class Corner
        {
            List<int> Coords { get; set; }
        }

        private Electrode[] initializeBoard(Electrode[] electrodes, ElectrodesWithNeighbours[] electrodesWithNeighbours)
        {


            //Electrode[] electrodeBoard = new Electrode[electrodes.Count];
            for (int i = 0; i < electrodes.Length; i++)
            {
                //electrodeBoard[i] = new Electrodes("arrel", i, i, i, 0, (i % 32) * 20, (i / 32) * 20, 20, 20, 0, null);






                //int[,] cornersGetter = new int[electrodes[i].corners.Count, 2];
                //for (int j = 0; j < electrodes[i].corners.Count; j++)
                //{

                //    var res = System.Text.Json.JsonSerializer.Deserialize<List<int>>(electrodes[i].corners[j].ToString());
                //    for (int k = 0; k < 2; k++)
                //    {
                //        cornersGetter[j, k] = res[k];
                //    }
                //}





                //electrodeBoard[i] = new Electrode(electrodes[i].name, electrodes[i].ID, electrodes[i].electrodeID, electrodes[i].driverID, electrodes[i].shape,
                //electrodes[i].positionX, electrodes[i].positionY, electrodes[i].sizeX, electrodes[i].sizeY, electrodes[i].status, electrodes[i].corners);

                if (electrodesWithNeighbours != null)
                {
                    
                    for(int j = 0; j < electrodesWithNeighbours[i].Neighbours.Count; j++)
                    {
                        electrodes[i].neighbours.Add(electrodesWithNeighbours[i].Neighbours[j]);
                    }
                    
                    //int[] neighboursGetter = new int[electrodesWithNeighbours[i].Neighbours.Count];
                    //for (int j = 0; j < electrodesWithNeighbours[i].Neighbours.Count; j++)
                    //{

                    //    var res = System.Text.Json.JsonSerializer.Deserialize<List<int>>(electrodesWithNeighbours[i].Neighbours[j].ToString());

                    //    for(int k = 0; k < res.Count; k++)
                    //    {
                    //        neighboursGetter[k] = res[k];
                    //    }

                    //    electrodeBoard[i].Neighbours.Add(neighboursGetter[j]);

                    //}

                }
                





            }

            return electrodes;


        }

        private List<Droplets> initializeDroplets(List<Droplets> droplets)
        {
            List<Droplets> dropletsArray = new List<Droplets>();
            int i = 0;
            foreach (Droplets droplet in droplets)
            {
                dropletsArray.Add(new Droplets(droplet.name, droplet.ID, droplet.substance_name, droplet.positionX, droplet.positionY, droplet.sizeX, droplet.sizeY, droplet.color, droplet.temperature, DropletUtillityFunctions.getVolumeOfDroplet(droplet.sizeX, 1), droplet.electrodeID, i));
                i++;
            }
            //    droplets.Add(new Droplets("test droplet", 0, "h20", 120, 10, 15, 15, "blue", 20, DropletModels.getVolumeOfDroplet(15, 1), 0));
            //((Droplets)droplets[0]).ElectrodeID = 1;

            //droplets.Add(new Droplets("test droplet", 1, "h20", 30, 10, 15, 15, "blue", 20));
            //((Droplets)droplets[0]).ElectrodeID = 1;

            //droplets.Add(new Droplets("test droplet", 2, "h20", 50, 10, 15, 15, "blue", 20));
            //((Droplets)droplets[0]).ElectrodeID = 2;

            //droplets.Add(new Droplets("test droplet2", 3, "h20", 160, 70, 15, 15, "yellow", 20, DropletModels.getVolumeOfDroplet(15, 1), 1));
            //((Droplets) droplets[1]).ElectrodeID = 99;
            return dropletsArray;
        }

        

        private void initializeSubscriptions(Container container)
        {
            List<Droplets> droplets = container.droplets;
            foreach (Droplets droplet in droplets)
            {
                Models.SubscriptionModels.dropletSubscriptions(container, droplet);
                foreach(int sub in droplet.subscriptions)
                {
                    Console.WriteLine("droplet subs id: " + droplet.ID + " subs: " + sub);
                }
                
            }


        }

       

    }
}

using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Container
    {
        //Electrodes[] electrodes;
        //ArrayList droplets;
        //Actuators[] actuators;
        //Sensors[] sensors;
        //Information information;
        //ArrayList subscribedDroplets;
        //ArrayList subscribedActuators;
        //float currentTime;
        //float timeStep;
        public Container()
        {
        }

        // To make a copy of a container
        public Container(Container container)
        {
            this.electrodes = container.electrodes;
            this.droplets = container.droplets;
            this.actuators = container.actuators;
            this.sensors = container.sensors;
            this.information = container.information;
            this.currentTime = container.currentTime;
            this.bubbles = container.bubbles;
            this.subscribedDroplets = container.subscribedDroplets;
            this.subscribedActuators = container.subscribedActuators;
            this.subscribedBubbles = container.subscribedBubbles;
            this.timeStep = container.timeStep;

            this.inputs = container.inputs;
            this.outputs = container.outputs;
            this.unclassified = container.unclassified;
        }


        public Container(Electrode[] electrodes, List<Droplets> droplets, Actuators[] actuators, Sensors[] sensors, Information information, List<Bubbles> bubbles, decimal currentTime)
        {
            this.electrodes = electrodes;
            this.droplets = droplets;
            this.actuators = actuators;
            this.sensors = sensors;
            this.information = information;
            this.currentTime = currentTime;
            this.bubbles = bubbles;
            subscribedDroplets = new ArrayList();
            subscribedActuators = new ArrayList();
            subscribedBubbles = new ArrayList();
            this.timeStep = 0;
       
        }


        public Electrode[] electrodes { get; set; }
        public List<Droplets> droplets { get; set; }
        public Actuators[] actuators { get; set; }
        public Sensors[] sensors { get; set; }
        public Information information { get; set; }
        public decimal currentTime { get; set; }
        public decimal timeStep { get; set; }
        public ArrayList subscribedDroplets { get; set; }
        public ArrayList subscribedActuators { get; set; }
        public ArrayList subscribedBubbles { get; set; }

        public List<Bubbles> bubbles { get; set; }

        public object[] inputs { get; set; }
        public object[] outputs { get; set; }
        public object[] unclassified { get; set; }

        
    }
}

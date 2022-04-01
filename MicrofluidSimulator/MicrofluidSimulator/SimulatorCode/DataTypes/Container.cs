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

        public Container(Electrode[] electrodes, List<Droplets> droplets, Actuators[] actuators, Sensors[] sensors, Information information, float currentTime)
        {
            this.electrodes = electrodes;
            this.droplets = droplets;
            this.actuators = actuators;
            this.sensors = sensors;
            this.information = information;
            this.currentTime = currentTime;
            subscribedDroplets = new ArrayList();
            subscribedActuators = new ArrayList();
            this.timeStep = 0;
       
        }


        public Electrode[] electrodes { get; set; }
        public List<Droplets> droplets { get; set; }
        public Actuators[] actuators { get; set; }
        public Sensors[] sensors { get; set; }
        public Information information { get; set; }
        public float currentTime { get; set; }
        public float timeStep { get; set; }
        public ArrayList subscribedDroplets { get; set; }
        public ArrayList subscribedActuators { get; set; }

        public object[] inputs { get; set; }
        public object[] outputs { get; set; }
        public object[] bubbles { get; set; }
        public object[] unclassified { get; set; }

        
    }
}

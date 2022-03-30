using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Container
    {
        Electrodes[] electrodes;
        ArrayList droplets;
        Actuators[] actuators;
        Sensors[] sensors;
        Information information;
        ArrayList subscribedDroplets;
        ArrayList subscribedActuators;
        float currentTime;
        float timeStep;
        public Container()
        {
        }

        public Container(Electrodes[] electrodes, ArrayList droplets, Actuators[] actuators, Sensors[] sensors, Information information, float currentTime)
        {
            Electrodes = electrodes;
            Droplets = droplets;
            Actuators = actuators;
            Sensors = sensors;
            Information = information;
            CurrentTime = currentTime;
            subscribedDroplets = new ArrayList();
            subscribedActuators = new ArrayList();
            this.timeStep = 0;
       
        }


        public Electrodes[] Electrodes { get => electrodes; set => electrodes = value; }
        public ArrayList Droplets { get => droplets; set => droplets = value; }
        public Actuators[] Actuators { get => actuators; set => actuators = value; }
        public Sensors[] Sensors { get => sensors; set => sensors = value; }
        public Information Information { get => information; set => information = value; }
        public float CurrentTime { get => currentTime; set => currentTime = value; }
        public float TimeStep { get => timeStep; set => timeStep = value; }
        public ArrayList SubscribedDroplets { get => subscribedDroplets; set => subscribedDroplets = value; }
        public ArrayList SubscribedActuators { get => subscribedActuators; set => subscribedActuators = value; }
    }
}

using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Container
    {
        Electrodes[] electrodes;
        ArrayList droplets;
        Actuators[] actuators;

        public Container()
        {
        }

        public Container(Electrodes[] electrodes, ArrayList droplets, Actuators[] actuators)
        {
            Electrodes = electrodes;
            Droplets = droplets;
            Actuators = actuators;
       
        }


        public Electrodes[] Electrodes { get => electrodes; set => electrodes = value; }
        public ArrayList Droplets { get => droplets; set => droplets = value; }
        public Actuators[] Actuators { get => actuators; set => actuators = value; }
    }
}

using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Container
    {
        Electrodes[] electrodes;
        ArrayList droplets;

        public Container()
        {
        }

        public Container(Electrodes[] electrodes, ArrayList droplets)
        {
            Electrodes = electrodes;
            Droplets = droplets;
       
        }


        public Electrodes[] Electrodes { get => electrodes; set => electrodes = value; }
        public ArrayList Droplets { get => droplets; set => droplets = value; }
    }
}

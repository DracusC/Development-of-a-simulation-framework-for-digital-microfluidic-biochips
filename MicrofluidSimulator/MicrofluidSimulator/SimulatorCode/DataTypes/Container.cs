namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Container
    {
        Electrodes[] electrodes;
        Droplets[] droplets;

        public Container()
        {
        }

        public Container(Electrodes[] electrodes, Droplets[] droplets)
        {
            Electrodes = electrodes;
            Droplets = droplets;
       
        }


        public Electrodes[] Electrodes { get => electrodes; set => electrodes = value; }
        public Droplets[] Droplets { get => droplets; set => droplets = value; }
    }
}

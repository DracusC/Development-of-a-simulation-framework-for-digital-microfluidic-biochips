using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Actuators
    {
        

        // actuators is a parent class for all kinds of actuators, e.g. heaters
        public Actuators(string name, int ID, int actuatorID, string type, int positionX, int positionY, int sizeX, int sizeY)
        {
            this.name = name;
            this.type = type;
            this.ID = ID;
            this.actuatorID = actuatorID;
            this.positionX = positionX;
            this.positionY = positionY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            // contain an arraylist of subscribers to the actuator
            this.subscriptions = new ArrayList();
        }

        public Actuators()
        {
        }

        public string name { get; set; }
        public string type { get; set; }
        public int ID { get; set; }
        public int actuatorID { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int valueActualTemperature { get; set; }
        public int valueDesiredTemperature { get; set; }
        public int valuePowerStatus { get; set; }
        public ArrayList subscriptions { get; set; }
    }
}

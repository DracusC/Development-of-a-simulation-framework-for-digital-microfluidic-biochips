using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Actuators
    {
        //protected string name, type;
        //protected int ID, actuatorID = -1, positionX, positionY, sizeX, sizeY;
        //ArrayList subscriptions;


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

using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Actuators
    {
        protected string name, type;
        protected int ID, actuatorID = -1, positionX, positionY, sizeX, sizeY;
        ArrayList subscriptions;


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

        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public int ID1 { get => ID; set => ID = value; }
        public int ActuatorID { get => actuatorID; set => actuatorID = value; }
        public int PositionX { get => positionX; set => positionX = value; }
        public int PositionY { get => positionY; set => positionY = value; }
        public int SizeX { get => sizeX; set => sizeX = value; }
        public int SizeY { get => sizeY; set => sizeY = value; }
        public ArrayList Subscriptions { get => subscriptions; set => subscriptions = value; }
    }
}

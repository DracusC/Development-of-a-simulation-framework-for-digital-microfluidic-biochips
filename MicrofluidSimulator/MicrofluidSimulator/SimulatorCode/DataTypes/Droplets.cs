using System.Collections;
namespace MicrofluidSimulator.SimulatorCode
{
    public class Droplets
    {
        string name, substance_name, color;
        int ID, positionX, positionY, sizeX, sizeY;
        float temperature;
        float volume;
        int group;
        int electrodeID;

        ArrayList subscriptions;
        int nextModel;
        String[] modelOrder;


        public Droplets(string name, int ID, string substance_name, int positionX, int positionY, int sizeX, int sizeY, string color, float temperature, float volume, int electrodeID, int group)
        {
            this.name = name;
            this.substance_name = substance_name;
            this.color = color;
            this.ID = ID;
            this.positionX = positionX;
            this.positionY = positionY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.temperature = temperature;
            this.volume = volume;
            this.group = group;
            this.electrodeID = electrodeID;
            this.subscriptions = new ArrayList();
            NextModel = 0;
            ModelOrder = new string[] {"split","merge", "temperature", "color"};
        }

        public Droplets()
        {
        }

        public string Name { get => name; set => name = value; }
        public string Substance_name { get => substance_name; set => substance_name = value; }
        public string Color { get => color; set => color = value; }
        public int ID1 { get => ID; set => ID = value; }
        public int PositionX { get => positionX; set => positionX = value; }
        public int PositionY { get => positionY; set => positionY = value; }
        public int SizeX { get => sizeX; set => sizeX = value; }
        public int SizeY { get => sizeY; set => sizeY = value; }
        public float Temperature { get => temperature; set => temperature = value; }
        public int ElectrodeID { get => electrodeID; set => electrodeID = value; }
        public ArrayList Subscriptions { get => subscriptions; set => subscriptions = value; }
        public float Volume { get => volume; set => volume = value; }
        public int Group { get => group; set => group = value; }
        public int NextModel { get => nextModel; set => nextModel = value; }
        public string[] ModelOrder { get => modelOrder; set => modelOrder = value; }

        public override string ToString()
        {
            String concat = "Name: " + Name + "\nID: " + ID1.ToString() + "\nPositionX: " + PositionX.ToString() + "\nPositionY: "
                + PositionY.ToString() + "\nSizeX: " + SizeX.ToString() + "\nSizeY: " + SizeY.ToString();
            return concat;
        }

    }
}

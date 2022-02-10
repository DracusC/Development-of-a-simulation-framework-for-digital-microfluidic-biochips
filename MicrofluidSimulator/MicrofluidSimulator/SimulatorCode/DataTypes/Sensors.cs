namespace MicrofluidSimulator.SimulatorCode
{
    public class Sensors
    {
        string name, type;
        int ID, sensorID = -1, positionX, positionY, sizeX, sizeY;

        

        public Sensors(string name, int ID, int sensorID, string type, int positionX, int positionY, int sizeX, int sizeY)
        {
            this.name = name;
            this.type = type;
            this.ID = ID;
            this.sensorID = sensorID;
            this.positionX = positionX;
            this.positionY = positionY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }

        public Sensors()
        {
        }

        public string Name { get => name; set => name = value; }
        public string Type { get => type; set => type = value; }
        public int ID1 { get => ID; set => ID = value; }
        public int SensorID { get => sensorID; set => sensorID = value; }
        public int PositionX { get => positionX; set => positionX = value; }
        public int PositionY { get => positionY; set => positionY = value; }
        public int SizeX { get => sizeX; set => sizeX = value; }
        public int SizeY { get => sizeY; set => sizeY = value; }
    }
}

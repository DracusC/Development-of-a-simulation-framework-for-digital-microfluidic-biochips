namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class Sensors
    {
        //string name, type;
        //int ID, sensorID, positionX, positionY, sizeX, sizeY;
        //public int electrodeId;




        public Sensors(string name, int ID, int sensorID, string type, int positionX, int positionY, int sizeX, int sizeY, int electrodeID)
        {
            this.name = name;
            this.type = type;
            this.ID = ID;
            this.sensorID = sensorID;
            this.positionX = positionX;
            this.positionY = positionY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.electrodeID = electrodeID;

        }

        public Sensors()
        {
        }

        public string name { get; set; }
        public string type { get; set; }
        public int ID { get; set; }
        public int sensorID { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int electrodeID { get; set; }
        public int valueRed { get; set; }
        public int valueGreen { get; set; }
        public int valueBlue { get; set; }
        public float valueTemperature { get; set; }
    }
}

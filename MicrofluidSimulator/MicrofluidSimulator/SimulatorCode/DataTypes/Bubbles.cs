namespace MicrofluidSimulator.SimulatorCode
{
    public class Bubbles
    {
        //string name;
        //int ID, positionX, positionY, sizeX, sizeY;
        

        public Bubbles(string name, int ID, int positionX, int positionY, int sizeX, int sizeY)
        {
            this.name = name;
            this.ID = ID;
            this.positionX = positionX;
            this.positionY = positionY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }

        public Bubbles()
        {
        }

        public string name { get; set; }
        public int ID { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
    }
}

namespace MicrofluidSimulator.SimulatorCode
{
    public class Bubbles
    {
        string name;
        int ID, positionX, positionY, sizeX, sizeY;
        

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

        public string Name { get => name; set => name = value; }
        public int ID1 { get => ID; set => ID = value; }
        public int PositionX { get => positionX; set => positionX = value; }
        public int PositionY { get => positionY; set => positionY = value; }
        public int SizeX { get => sizeX; set => sizeX = value; }
        public int SizeY { get => sizeY; set => sizeY = value; }
    }
}

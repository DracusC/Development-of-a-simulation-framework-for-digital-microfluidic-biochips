namespace MicrofluidSimulator.SimulatorCode
{
    public class Droplets
    {
        string name, substance_name, color;
        int ID, positionX, positionY, sizeX, sizeY;
        float temperature;

        

        public Droplets(string name, int ID, string substance_name, int positionX, int positionY, int sizeX, int sizeY, string color, float temperature)
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

        public override string? ToString()
        {
            return base.ToString();
        }

    }
}

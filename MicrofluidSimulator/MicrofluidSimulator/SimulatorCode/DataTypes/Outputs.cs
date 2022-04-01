namespace MicrofluidSimulator.SimulatorCode
{
    public class Outputs
    {
        //string name;
        //int ID, outputID = -1, positionX, positionY;

        
        public Outputs(string name, int ID, int outputID, int positionX, int positionY)
        {
            this.name = name;
            this.ID = ID;
            this.outputID = outputID;
            this.positionX = positionX;
            this.positionY = positionY;
        }

        public Outputs()
        {
        }
        public string name { get => name; set => name = value; }
        public int ID { get => ID; set => ID = value; }
        public int outputID { get => outputID; set => outputID = value; }
        public int positionX { get => positionX; set => positionX = value; }
        public int positionY { get => positionY; set => positionY = value; }

    }
}

namespace MicrofluidSimulator.SimulatorCode
{
    public class Inputs
    {
        //string name;
        //int ID, inputID = -1, positionX, positionY;

        

        public Inputs(string name, int ID, int inputID, int positionX, int positionY)
        {
            this.name = name;
            this.ID = ID;
            this.inputID = inputID;
            this.positionX = positionX;
            this.positionY = positionY;
            this.inputID = -1;
        }

        public Inputs()
        {
        }

        public string name { get; set; }
        public int ID { get; set; }
        public int inputID { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
    }
}

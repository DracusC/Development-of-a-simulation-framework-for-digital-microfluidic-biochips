namespace MicrofluidSimulator.SimulatorCode
{
    public class Inputs
    {
        string name;
        int ID, inputID = -1, positionX, positionY;

        

        public Inputs(string name, int ID, int inputID, int positionX, int positionY)
        {
            this.name = name;
            this.ID = ID;
            this.inputID = inputID;
            this.positionX = positionX;
            this.positionY = positionY;
        }

        public Inputs()
        {
        }

        public string Name { get => name; set => name = value; }
        public int ID1 { get => ID; set => ID = value; }
        public int InputID { get => inputID; set => inputID = value; }
        public int PositionX { get => positionX; set => positionX = value; }
        public int PositionY { get => positionY; set => positionY = value; }
    }
}

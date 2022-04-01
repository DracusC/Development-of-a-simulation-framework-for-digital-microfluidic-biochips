using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode
{
    public class Electrode
    {
        //string name;
        //int ID, electrodeID, driverID, shape, positionX, positionY, sizeX, sizeY, status;
        //int[,] corners;

        

        //ArrayList subscriptions;
        //ArrayList neighbours;

        public Electrode(string name, int ID, int electrodeID, int driverID, int shape, int positionX, int positionY, int sizeX, int sizeY, int status, List<List<int>> corners)
        {
            this.name = name;
            this.ID = ID;
            this.electrodeID = electrodeID;
            this.driverID = driverID;
            this.shape = shape;
            this.positionX = positionX;
            this.positionY = positionY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.status = status;
            this.corners = corners;

            // additional additions
            this.subscriptions = new ArrayList();
            this.neighbours = new ArrayList();
        }

        public string name { get; set; }
        public int ID { get; set; }
        public int electrodeID { get; set; }
        public int driverID { get; set; }
        public int shape { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int status { get; set; }
        public List<List<int>> corners { get; set; }
        public ArrayList subscriptions { get; set; }
        public ArrayList neighbours { get; set; }

       
    }
}

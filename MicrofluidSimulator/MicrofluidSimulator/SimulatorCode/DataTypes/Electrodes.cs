using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode
{
    public class Electrodes
    {
        string name;
        int ID, electrodeID, driverID, shape, positionX, positionY, sizeX, sizeY, status;
        int[,] corners;
        ArrayList subscriptions;

        public Electrodes(string name, int ID, int electrodeID, int driverID, int shape, int positionX, int positionY, int sizeX, int sizeY, int status, int[,] corners, ArrayList subscriptions)
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
            this.subscriptions = subscriptions;
        }

        public Electrodes()
        {

        }

        public string Name { get => name; set => name = value; }
        public int ID1 { get => ID; set => ID = value; }
        public int ElectrodeID { get => electrodeID; set => electrodeID = value; }
        public int DriverID { get => driverID; set => driverID = value; }
        public int Shape { get => shape; set => shape = value; }
        public int PositionX { get => positionX; set => positionX = value; }
        public int PositionY { get => positionY; set => positionY = value; }
        public int SizeX { get => sizeX; set => sizeX = value; }
        public int SizeY { get => sizeY; set => sizeY = value; }
        public int Status { get => status; set => status = value; }
        public int[,] Corners { get => corners; set => corners = value; }
        public ArrayList Subscriptions { get => subscriptions; set => subscriptions = value; }
    }
}

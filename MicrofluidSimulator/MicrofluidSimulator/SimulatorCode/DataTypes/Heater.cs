using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    
    public class Heater : Actuators
    {
        int actualTemperature, desiredTemperature, nextDesiredTemperature;
        bool status, nextStatus;
        int[,] corners;
        ArrayList subscriptions;
        
        public Heater(string name, int ID, int actuatorID, string type, int positionX, int positionY, int sizeX, int sizeY, int actualTemperature, int desiredTemperature, 
            bool status, int nextDesiredTemperature, bool nextStatus, int[,] corners)
            :base(name, ID, actuatorID, type, positionX, positionY, sizeX, sizeY)
        {
            this.actualTemperature = actualTemperature;
            this.desiredTemperature = desiredTemperature;
            this.nextDesiredTemperature = nextDesiredTemperature;
            this.status = status;
            this.nextStatus = nextStatus;
            this.corners = corners;
            this.subscriptions = new ArrayList();
        }

        
        public int ActualTemperature { get => actualTemperature; set => actualTemperature = value; }
        public int DesiredTemperature { get => desiredTemperature; set => desiredTemperature = value; }
        public int NextDesiredTemperature { get => nextDesiredTemperature; set => nextDesiredTemperature = value; }
        public bool Status { get => status; set => status = value; }
        public bool NextStatus { get => nextStatus; set => nextStatus = value; }
        public int[,] Corners { get => corners; set => corners = value; }
        public ArrayList Subscriptions { get => subscriptions; set => subscriptions = value; }
    }
}

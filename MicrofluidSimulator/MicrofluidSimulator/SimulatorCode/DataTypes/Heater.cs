using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    
    public class Heater : Actuators
    {
        //int valueActualTemperature, valueDesiredTemperature, valuePowerStatus;

        //ArrayList subscriptions;
        
        public Heater(string name, int ID, int actuatorID, string type, int positionX, int positionY, int sizeX, int sizeY, int valueActualTemperature, int valueDesiredTemperature, 
            int valuePowerStatus)
            :base(name, ID, actuatorID, type, positionX, positionY, sizeX, sizeY)
        {
            this.valueActualTemperature = valueActualTemperature;
            this.valueDesiredTemperature = valueDesiredTemperature;
            this.valuePowerStatus = valuePowerStatus;
            
            
        }

        public int valueActualTemperature { get; set; }
        public int valueDesiredTemperature { get; set; }
        public int valuePowerStatus { get; set; }

    }
}

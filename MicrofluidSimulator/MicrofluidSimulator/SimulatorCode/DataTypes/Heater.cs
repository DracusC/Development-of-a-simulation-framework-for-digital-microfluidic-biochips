using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    
    public class Heater : Actuators
    {
        int valueActualTemperature, valueDesiredTemperature, valuePowerStatus;

        ArrayList subscriptions;
        
        public Heater(string name, int ID, int actuatorID, string type, int positionX, int positionY, int sizeX, int sizeY, int valueActualTemperature, int valueDesiredTemperature, 
            int valuePowerStatus)
            :base(name, ID, actuatorID, type, positionX, positionY, sizeX, sizeY)
        {
            this.valueActualTemperature = valueActualTemperature;
            this.valueDesiredTemperature = valueDesiredTemperature;
            this.valuePowerStatus = valuePowerStatus;
            
            
        }

        public int ValueActualTemperature { get => valueActualTemperature; set => valueActualTemperature = value; }
        public int ValueDesiredTemperature { get => valueDesiredTemperature; set => valueDesiredTemperature = value; }
        public int ValuePowerStatus { get => valuePowerStatus; set => valuePowerStatus = value; }
       
    }
}

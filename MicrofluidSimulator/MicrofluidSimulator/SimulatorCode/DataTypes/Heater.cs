using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    
    public class Heater : Actuators
    {
        
        public Heater(string name, int ID, int actuatorID, string type, int positionX, int positionY, int sizeX, int sizeY, float valueActualTemperature, float valueDesiredTemperature, 
            float valuePowerStatus)
            :base(name, ID, actuatorID, type, positionX, positionY, sizeX, sizeY)
        {
            this.valueActualTemperature = valueActualTemperature;
            this.valueDesiredTemperature = valueDesiredTemperature;
            this.valuePowerStatus = valuePowerStatus;
            
            
        }

        public float GetTargetTemperature()
        {
            return valueDesiredTemperature;
        }

        public float GetCurrentTemperature()
        {
            return valueActualTemperature;
        }

        public float GetCurrentPower()
        {
            return valuePowerStatus;
        }

        public void SetTargetTemperature(float targetTemperature)
        {
            
            this.valueDesiredTemperature = targetTemperature;

        }

        public void SetPowerStatus()
        {
            if (this.valueActualTemperature == this.valueDesiredTemperature)
            {
                this.valuePowerStatus = 0;
            }
            else if (this.valueActualTemperature < this.valueDesiredTemperature)
            {
                
                this.valuePowerStatus = 0.78F;
                
            }
            else
            {
               
                //currently we are using a negative powerstatus to decrease the temperature of the heater
                this.valuePowerStatus = -0.58F;
                
            }
        }

    }
}

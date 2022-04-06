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
            Console.WriteLine("targetTemperature SET TO 100");
            this.valueDesiredTemperature = targetTemperature;

        }

        public void SetPowerStatus()
        {
            if (this.valueDesiredTemperature == this.valueActualTemperature)
            {
                this.valuePowerStatus = 0;
            }
            else if (this.valueActualTemperature < this.valueDesiredTemperature)
            {
                Console.WriteLine("POWERSTATUS SET TO 1");
                this.valuePowerStatus = 1;
                
            }
            else
            {
                this.valuePowerStatus = -1;
                
            }
        }

    }
}

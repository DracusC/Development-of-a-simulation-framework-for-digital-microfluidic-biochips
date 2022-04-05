using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    
    public class Heater : Actuators
    {
        //int valueActualTemperature, valueDesiredTemperature, valuePowerStatus;

        //ArrayList subscriptions;
        
        public Heater(string name, int ID, int actuatorID, string type, int positionX, int positionY, int sizeX, int sizeY, float valueActualTemperature, float valueDesiredTemperature, 
            float valuePowerStatus)
            :base(name, ID, actuatorID, type, positionX, positionY, sizeX, sizeY)
        {
            this.valueActualTemperature = valueActualTemperature;
            this.valueDesiredTemperature = valueDesiredTemperature;
            this.valuePowerStatus = valuePowerStatus;
            
            
        }

        public float valueActualTemperature { get; set; }
        public float valueDesiredTemperature { get; set; }
        public float valuePowerStatus { get; set; }

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

        public ArrayList SetTargetTemperature(float targetTemperature)
        {
            this.valueDesiredTemperature = targetTemperature;

            if (this.valueDesiredTemperature == this.valueActualTemperature)
            {
                return this.subscriptions;
            }
            else if (this.valueActualTemperature < this.valueDesiredTemperature)
            {
                this.valuePowerStatus = 1;
                return this.subscriptions;
            }
            else
            {
                this.valuePowerStatus = -1;
                return this.subscriptions;
            }

        }

    }
}

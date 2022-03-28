using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HeaterModels
    {
        public static ArrayList heaterTemperatureChange(Container container, Heater heater, SimulatorAction action, float deltaTime)
        {
            // get the desired temperature
            heater.ValueDesiredTemperature = action.ActionChange;

            heater.ValuePowerStatus = 1; // i dont know yet

            if (heater.ValueDesiredTemperature == heater.ValueActualTemperature)
            {
                return heater.Subscriptions;
            }else if(heater.ValueActualTemperature < heater.ValueDesiredTemperature)
            {
                for(int i = 0; i < (int) deltaTime; i++)
                {
                    heater.ValueActualTemperature += 2; // temp increase is currently 2 per second
                }
                
            }
            else
            {
                for(int i = 0; i < (int) deltaTime; i++)
                {
                    heater.ValueActualTemperature -= 2; // temp decrease is currently 2 per second
                }
                
            }
            
            // give back the subscribers of the heater
            return heater.Subscriptions;
        }
    }
}

using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HeaterActuatorModels
    {
        
        public static ArrayList heaterTemperatureChange(Container container, Heater heater)
        {
            
            heater.valueActualTemperature = (int) (heater.valueActualTemperature + container.timeStep * heater.valuePowerStatus);
            
            return heater.subscriptions;

            
        }
    }
}

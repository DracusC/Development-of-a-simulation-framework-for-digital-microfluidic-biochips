using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HeaterActuatorModels
    {
        /// <summary>
        /// Model for changing the temprature of the heater actuator
        /// </summary>
        /// <param name="container"></param>
        /// <param name="heater"></param>
        /// <returns></returns>
        public static ArrayList heaterTemperatureChange(Container container, Heater heater)
        {
            heater.SetPowerStatus();
            heater.valueActualTemperature = (heater.valueActualTemperature + (float) container.timeStep * heater.valuePowerStatus);
            
            return heater.subscriptions;

            
        }
    }
}

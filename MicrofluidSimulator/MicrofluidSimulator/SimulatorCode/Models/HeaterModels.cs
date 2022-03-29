using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HeaterModels
    {
        public static ArrayList heaterTemperatureCalled(Container container, Heater heater, SimulatorAction action)
        {
            heater.ValueDesiredTemperature = action.ActionChange;

            if (heater.ValueDesiredTemperature == heater.ValueActualTemperature)
            {
                return heater.Subscriptions;
            }
            else if (heater.ValueActualTemperature < heater.ValueDesiredTemperature)
            {
                heater.ValuePowerStatus = 1;
                return heater.Subscriptions;
            }
            else
            {
                heater.ValuePowerStatus = -1;
                return heater.Subscriptions;
            }

        }
        public static ArrayList heaterTemperatureChange(Container container, Heater heater, SimulatorAction action)
        {
            heater.ValueActualTemperature = (int) (heater.ValueActualTemperature + container.TimeStep * heater.ValuePowerStatus);

            // give back the subscribers of the heater
            return heater.Subscriptions;

            
        }
    }
}

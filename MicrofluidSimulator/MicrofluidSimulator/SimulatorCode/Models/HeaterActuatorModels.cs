using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HeaterActuatorModels
    {
        public static ArrayList heaterTemperatureCalled(Container container, Heater heater, SimulatorAction action)
        {
            heater.valueDesiredTemperature = action.actionChange;

            if (heater.valueDesiredTemperature == heater.valueActualTemperature)
            {
                return heater.subscriptions;
            }
            else if (heater.valueActualTemperature < heater.valueDesiredTemperature)
            {
                heater.valuePowerStatus = 1;
                return heater.subscriptions;
            }
            else
            {
                heater.valuePowerStatus = -1;
                return heater.subscriptions;
            }

        }
        public static ArrayList heaterTemperatureChange(Container container, Heater heater, SimulatorAction action)
        {
            heater.valueActualTemperature = (int) (heater.valueActualTemperature + container.timeStep * heater.valuePowerStatus);

            // give back the subscribers of the heater
            return heater.subscriptions;

            
        }
    }
}

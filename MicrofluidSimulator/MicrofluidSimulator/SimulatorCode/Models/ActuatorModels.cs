using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class ActuatorModels
    {
        internal static ArrayList heaterTemperatureChange(Container container, Heater heater, SimulatorAction action)
        {
            
            heater.ActualTemperature = action.ActionChange;
            
            // give back the subscribers of the heater
            return heater.Subscriptions;
        }
    }
}

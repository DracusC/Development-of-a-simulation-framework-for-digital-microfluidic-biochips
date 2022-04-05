﻿using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HeaterActuatorModels
    {
        
        public static ArrayList heaterTemperatureChange(Container container, Heater heater, SimulatorAction action)
        {
            heater.valueActualTemperature = (int) (heater.valueActualTemperature + container.timeStep * heater.valuePowerStatus);

            // give back the subscribers of the heater
            return heater.subscriptions;

            
        }
    }
}

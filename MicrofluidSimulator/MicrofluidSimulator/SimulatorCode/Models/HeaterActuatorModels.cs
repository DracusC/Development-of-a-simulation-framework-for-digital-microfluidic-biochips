﻿using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;

namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HeaterActuatorModels
    {
        
        public static ArrayList heaterTemperatureChange(Container container, Heater heater)
        {
            heater.SetPowerStatus();
            heater.valueActualTemperature = (heater.valueActualTemperature + (float) container.timeStep * heater.valuePowerStatus);
            
            return heater.subscriptions;

            
        }
    }
}

﻿using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletTemperatureModels
    {
        public static ArrayList dropletTemperatureChange(Container container, Droplets caller)
        {
            ArrayList subscribers = new ArrayList();
            subscribers.Add(caller.ID);
            //return subscribers;

            Heater heater = HelpfullRetreiveFunctions.getHeaterOnDroplet(container, caller);
            
            if(heater == null)
            {
                return subscribers;
            }
            else
            {
                float mass = HelpfullRetreiveFunctions.getMassOfDropletGivenSubstance(caller);
                float hC = HelpfullRetreiveFunctions.getHeatCapacityOfDropletGivenSubstance(caller);
                float k = HelpfullRetreiveFunctions.getThermalConductivityOfDroplet(caller);
                float A = HelpfullRetreiveFunctions.getAreaOfDroplet(caller);
                float heatTransfer = k * A * (heater.valueActualTemperature - caller.temperature) * container.timeStep;

                float temperatureChange = heatTransfer / (mass * hC);
                


                // droplet temperature change is right now dependent on the powerstatus of the heater, this should later be changed to be dependent on the heater temperature
                //float temperatureChange = (container.timeStep * heater.valuePowerStatus) / (mass * hC);
                caller.temperature += temperatureChange;
                return subscribers;
            }
            
            
            return subscribers;

        }

        

        
    }
}
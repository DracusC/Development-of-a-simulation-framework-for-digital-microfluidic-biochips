using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletTemperatureModels
    {
        public static ArrayList dropletTemperatureChange(Container container, Droplets caller)
        {
            ArrayList subscribers = new ArrayList();
            
            Heater heater = HelpfullRetreiveFunctions.getHeaterOnDroplet(container, caller);
            
            if(heater == null)
            {
                subscribers.Add(caller.ID1);
                return subscribers;
            }
            else
            {
                float mass = HelpfullRetreiveFunctions.getMassOfDropletGivenSubstance(caller);
                float hC = HelpfullRetreiveFunctions.getHeatCapacityOfDropletGivenSubstance(caller);
                

                // droplet temperature change is right now dependent on the powerstatus of the heater, this should later be changed to be dependent on the heater temperature
                float temperatureChange = (container.TimeStep * heater.ValuePowerStatus) / (mass * hC);
                caller.Temperature += temperatureChange;
                return subscribers;
            }
            
            subscribers.Add(caller.ID1);
            return subscribers;

        }

        

        
    }
}

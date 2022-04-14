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
            subscribers.Add(caller.ID);
            //return subscribers;

            Heater heater = HelpfullRetreiveFunctions.getHeaterOnDroplet(container, caller);
            
            if(heater == null)
            {
                float mass = HelpfullRetreiveFunctions.getMassOfDropletGivenSubstance(caller);
                float hC = HelpfullRetreiveFunctions.getHeatCapacityOfDropletGivenSubstance(caller);
                float k = HelpfullRetreiveFunctions.getThermalConductivityOfDroplet(caller);
                float A = HelpfullRetreiveFunctions.getAreaOfDroplet(caller);
                float heatTransfer = k * A * (GlobalVariables.ROOMTEMPERATURE - caller.temperature) * container.timeStep;

                float temperatureChange = heatTransfer / (mass * hC);



                // droplet temperature change is right now dependent on the powerstatus of the heater, this should later be changed to be dependent on the heater temperature
                //float temperatureChange = (container.timeStep * heater.valuePowerStatus) / (mass * hC);
                caller.temperature += temperatureChange;
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

        
        public static void updateGroupTemperature(Container container, int group, Droplets droplet)
        {
            
            float groupMassTempHeatCapacity = 0;
            float groupMassHeatCapacity = 0;
                

            List<Droplets> groupMembers = DropletUtillityFunctions.findGroupMembers(container, group);

            foreach (Droplets dropletInGroup in groupMembers)
            {
                if(dropletInGroup.ID != droplet.ID)
                {
                    float temperatureOfDropletInGroup = dropletInGroup.temperature;
                    float massOfDropletInGroup = HelpfullRetreiveFunctions.getMassOfDropletGivenSubstance(dropletInGroup);
                    float heatCapacityOfDropletInGroup = HelpfullRetreiveFunctions.getHeatCapacityOfDropletGivenSubstance(dropletInGroup);

                    groupMassTempHeatCapacity += temperatureOfDropletInGroup * massOfDropletInGroup * heatCapacityOfDropletInGroup;
                    groupMassHeatCapacity += massOfDropletInGroup * heatCapacityOfDropletInGroup;
                }
                
            }
            float dropletMassTempHeatCapacity = droplet.temperature * HelpfullRetreiveFunctions.getMassOfDropletGivenSubstance(droplet) * HelpfullRetreiveFunctions.getHeatCapacityOfDropletGivenSubstance(droplet);
            float dropletMassHeatCapacity = HelpfullRetreiveFunctions.getMassOfDropletGivenSubstance(droplet) * HelpfullRetreiveFunctions.getHeatCapacityOfDropletGivenSubstance(droplet);
            float newTemperature = (groupMassTempHeatCapacity + dropletMassTempHeatCapacity) / (groupMassHeatCapacity + dropletMassHeatCapacity);
            foreach (Droplets dropletInGroup in groupMembers)
            {
                dropletInGroup.temperature = newTemperature;
            }
            droplet.temperature = newTemperature;
        }


    }
}

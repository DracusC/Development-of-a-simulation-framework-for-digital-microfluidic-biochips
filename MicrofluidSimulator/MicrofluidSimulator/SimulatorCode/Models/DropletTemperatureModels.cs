using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletTemperatureModels
    {

        /// <summary>
        /// Models that changes the temperature of the droplet
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static ArrayList dropletTemperatureChange(Container container, Droplets caller)
        {
            ArrayList subscribers = new ArrayList
            {
                caller.ID
            };

            Heater heater = (Heater) HelpfullRetreiveFunctions.getActuatorOnDroplet(container, caller, "heater");
            
            if(heater == null)
            {
                float mass = HelpfullRetreiveFunctions.getMassOfDropletGivenSubstance(caller);
                float hC = HelpfullRetreiveFunctions.getHeatCapacityOfDropletGivenSubstance(caller);
                float k = HelpfullRetreiveFunctions.getThermalConductivityOfDroplet(caller);
                float A = HelpfullRetreiveFunctions.getAreaOfDroplet(caller);
                float heatTransfer = k * A * (GlobalVariables.ROOMTEMPERATURE - caller.temperature) * (float) container.timeStep;

                float temperatureChange = heatTransfer / (mass * hC);



                
                caller.temperature += temperatureChange;
                return subscribers;
            }
            else
            {
                
                float mass = HelpfullRetreiveFunctions.getMassOfDropletGivenSubstance(caller);
                float hC = HelpfullRetreiveFunctions.getHeatCapacityOfDropletGivenSubstance(caller);
                float k = HelpfullRetreiveFunctions.getThermalConductivityOfDroplet(caller);
                float A = HelpfullRetreiveFunctions.getAreaOfDroplet(caller);
                float heatTransfer = k * A * (heater.valueActualTemperature - caller.temperature) * (float) container.timeStep;
                
                float temperatureChange = heatTransfer / (mass * hC);
                

                
                caller.temperature += temperatureChange;

                
                return subscribers;
            }

        }

        /// <summary>
        /// Function that takes all group members based on a group ID and update their temperature weigted
        /// </summary>
        /// <param name="container"></param>
        /// <param name="group"></param>
        /// <param name="droplet"></param>
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

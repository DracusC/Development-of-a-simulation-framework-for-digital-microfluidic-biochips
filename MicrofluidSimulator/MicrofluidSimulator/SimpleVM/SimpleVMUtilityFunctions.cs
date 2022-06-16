using MicrofluidSimulator.SimulatorCode;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;

namespace MicrofluidSimulator.SimpleVM
{
    public class SimpleVMUtilityFunctions
    {
        /// <summary>
        /// Utility functions for simpleVM
        /// </summary>
        /// <param name="heaterID"></param>
        /// <param name="desiredValue"></param>
        /// <returns></returns>
        public static int setActuatorTargetTemperature(int heaterID, float desiredValue, Container container)
        {
            Heater heater = (Heater)HelpfullRetreiveFunctions.getActuatorByID(container, heaterID);
            if (heater == null) { return 1; }

            heater.SetTargetTemperature(desiredValue);
            return 0;
        }
        /// <summary>
        /// Function that returns the color read by a sensor if the sensor is of type "RGB_color"
        /// </summary>
        /// <param name="sensorID"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static int[] getColorOfSensorWithID(int sensorID, Container container)
        {
            ColorSensor colorSensor = (ColorSensor)HelpfullRetreiveFunctions.getSensorByID(container, sensorID);
            if (colorSensor == null)
            {
                return new int[] { -1, -1, -1 };
            }
            else
            {
                int[] colorArray = ColorSensorModels.colorSensor(container, colorSensor);
                if (colorArray != null)
                {
                    colorSensor.valueRed = colorArray[0];
                    colorSensor.valueGreen = colorArray[1];
                    colorSensor.valueBlue = colorArray[2];
                    return colorSensor.GetColor();
                }

                return colorSensor.GetColor();
            }
        }
        /// <summary>
        /// Function that returns the temperature read by a sensor if the sensor is of type "temperature"
        /// </summary>
        /// <param name="sensorID"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static float getTemperatureOfSensorWithID(int sensorID, Container container)
        {
            // -1 might not be the best option for a return value, reconsider
            TemperatureSensor temperatureSensor = (TemperatureSensor)HelpfullRetreiveFunctions.getSensorByID(container, sensorID);
            if (temperatureSensor == null)
            {
                return -1;
            }
            else
            {
                float temp = TemperatureSensorModels.temperatureSensor(container, temperatureSensor);
                Droplets droplet = HelpfullRetreiveFunctions.getDropletOnSensor(container, temperatureSensor);
                if (temp != -1)
                {
                    temperatureSensor.valueTemperature = temp;
                    return temperatureSensor.GetTemperature();
                }
                else if (droplet == null)
                {
                    return -1;
                }


                return temperatureSensor.GetTemperature();
            }
        }



    }
}

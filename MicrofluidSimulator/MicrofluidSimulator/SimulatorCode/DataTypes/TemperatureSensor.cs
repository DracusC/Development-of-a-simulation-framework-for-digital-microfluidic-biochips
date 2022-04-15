namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class TemperatureSensor : Sensors
    {

        //int valueTemperature;
        public TemperatureSensor(string name, int ID, int sensorID, string type, int positionX, int positionY, int sizeX, int sizeY, float valueTemperature, int electrodeId) 
            : base(name, ID, sensorID, type, positionX, positionY, sizeX, sizeY, electrodeId)
        {
            this.valueTemperature = valueTemperature;
        }


        public float GetTemperature()
        {
            return valueTemperature;
        }
    }
}

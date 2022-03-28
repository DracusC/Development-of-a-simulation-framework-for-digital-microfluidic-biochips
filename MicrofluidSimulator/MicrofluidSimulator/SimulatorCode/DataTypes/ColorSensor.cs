﻿namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class ColorSensor : Sensors
    {
        int valueRed, valueGreen, valueBlue;
        
        public ColorSensor(string name, int ID, int sensorID, string type, int positionX, int positionY, int sizeX, int sizeY, int valueRed, int valueGreen, int valueBlue, int electrodeId) 
            :base(name, ID, sensorID, type, positionX, positionY, sizeX, sizeY, electrodeId)
        {
            this.valueRed = valueRed;
            this.valueGreen = valueGreen;
            this.valueBlue = valueBlue;
        }

        public int ValueRed { get => valueRed; set => valueRed = value; }
        public int ValueGreen { get => valueGreen; set => valueGreen = value; }
        public int ValueBlue { get => valueBlue; set => valueBlue = value; }
    }
}
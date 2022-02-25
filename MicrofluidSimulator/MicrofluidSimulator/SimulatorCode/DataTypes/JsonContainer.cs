﻿namespace MicrofluidSimulator.SimulatorCode.DataTypes
{
    public class JsonContainer
    {
        public Information information { get; set; }
        public List<Electrode> electrodes { get; set; }
        public List<object> actuators { get; set; }
        public List<object> sensors { get; set; }
        public List<object> inputs { get; set; }
        public List<object> outputs { get; set; }
        public List<object> droplets { get; set; }
        public List<object> bubbles { get; set; }
        public List<object> unclassified { get; set; }

        public string toString(int i)
        {
            return "electrodeName: " + electrodes[i].toString() + " platformName: " + information.toString();
        }
    }

    public class Information
    {
        public string platform_name { get; set; }
        public string platform_type { get; set; }
        public int platform_ID { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }

        public string toString()
        {
            return platform_name;
        }
    }

    public class Electrode
    {
        public string name { get; set; }
        public int ID { get; set; }
        public int electrodeID { get; set; }
        public int driverID { get; set; }
        public int shape { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int status { get; set; }
        public List<object> corners { get; set; }

        public string toString()
        {
            return name;
        }
    }

   
}

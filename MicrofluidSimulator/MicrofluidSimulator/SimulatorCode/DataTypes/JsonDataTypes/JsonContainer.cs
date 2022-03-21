namespace MicrofluidSimulator.SimulatorCode.DataTypes.JsonDataTypes
{
    public class JsonContainer
    {
        public Information information { get; set; }
        public List<Electrode> electrodes { get; set; }
        public List<Actuators> actuators { get; set; }
        public List<object> sensors { get; set; }
        public List<object> inputs { get; set; }
        public List<object> outputs { get; set; }
        public List<Droplets> droplets { get; set; }
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

    public class Actuators
    {
        public string name { get; set; }
        public int ID { get; set; }
        public int actuatorID { get; set; }
        public string type { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int actualTemperature { get; set; }
        public int desiredTemperature { get; set; }
        public bool status { get; set; }
        public int nextDesiredTemperature { get; set; }
        public bool nextStatus { get; set; }
        public List<object> corners { get; set; }
    }

    public class Droplets
    {
        public string name { get; set; }
        public int ID { get; set; }
        public string substance_name { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public string color { get; set; }
        public int temperature { get; set; }
        public int electrodeID { get; set; }

    }

}

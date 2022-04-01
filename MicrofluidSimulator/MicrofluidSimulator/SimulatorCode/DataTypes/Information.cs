
namespace MicrofluidSimulator.SimulatorCode
{

    public class Information
    {
        //string platform_name, platform_type;
        //int sizeX, sizeY, platform_ID;

        

        public Information(string platform_name, string platform_type, int platform_ID, int sizeX, int sizeY)
        {
            this.platform_name = platform_name;
            this.platform_type = platform_type;
            this.platform_ID = platform_ID;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }



        public Information()
        {

        }

        public string platform_name { get; set; }
        public string platform_type { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int platform_ID { get; set; }


    }
}

    


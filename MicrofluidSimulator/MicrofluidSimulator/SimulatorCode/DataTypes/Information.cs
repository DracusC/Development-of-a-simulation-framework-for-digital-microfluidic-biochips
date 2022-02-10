namespace MicrofluidSimulator.SimulatorCode
{
    public class Information
    {
        string platform_name, platform_type;
        int sizeX, sizeY, platform_ID;

        

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

        public string Platform_name { get => platform_name; set => platform_name = value; }
        public string Platform_type { get => platform_type; set => platform_type = value; }
        public int SizeX { get => sizeX; set => sizeX = value; }
        public int SizeY { get => sizeY; set => sizeY = value; }
        public int Platform_ID { get => platform_ID; set => platform_ID = value; }


    }
}

    


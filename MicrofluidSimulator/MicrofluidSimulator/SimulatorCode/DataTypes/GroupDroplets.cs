using System.Collections;
namespace MicrofluidSimulator.SimulatorCode
{
    public class GroupDroplets
    {
        /*
            Group_ID: id,
            Substance_name: group[0].Substance_name,
            Color: group[0].Color,
            Temperature: 0,
            Volume: 0,
            Droplets: []
         */


        public GroupDroplets(int groupID, string substance_name, string color, float temperature, float volume)
        {
            this.groupID = groupID;
            this.substance_name = substance_name;
            this.color = color;
            this.temperature = temperature;
            this.volume = volume;
            this.droplets = new List<int>();
        }

        public GroupDroplets()
        {
        }

        public int groupID { get; set; }
        public string substance_name { get; set; }
        public string color { get; set; }
        public float temperature { get; set; }
        public float volume { get; set; }
        public List<int> droplets { get; set; }

    }
}

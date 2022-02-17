using MicrofluidSimulator.SimulatorCode.DataTypes
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HelpfullRetreiveFunctions
    {
        public static int getIndexOfElectrodeByID(int ID, Container container)
        {

            Electrodes[] electrodes = container.Electrodes;
            if(ID < electrodes.Count())
            {
                if(electrodes[ID].ID1 == ID)
                {
                    return ID;
                }

                


            }
            return binarySearchElectrode(ID, container);
        }

        public static int binarySearchElectrode(int ID, Container container)
        {
            Electrodes[] electrodes = container.Electrodes;
            int min = 0;
            int max = electrodes.Count() - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (ID == electrodes[mid].ID1)
                {
                    return mid;
                }
                else if (ID < electrodes[mid].ID1)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }
            return -1;
        }
    }

}

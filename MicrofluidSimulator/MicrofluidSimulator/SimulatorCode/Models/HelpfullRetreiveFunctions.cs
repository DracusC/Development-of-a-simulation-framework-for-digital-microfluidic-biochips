using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class HelpfullRetreiveFunctions
    {
        /* Function that is used when a id of an electrode is known and the index 
         * in the electrode array i needed
         * It first tries the id as index, if this dosent work it does a binary search*/

        public static int getIndexOfDropletByID(int ID, Container container)
        {
            ArrayList droplets = container.Droplets;
            int i = 0;
            foreach(Droplets droplet in droplets)
            {
               if(droplet.ID1 == ID)
               {
                    return i;
               }
               i++;
            }
            return -1;

        }

        public static int getIdOfElectrodByElectrodId(int electrodeId, int driverId, Container container)
        {
            Electrodes[] electrodes = container.Electrodes;

            foreach(Electrodes electrode in electrodes)
            {
                if(electrode.ElectrodeID == electrodeId && electrode.DriverID == driverId)
                {
                    return electrode.ID1;
                }
            }
            return -1;
        }

        public static int getIndexOfElectrodeByID(int ID, Container container)
        {

            Electrodes[] electrodes = container.Electrodes;
            //if (ID < electrodes.Count())
            //{
                if (electrodes[ID].ID1 == ID)
                {
                    return ID;
                }
                //for (int i = 0; i < electrodes.Count(); i++)
                //{
                //    if (electrodes[i].ID1 == ID)
                //    {
                //        return i;
                //    }
                //}


            //}
            //Console.WriteLine("WFT!" + ID);
            //return 0; 
            return binarySearchElectrode(ID, container);
        }

        public static int getIndexOfActuatorByID(int ID, Container container)
        {

            Actuators[] actuators = container.Actuators;
            
            if (actuators[ID].ID1 == ID)
            {
                return ID;
            }
            
            return binarySearchActuators(ID, container);
        }

        public static int getIDofElectrodeByPosition(int positionX, int positionY, Electrodes[] electrodeBoard)
        {
            
            for(int i = 0; i < electrodeBoard.Length; i++)
            {
                Electrodes electrode = electrodeBoard[i];
                if (electrode.PositionX <= positionX && electrode.PositionX+ electrode.SizeX >= positionX && electrode.PositionY <= positionY && electrode.PositionY + electrode.SizeY >= electrode.PositionY)
                {
                    return electrodeBoard[i].ID1;
                }
            }
            return -1;
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

        public static int binarySearchActuators(int ID, Container container)
        {
            Actuators[] actuators = container.Actuators;
            int min = 0;
            int max = actuators.Count() - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (ID == actuators[mid].ID1)
                {
                    return mid;
                }
                else if (ID < actuators[mid].ID1)
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

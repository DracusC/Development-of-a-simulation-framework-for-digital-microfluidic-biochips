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
            List<Droplets> droplets = container.droplets;
            int i = 0;
            foreach(Droplets droplet in droplets)
            {
               if(droplet.ID == ID)
               {
                    return i;
               }
               i++;
            }
            return -1;

        }

        public static float getMassOfDropletGivenSubstance(Droplets caller)
        {
            switch (caller.substance_name)
            {
              
                case "h20":
                    int waterDensity = 997;
                    return caller.volume * waterDensity;
                    break;
            }
            return -1;
        }

        public static float getHeatCapacityOfDropletGivenSubstance(Droplets caller)
        {
            switch (caller.substance_name)
            {

                case "h20":
                    int waterHeatCapacity = 4182;
                    return waterHeatCapacity;
                    break;
            }
            return -1;
        }

        public static Heater getHeaterOnDroplet(Container container, Droplets caller)
        {
            foreach (Actuators actuator in container.actuators)
            {
                if (actuator.type.Equals("heater"))
                {
                    int minMarginX = ((Heater)actuator).positionX;
                    int maxMarginX = ((Heater)actuator).positionX + ((Heater)actuator).sizeX;
                    int minMarginY = ((Heater)actuator).positionY;
                    int maxMarginY = ((Heater)actuator).positionY + ((Heater)actuator).sizeY;

                    if (caller.positionX <= minMarginX && caller.positionX >= maxMarginX && caller.positionY <= minMarginY && caller.positionY >= maxMarginY)
                    {
                        return (Heater)actuator;
                    }
                }
            }
            return null;
        }

        public static int getIdOfElectrodByElectrodId(int electrodeId, int driverId, Container container)
        {
            Electrode[] electrodes = container.electrodes;

            foreach(Electrode electrode in electrodes)
            {
                if(electrode.electrodeID == electrodeId && electrode.driverID == driverId)
                {
                    return electrode.ID;
                }
            }
            return -1;
        }

        public static int getIndexOfElectrodeByID(int ID, Container container)
        {

            Electrode[] electrodes = container.electrodes;
            //if (ID < electrodes.Count())
            //{
                if (electrodes[ID].ID == ID)
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

            Actuators[] actuators = container.actuators;
            
            if (actuators[ID].ID == ID)
            {
                return ID;
            }
            
            return binarySearchActuators(ID, container);
        }

        public static int getIDofElectrodeByPosition(int positionX, int positionY, Electrode[] electrodeBoard)
        {
            
            for(int i = 0; i < electrodeBoard.Length; i++)
            {
                Electrode electrode = electrodeBoard[i];
                if (electrode.positionX <= positionX && electrode.positionX+ electrode.sizeX >= positionX && electrode.positionY <= positionY && electrode.positionY + electrode.sizeY >= electrode.positionY)
                {
                    return electrodeBoard[i].ID;
                }
            }
            return -1;
        }

        public static int binarySearchElectrode(int ID, Container container)
        {
            Electrode[] electrodes = container.electrodes;
            int min = 0;
            int max = electrodes.Count() - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (ID == electrodes[mid].ID)
                {
                    return mid;
                }
                else if (ID < electrodes[mid].ID)
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
            Actuators[] actuators = container.actuators;
            int min = 0;
            int max = actuators.Count() - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (ID == actuators[mid].ID)
                {
                    return mid;
                }
                else if (ID < actuators[mid].ID)
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

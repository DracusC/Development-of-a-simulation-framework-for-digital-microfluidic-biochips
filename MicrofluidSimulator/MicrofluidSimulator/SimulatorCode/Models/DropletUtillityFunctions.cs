using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletUtillityFunctions
    {
        public static float getGroupVolume(Container container, ArrayList groupMembers)
        {
            List<Droplets> droplets = container.droplets;

            float volume = 0;
            foreach (Droplets droplet in groupMembers)
            {
                volume = volume + droplet.volume;
            }
            return volume;
        }

        public static void updateGroupNumber(Container container, Droplets caller, int newGroupID)
        {
            ArrayList connectedDroplets = new ArrayList();
            findAllConnectedDroplets(container, caller, connectedDroplets);
            foreach (Droplets droplet in connectedDroplets)
            {
                droplet.group = newGroupID;
            }
        }

        public static void findAllConnectedDroplets(Container container, Droplets caller, ArrayList members)
        {
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            members.Add(caller);
            foreach (int neigbour in dropletElectrode.neighbours)
            {

                int tempElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neigbour, container);
                Electrode tempElectrode = electrodeBoard[tempElectrodeIndex];

                Droplets tempDroplet = ElectrodeModels.electrodeHasDroplet(tempElectrode, container);
                if (tempDroplet != null && !members.Contains(tempDroplet))
                {
                    findAllConnectedDroplets(container, tempDroplet, members);
                }
            }
        }

        public static ArrayList findGroupMembers(Container container, int groupID)
        {
            ArrayList groupMembers = new ArrayList();
            List<Droplets> droplets = container.droplets;
            foreach (Droplets droplet in droplets)
            {
                if (droplet.group == groupID)
                {
                    groupMembers.Add(droplet);
                }
            }
            return groupMembers;

        }

        public static void updateGroupVolume(Container container, int groupID, float extraVolume)
        {
            List<Droplets> droplets = container.droplets;
            ArrayList groupMembers = DropletUtillityFunctions.findGroupMembers(container, groupID);
            float volume = DropletUtillityFunctions.getGroupVolume(container, groupMembers) + extraVolume;
            float newVolume = volume / groupMembers.Count;
            int diam = DropletUtillityFunctions.getDiameterOfDroplet(newVolume, 1);
            foreach (Droplets droplet in groupMembers)
            {
                droplet.volume = newVolume;
                droplet.sizeX = diam;
                droplet.sizeY = diam;
            }

        }

        public static double electrodeDistance(int electrodeCenterX, int electrodeCenterY, int dropletX, int dropletY)
        {
            double x = Math.Pow(dropletX - electrodeCenterX, 2);
            double y = Math.Pow(dropletY - electrodeCenterY, 2);

            return Math.Sqrt(x + y);
        }

        public static float getVolumeOfDroplet(float diameter, float height)
        {
            float pi = (float)Math.PI;
            return ((float)pi) * ((float)Math.Pow((diameter / 2), 2)) * height;
        }

        public static int getDiameterOfDroplet(float volume, float height)
        {
            float pi = (float)Math.PI;
            return (int)(2 * (float)(Math.Sqrt(volume / (pi * height))));
        }

    }
}

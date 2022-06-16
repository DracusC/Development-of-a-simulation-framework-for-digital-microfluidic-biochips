using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    /// <summary>
    /// Model for merging droplets
    /// </summary>
    public class DropletMerge
    {
        /// <summary>
        /// Model that decides when to merge and peform the merge
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static ArrayList dropletMerge(Container container, Droplets caller)
        {
            int callerID = caller.ID;
            ArrayList subscribers = new ArrayList();
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];

            //Merge in case a droplet merges into another droplet, the status of the lectrode must be off
            if (dropletElectrode.status == 0)
            {
                //find out how many it must merge to
                ArrayList onNeighbours = new ArrayList();
                foreach (int neighbour in dropletElectrode.neighbours)
                {
                    int electrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                    Electrode electrode = electrodeBoard[electrodeIndex];
                    if (electrode.status != 0 && (ElectrodeModels.electrodeHasDroplet(electrode, container) != null))
                    {
                        onNeighbours.Add(neighbour);
                    }
                }
                //if there are neigbouring droplets that are on ON electrodes merge is initiated
                if (onNeighbours.Count > 0)
                {
                    
                    DropletTemperatureModels.updateGroupTemperature(container, caller.group, caller);
                    ArrayList dropletSubscritions = caller.subscriptions;
                    foreach (int n in dropletSubscritions)
                    {

                        electrodeBoard[n].subscriptions.Remove(caller.ID);
                    }
                    container.subscribedDroplets.Remove(caller.ID);
                    //caller.Subscriptions = new ArrayList();
                    float volume = caller.volume;
                    int groupId = caller.group;
                    Color color = ColorTranslator.FromHtml(caller.color);
                    
                    int removedDropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
                    Electrode removedDropletElectrode = electrodeBoard[removedDropletElectrodeIndex];

                    
                    droplets.Remove(caller);



                    ArrayList handeledDroplet = new ArrayList();
                    ArrayList neighbouringDroplets = new ArrayList();


                    foreach (int neigbour in removedDropletElectrode.neighbours)
                    {
                        int tempElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neigbour, container);
                        Electrode tempElectrode = electrodeBoard[tempElectrodeIndex];

                        Droplets tempDroplet = ElectrodeModels.electrodeHasDroplet(tempElectrode, container);
                        if (tempDroplet != null)
                        {
                            neighbouringDroplets.Add(tempDroplet);
                        }
                    }
                    if (neighbouringDroplets.Count > 0)
                    {
                        Droplets firstDroplet = (Droplets)neighbouringDroplets[0];

                        DropletUtillityFunctions.updateGroupNumber(container, firstDroplet, firstDroplet.group);
                        handeledDroplet.Add(firstDroplet);

                        ArrayList connectedDroplets = new ArrayList();
                        DropletUtillityFunctions.findAllConnectedDroplets(container, firstDroplet, connectedDroplets);
                        foreach (Droplets d in connectedDroplets)
                        {
                            if (!handeledDroplet.Contains(d) && neighbouringDroplets.Contains(d))
                            {
                                handeledDroplet.Add(d);
                            }
                        }

                        foreach (Droplets droplet in neighbouringDroplets)
                        {
                            if (!handeledDroplet.Contains(droplet))
                            {
                                Random rnd = new Random();
                                int id = rnd.Next(10000000);
                                DropletUtillityFunctions.updateGroupNumber(container, droplet, id);
                                handeledDroplet.Add(droplet);

                                ArrayList connectedDroplets2 = new ArrayList();
                                DropletUtillityFunctions.findAllConnectedDroplets(container, droplet, connectedDroplets2);
                                foreach (Droplets d in connectedDroplets2)
                                {
                                    if (!handeledDroplet.Contains(d) && neighbouringDroplets.Contains(d))
                                    {
                                        handeledDroplet.Add(d);
                                    }
                                }


                            }
                        }

                        foreach (Droplets droplet in neighbouringDroplets)
                        {
                            DropletUtillityFunctions.updateGroupVolume(container, droplet.group, volume / neighbouringDroplets.Count);

                        }
                    }


                }
            }
            // part of the merge that allows large droplet to absorb small droplets
            if (caller.subscriptions.Count > 5)
            {
                //subscribers.Add(caller.ID);

                ArrayList onNeighbours = new ArrayList();
                foreach (int neighbour in dropletElectrode.neighbours)
                {
                    int electrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                    Electrode electrode = electrodeBoard[electrodeIndex];
                    if ((ElectrodeModels.electrodeHasDroplet(electrode, container) != null))
                    {
                        return subscribers;
                    }
                }
                //The large droplet can only absorb where it overlaps the electrode the subscribtions hold all electrodes it overlaps
                foreach (int sub in caller.subscriptions)
                {

                    Electrode tempElectrode = electrodeBoard[sub];
                    Droplets otherDroplet = ElectrodeModels.electrodeHasDroplet(tempElectrode, container);
                    if ((otherDroplet != null))
                    {

                        //Check to see if a droplet should be absorbed or stay on the edge due to pull of nearby electrodes
                        onNeighbours = new ArrayList();
                        foreach (int neighbour in tempElectrode.neighbours)
                        {
                            int electrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                            Electrode electrode = electrodeBoard[electrodeIndex];
                            if ((ElectrodeModels.electrodeHasDroplet(electrode, container) != null))
                            {
                                return subscribers;
                            }
                        }

                        if(onNeighbours.Count > 0)
                        {
                            subscribers.Add(callerID);
                            return subscribers;
                        }


                        //if the droplet is on an OFF electrode it only need to be partyally overlapped to be merged, if the electrod is on in must be ompletely overlapped
                        if (tempElectrode.status == 0 )
                        {
                            if (DropletUtillityFunctions.getGroupVolume(container, caller.group) > DropletUtillityFunctions.getGroupVolume(container, otherDroplet.group))
                            {
                                double x = Math.Pow(caller.positionX - otherDroplet.positionX, 2);
                                double y = Math.Pow(caller.positionY - otherDroplet.positionY, 2);

                                double dist = Math.Sqrt(x + y);

                                if (dist < (caller.sizeX/2 + otherDroplet.sizeX/2))
                                {
                                    absorbDropletAndUpdateVariables(container, caller, otherDroplet);

                                }
                                
                            }
                        }
                        else
                        {

                            double x = Math.Pow(caller.positionX - otherDroplet.positionX, 2);
                            double y = Math.Pow(caller.positionY - otherDroplet.positionY, 2);

                            double dist = Math.Sqrt(x + y);

                            if (dist + (otherDroplet.sizeX / 2)  < caller.sizeX/2)
                            {
                                if (DropletUtillityFunctions.getGroupVolume(container, caller.group) > DropletUtillityFunctions.getGroupVolume(container, otherDroplet.group))
                                {
                                    absorbDropletAndUpdateVariables(container, caller, otherDroplet);

                                }
                            }

                        }
                    }
                    
                }


            }
            else
            {
                //subscribers.Add(caller.ID);
            }
            subscribers.Add(callerID);
            return subscribers;
        }
        /// <summary>
        /// Function used to merge small droplets into big droplets
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <param name="otherDroplet"></param>
        private static void absorbDropletAndUpdateVariables(Container container, Droplets caller, Droplets otherDroplet)
        {

            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            DropletTemperatureModels.updateGroupTemperature(container, caller.group, otherDroplet);
            //Cleanup before removing droplet

            ArrayList dropletSubscritions = otherDroplet.subscriptions;
            foreach (int n in dropletSubscritions)
            {

                electrodeBoard[n].subscriptions.Remove(otherDroplet.ID);
            }
            container.subscribedDroplets.Remove(otherDroplet.ID);
            float volume = otherDroplet.volume;
            int groupId = otherDroplet.group;
            Color color = ColorTranslator.FromHtml(otherDroplet.color);
            droplets.Remove(otherDroplet);


            DropletUtillityFunctions.updateGroupColor(container, caller.group, color, volume);
            DropletUtillityFunctions.updateGroupVolume(container, caller.group, volume);
        }
    }
}

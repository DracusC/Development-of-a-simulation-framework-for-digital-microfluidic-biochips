using MicrofluidSimulator.SimulatorCode.DataTypes;
using MicrofluidSimulator.SimulatorCode.Models;
using System.Collections;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletMerge
    {
        public static ArrayList dropletMerge(Container container, Droplets caller)
        {
            ArrayList subscribers = new ArrayList();
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];

            if (dropletElectrode.status == 0)
            {

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
                if (onNeighbours.Count > 0)
                {
                    ArrayList dropletSubscritions = caller.subscriptions;
                    foreach (int n in dropletSubscritions)
                    {

                        electrodeBoard[n].subscriptions.Remove(caller.ID);
                    }
                    container.subscribedDroplets.Remove(caller.ID);
                    //caller.Subscriptions = new ArrayList();
                    float volume = caller.volume;
                    int groupId = caller.group;
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
            if (caller.subscriptions.Count > 5)
            {
                //subscribers.Add(caller.ID);

                ArrayList onNeighbours = new ArrayList();
                foreach (int neighbour in dropletElectrode.neighbours)
                {
                    int electrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                    Electrode electrode = electrodeBoard[electrodeIndex];
                    if (electrode.status != 0 || (ElectrodeModels.electrodeHasDroplet(electrode, container) != null))
                    {
                        return subscribers;
                    }
                }
                foreach (int sub in caller.subscriptions)
                {
                    Electrode tempElectrode = electrodeBoard[sub];
                    Droplets otherDroplet = ElectrodeModels.electrodeHasDroplet(tempElectrode, container);
                    if (tempElectrode.status == 0 && (otherDroplet != null))
                    {
                        if (DropletUtillityFunctions.getGroupVolume(container,caller.group) > DropletUtillityFunctions.getGroupVolume(container, otherDroplet.group))
                        {
                            ArrayList dropletSubscritions = otherDroplet.subscriptions;
                            foreach (int n in dropletSubscritions)
                            {

                                electrodeBoard[n].subscriptions.Remove(otherDroplet.ID);
                            }
                            container.subscribedDroplets.Remove(otherDroplet.ID);
                            //caller.Subscriptions = new ArrayList();
                            float volume = otherDroplet.volume;
                            int groupId = otherDroplet.group;

                            droplets.Remove(otherDroplet);
                            Console.WriteLine("removed a droplet");
                            DropletUtillityFunctions.updateGroupVolume(container, caller.group, volume);
                        }
                        

                    }
                }


            }
            else
            {
                //subscribers.Add(caller.ID);
            }
            subscribers.Add(caller.ID);
            return subscribers;
        }
    }
}

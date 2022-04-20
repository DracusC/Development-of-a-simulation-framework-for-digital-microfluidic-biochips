using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models

{
    public class ElectrodeModels
    {
        public static ArrayList electrodeOnOff(Container values, Electrode electrode, DataTypes.SimulatorAction action)
        {
            // electrode on/off
            electrode.status = action.actionChange;
            //if (action.ActionChange != 0)
            //{
                // give back the subscribers of the electrode
                return electrode.subscriptions;
            //}
            //ArrayList empty = new ArrayList();
            //return empty;
        }
        public static Droplets electrodeHasDroplet(Electrode Caller, Container container)
        {
            List<Droplets> droplets = container.droplets;
            foreach (Droplets droplet in droplets)
            {
                if (droplet.electrodeID == Caller.ID)
                {
                    return droplet;
                }
            }
            return null;
        }
        public static int[] getCenterOfElectrode(Electrode electrode)
        {
            int[] centre = new int[2];
            if (electrode.shape == 0)
            {
                centre[0] = electrode.positionX + (electrode.sizeX / 2);
                centre[1] = electrode.positionY + (electrode.sizeY / 2);
            }
            else
            {
                centre[0] = electrode.positionX + 5;
                centre[1] = electrode.positionY + 5;
            }

            return centre;

        }

        public static int getAreaOfElectrode(Electrode electrode)
        {
            if(electrode.shape == 0)
            {
                return electrode.sizeX * electrode.sizeY;
            }
            else
            {
                return (int) getAreaOfIregularShapedElectrode(electrode);
            }
        }

        private static float getAreaOfIregularShapedElectrode(Electrode electrode)
        {
            
            float area = 0;
            
            for(int i = 0; i < electrode.corners.Count-1; i++)
            {
                area += (electrode.corners[i][0] * electrode.corners[i+1][1] - electrode.corners[i+1][0] * electrode.corners[i][1]);
            }
            area += electrode.corners[0][0] * electrode.corners[electrode.corners.Count - 2][1] - electrode.corners[electrode.corners.Count - 2][0] * electrode.corners[0][1];
            return area/2;
        }

    }


    
}

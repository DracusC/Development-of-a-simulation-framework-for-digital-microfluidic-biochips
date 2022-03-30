using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models

{
    public class ElectrodeModels
    {
        public static ArrayList electrodeOnOff(Container values, Electrodes electrode, DataTypes.SimulatorAction action)
        {
            // electrode on/off
            electrode.Status = action.ActionChange;
            //if (action.ActionChange != 0)
            //{
                // give back the subscribers of the electrode
                return electrode.Subscriptions;
            //}
            //ArrayList empty = new ArrayList();
            //return empty;
        }
        public static Droplets electrodeHasDroplet(Electrodes Caller, Container container)
        {
            ArrayList droplets = container.Droplets;
            foreach (Droplets droplet in droplets)
            {
                if (droplet.ElectrodeID == Caller.ID1)
                {
                    return droplet;
                }
            }
            return null;
        }
        public static int[] getCenterOfElectrode(Electrodes electrode)
        {
            int[] centre = new int[2];
            if (electrode.Shape == 0)
            {
                centre[0] = electrode.PositionX + (electrode.SizeX / 2);
                centre[1] = electrode.PositionY + (electrode.SizeY / 2);
            }
            else
            {
                centre[0] = electrode.PositionX + 5;
                centre[1] = electrode.PositionY + 5;
            }

            return centre;

        }

        public static float getAreaOfElectrode(Electrodes electrode)
        {
            if(electrode.Shape == 0)
            {
                return electrode.SizeX * electrode.SizeY;
            }
            else
            {
                return 0;
            }
        }

        private static float getAreaOfIregularShapedElectrode(Electrodes electrode)
        {
            int[,] corners = electrode.Corners;
            float area = 0;
            for(int i = 0; i < corners.GetLength(0)-1; i++)
            {
                area = area + (corners[i,0]* corners[i+1,1] - corners[i+1,0]* corners[i,1]);
            }
            area = area + (corners[0, 0] * corners[corners.GetLength(0) - 2, 1] - corners[corners.GetLength(0) - 2, 0] * corners[0, 1]);
            return area/2;
        }

    }


    
}

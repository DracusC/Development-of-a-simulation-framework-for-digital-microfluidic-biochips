using System.Collections;
using MicrofluidSimulator.SimulatorCode.DataTypes;
namespace MicrofluidSimulator.SimulatorCode.Models

{
    /// <summary>
    /// Models and utlity functions for electrodes
    /// </summary>
    public class ElectrodeModels
    {
        /// <summary>
        /// Models that switrches a electrode on or off
        /// </summary>
        /// <param name="container"></param>
        /// <param name="electrode"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ArrayList electrodeOnOff(Container container, Electrode electrode, SimulatorAction action)
        {

            electrode.status = action.actionChange;

            return electrode.subscriptions;
        }
        /// <summary>
        /// Function to check if a electrode contains a droplet, retunrs the droplet or NULL if there is none
        /// </summary>
        /// <param name="Caller"></param>
        /// <param name="container"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Function to find the center of a electrode
        /// For non square electrodes takes the avaerage of all coordinates as center
        /// </summary>
        /// <param name="electrode"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Function that gives the area of an electrode
        /// </summary>
        /// <param name="electrode"></param>
        /// <returns></returns>
        public static int getAreaOfElectrode(Electrode electrode)
        {
            if(electrode.shape == 0)
            {
                return electrode.sizeX * electrode.sizeY;
            }
            else
            {
                return (int) getAreaOfIrregularShapedElectrode(electrode);
            }
        }
        /// <summary>
        /// Function used for getAreaOfElectrode if the droplet is not a rectangle
        /// </summary>
        /// <param name="electrode"></param>
        /// <returns></returns>
        private static float getAreaOfIrregularShapedElectrode(Electrode electrode)
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

using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
using System.Drawing;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletUtillityFunctions
    {

        /// <summary>
        /// get volume of all droplets in a group
        /// </summary>
        /// <param name="container"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static float getGroupVolume(Container container, int groupID)
        {
            List<Droplets> droplets = findGroupMembers(container, groupID);

            float volume = 0;
            foreach (Droplets droplet in droplets)
            {
                volume += droplet.volume;
            }
            return volume;
        }

        /// <summary>
        /// Get all droplets that are connected and set their group number to the desired value
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <param name="newGroupID"></param>
        public static void updateGroupNumber(Container container, Droplets caller, int newGroupID)
        {
            ArrayList connectedDroplets = new ArrayList();
            findAllConnectedDroplets(container, caller, connectedDroplets);
            foreach (Droplets droplet in connectedDroplets)
            {
                droplet.group = newGroupID;
            }
        }

        /// <summary>
        /// Recursivly find all connected electrodes that are ON, by running through the neigbours of electrodes
        /// And sum the area for return.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="electrode"></param>
        /// <param name="alreadyChecked"></param>
        /// <returns></returns>
        public static int findAreaOfAllConnectedElectrodes(Container container, Electrode electrode, ArrayList alreadyChecked)
        {
            int area = ElectrodeModels.getAreaOfElectrode(electrode);
            Electrode[] electrodeBoard = container.electrodes;
            alreadyChecked.Add(electrode.ID);

            foreach (int neigbour in electrode.neighbours)
            {
                int tempElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neigbour, container);
                Electrode tempElectrode = electrodeBoard[tempElectrodeIndex];

                if (tempElectrode.status > 0 && !alreadyChecked.Contains(tempElectrode.ID))
                {
                    area += findAreaOfAllConnectedElectrodes(container, tempElectrode, alreadyChecked);
                }
            }
            return area;
        }
        /// <summary>
        /// Recursicly find all connected droplets using the neigbours of electrodes
        /// </summary>
        /// <param name="container"></param>
        /// <param name="caller"></param>
        /// <param name="members"></param>
        public static void findAllConnectedDroplets(Container container, Droplets caller, ArrayList members)
        {
            
            Electrode[] electrodeBoard = container.electrodes;
            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);
            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            members.Add(caller);

            //go through the neighbours of the electrode the droplet is on
            //if the electrode contains a droplet and the droplet have not already been explored do recursion
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
        /// <summary>
        /// Run through all droplets and return those who have the desired groupID
        /// </summary>
        /// <param name="container"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public static List<Droplets> findGroupMembers(Container container, int groupID)
        {
            List<Droplets> groupMembers = new List<Droplets>();
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

        /// <summary>
        /// Given a group id all droplets in the group gets their respective share of the total volume, based on the size of the electrodes they are on
        /// </summary>
        /// <param name="container"></param>
        /// <param name="groupID"></param>
        /// <param name="extraVolume"></param>
        public static void updateGroupVolume(Container container, int groupID, float extraVolume)
        {
            
            List<Droplets> groupMembers = DropletUtillityFunctions.findGroupMembers(container, groupID);
            Electrode[] electrodeBoard = container.electrodes;
            int totalAreaOfElectrode = 0;

            //Find the total area of all the electrodes the droplets in the group are on
            foreach (Droplets droplet in groupMembers)
            {
                int tempElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(droplet.electrodeID, container);
                Electrode tempElectrode = electrodeBoard[tempElectrodeIndex];
                totalAreaOfElectrode += ElectrodeModels.getAreaOfElectrode(tempElectrode);

            }

            float volume = DropletUtillityFunctions.getGroupVolume(container, groupID) + extraVolume;

            //distribute the volume to the droplet and update their diameters
            foreach (Droplets droplet in groupMembers)
            {
                int tempElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(droplet.electrodeID, container);
                Electrode tempElectrode = electrodeBoard[tempElectrodeIndex];
                float newVolume = (volume * ElectrodeModels.getAreaOfElectrode(tempElectrode)) / totalAreaOfElectrode;
                droplet.volume = newVolume;
                int diam = DropletUtillityFunctions.getDiameterOfDroplet(newVolume);
                droplet.sizeX = diam;
                droplet.sizeY = diam;
                SubscriptionModels.dropletSubscriptions(container, droplet);

            }


        }

        /// <summary>
        /// Calculate distence from a droplet to an electrode
        /// </summary>
        /// <param name="electrodeCenterX"></param>
        /// <param name="electrodeCenterY"></param>
        /// <param name="dropletX"></param>
        /// <param name="dropletY"></param>
        /// <returns></returns>
        public static double electrodeDistance(int electrodeCenterX, int electrodeCenterY, int dropletX, int dropletY)
        {
            double x = Math.Pow(dropletX - electrodeCenterX, 2);
            double y = Math.Pow(dropletY - electrodeCenterY, 2);

            return Math.Sqrt(x + y);
        }
        /// <summary>
        /// Get volume of droplet
        /// </summary>
        /// <param name="diameter"></param>
        /// <returns></returns>
        public static float getVolumeOfDroplet(float diameter)
        {
            float pi = (float)Math.PI;
            return ((float)pi) * ((float)Math.Pow((diameter / 2), 2)) * GlobalVariables.HEIGHT;
        }
        /// <summary>
        /// Get the diameter of droplet
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static int getDiameterOfDroplet(float volume)
        {
            float pi = (float)Math.PI;
            return (int)(2 * (float)(Math.Sqrt(volume / (pi * GlobalVariables.HEIGHT))));
        }
        /// <summary>
        /// Get the area of a droplet
        /// </summary>
        /// <param name="droplet"></param>
        /// <returns></returns>
        public static float getAreaOfDroplet(Droplets droplet)
        {
            float pi = (float)Math.PI;
            int diam = getDiameterOfDroplet(droplet.volume);
            return (diam / 2) * (diam / 2) * pi;
        }
        /// <summary>
        /// Function that determines if a droplet overlaps and electrode fully or partially
        /// </summary>
        /// <param name="droplet"></param>
        /// <param name="electrode"></param>
        /// <returns></returns>
        public static bool dropletOverlapElectrode(Droplets droplet, Electrode electrode)
        {
            Point dropletCenter = new Point(droplet.positionX, droplet.positionY);
            double dropletRadius = (double)droplet.sizeX / 2;
            ArrayList points = new ArrayList();
            ArrayList disctances = new ArrayList();

            Point p1 = new Point(electrode.positionX, electrode.positionY);
            points.Add(p1);
            Point p2 = new Point(electrode.positionX + electrode.sizeX, electrode.positionY);
            points.Add(p2);
            Point p3 = new Point(electrode.positionX + electrode.sizeX, electrode.positionY + electrode.sizeY);
            points.Add(p3);
            Point p4 = new Point(electrode.positionX, electrode.positionY + electrode.sizeY);
            points.Add(p4);

            double distance1 = distanceBetweenPoints(dropletCenter, p1);
            if (distance1 < dropletRadius/2)
            {
                return true;
            }
            disctances.Add(distance1);
            double distance2 = distanceBetweenPoints(dropletCenter, p2);
            if (distance2 < dropletRadius/2)
            {
                return true;
            }
            disctances.Add(distance2);
            double distance3 = distanceBetweenPoints(dropletCenter, p3);
            if (distance3 < dropletRadius/2)
            {
                return true;
            }
            disctances.Add(distance3);
            double distance4 = distanceBetweenPoints(dropletCenter, p4);
            if (distance1 < dropletRadius/2)
            {
                return true;
            }
            disctances.Add(distance4);

            int minIndex = getIndexOfMin(disctances);
            Point min1 = (Point)points[minIndex];

            points.RemoveAt(minIndex);
            disctances.RemoveAt(minIndex);

            minIndex = getIndexOfMin(disctances);
            Point min2 = (Point)points[minIndex];

            double minimumDistance = minDistance(min1, min2, dropletCenter);

            if (minimumDistance < dropletRadius)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Get the distance between two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static double distanceBetweenPoints(Point a, Point b)
        {
            return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }
        /// <summary>
        /// Get the index of the minimum value in the arraylist
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static int getIndexOfMin(ArrayList list)
        {
            int minIndex = 0;
            int i = 0;
            double min = (double)list[0];
            foreach (double number in list)
            {
                if (min > (double)list[i])
                {
                    min = (double)list[i];
                    minIndex = i;
                }
                i++;
            }
            return minIndex;
        }

        /// <summary>
        /// Update color of all group members to the same color, an weighted average color of all members
        /// Ïdea for hex color average from https://stackoverflow.com/questions/3722307/is-there-an-easy-way-to-blend-two-system-drawing-color-values
        /// </summary>
        /// <param name="container"></param>
        /// <param name="group"></param>
        /// <param name="color"></param>
        /// <param name="volume"></param>
        public static void updateGroupColor(Container container, int group, Color color, float volume)
        {
            ArrayList groupColors = new ArrayList();
            float groupVolume = DropletUtillityFunctions.getGroupVolume(container, group);
            List<Droplets> groupMembers = DropletUtillityFunctions.findGroupMembers(container, group);

            foreach (Droplets dropletInGroup in groupMembers)
            {
                groupColors.Add(ColorTranslator.FromHtml(dropletInGroup.color));
            }

            int r = 0;
            int g = 0;
            int b = 0;
            foreach (Color c in groupColors)
            {
                r += c.R;
                g += c.G;
                b += c.B;
            }
            r /= groupColors.Count;
            g /= groupColors.Count;
            b /= groupColors.Count;
            
            float amount = volume / groupVolume;
 

            float red = (color.R * amount + r * (1 - amount));
            float green = (color.G * amount + g * (1 - amount));
            float blue = (color.B * amount + b * (1 - amount));


            foreach (Droplets droplet in groupMembers)
            {
                droplet.color = $"#{(int)red:X2}{(int)green:X2}{(int)blue:X2}";
            }
        }

        /// <summary>
        /// Minimum distance from point to line segement
        /// Implementation mostly from https://www.geeksforgeeks.org/minimum-distance-from-a-point-to-the-line-segment-using-vectors/
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="E"></param>
        /// <returns></returns>
        static double minDistance(Point A, Point B, Point E)
        {

            // vector AB
            Point AB = new Point
            {
                x = B.x - A.x,
                y = B.y - A.y
            };

            // vector BP
            Point BE = new Point
            {
                x = E.x - B.x,
                y = E.y - B.y
            };

            // vector AP
            Point AE = new Point
            {
                x = E.x - A.x,
                y = E.y - A.y
            };

            // Variables to store dot product
            double AB_BE, AB_AE;

            // Calculating the dot product
            AB_BE = (AB.x * BE.x + AB.y * BE.y);
            AB_AE = (AB.x * AE.x + AB.y * AE.y);

            // Minimum distance from
            // point E to the line segment
            double reqAns = 0;

            // Case 1
            if (AB_BE > 0)
            {

                // Finding the magnitude
                double y = E.y - B.y;
                double x = E.x - B.x;
                reqAns = Math.Sqrt(x * x + y * y);
            }

            // Case 2
            else if (AB_AE < 0)
            {
                double y = E.y - A.y;
                double x = E.x - A.x;
                reqAns = Math.Sqrt(x * x + y * y);
            }

            // Case 3
            else
            {

                // Finding the perpendicular distance
                double x1 = AB.x;
                double y1 = AB.y;
                double x2 = AE.x;
                double y2 = AE.y;
                double mod = Math.Sqrt(x1 * x1 + y1 * y1);
                reqAns = Math.Abs(x1 * y2 - y1 * x2) / mod;
            }
            return reqAns;
        }
    }

    class Point
    {
        public double x, y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Point()
        {
        }
    }
}

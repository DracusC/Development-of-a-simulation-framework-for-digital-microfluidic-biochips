using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class SubscriptionModels
    {
        public static void dropletSubscriptions(Container container, Droplets caller)
        {
            // Gets the needed data out of the container
            List<Droplets> droplets = container.droplets;
            Electrode[] electrodeBoard = container.electrodes;
            Actuators[] actuators = container.actuators;

            // Clear all previous subscriptions before adding new to avoid duplicates
            // and deprecated subscriptions
            ArrayList dropletSubscritions = caller.subscriptions;
            foreach (int n in dropletSubscritions)
            {
                electrodeBoard[n].subscriptions.Remove(caller.ID);
                //actuators[n].Subscriptions.Remove(caller.ID1); // Carl this is not correct
            }
            caller.subscriptions = new ArrayList();

            //Add the new subscriptions

            //find the index of the electrode we are on

            int dropletElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(caller.electrodeID, container);

            Electrode dropletElectrode = electrodeBoard[dropletElectrodeIndex];
            //dropletElectrode.subscriptions.Add(caller.ID);
            //caller.subscriptions.Add(dropletElectrodeIndex);
            ArrayList alreadyChecked = new ArrayList();
            //alreadyChecked.Add(caller.ID);
            makeNewSubscribtions(container, caller, dropletElectrode, alreadyChecked);

            // add the droplet to the sublist of the electrode it is on then run for all neighbours
            //dropletElectrode.subscriptions.Add(caller.ID);
            //caller.subscriptions.Add(dropletElectrodeIndex);

            //foreach (int neighbour in dropletElectrode.neighbours)
            //{
            //    int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
            //    electrodeBoard[index].subscriptions.Add(caller.ID);
            //    caller.subscriptions.Add(index);
            //}
        }

        private static void makeNewSubscribtions(Container container, Droplets droplet, Electrode electrode, ArrayList alreadyChecked)
        {
            alreadyChecked.Add(electrode.ID);

            if (dropletOverlapElectrode(container, droplet, electrode))
            {
                electrode.subscriptions.Add(droplet.ID);
                int index = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(electrode.ID, container);
                droplet.subscriptions.Add(index);
                foreach (int neighbour in electrode.neighbours)
                {
                    if (!alreadyChecked.Contains(neighbour))
                    {
                        Electrode[] electrodeBoard = container.electrodes;
                        int indexNeigbhour = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(neighbour, container);
                        Electrode electrodeNeigbour = electrodeBoard[indexNeigbhour];
                        makeNewSubscribtions(container, droplet, electrodeNeigbour, alreadyChecked);
                    }
                    
                }

            }
   

        }


        private static bool pointIsInsideDroplet(Container container, Droplets droplet, int x, int y)
        {
            if ((x - droplet.positionX) * (x - droplet.positionX) +
                    (y - droplet.positionY) * (y - droplet.positionY) <= droplet.sizeX * droplet.sizeX)
                return true;
            else
                return false;
        }

        private static bool dropletOverlapElectrode(Container container, Droplets droplet, Electrode electrode) 
        {
            Point dropletCenter = new Point(droplet.positionX, droplet.positionY);
            double dropletRadius = (double) droplet.sizeX / 2;
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
            if (distance1 < dropletRadius)
            {
                return true;
            }
            disctances.Add(distance1);
            double distance2 = distanceBetweenPoints(dropletCenter, p2);
            if (distance2 < dropletRadius)
            {
                return true;
            }
            disctances.Add(distance2);
            double distance3 = distanceBetweenPoints(dropletCenter, p3);
            if (distance3 < dropletRadius)
            {
                return true;
            }
            disctances.Add(distance3);
            double distance4 = distanceBetweenPoints(dropletCenter, p4);
            if (distance1 < dropletRadius)
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

            double minimumDistance = minDistance(min1,min2,dropletCenter);

            if (minimumDistance < dropletRadius)
            {
                return true;
            }
            return false;
            //int elecX = electrode.positionX;
            //int elecY = electrode.positionY;
            //int X = Math.Max(elecX + electrode.sizeX ,Math.Min(droplet.positionX, elecX));
            //int Y = Math.Max(elecY + electrode.sizeY, Math.Min(droplet.positionY, elecY));
            //return pointIsInsideDroplet(container, droplet, X, Y);
        }

        private static double distanceBetweenPoints(Point a, Point b)
        {
            return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }

        private static int getIndexOfMin(ArrayList list)
        {
            int minIndex = 0;
            int i = 0;
            double min = (double) list[0];
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


        static double minDistance(Point A, Point B, Point E)
        {

            // vector AB
            Point AB = new Point();
            AB.x = B.x - A.x;
            AB.y = B.y - A.y;

            // vector BP
            Point BE = new Point();
            BE.x = E.x - B.x;
            BE.y = E.y - B.y;

            // vector AP
            Point AE = new Point();
            AE.x = E.x - A.x;
            AE.y = E.y - A.y;

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

    // Function to return the minimum distance
    // between a line segment AB and a point E
    



}



// LOL
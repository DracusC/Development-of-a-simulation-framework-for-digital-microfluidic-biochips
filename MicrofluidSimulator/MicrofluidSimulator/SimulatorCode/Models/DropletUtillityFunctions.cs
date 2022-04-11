﻿using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.Collections;
namespace MicrofluidSimulator.SimulatorCode.Models
{
    public class DropletUtillityFunctions
    {
        public static float getGroupVolume(Container container, int groupID)
        {
            List<Droplets> droplets = findGroupMembers(container, groupID);

            float volume = 0;
            foreach (Droplets droplet in droplets)
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

        public static int findAreaAllConnectedElectrodes(Container container, Electrode electrode, ArrayList alreadyChecked)
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
                    area += findAreaAllConnectedElectrodes(container, tempElectrode, alreadyChecked);
                }
            }
            return area;
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

        public static void updateGroupVolume(Container container, int groupID, float extraVolume)
        {
            List<Droplets> droplets = container.droplets;
            List<Droplets> groupMembers = DropletUtillityFunctions.findGroupMembers(container, groupID);
            Electrode[] electrodeBoard = container.electrodes;
            int totalAreaOfElectrode = 0;
            foreach (Droplets droplet in groupMembers)
            {
                int tempElectrodeIndex = HelpfullRetreiveFunctions.getIndexOfElectrodeByID(droplet.electrodeID, container);
                Electrode tempElectrode = electrodeBoard[tempElectrodeIndex];
                totalAreaOfElectrode += ElectrodeModels.getAreaOfElectrode(tempElectrode);

            }

            float volume = DropletUtillityFunctions.getGroupVolume(container, groupID) + extraVolume;
            //float newVolume = volume / groupMembers.Count;
            //int diam = DropletUtillityFunctions.getDiameterOfDroplet(newVolume, 1);
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

        public static double electrodeDistance(int electrodeCenterX, int electrodeCenterY, int dropletX, int dropletY)
        {
            double x = Math.Pow(dropletX - electrodeCenterX, 2);
            double y = Math.Pow(dropletY - electrodeCenterY, 2);

            return Math.Sqrt(x + y);
        }

        public static float getVolumeOfDroplet(float diameter)
        {
            float pi = (float)Math.PI;
            return ((float)pi) * ((float)Math.Pow((diameter / 2), 2)) * GlobalVariables.HEIGHT;
        }

        public static int getDiameterOfDroplet(float volume)
        {
            float pi = (float)Math.PI;
            return (int)(2 * (float)(Math.Sqrt(volume / (pi * GlobalVariables.HEIGHT))));
        }
        public static float getAreaOfDroplet(Droplets droplet)
        {
            float pi = (float)Math.PI;
            int diam = getDiameterOfDroplet(droplet.volume);
            return (diam / 2) * (diam / 2) * pi;
        }

        public static bool dropletOverlapElectrode(Container container, Droplets droplet, Electrode electrode)
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

            double minimumDistance = minDistance(min1, min2, dropletCenter);

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
}

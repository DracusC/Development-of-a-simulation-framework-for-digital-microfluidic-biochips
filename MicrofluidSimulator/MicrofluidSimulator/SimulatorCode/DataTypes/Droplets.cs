﻿using System.Collections;
namespace MicrofluidSimulator.SimulatorCode
{
    public class Droplets
    {
        //string name, substance_name, color;
        //int ID, positionX, positionY, sizeX, sizeY;
        //float temperature;
        //float volume;
        //int group;
        //int electrodeID;

        //ArrayList subscriptions;
        //int nextModel;
        //String[] modelOrder;


        public Droplets(string name, int ID, string substance_name, int positionX, int positionY, int sizeX, int sizeY, string color, float temperature, float volume, int electrodeID, int group)
        {
            this.name = name;
            this.substance_name = substance_name;
            this.color = color;
            this.ID = ID;
            this.positionX = positionX;
            this.positionY = positionY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.temperature = temperature;
            this.volume = volume;
            this.group = group;
            this.electrodeID = electrodeID;
            this.subscriptions = new ArrayList();
            nextModel = 0;
            modelOrder = new string[] {"split", "merge", "temperature", "color"};
        }

        public Droplets()
        {
        }

        public string name { get; set; }
        public string substance_name { get; set; }
        public string color { get; set; }
        public int ID { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public float temperature { get; set; }
        public int electrodeID { get; set; }
        public ArrayList subscriptions { get; set; }
        public float volume { get; set; }
        public int group { get; set; }
        public int nextModel { get; set; }
        public string[] modelOrder { get; set; }

        public override string ToString()
        {
            String concat = "Name: " + name + "\nID: " + ID.ToString() + "\nPositionX: " + positionX.ToString() + "\nPositionY: "
                + positionY.ToString() + "\nSizeX: " + sizeX.ToString() + "\nSizeY: " + sizeY.ToString();
            return concat;
        }

    }
}

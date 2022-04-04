﻿using System.Collections;
namespace MicrofluidSimulator.SimulatorCode
{
    public class Bubbles
    {
        
        // bubbles not currently in use
        public Bubbles(string name, int ID, int positionX, int positionY, int sizeX, int sizeY, string color)
        {
            this.name = name;
            this.ID = ID;
            this.positionX = positionX;
            this.positionY = positionY;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.color = color;

            this.subscriptions = new ArrayList();
        }

        public Bubbles()
        {
        }
        public string color { get; set; }
        public string name { get; set; }
        public int ID { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public int sizeX { get; set; }
        public int sizeY { get; set; }

        public ArrayList subscriptions { get; set; }
    }
}

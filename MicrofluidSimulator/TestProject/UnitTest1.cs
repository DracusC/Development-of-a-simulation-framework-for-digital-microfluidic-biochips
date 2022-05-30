
using NUnit.Framework;
using MicrofluidSimulator.SimulatorCode;
using MicrofluidSimulator.SimulatorCode.Initialize;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.IO;
using System.Text.Json;
using MicrofluidSimulator.SimulatorCode.Simulator;

namespace TestProject
{
    public class Tests
    {

        //static Container container = new Container(null, null, null);
        [SetUp]
        public void Setup()
        {

        }


        [Test]
        public void TestNeighbouringRectangularElectrodes()
        {

            StreamReader r = new StreamReader("../../../../MicrofluidSimulator/wwwroot/sample-data/platform640v2.json");
            string jsonString = r.ReadToEnd();
            Container getContainer = JsonSerializer.Deserialize<Container>(jsonString);
            Initialize init = new Initialize();
            Container container = init.initialize(getContainer, null);
            int[] neighbours = container.electrodes[0].neighbours.ToArray(typeof(int)) as int[];
            Assert.That(neighbours, Has.Exactly(1).EqualTo(2));
            Assert.That(neighbours, Has.Exactly(1).EqualTo(33));
        }

        [Test]
        public void TestSubscriptionInitialization()
        {
            StreamReader r = new StreamReader("../../../../MicrofluidSimulator/wwwroot/sample-data/platform640v2.json");
            string jsonString = r.ReadToEnd();
            Container getContainer = JsonSerializer.Deserialize<Container>(jsonString);
            Initialize init = new Initialize();
            Container container = init.initialize(getContainer, null);
            int[] subscribers = container.electrodes[107].subscriptions.ToArray(typeof(int)) as int[];
            Assert.That(subscribers, Has.Exactly(1).EqualTo(1));
            
        }

        [Test]
        public void TestDropletGroupsInitialization()
        {
            StreamReader r = new StreamReader("../../../../MicrofluidSimulator/wwwroot/sample-data/platform640v2.json");
            string jsonString = r.ReadToEnd();
            Container getContainer = JsonSerializer.Deserialize<Container>(jsonString);
            Initialize init = new Initialize();
            Container container = init.initialize(getContainer, null);
            Assert.That(container.droplets[0].group == container.droplets[7].group);
        }

       [Test]
        public void TestDropletMovement()
        {
            
            
            StreamReader rContainer = new StreamReader("../../../../MicrofluidSimulator/wwwroot/sample-data/platform640vCenterPath.json");
            StreamReader rActions = new StreamReader("../../../../MicrofluidSimulator/wwwroot/sample-data/center_path.txt");
            string jsonStringContainer = rContainer.ReadToEnd();
            string jsonStringActions = rActions.ReadToEnd();
            Container getContainer = JsonSerializer.Deserialize<Container>(jsonStringContainer);
            

            Simulator simulator = new Simulator(null, getContainer, null, jsonStringActions);
            Assert.That(simulator.container.droplets[0].positionX == 200);
            Assert.That(simulator.container.droplets[0].positionY == 190);
            for(int i = 0; i < 21; i++)
            {
                simulator.simulatorStep(-1);
            }
            Assert.That(simulator.container.droplets[0].positionX == 400);
            Assert.That(simulator.container.droplets[0].positionY == 190);
            for (int i = 0; i < 181; i++)
            {
                simulator.simulatorStep(-1);
            }
            Assert.That(simulator.container.droplets[0].positionX == 360);
            Assert.That(simulator.container.droplets[0].positionY == 190);
        }

        [Test]
        public void TestSplitMerge()
        {


            StreamReader rContainer = new StreamReader("../../../../MicrofluidSimulator/wwwroot/sample-data/platform640vSplitMerge.json");
            StreamReader rActions = new StreamReader("../../../../MicrofluidSimulator/wwwroot/sample-data/split_merge_1.txt");
            string jsonStringContainer = rContainer.ReadToEnd();
            string jsonStringActions = rActions.ReadToEnd();
            Container getContainer = JsonSerializer.Deserialize<Container>(jsonStringContainer);


            Simulator simulator = new Simulator(null, getContainer, null, jsonStringActions);
            Assert.That(simulator.container.droplets.Count == 2);
            
            for (int i = 0; i < 20; i++)
            {
                simulator.simulatorStep(-1);
            }
            Assert.That(simulator.container.droplets.Count == 1);
            for (int i = 0; i < 2; i++)
            {
                simulator.simulatorStep(-1);
            }
            Assert.That(simulator.container.droplets.Count == 5);
            for (int i = 0; i < 12; i++)
            {
                simulator.simulatorStep(-1);
            }
            Assert.That(simulator.container.droplets.Count == 2);
        }
    }
}
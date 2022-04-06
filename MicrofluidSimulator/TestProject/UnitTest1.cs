
using NUnit.Framework;
using MicrofluidSimulator.SimulatorCode;
using MicrofluidSimulator.SimulatorCode.Initialize;
using MicrofluidSimulator.SimulatorCode.DataTypes;
using System.IO;
using System.Text.Json;
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

            StreamReader r = new StreamReader("C:/Users/carlj/OneDrive/Uni/6. semester/Bachelor/Development-of-a-simulation-framework-for-digital-microfluidic-biochips/MicrofluidSimulator/MicrofluidSimulator/wwwroot/sample-data/platform640v2.json");
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
            StreamReader r = new StreamReader("C:/Users/carlj/OneDrive/Uni/6. semester/Bachelor/Development-of-a-simulation-framework-for-digital-microfluidic-biochips/MicrofluidSimulator/MicrofluidSimulator/wwwroot/sample-data/platform640v1.json");
            string jsonString = r.ReadToEnd();
            Container getContainer = JsonSerializer.Deserialize<Container>(jsonString);
            Initialize init = new Initialize();
            Container container = init.initialize(getContainer, null);
            int[] subscribers = container.electrodes[1].subscriptions.ToArray(typeof(int)) as int[];
            Assert.That(subscribers, Has.Exactly(1).EqualTo(0));
            Assert.That(subscribers, Has.Exactly(1).EqualTo(1));
            Assert.That(subscribers, Has.Exactly(1).EqualTo(2));
        }

        [Test]
        public void TestDropletGroupsInitialization()
        {
            StreamReader r = new StreamReader("C:/Users/carlj/OneDrive/Uni/6. semester/Bachelor/Development-of-a-simulation-framework-for-digital-microfluidic-biochips/MicrofluidSimulator/MicrofluidSimulator/wwwroot/sample-data/platform640v2.json");
            string jsonString = r.ReadToEnd();
            Container getContainer = JsonSerializer.Deserialize<Container>(jsonString);
            Initialize init = new Initialize();
            Container container = init.initialize(getContainer, null);
            Assert.That(container.droplets[0].group == container.droplets[7].group);
        }
    }
}
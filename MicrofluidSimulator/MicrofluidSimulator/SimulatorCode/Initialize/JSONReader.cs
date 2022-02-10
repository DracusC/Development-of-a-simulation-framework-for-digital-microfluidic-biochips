namespace MicrofluidSimulator.SimulatorCode.Initialize
{
    using Newtonsoft.Json;
    public class JSONReader
    {
        // attempt of creating json reader
        string path, json;

        public JSONReader(string path)
        {
            this.path = path;
        }

        

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader(path))
            {
                json = r.ReadToEnd();

            }
            
        }

        public string Json { get => json; set => json = value; }
        //dynamic information = JObject.Parse("{ 'Name': 'Jon Smith', 'Address': { 'City': 'New York', 'State': 'NY' }, 'Age': 42 }");


    } 
}

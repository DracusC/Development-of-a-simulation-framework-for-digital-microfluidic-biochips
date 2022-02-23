/*
 * Written by Joel A. V. Madsen
 */
using Microsoft.JSInterop;
using System.Text.Json;

//inject IJSRuntime JsRuntime;

namespace MicrofluidSimulator.SimulatorCode.View
{
    public class GUIInfo {
        public bool gui_status { get; set; }
        public Droplets[] droplets { get; set; }
        public Electrodes[] electrodes { get; set;}
    }

    public class GUIBroker {


        /*
         * JSRuntime is used to run javascript code within c#
         */
        private IJSRuntime _jSRuntime;

        public GUIBroker(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public GUIBroker() { }

        public void set_jsruntime(IJSRuntime jSRuntime) {
            _jSRuntime = jSRuntime;
        }
        

        //[Inject]
        //public IJSRuntime JSRuntime { get; set; }
        //private IJSObjectReference _jsModule;

        /*public async void get_gui_broker(bool sim_data, Droplets[] droplets) {
            var data = await _jSRuntime.InvokeAsync<object>("get_gui_broker", sim_data, droplets);
            Console.WriteLine(data);
            var string_data = data.ToString();
            Console.WriteLine(string_data);
        }*/

        public async void update_droplets(Droplets[] droplets) {
            var json_droplets = Newtonsoft.Json.JsonConvert.SerializeObject(droplets);
            var data = await _jSRuntime.InvokeAsync<object>("update_droplets", json_droplets);
        }

        public async Task<bool> get_gui_status() {
            var result = await _jSRuntime.InvokeAsync<bool>("get_gui_status");
            return result;
        }
    }
}

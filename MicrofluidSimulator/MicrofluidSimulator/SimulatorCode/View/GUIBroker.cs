/*
 * Written by Joel A. V. Madsen
 */
using Microsoft.JSInterop;
using System.Text.Json;
using MicrofluidSimulator.SimulatorCode.DataTypes;

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

        public GUIBroker(IJSRuntime jSRuntime) {
            _jSRuntime = jSRuntime;
        }

        public GUIBroker() { }

        public void set_jsruntime(IJSRuntime jSRuntime) {
            _jSRuntime = jSRuntime;
        }
        
        public async void initialize_board(MicrofluidSimulator.SimulatorCode.DataTypes.Information container) {
            var json_string = Newtonsoft.Json.JsonConvert.SerializeObject(container);
            var data = await _jSRuntime.InvokeAsync<object>("initialize_board", json_string);
        }

        public async void update_board(Container container) {
            var json_container = Newtonsoft.Json.JsonConvert.SerializeObject(container);
            var data = await _jSRuntime.InvokeAsync<object>("update_board", json_container);
        }

        public async Task<bool> get_gui_status() {
            var result = await _jSRuntime.InvokeAsync<bool>("get_gui_status");
            return result;
        }

        public async void change_play_status() {
            var data = await _jSRuntime.InvokeAsync<object>("change_play_status");
        }
    }
}

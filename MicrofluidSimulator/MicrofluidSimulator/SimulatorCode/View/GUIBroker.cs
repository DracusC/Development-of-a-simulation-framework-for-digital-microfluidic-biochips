/*
 * Written by Joel A. V. Madsen
 */
using Microsoft.JSInterop;
using System.Text.Json;
using System.Diagnostics;
using MicrofluidSimulator.SimulatorCode.DataTypes;

namespace MicrofluidSimulator.SimulatorCode.View
{

    // TODO: Delete GUIInfo class?
    public class GUIInfo {
        public bool gui_status { get; set; }
        public Droplets[] droplets { get; set; }
        public Electrode[] electrodes { get; set;}
    }

    /// <summary>
    /// The GUIBroker class is used to send and receive data to and from the GUI.
    /// </summary>
    public class GUIBroker {


        /*
         * JSRuntime is used to call JavaScript code from c#
         */
        private IJSInProcessRuntime _JSInProcessRuntime;
        private IJSUnmarshalledRuntime _JSUnmarshalledRuntime;

        public GUIBroker() { }

        public void set_jsprocess(IJSInProcessRuntime JSInProcessRuntime)
        {
            _JSInProcessRuntime = JSInProcessRuntime;
        }

        public void set_unmarshall(IJSUnmarshalledRuntime JSUnmarshalledRuntime)
        {
            _JSUnmarshalledRuntime = JSUnmarshalledRuntime;
        }


        public void setP5()
        {
            _JSInProcessRuntime.Invoke<object>("setp5"); // ADD TO GUI BROKER CS
        }

        /// <summary>
        /// The function initialize_board is used to initialize the GUI with
        /// initial board information.
        /// </summary>
        /// <param name="container"></param>
        public void initialize_board(Information container) {
            /*var stopwatch = new Stopwatch();
            stopwatch.Start();
            string jsonuft = Utf8Json.JsonSerializer.ToJsonString(container);
            stopwatch.Stop();

            Console.WriteLine("Init Utf8Json serialize time: " + stopwatch.ElapsedMilliseconds + " ms");

            stopwatch.Reset();
            stopwatch.Start();
            string json = JsonSerializer.Serialize(container);
            stopwatch.Stop();

            Console.WriteLine("Init System.Text.Json serialize time: " + stopwatch.ElapsedMilliseconds + " ms");*/

            var json_string = Utf8Json.JsonSerializer.ToJsonString(container);
            _JSInProcessRuntime.InvokeVoid("initialize_board", json_string);
        }

        /// <summary>
        /// The function update_board is used to update the GUI board, with
        /// information from the simulator.
        /// </summary>
        /// <param name="container"></param>
        public void update_board(Container container) {
            /*var stopwatch = new Stopwatch();
            stopwatch.Start();
            string jsonuft = Utf8Json.JsonSerializer.ToJsonString(container);
            stopwatch.Stop();
            
            Console.WriteLine("Utf8Json serialize time: " + stopwatch.ElapsedMilliseconds + " ms");*/
            /*
            stopwatch.Reset();
            stopwatch.Start();
            string json = JsonSerializer.Serialize(container);
            stopwatch.Stop();

            Console.WriteLine("System.Text.Json serialize time: " + stopwatch.ElapsedMilliseconds + " ms");*/

            // Start execution time.
            start_simulator_time();
            _JSUnmarshalledRuntime.InvokeUnmarshalled<string, string>("update_board", Utf8Json.JsonSerializer.ToJsonString(container));
            //_JSInProcessRuntime.InvokeVoid("update_board", container);
        }

        /// <summary>
        /// The function animate_once is used to trigger one GUI animation cycle.
        /// </summary>
        public void animate_once() {
            _JSInProcessRuntime.InvokeVoid("animate_once");
        }

        /// <summary>
        /// The function send_download_data is used to send simulator data,
        /// that is then used for download.
        /// </summary>
        /// <param name="jsonData"></param>
        public void send_download_data(string jsonData)
        {
            _JSInProcessRuntime.InvokeVoid("send_download_data", jsonData);
        }

        public void download_data()
        {
            _JSInProcessRuntime.InvokeVoid("download_data");
        }

        public bool get_gui_status() {
            var result = _JSInProcessRuntime.Invoke<bool>("get_gui_status");
            return result;
        }

        /// <summary>
        /// The function change_play_status is used to change the play status
        /// of the GUI.
        /// </summary>
        public void change_play_status() {
            _JSInProcessRuntime.InvokeVoid("change_play_status");
        }

        /// <summary>
        /// The function restart_board is used to flag a restart of the simulation.
        /// </summary>
        public void restart_board()
        {
            _JSInProcessRuntime.InvokeVoid("restart_board");
        }

        public string get_selected_element()
        {
            var result = _JSInProcessRuntime.Invoke<string>("get_selected_element");
            return result;
        }

        /// <summary>
        /// The function start_simulator_time flags the GUI to start
        /// the simulator timer, used for realtime execution.
        /// </summary>
        public void start_simulator_time()
        {
            _JSInProcessRuntime.InvokeVoid("start_simulator_time");
        }

        /// <summary>
        /// The function is used to set the simulator time in the GUI to a specific time.
        /// </summary>
        /// <param name="time"></param>
        public void set_simulator_time(float time)
        {
            _JSInProcessRuntime.InvokeVoid("start_simulator_time", time);
        }

        // Used for debugging
        public void start_update_timer()
        {
            _JSInProcessRuntime.InvokeVoid("start_update_timer");
        }
        public void end_update_timer()
        {
            _JSInProcessRuntime.InvokeVoid("end_update_timer");
        }
    }
}

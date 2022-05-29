/* BEGIN GUIController.js */

/*
 * The gui_controller object is used to store information about the gui,
 * which are not a part of the actual sketch.
 */
let gui_controller = {
    //simulatorGUI: document.querySelector("#simulatorGUI"),
    //getLayerPanel: () => { return this.simulatorGUI.querySelector("#selectionPanel") },
    //getInputNodes: () => { return this.simulatorGUI.querySelector("#selectionPanel").getElementsByTagName('INPUT'); },
    //getInformaitonPanel: () => { return this.simulatorGUI.querySelector("#information"); },
    changeBoardName: (name) => { this.simulatorGUI.querySelector("#simulatorView span").innerHTML = name; },
    showGUI: () => { simulatorGUI.style.visibility = "visible"; },

    /**
     * Play status of changed by the play button
     */
    play_status: false,

    /**
     * Stores the data that is to be downloaded.
     */
    data_for_download: [],

    /**
     * The function download_data is used to download simulator data,
     * into JSON format.
     */
    download_data: function() {
        let jsonData = JSON.stringify(this.data_for_download);

        /* Download the data by forcing a click on an anchor element. */
        var dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(jsonData);
        var dlAnchorElem = document.getElementById('downloadAnchorElem');
        dlAnchorElem.setAttribute("href", dataStr);
        dlAnchorElem.setAttribute("download", "data.json");
        dlAnchorElem.click();

        this.data_for_download = [];
    },


    /**
     * The function send_download_data is used by the simulator to send the data
     * to the GUI, which will then be formatted, so that it's ready for download.
     * @param {any} jsonData
     */
    store_download_data: function (jsonData) {
        let obj = JSON.parse(jsonData);
        obj.droplet_groups = gui_broker.get_droplet_groups();
        this.data_for_download.push(obj);
    }
}







/* END GUIController.js */
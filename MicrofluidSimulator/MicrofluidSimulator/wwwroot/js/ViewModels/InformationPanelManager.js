let information_panel_manager = {
    saveclose_button_div: null,
    edit_button: null,
    save_button: null,
    close_button: null,
    selected_element: null,
    selected_element_type: null,
    information_element: null,
    editing: false,
    before_edit_values: {},
    display_info: {
        Droplet: {
            ID1: "",
            Substance_name: "",
            Color: "",
            SizeX: 0,
            PositionX: 0,
            PositionY: 0,
            Temperature: 0,
            Volume: 0,
            Group: 0
        },
        Droplet_editable: ["Volume", "Temperature", "Color"],
        Electrode: {
            Name: "",
            ID1: "",
            Status: 0,
            PositionX: 0,
            PositionY: 0,
            Subscriptions: []
        },
        Electrode_editable: ["Status"],
        Group: {
            Group_ID: 0,
            Substance_name: "",
            Color: "",
            Temperature: 0,
            Volume: 0,
            Droplets: []
        },
        Group_editable: ["Volume", "Temperature", "Color"]
    },
    information_filter: function (type) {
        let returnVal;
        this.selected_element_type = type;
        switch (type) {
            case ("Droplet"):
                let droplet = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = droplet[key];
                }

                returnVal.Type = type;
                return returnVal;

                break;
            case ("Electrode"):
                let electrode = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = electrode[key];
                }

                returnVal.Type = type;
                return returnVal;

                break;
            case ("Group"):

                let id = arguments[1];
                let group = arguments[2];

                returnVal = { ...this.display_info[type] };
                returnVal.Droplets = [];
                returnVal.Group_ID = id;

                returnVal.Substance_name = group[0].Substance_name;
                returnVal.Color = group[0].Color;

                for (let i = 0; i < group.length; i++) {
                    let droplet = group[i];
                    returnVal.Droplets.push(droplet.ID1);
                    for (let key in droplet) {
                        if (key == "Volume") { returnVal.Volume += droplet[key] }
                        if (key == "Temperature") { returnVal.Temperature += droplet[key] }
                    }
                }

                returnVal.Temperature = returnVal.Temperature / group.length;

                returnVal.Type = type;
                return returnVal;
                break;
        }
    },
    draw_information: function (element) {

        this.onCancel();

        let informationPanel = gui_controller.getInformaitonPanel();
        let informationView = informationPanel.querySelector("#informationElements");
        informationView.innerHTML = "";

        let div = document.createElement("div");

        for (let key in element) {
            let innerDiv = document.createElement("div");
            let innerInput = document.createElement("input");

            innerDiv.innerHTML = key + ": ";
            innerDiv.classList.add("information_item");
            innerInput.value = element[key];
            innerInput.readOnly = true;
            innerInput.classList.add("input_readonly");

            innerDiv.append(innerInput);
            div.append(innerDiv);
        }

        informationView.append(div);
    },
    onEdit: function () {

        this.before_edit_values = {};

        this.edit_button.style.visibility = "hidden";
        this.saveclose_button_div.style.visibility = "visible";
        this.editing = true;

        let element = this.information_element;

        let type = element.Type;
        let editable_values = this.display_info[type + "_editable"];

        let divs = document.querySelector("#informationElements").getElementsByClassName("information_item");
        let inputs = document.querySelector("#informationElements").getElementsByTagName("input");

        var div_arr = Array.prototype.slice.call(divs);
        var input_arr = Array.prototype.slice.call(inputs);

        for (let i in input_arr) {
            let attribute = div_arr[i].textContent.slice(0, div_arr[i].textContent.length - 2);
            let input = input_arr[i];

            if (editable_values.includes(attribute)) {
                this.before_edit_values[attribute] = input.value;
                input.readOnly = false;
                input.classList.remove("input_readonly");
                // TODO: Add a write class
            }
        }
    },
    onCancel: function () {
        this.edit_button.style.visibility = "visible";
        this.saveclose_button_div.style.visibility = "hidden";

        this.editing = false;

        let element = this.information_element;

        let type = element.Type;
        let editable_values = this.display_info[type + "_editable"];

        let divs = document.querySelector("#informationElements").getElementsByClassName("information_item");
        let inputs = document.querySelector("#informationElements").getElementsByTagName("input");

        var div_arr = Array.prototype.slice.call(divs);
        var input_arr = Array.prototype.slice.call(inputs);

        for (let i in input_arr) {
            let attribute = div_arr[i].textContent.slice(0, div_arr[i].textContent.length - 2);
            let input = input_arr[i];

            if (editable_values.includes(attribute)) {
                input.value = this.before_edit_values[attribute];
                input.readOnly = true;
                input.classList.add("input_readonly");
            }
        }
    },
    onSave: function () {
        console.log("Save");
        this.edit_button.style.visibility = "visible";
        this.saveclose_button_div.style.visibility = "hidden";

        this.editing = false;

        let element = this.information_element;

        let type = element.Type;
        let editable_values = this.display_info[type + "_editable"];

        let divs = document.querySelector("#informationElements").getElementsByClassName("information_item");
        let inputs = document.querySelector("#informationElements").getElementsByTagName("input");

        let div_arr = Array.prototype.slice.call(divs);
        let input_arr = Array.prototype.slice.call(inputs);

        let values_to_send = {};

        for (let i in input_arr) {
            let attribute = div_arr[i].textContent.slice(0, div_arr[i].textContent.length - 2);
            let input = input_arr[i];

            if (editable_values.includes(attribute)) {
                input.readOnly = true;
                input.classList.add("input_readonly");

                // TODO: Check if the input value is ok!
                this.selected_element[attribute] = input.value;
                values_to_send[attribute] = input.value;
            }
        }

        console.log(this.selected_element);
        if (this.selected_element_type == "Group") {
            console.log("GROUP SELECT");

            let array_of_droplets = [];
            console.log(this.selected_element.Droplets);




            //console.log(gui_broker.board[this.selected_element_type + "s"].find(o => o.ID1 === this.selected_element.ID1));
        } else {
            values_to_send.ID1 = this.selected_element.ID1;
        }

        console.log(values_to_send);

        gui_broker.update_simulator_container(this.selected_element_type, JSON.stringify(values_to_send));
    }

}
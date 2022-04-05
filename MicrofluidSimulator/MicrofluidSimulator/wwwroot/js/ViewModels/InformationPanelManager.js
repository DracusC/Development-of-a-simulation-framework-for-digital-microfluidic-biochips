let information_panel_manager = {
    saveclose_button_div: null,
    edit_button: null,
    save_button: null,
    close_button: null,
    multiple_selection: null,
    double_clicked: false,
    selected_element: null,
    selected_element_type: null,
    information_element: null,
    editing: false,
    before_edit_values: {},
    display_info: {
        Droplet: {
            ID: "",
            substance_name: "",
            color: "",
            sizeX: 0,
            positionX: 0,
            positionY: 0,
            temperature: 0,
            volume: 0,
            group: 0
        },
        Droplet_editable: ["volume", "temperature", "color"],
        Electrode: {
            name: "",
            ID: "",
            status: 0,
            positionX: 0,
            positionY: 0,
            subscriptions: []
        },
        Electrode_editable: ["status"],
        Group: {
            groupID: 0,
            substance_name: "",
            color: "",
            temperature: 0,
            volume: 0,
            droplets: []
        },
        Group_editable: ["volume", "temperature", "color"],
        Actuator: {
            name: "",
            actuatorID: 0,
            type: "",
            valueActualTemperature: 0,
            valueDesiredTemperature: 0,
            valuePowerStatus: 0
        },
        Actuator_editable: [],
        Sensor: {
            name: "",
            sensorID: 0,
            type: "",
            valueRed: 0,
            valueBlue: 0,
            valueGreen: 0,
            valueTemperature: 0
        },
        Sensor_editable: []
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

                let id = arguments[2];
                let group = arguments[1];

                returnVal = { ...this.display_info[type] };
                returnVal.droplets = [];
                returnVal.groupID = id;

                returnVal.substance_name = group[0].substance_name;
                returnVal.color = group[0].color;

                for (let i = 0; i < group.length; i++) {
                    let droplet = group[i];
                    returnVal.droplets.push(droplet.ID);
                    for (let key in droplet) {
                        if (key == "volume") { returnVal.volume += droplet[key] }
                        if (key == "temperature") { returnVal.temperature += droplet[key] }
                    }
                }

                returnVal.temperature = returnVal.temperature / group.length;

                returnVal.Type = type;
                return returnVal;
                break;

            case ("Actuator"):
                let actuator = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = actuator[key];
                }

                returnVal.Type = type;
                return returnVal;

                break;

            case ("Sensor"):
                let sensor = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = sensor[key];
                }

                returnVal.Type = type;
                return returnVal;

                break;
        }
    },
    draw_multiple_selection: function (p) {

        let s_list = [];
        for (let i = 0; i < Object.keys(information_panel_manager.multiple_selection).length; i++) {
            s_list.push((i + 1) + " " + Object.keys(information_panel_manager.multiple_selection)[i]);
        }

        p.textSize(14);
        p.fill("#1b6ec2");
        let max_width = Math.max.apply(Math, s_list.map(function (o) { return p.textWidth(o); }))
        let max_height = p.textAscent(s_list[0]) * s_list.length + 10 + 5 * s_list.length;

        p.stroke("#000000");
        p.rect(p.mouseX, p.mouseY, max_width + 20, max_height, 5);

        p.noStroke();
        p.fill("#ffffff");
        for (let i in s_list) {
            p.text(s_list[i], p.mouseX + 10, p.mouseY + (p.textAscent(s_list[i]) * (parseInt(i) + 1)) + 5 * (parseInt(i) + 1));
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


        if (this.selected_element == null) { this.onCancel(); return; }

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

        if (this.selected_element_type == "Group") {
            values_to_send.droplets = this.selected_element.droplets;
            values_to_send.groupID = this.selected_element.groupID;
            //console.log(gui_broker.board[this.selected_element_type + "s"].find(o => o.ID1 === this.selected_element.ID1));
        } else {
            values_to_send.ID = this.selected_element.ID;
        }

        
        //values_to_send = this.selected_element;

        gui_broker.update_simulator_container(this.selected_element_type, JSON.stringify(values_to_send));
    },
    clear: function () {
        this.edit_button.style.visibility = "visible";
        this.saveclose_button_div.style.visibility = "hidden";

        let div = document.querySelector("#informationElements");
        div.innerHTML = "";
    }

}
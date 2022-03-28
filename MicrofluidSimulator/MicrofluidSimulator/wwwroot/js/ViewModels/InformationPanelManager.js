let information_panel_manager = {
    edit_btn: document.querySelector("#edit_button"),
    selected_element: null,
    information_element: null,
    editing: false,
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
    draw_information: (element) => {
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
    onSelect: function () {

    },
    onEdit: function () {

        this.edit_btn.style.visibility = "hidden";
        this.editing = true;

        let element = this.information_element;
        console.log(element);

        let type = element.Type;
        let editable_values = this.display_info[type + "_editable"];
        console.log(editable_values);

        let divs = document.querySelector("#informationElements").getElementsByClassName("information_item");
        let inputs = document.querySelector("#informationElements").getElementsByTagName("input");
        console.log(inputs);

        var div_arr = Array.prototype.slice.call(divs);
        var input_arr = Array.prototype.slice.call(inputs);
        console.log(div_arr);

        for (let i in input_arr) {
            let attribute = div_arr[i].textContent.slice(0, div_arr[i].textContent.length - 2);
            let input = input_arr[i];

            console.log(input_arr[i].value, div_arr[i].textContent.slice(0, div_arr[i].textContent.length - 2));
            if (editable_values.includes(attribute)) {
                input.readOnly = false;
                input.classList.remove("input_readonly");
                // TODO: Add a write class
            }
        }


    }

}
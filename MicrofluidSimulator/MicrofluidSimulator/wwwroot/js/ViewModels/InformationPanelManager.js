let information_panel_manager = {

    display_info: {
        droplet: {
            ID1: "",
            Substance_name: "",
            Color: "",
            SizeX: 0,
            Temperature: 0,
            Volume: 0,
            Group: 0
        },
        electrode: {
            Name: "",
            ID1: "",
            Status: 0,
            PositionX: 0,
            PositionY: 0,
            Subscriptions: []
        },
        group: {
            Group_ID: 0,
            Substance_name: "",
            Color: "",
            Temperature: 0,
            Volume: 0,
            Droplets: []
        }
    },

    information_filter: function (type) {
        let returnVal;
        switch (type) {
            case ("droplet"):
                let droplet = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = droplet[key];
                }

                return returnVal;

                break;
            case ("electrode"):
                let electrode = arguments[1];

                returnVal = this.display_info[type];

                for (let key in returnVal) {
                    returnVal[key] = electrode[key];
                }

                return returnVal;

                break;
            case ("group"):

                let id = arguments[1];
                let group = arguments[2];

                returnVal = { ...this.display_info[type] };
                returnVal.Droplets = [];
                returnVal.Group_ID = id;

                for (let i = 0; i < group.length; i++) {
                    let droplet = group[i];
                    returnVal.Droplets.push(droplet.ID1);
                    for (let key in droplet) {
                        if (key == "Volume") { returnVal.Volume += droplet[key] }
                        if (key == "Temperature") { returnVal.Temperature += droplet[key] }
                    }
                }

                returnVal.Temperature = returnVal.Temperature / group.length;

                return returnVal;
                break;
        }
    },


    selected_element: null,
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
            innerInput.readOnly = false;
            innerInput.classList.add("input_readonly");

            innerDiv.append(innerInput);
            div.append(innerDiv);
        }

        informationView.append(div);
    }
    
}
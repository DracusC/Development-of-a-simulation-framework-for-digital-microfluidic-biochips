let information_panel_manager = {
    //information_panel: gui_controller.getInformaitonPanel(),
    selected_element: null,
    draw_information: (element) => {
        gui_controller.getInformaitonPanel().innerHTML = "";

        let div = document.createElement("div");

        for (let key in element) {
            let innerDiv = document.createElement("div");
            let innerInput = document.createElement("input");

            innerDiv.innerHTML = key + ": ";
            innerDiv.classList.add("information_item");
            innerInput.value = element[key];
            innerInput.readOnly = false;
            innerInput.classList.add("input_readonly");

            //innerInput.innerHTML = element[key];
            innerDiv.append(innerInput);
            div.append(innerDiv);
        }


        //div.innerHTML = JSON.stringify(element);
        //console.log(JSON.stringify(element));
        gui_controller.getInformaitonPanel().append(div);
    }
}
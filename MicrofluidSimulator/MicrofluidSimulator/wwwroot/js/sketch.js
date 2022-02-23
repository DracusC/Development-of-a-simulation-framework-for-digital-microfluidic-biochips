/* 
 * Written by Joel A. V. Madsen
 */


// Setup the p5js instance
window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

// Global methods called by c#
window.get_gui_status = () => {
    return gui_broker.gui_status;
};
window.update_droplets = (droplet_arr) => {
    gui_broker.gui_status = false;

    var droplets = JSON.parse(droplet_arr);
    console.log(droplets);
    gui_broker.droplets = droplets;


    droplet_info.old = droplet_info.new;
    droplet_info.new = droplets;

    return gui_broker;
}
window.update_electrodes = (electrodes) => {
    gui_broker.electrodes = electrodes;
    return gui_broker;
}
window.update_board = (container_string) => {
    var board = JSON.parse(container_string);
    gui_broker.droplets = board.droplets;
    gui_broker.electrodes = board.electrodes;
}


// Declare global variables
let arr = [];
let d_info = [];
let t = "";

// Store simulator board info
let simulator_droplets = [];
let simulator_electrodes = [];


// gui_broker acts as a connection between the simulator and gui
let gui_broker = {
    gui_status: true, // Ready for a new update (true/false)
    droplets: [],
    electrodes: []
};
let droplet_info = {
    old: [],
    new: []
};



let simulator_data = {
    changet: (nt) => {
        t = nt;
    }
};

window.simulator_data = simulator_data;
window.gui_broker = gui_broker;
 
let sketch = function (p) {
    

    p.setup = function(){ 
        canvas = p.createCanvas(401, 401);
        console.log(gui_broker);
        arr = [];
        //Testing
        t = "BEFORE";
    }

    let step = 0.02;
    let amount = 0;
    var count = 0

    p.draw = function(){
        p.background(240);
        amount += step;

        // Draw grid
        for (let i = 0; i < p.width; i += 20) {
            p.line(i, 0, i, p.height);
            p.line(0, i, p.width, i);
        }

        // Draw droplets
        for (let i = 0; i < d_info.length; i++) {
            arr = d_info[i].pos
            p.fill(d_info[i].color.r, d_info[i].color.g, d_info[i].color.b, 127);
            anim_move();
        }

        //console.log(gui_broker.droplets);

        for (let i = 0; i < gui_broker.droplets.length; i++) {
            let droplet = gui_broker.droplets[i];
            p.fill(droplet.Color);
            p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);
        }

    }

    function anim_move() {
        // Reset count
        if (count == (arr.length - 1)) {
            count = 0;
        }

        if (amount > 1 || amount < 0) {
            amount = 0;
            count++;
        }

        let d1x = p.lerp(arr[count].x, arr[(count + 1) % arr.length].x, amount);
        let d1y = p.lerp(arr[count].y, arr[(count + 1) % arr.length].y, amount);
        
        p.ellipse(d1x, d1y, 20, 20);
    }


    function custom_input() {
        console.log("INPUT");
    }
};



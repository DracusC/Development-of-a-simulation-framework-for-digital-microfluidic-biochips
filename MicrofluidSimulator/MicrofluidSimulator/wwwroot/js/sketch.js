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
    gui_broker.droplets = board.Droplets;
    gui_broker.electrodes = board.Electrodes;

    droplet_info.old = droplet_info.new;
    droplet_info.new = gui_broker.droplets;

    amount = 0;
}
window.change_play_status = (status) => {
    gui_broker.play_status = !gui_broker.play_status;
};


// Declare global variables
let arr = [];
let d_info = [];

// Store simulator board info
let simulator_droplets = [];
let simulator_electrodes = [];


// gui_broker acts as a connection between the simulator and gui
let gui_broker = {
    gui_status: true, // Ready for a new update (true/false)
    play_status: false,
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

let amount = 0;
 
let sketch = function (p) {
    
    let step;
    p.setup = function(){ 
        canvas = p.createCanvas(700, 640);
        //p.frameRate(10);
        //arr = [];
        step = 0.05;
    }

    
    

    p.draw = function(){
        p.background(240);

        // Check for next step
        if (amount < 1) {
            amount += step; //console.log(step, amount);
        } else if (gui_broker.play_status) {
            DotNet.invokeMethodAsync('MicrofluidSimulator', 'JSSimulatorNextStep');
        }

        draw_electrodes();
        draw_droplet();
        /*for (let i = 0; i < gui_broker.droplets.length; i++) {
            let droplet = gui_broker.droplets[i];
            p.fill(droplet.Color);
            p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);
        }*/

    }


    function draw_electrodes() {
        for (let i = 0; i < gui_broker.electrodes.length; i++) {
            let electrode = gui_broker.electrodes[i];
            if (electrode.shape) { console.log(electrode.ID, "POLYGON!"); return; }

            p.stroke("black");
            p.fill("white");
            if (electrode.Status != 0) { p.fill("red");}
            p.rect(electrode.PositionX, electrode.PositionY, electrode.SizeX, electrode.SizeY);

            // TEXT FOR DEBUGGING
            p.textSize(8);
            //p.textAlign(p.LEFT, p.BOTTOM);
            p.text(electrode.ID1, electrode.PositionX, electrode.PositionY + electrode.SizeY / 2);
        }
    }


    function draw_droplet() {
        
        for (let i = 0; i < gui_broker.droplets.length; i++) {
            let droplet = gui_broker.droplets[i];

            //p.fill(droplet.Color);
            //p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);
            anim_move(droplet, i);
            //anim_move(droplet, i);
        }
    }

    function anim_move(droplet, i) {
        
        /*if (amount > 1) {
            amount = 0;
            animate = false;
        }*/

        p.fill(droplet.Color);
        //p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);

        if (droplet_info.old.length == 0) {
            p.ellipse(droplet.PositionX, droplet.PositionY, droplet.SizeX, droplet.SizeY);
        } else {
            let d1x = p.lerp(droplet_info.old[i].PositionX, droplet_info.new[i].PositionX, amount);
            let d1y = p.lerp(droplet_info.old[i].PositionY, droplet_info.new[i].PositionY, amount);
            //console.log(d1x, d1y);
            p.ellipse(d1x, d1y, droplet.SizeX, droplet.SizeY);
        }        
    }
};



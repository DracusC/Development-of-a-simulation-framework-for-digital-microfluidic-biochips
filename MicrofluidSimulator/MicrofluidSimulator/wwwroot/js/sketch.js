/* 
 * Written by Joel A. V. Madsen
 */

window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

let arr = [];
let d_info = [];
 
let sketch = function(p){
 
    p.setup = function(){ 
        canvas = p.createCanvas(401, 401);

        arr = [];


        d_info = [
            {
                name: "droplet_1", color: { r: 255, g: 0, b: 0 },
                pos: [p.createVector(170, 10),
                p.createVector(170, 10),
                p.createVector(170, 30),
                p.createVector(190, 30),
                p.createVector(190, 30),
                p.createVector(210, 30),
                p.createVector(210, 10),
                p.createVector(190, 10),
                p.createVector(170, 10),
                p.createVector(170, 10)],
                size: { w: 20, h: 20 }
            },
            {
                name: "droplet_2", color: { r: 0, g: 255, b: 0 },
                pos: [p.createVector(p.width - 171, 10),
                p.createVector(p.width - 171, 10),
                p.createVector(p.width - 171, 30),
                p.createVector(p.width - 191, 30),
                p.createVector(p.width - 191, 10),
                p.createVector(p.width - 211, 10),
                p.createVector(p.width - 211, 30),
                p.createVector(p.width - 191, 30),
                p.createVector(p.width - 171, 30),
                p.createVector(p.width - 171, 10)],
                size: { w: 20, h: 20 }
            },
            {
                name: "droplet_3", color: { r: 0, g: 255, b: 0 },
                pos: [p.createVector(170, 110),
                p.createVector(170, 110),
                p.createVector(170, 130),
                p.createVector(190, 130),
                p.createVector(190, 130),
                p.createVector(190, 130),
                p.createVector(190, 130),
                p.createVector(190, 110),
                p.createVector(170, 110),
                p.createVector(170, 110)],
                size: { w: 20, h: 20 }
            },
            {
                name: "droplet_4", color: { r: 255, g: 0, b: 0 },
                pos: [p.createVector(p.width - 171, 110),
                p.createVector(p.width - 171, 110),
                p.createVector(p.width - 171, 130),
                p.createVector(p.width - 191, 130),
                p.createVector(p.width - 211, 130),
                p.createVector(p.width - 211, 130),
                p.createVector(p.width - 211, 130),
                p.createVector(p.width - 191, 130),
                p.createVector(p.width - 171, 130),
                p.createVector(p.width - 171, 110)],
                size: { w: 20, h: 20 }
            }
        ];
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

};
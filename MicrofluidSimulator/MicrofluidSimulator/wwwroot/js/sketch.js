let arr = [];
let d_info = [];

/*

drop {
  id:
  color:
  volume:
  instructions:
}


instructions = [
  {i_id: "create", step: 3, x: 5, y:5, w:10, h:10, color:rgb, size:20},
  {i_id: "move", x:10, y:5},
  {i_id: "merge", d1, d2, x: 11, y:5}
  {i_id: "end"}
]

instr = droplet[i];
switch(instr.i_id) {
  case("create"):
    if(step != instr.step) {break;}
    // Do something with the droplet info
  break;
  case("end")
}

*/

class droplet {
  constructor() {
    
  }
}

function setup() {
  canvas = createCanvas(401, 401);
  arr = [];
  
  d_info = [
    {
      name:"droplet_1", color: {r:255,g:0,b:0},
      pos: [createVector(170,10, 0),
        createVector(170,10, 0),
        createVector(170,30, 0),
        createVector(190,30, 0),
        createVector(190,30, 0),
        createVector(210,30, 0),
        createVector(210,10, 0),
        createVector(190,10, 0),
        createVector(170,10, 0),
        createVector(170,10, 0)],
      size: {w:20,h:20}
    },
    {
      name:"droplet_2", color: {r:0,g:255,b:0},
      pos: [createVector(width-171,10, 0),
        createVector(width-171,10, 0),
        createVector(width-171,30, 0),
        createVector(width-191,30, 0),
        createVector(width-191,10, 0),
        createVector(width-211,10, 0),
        createVector(width-211,30, 0),
        createVector(width-191,30, 0),
        createVector(width-171,30, 0),
        createVector(width-171,10, 0)],
      size: {w:20,h:20}
    },
    {
      name:"droplet_3", color: {r:0,g:255,b:0},
      pos: [createVector(170,110, 0),
        createVector(170,110, 0),
        createVector(170,130, 0),
        createVector(190,130, 0),
        createVector(190,130, 0),
        createVector(190,130, 0),
        createVector(190,130, 0),
        createVector(190,110, 0),
        createVector(170,110, 0),
        createVector(170,110, 0)],
      size: {w:20,h:20}
    },
    {
      name:"droplet_4", color: {r:255,g:0,b:0},
      pos: [createVector(width-171,110, 0),
        createVector(width-171,110, 0),
        createVector(width-171,130, 0),
        createVector(width-191,130, 0),
        createVector(width-211,130, 0),
        createVector(width-211,130, 0),
        createVector(width-211,130, 0),
        createVector(width-191,130, 0),
        createVector(width-171,130, 0),
        createVector(width-171,110, 0)],
      size: {w:20,h:20}
    }
  ];
}

let step = 0.02;
let amount = 0;
var count = 0

function draw() {
  background(240);
  amount += step;
  
  // Draw grid
  for(let i = 0; i < width; i += 20) {
    line(i, 0, i, height);
    line(0, i, width, i);
  }
  
  // Draw droplets
  for(let i = 0; i < d_info.length; i++){

    arr = d_info[i].pos
    fill(d_info[i].color.r,d_info[i].color.g,d_info[i].color.b,127);
    anim_move();
  }
  
  
}




function anim_move() {  
  // Reset count
  if(count == (arr.length-1)) {
    count = 0;
  }
  
  
  if (amount > 1 || amount < 0) {
    //step *= -1;
    amount = 0;
    count++;
  }
  
  
  let d1 = p5.Vector.lerp(arr[count], arr[count+1], amount);  

  ellipse(d1.x,d1.y, 20, 20);
}


/*

drop {
  id:
  color:
  volume:
  instructions:
}


instructions = [
  {i_id: "create", step: 3, x: 5, y:5, w:10, h:10, color:rgb, size:20},
  {i_id: "move", x:10, y:5},
  {i_id: "merge", d1, d2, x: 11, y:5}
  {i_id: "end"}
]

instr = droplet[i];
switch(instr.i_id) {
  case("create"):
    if(step != instr.step) {break;}
    // Do something with the droplet info
  break;
  case("end")
}

*/
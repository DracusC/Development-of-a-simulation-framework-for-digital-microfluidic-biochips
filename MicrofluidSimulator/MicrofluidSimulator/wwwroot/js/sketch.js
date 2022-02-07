window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

window.addEventListener('load', (event) => {
  setp5();
});
 
let sketch = function(p){
 
    p.setup = function(){ 
      canvas = p.createCanvas(401, 401);
    }
 
    p.draw = function(){
      p.stroke(0);
      p.fill(255,0,0);
      p.ellipse(50,50,20,20);
    }
};
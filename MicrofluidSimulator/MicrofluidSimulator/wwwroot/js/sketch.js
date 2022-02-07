window.setp5 = () => {
    new p5(sketch, window.document.getElementById('container'));
    return true;
};

window.addEventListener('load', (event) => {
  setp5();
});
 
let sketch = function(p){
 
    p.setup = function(){ 
      canvas = createCanvas(401, 401);
    }
 
    p.draw = function(){
      stroke(0);
      fill(255,0,0);
      ellipse(50,50,20,20);
    }
};
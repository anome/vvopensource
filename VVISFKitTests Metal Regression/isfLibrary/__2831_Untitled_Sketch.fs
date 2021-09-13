/*{
  "CREDIT": "by thedantheman",
  "DESCRIPTION": "",
  "CATEGORIES": [],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "scale",
      "LABEL": "scale",
      "TYPE": "float",
      "DEFAULT": 5,
      "MIN": 0,
      "MAX": 500
    },
    {
      "NAME": "offset",
      "LABEL": "offset",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "lambda",
      "LABEL": "lambda",
      "TYPE": "float",
      "DEFAULT": 0.9,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "fade",
      "LABEL": "fade",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0,
      "MAX": 1
    }
  ]
}*/

vec4 getColorCoded(vec2 _vel) {
    vec2 xout = vec2(max(_vel.x,0.0),abs(min(_vel.x,0.0)))*scale;
    vec2 yout = vec2(max(_vel.y,0.0),abs(min(_vel.y,0.0)))*scale;
    float dirY = 1.0;
    if (yout.x > yout.y)
        dirY=0.90;
    return vec4(xout.xy,max(yout.x,yout.y),dirY);
}

vec4 getNormal(vec2 _vel){
    return vec4((_vel.x*0.5)+0.5, (_vel.y*0.5)+0.5, 0.5, 1.0);
}

void main(){
    

    vec2 st = isf_FragNormCoord;
    float a = IMG_NORM_PIXEL(inputImage, st).r;

    
    vec2 x1 = vec2(offset*2.0,0.0);
    vec2 y1 = vec2(0.0,offset*2.0);
    
    //get the difference
    float curdif = b-a;
    
    //calculate the gradient
    //for X________________
    float gradx = IMG_NORM_PIXEL(lastFrame, st+x1).r-IMG_NORM_PIXEL(lastFrame, st-x1).r;
    gradx += IMG_NORM_PIXEL(inputImage, st+x1).r-IMG_NORM_PIXEL(inputImage, st-x1).r;
    
    //for Y________________
    float grady = IMG_NORM_PIXEL(lastFrame, st+y1).r-IMG_NORM_PIXEL(lastFrame, st-y1).r;
    grady += IMG_NORM_PIXEL(inputImage, st+y1).r-IMG_NORM_PIXEL(inputImage, st-y1).r;
    
    float gradmag = sqrt((gradx*gradx)+(grady*grady)+lambda*0.1);
    
    vec2 vel = vec2(curdif*(gradx/gradmag),
                    curdif*(grady/gradmag));
    
    pVel -= 0.5;
    pVel *= 2.0;
    
    pVel *= (1.0-fade*0.5);
    pVel += vel;
    
    pVel *= 0.5;
    pVel += 0.5;
    
    gl_FragColor = vec4(pVel.x,pVel.y,1.0,1.0);
    
}

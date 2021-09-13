/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/MltXWn by blackpolygon.  :D",
    "IMPORTED": {
    },
    "INPUTS": [
    ]
}

*/


// Author: blackpolygon
// Title:  Double Galaxy

// Based on 'Audio Eclipse' by airtight
// https://www.shadertoy.com/view/MdsXWM


const float dots = 600.; 
float radius = 0.232125; 
const float brightness = 0.0002131211;

//convert HSV to RGB
vec3 hsv2rgb(vec3 c){
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}
		

void main() {



    vec2 st=(gl_FragCoord.xy-.5*RENDERSIZE.xy)/min(RENDERSIZE.x,RENDERSIZE.y);
    vec3 c=vec3(0.0);
    
    st = rotate2d( sin(TIME/5.)*3.14 ) * st;
	float b1a0 = 1.5+sin(TIME)*.5;
    float b1a02 = 1.5+cos(TIME)*0.5;
    float ra =  (0.15+sin(TIME/.1)*.3)*0.002;
    
    //inner
    for(float i=0.;i<dots*.341; i++){
			
        radius +=ra/2000.;
        
		//get location of dot
        float x = radius*cos(2.*3.14*float(i)/(dots/(15.+b1a0)));
        float y = radius*sin(2.*3.14*float(i)/(dots/(14. +b1a02)));
        vec2 o = vec2(x,y);
	    
		//get color of dot based on its index in the 
		//circle + time to rotate colors
		vec3 dotCol = hsv2rgb(vec3((i + TIME*5.)/ (dots/14.),1.,1.0));
	    
        //get brightness of this pixel based on distance to dot
		c += brightness/(length(st-o))*dotCol;
    }
    
    //outer
    for(float i=0.;i<dots*1.2; i++){
        radius += ra*.15;
        float y = radius*cos(2.*3.14*float(i)/(dots/(10.+b1a0)));
        float x = radius*sin(2.*3.14*float(i)/(dots/(10. +b1a02)));
        vec2 o = vec2(x,y);
		vec3 dotCol = hsv2rgb(vec3((i + TIME*5.)/ (dots/10.),1.,1.0));
		c += brightness/(length(st-o))*dotCol*1.7;
    }
	 
	gl_FragColor = vec4(c,1);
}

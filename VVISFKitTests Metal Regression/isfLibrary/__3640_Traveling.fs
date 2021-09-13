/*
{
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE": "image"
    }
  ],
  "CATEGORIES" : [
    "depth",
    "burnout",
    "texturedeformation",
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/4s3GzH by kinerius.  First custom shader here.\nThis is just a test for a \"burnout\" style shader for a game made in Phaser, its Mobile friendly.\nAny feedback is mostly welcome.",
  "ISFVSN" : "2"
}
*/


// Created by Kinerius
// inspired by: https://www.shadertoy.com/view/XsXGWM

// phaser integration stuff

#define PI 3.141592654
#define TWO_PI 6.283185308

float time;

float fp_mountains = 0.2;
float hCut = 0.495;
float globalSpeed = 1.9;

vec2 puntoFuga = vec2(0.5,0.45);

vec2 hDef;
vec2 vDef;
vec2 uv;
vec2 texcoord;
vec2 texcoord2;

vec2 resolution;

void calcHorizon ( in float speed, in float persp  ) 
{
 	texcoord.y = mod(-time*speed*globalSpeed,1.0);
   	float zz = 1.0/(1.0-uv.y*persp*1.14);
    texcoord.y -= zz * sign(zz);
    hDef = texcoord.xy * vec2(zz*1.13, 1.0) - vec2(zz, 0.0);
}

void calcVertical( in float speed, in float persp  ) 
{  
    texcoord2.x = mod(-time*speed*globalSpeed,1.0);
   	float zz = 2.0/(1.0-uv.x*persp);
    texcoord2.x -= zz * sign(zz);
    vDef = texcoord2.xy * vec2(1.0, zz*(2.0)) - vec2(0.5, zz) ;  
}

void addRoad(in vec4 background, out vec4 tex ) 
{    
    if ( uv.x < uv.y * (0.5 + puntoFuga.y) ) // left side
    {
     	tex = background;
    }else  
    if ( uv.x > (1.0 - (uv.y  * (0.5 + puntoFuga.y)) ) ) // right side
    {
     	tex = background;
    } else     
    if (uv.y > hCut ) // horizontal cut
    {
     	tex = background;   
    } else {
        calcHorizon(2.5, 1.7);
        tex = vec4(0.2) + (cos(hDef.y * TWO_PI) * .01);

        tex += vec4(0.2,0.2,0.0,1.0);
    }
}

void addSideRoad (in vec4 background, out vec4 tex ) 
{   
    if ( uv.x < (uv.y - 0.42) * (0.5 + puntoFuga.y * 12.0) ) // left side
    {
     	tex = background;
    }else  
    if ( uv.x > (1.0 - ((uv.y - 0.42)  * (0.5 + puntoFuga.y * 12.0)) ) ) // right side
    {
     	tex = background;
    }else
    if (uv.y > hCut ) // horizontal cut
    {
     	tex = background;
    } else {
        calcHorizon(1.50, 1.7);
        tex = vec4(0.5) + (sin(hDef.y * TWO_PI) * .05);
        tex.r *= 0.2;
        tex += vec4(0.2,0.2,0.0,1.0);
    }
    
    
}

void main() {



    /// These are for shadertoy only
    time = TIME;
    resolution = RENDERSIZE.xy;
    ////////////////////////////////
    
	uv = gl_FragCoord.xy / resolution.xy;
    vec4 background = vec4(0.3,0.3,0.9,1.0);
    background.rgb *= 1.2 + uv.y;
    texcoord = gl_FragCoord.xy / vec2(resolution.y);
    texcoord2 = gl_FragCoord.xy / vec2(resolution.y);
    
   
    addSideRoad( background, background);
    addRoad( background, background);
    
	gl_FragColor = background;
}

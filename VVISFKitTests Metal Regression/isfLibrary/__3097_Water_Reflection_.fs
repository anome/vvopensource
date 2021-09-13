/*{
  "DESCRIPTION": "Your shader description",
  "CREDIT": "by you",
  "CATEGORIES": [
    "Your category"
  ],
  "INPUTS": [
    {
      "NAME": "iChannel0",
      "TYPE": "image"
    },
    {
      "NAME": "Watercolor",
      "TYPE": "color",
      "DEFAULT": [
        0.23,
        0.38,
        0.6,
        1
      ]
    },
    {
      "NAME": "MIX",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.5
    },
    {
      "NAME": "AMPLITUDE",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.1
    },
    {
      "NAME": "FREQUENCY",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.2
    },
    {
      "NAME": "XFACTOR",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.2
    },
    {
      "NAME": "POSITION",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.4
    }
  ]
}*/

// Ported from "Water Reflection" by jpweiyi: https://www.shadertoy.com/view/MsySWh

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    
    //vec4 waterColor = vec4(1.0);
    float reflectionY = POSITION;
    if(uv.y <= reflectionY)
    {        
        float oy = uv.y;
        uv.y = 2.0*reflectionY - uv.y;
        uv.x = uv.x - ((uv.x-0.5)*XFACTOR*5.) * (1.0-oy/reflectionY);
        uv.y = uv.y + sin(FREQUENCY*10./(oy-reflectionY)+iGlobalTime*10.0)*AMPLITUDE/10.;
    }
    
   if (fragCoord.y < RENDERSIZE.y*POSITION) {fragColor = IMG_NORM_PIXEL(iChannel0, uv)*(1.0-MIX) + Watercolor*MIX;}
    else {fragColor = IMG_NORM_PIXEL(iChannel0, uv);}
}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
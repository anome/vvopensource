/*
{   "CREDIT": "by mojovideotech",
    "CATEGORIES": [
        "polar coordinates",
        "circles",
        "rings",
        "vortex"
    ],
    "DESCRIPTION": "",
    "INPUTS": [
    {
        "NAME":     "inputImage",
        "TYPE":     "image"
    },
    {
        "NAME" :    "inputCenter",
        "TYPE" :    "point2D",
        "DEFAULT" : [ 0.0, 0.0 ],
        "MAX" :     [ 1.0, 1.0 ],
        "MIN" :     [ -1.0, -1.0 ]
    },
    {
        "NAME" :    "center",
        "TYPE" :    "point2D",
        "DEFAULT" : [ 0.0, 0.0 ],
        "MAX" :     [ 0.75, 0.75 ],
        "MIN" :     [ -0.75, -0.75 ]
    },

    {
        "NAME" :    "rate",
        "TYPE" :    "float",
        "DEFAULT" : 0.125,
        "MIN" :     -1.0,
        "MAX" :     1.0
    }
  ],
  "ISFVSN" : "2"
}
*/


////////////////////////////////////////////////////////////
// PolarVortexDistortion  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



void main() 
{
    vec2 res = RENDERSIZE.xy;
    vec2 uv = gl_FragCoord.xy / res; 
    uv.x += 0.5;     
    float p = (res.x/res.y);
    float d = distance(vec2(p+center.x, 0.5+center.y),vec2(uv.x * p, uv.y));

	gl_FragColor = IMG_NORM_PIXEL(inputImage,mod(vec2(d-TIME*rate)+inputCenter,1.0));
}

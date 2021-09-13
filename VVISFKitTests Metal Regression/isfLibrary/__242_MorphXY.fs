/*{
    "CATEGORIES": [
        "Distortion"
    ],
    "CREDIT": "Massimiliano Cerioni (Inspired from paniq Morph and VIDVOX)",
    "DESCRIPTION": "Morphing with independent x-y values",
    "INPUTS": [
        {
            "NAME": "startImage",
            "TYPE": "image"
        },
        {
            "NAME": "endImage",
            "TYPE": "image"
        },
        {
            "DEFAULT": 0,
            "MAX": 1,
            "MIN": 0,
            "NAME": "progress",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "MAX": 1,
            "MIN": 0,
            "NAME": "strength",
            "TYPE": "float"
        },
        {
     		 "NAME" : "xGain",
    		  "TYPE" : "float",
    		  "MAX" : 2,
    		  "DEFAULT" : 1,
    		  "MIN" : -2
  	  	},
    	{
     		 "NAME" : "yGain",
     		 "TYPE" : "float",
     		 "MAX" : 2,
     		 "DEFAULT" : 1,
     		 "MIN" : -2
   		}
    ],
    "ISFVSN": "2"
}
*/



vec4 getFromColor(vec2 inUV)	{
	return IMG_NORM_PIXEL(startImage, inUV);
}
vec4 getToColor(vec2 inUV)	{
	return IMG_NORM_PIXEL(endImage, inUV);
}



// Author: Massimiliano Cerioni
// License: 

vec4 transition(vec2 p) {
  vec4 ca = getFromColor(p);
  vec4 cb = getToColor(p);
  
  vec2 oa = (((ca.rg+ca.b)*0.5)*2.0-1.0);
  vec2 ob = (((cb.rg+cb.b)*0.5)*2.0-1.0);
  
  vec2 oc = vec2(0-0);
  oc.x = ((ca.r + ca.b) - 1.0) * xGain;
  oc.y = ((ca.g + ca.b) - 1.0) * yGain;
  oc = oc * strength;
  
  float w0 = progress;
  float w1 = 1.0-w0;
  return mix(getFromColor(p+oc*w0), getToColor(p-oc*w1), progress);
}



void main()	{
	gl_FragColor = transition(isf_FragNormCoord.xy);
}
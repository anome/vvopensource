/*
	{
	"DESCRIPTION": "Invisible Squircle Mask",
	"CATEGORIES": 
		[
		"filter"
		],
	"ISFVSN": "2",
	"CREDIT": "shirleyquirk",
	"VSN": "1.0.3",
	"INPUTS": 
		[
		    {
		        "NAME": "fgcolour",
		        "TYPE": "color",
		        "DEFAULT": [1.0,1.0,1.0,1.0]
		    },
		    {
		        "NAME": "bgcolour",
		        "TYPE": "color",
		        "DEFAULT": [0.0,0.0,0.0,1.0]
		    },
		    {
		        "LABEL": "Squirclishness: ",
		        "NAME": "squirclishness",
		        "TYPE": "float",
		        "MAX": 10.0,
		        "MIN": -10.0,
		        "DEFAULT": 0.0
		    },
		    {
		        "LABEL": "Smoothing: ",
		        "NAME": "smoothing",
		        "TYPE": "float",
		        "MAX": 1.0,
		        "MIN": 0.0,
		        "DEFAULT": 1.0
		    },
		    {
		        "LABEL": "Inversion: ",
		        "NAME": "inversion",
		        "TYPE": "float",
		        "MAX": 3.1415926535,
		        "MIN": -3.1415926535,
		        "DEFAULT": 1.0
		    },
			{
			"LABEL": "Center: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Size ",
			"NAME": "uSize",
			"TYPE": "point2D",
			"MIN": [0.0,0.0],
			"MAX": [1.0,1.0],
			"DEFAULT": [0.5,0.5]
			},
			{
			 "LABEL": "Rotation ",
			 "NAME": "rotation",
			 "TYPE": "float",
			 "MIN": -3.14159,
			 "MAX": 3.14159,
			 "DEFAULT": 0.0
			}
		]
	}
*/

#define PI 3.1415926535897932384626433832795
/*float sinh(float x){
    return (exp(x) - exp(-x));
}*/
float tanh(float x){
    return (exp(x) - exp(-x))/(exp(x)+exp(-x));
}
float smooth(float x){
    //we're trying to avoid float overflow(?) here
    //with the clamping else produces artifacts.
    return tanh(min(x,pow(10.0,smoothing)));
}
float handle(float x){
    //nicer squircle handle
    return x+2.0;
}
vec4 blend(vec4 fg,vec4 bg,float c){
    return fg*c+bg*(1.0-c);
}
void main()
	{
	float a = RENDERSIZE.y * uSize.x;                // horizontal size of ellipse
	float b = RENDERSIZE.y * uSize.y;                // vertical size of ellipse

	float c;                                         // color and opacity

	vec2 uv = (gl_FragCoord.xy/RENDERSIZE.xy - (uOffset+1.0)*0.5 + 0.5);
	uv *= RENDERSIZE.xy;
	float x = (uv.x - RENDERSIZE.x/2.0);      // re-center
	float y = (uv.y - RENDERSIZE.y/2.0);

    float exponent = handle(squirclishness);//TODO:useful handle
	float xprime = x*cos(rotation) + y*sin(rotation);
	float yprime = x*sin(rotation) - y*cos(rotation);
	float ellipse = (pow(xprime,exponent)/pow(a,exponent) + pow(yprime,exponent)/pow(b,exponent));
    //-10 to 10 normal
    //10 to 20 we flip c and map to -5 to 0 ..-10
    //-10 to -5 flip c and map to 0 to 5 .. +10
    if (abs(inversion)>PI*0.5) {
        float inv_off;
        if (inversion > 0.0){
            inv_off = -PI;
        }else{
            inv_off = PI;
        }
        c = 1.0 - smooth(pow(ellipse,tan(inversion + inv_off)));
    }else{
        c = smooth(pow(ellipse,tan(inversion)));
    }
    
    //if (swap) {c = 1.0-c;}
	gl_FragColor = blend(fgcolour,bgcolour,c);//vec4(fgcolour.x*c,fgcolour.y,fgcolour.z, c);
	//TODO: two gradients
	}
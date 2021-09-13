/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "2d",
    "trochoid",
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/4dlSzS by nimitz.  Fork of: https://www.shadertoy.com/view/XdXXRS\nUsing polar coordinates and a simple step() function to clamp the axes. (click and drag)\nStill not sure how to vary the drawing distance to the center of the \"roulette\"",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "X",
      "TYPE" : "float"
    },
    {
      "NAME" : "Y",
      "TYPE" : "float"
    }, {
			"NAME": "color",
			"TYPE": "color",
			"DEFAULT":  [
				1.0,
				0.0,
				0.0,
				1.0
			]
		}
  ]
}
*/


float t = TIME;
#define PI 3.1415927
#define TWOPI 6.283185307

void main() {
	float b, v;
	vec4 O;
	float h = RENDERSIZE.y;
	//vec2 U = 2. * (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
	vec2 U = gl_FragCoord.xy / RENDERSIZE.xy-0.5;
	U.x *= RENDERSIZE.x/RENDERSIZE.y;

	//U = 2. * fract(U) -1.;  // fractions
	
	
	float phase = TIME * 0.5 * 0.628318;
	float s = sin(phase) * 0.9 * sin(TIME);
	float c = cos(phase) * 1.;
	
	float a = atan(U.y, U.x); // polar coordinates
	float r = length(U);
	
	U = vec2(a, r) * 1.25;
	U.y -= .5;
	
	U -= vec2(s, c) * 0.25;
	
	float L = ceil(X * 10.) * 0.01 * PI; //wavelength
	float A = Y * .11; //amplitude 

	float K = TWOPI / L; // wave number
	float y = U.y / A;
	
	if (abs(y)>1.) y /= abs(y);
	
	// solve for x :  y = Asin(phi) with phi = K(x-Ct)
	float phi = asin(y);
	float x = phi/K;
	//x = phi;
	float x1 = x+A*cos(phi);	
	// 2nd solution for asin
	phi = PI-phi;
	x = phi/K;
	//x = phi;
	float x2 =  x+A*cos(phi);
	
	// find branch closest to x,y
	x1 = U.x - x1;
	x1 = min(mod(x1, L), mod(-x1, L));
	x2 = U.x-x2;
	x2 = min(mod(x2, L), mod(-x2, L));
	
	v = smoothstep(0., .1, min(x1, x2));
	
	y = A * y;
	y = U.y - y;
	
	v += step(y,-0.002);
	v += 1.-step(y,0.002);
	
	v = smoothstep(.01, 0., v);
	
	O = v * color;
	gl_FragColor = vec4(vec3(O), 1.0); 
}
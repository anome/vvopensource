/*{
	"CREDIT": "by isak.burstrom",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}, {
			"NAME": "shape",
			"TYPE": "float"
			
		}
	]
}*/


//source: https://www.shadertoy.com/view/Ms3SzB
// and: http://glslsandbox.com/e#35580.1

void main() {
	
	
	vec4 O = vec4(0.0);
	vec2 U = gl_FragCoord.xy;
	float h = RENDERSIZE.y;
	U = 4. * U/h; // normalized coordinates
	U = 16. * (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
	vec2 K = ceil(U);
	U = 2.*fract(U)-1.;  // or K = 1.+2.*floor(U) to avoid non-fractionals
	float a = atan(U.y, U.x); // polar coordinates
	float r = length(U);
	float v = 0.0; 
	float A = 0.0;
	float b = 0.0;
	for(int i=0; i<7; i++) {
		// if fractional, there is K.y turns to close the loop via K.x wings.
		A = K.x/K.y*a + TIME;
		b = smoothstep(1., -1., 8.*abs(r-.2*sin(A)-.5)); // ribbon (antialiased)
		v = max(v, ( 1. + .8* cos(A) ) / 1.8 * b);  // 1+cos(A) = depth-shading
		a += 6.28;                                 // next turn
	}
	
	O = v * vec4(.8,1,.3,1);
	O.g = sqrt(O.g);                              // greenify
	//O = v*(.5+.5*sin(K.x+17.*K.y+60.0+vec4(0,2.1,-2.1,0)));           // random colors
	gl_FragColor = vec4(vec3(O), 1.0); //vec4(vec3(r),1.0);
	
	//gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
}


/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"stars"
	],
	"INPUTS": [
	]
}*/

float D=3., Z=2.0;               // D: duration of advection layers, Z: zoom factor

#define R(U,d) fract( 1e4* sin( U*mat2(1234,-53,457,-17)+d ) )

float M(vec2 U, float t) {           // --- texture layer
 vec2 iU = ceil(U/=exp2(t-8.)),              // quadtree cell Id - infinite zoom
//   vec2 iU = ceil(U/=exp2(t-8.)*D/(3.+t)),     // quadtree cell Id - with perspective
          P = .2+.6*R(iU,0.);                  // 1 star position per cell
    float r = 9.* R(iU,1.).x;                  // radius + proba of star ( = P(r<1) )
	return r > 1. ? 1. :   length( P - fract(U) ) * 8./(1.+5.*r) ;
}

void main() {
    gl_FragColor -= gl_FragColor;
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.y - .5;
    // --- prepare the timings and weightings of the 3  texture layers
    vec3 P = vec3(-1,0,1)/3., T,
         t = fract( TIME/D + P +.5 )-.5,  // layer time
         w = .5+.5*cos(6.28*t);                  // layer weight
    t = t*D+Z;  
    // --- prepare the 3 texture layers
    T.x = M(uv.xy,t.x),  T.y = M(-uv.xy,t.y),  T.z = M(uv.yx,t.z); // avoid using same gl_FragCoord.xy for all layers
   // T = sin(100.*uv.x/exp2(t))+sin(100.*uv.y/exp2(t));  // try this for obvious pattern
    T = .03/(T*T);
    // --- texture advection: cyclical weighted  sum
    gl_FragColor += dot(w,T);
    gl_FragColor.rgb += (T*t)/w*T;             // try this alternative to see the 3 layers of texture advection
}

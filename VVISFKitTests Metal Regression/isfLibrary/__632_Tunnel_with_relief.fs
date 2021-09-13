/*{
	"CREDIT": "by You",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	],
	 "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/4sXGRn by iq.  A 2D tunnel with fake relief"
}*/

void main() {
    vec2 p = (-RENDERSIZE.xy + 2.0*gl_FragCoord.xy)/RENDERSIZE.y;
    p *= 0.75;
    
    float a = atan( p.y, p.x );
    float r = sqrt( dot(p,p) );
    
    a += sin(0.5*r-0.5*TIME );
	
	float h = 0.5 + 0.5*cos(9.0*a);
	float s = smoothstep(0.4,0.5,h);
    vec2 uv;
    uv.x = TIME + 1.0/(r + .1*s);
    uv.y = 3.0*a/3.1416;
    uv = mod(uv, 1.0);
    vec3 col = IMG_NORM_PIXEL(inputImage, uv).xyz;
    float ao = smoothstep(0.0,0.3,h)-smoothstep(0.5,1.0,h);
    col *= 1.0 - 0.6*ao*r;
	col *= r;
    gl_FragColor = vec4( col, 1.0 );
}

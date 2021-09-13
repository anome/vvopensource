/*{
	"CREDIT": "by mojovideotech",
 "CATEGORIES" : [
    "Automatically Converted"
  ],
  "INPUTS" : [

  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#31011.1"
}
*/

// Pskyline by mojovideotech


#ifdef GL_ES
precision mediump float;
#endif


float noise2d(vec3 p) {
	return clamp((sin(dot(p.xy ,vec2(12.9898,78.233)))*tan(p.x+p.y)),1.0-p.z,1.0);
}

vec4 sample(int x, int y)
{
	vec2 p = ( (gl_FragCoord.xy + vec2(x, y)) / RENDERSIZE.xy );
	
	float a = 0.0;
	for (int i = 1; i < 9; i++) {
		float fi = float(i);
		float cd = 7.0;
		float s = floor(73.0*(p.x)/fi + 61.0*fi + TIME / 5.)- cd;
		if (p.y < noise2d(vec3(s,s,cd))*fi/17.0 - fi*.095 + 1.0 + 0.15*cos(TIME / 5. + float(i)/5.0 + p.x*5.0)) {
			a = float(i)/9.;
		cd -= float(i) ;
		}
	}

	return vec4(vec3(a*p.x, a*p.y, a * (1. - p.x) ), 1.0 );
}

#define EDGE_THRESHOLD 1e-2

bool edge()
{
	vec4 mid = sample(0, 0);
	return distance(mid, sample(0, 1)) > EDGE_THRESHOLD || distance(mid, sample(1, 0)) > EDGE_THRESHOLD;
}

void main( void )
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	float T = TIME *0.05;
//	uv.x = sin(uv.y*8. / T); //  + sin(uv.y/8.))  sin(uv.x*10.0)+TIME*0.5;
	float yThreshold = sin(uv.y/80. + T + cos(uv.y/8.)); //0.5+0.15*sin(TIME*0.5);
    float rOut= (mod(1./(-abs(uv[1]-yThreshold))/50.-T*1.2,0.5));
    rOut = step(rOut,0.05);
    rOut  = rOut*3.*abs(uv[1]-yThreshold);
    float gOut =  uv[1]>yThreshold?.2:.7;
    gOut = gOut*3.*(abs(uv[1]-yThreshold)+0.07);
    vec3 clr = vec3(rOut+0.5,gOut,0.5+0.15*sin(T*0.));
    gl_FragColor = vec4(1.0-clr,1.0);
	gl_FragColor = edge() ? vec4(clr, 1.0) : sample(0, 0);
//	gl_FragColor = sample(1, 1);
}


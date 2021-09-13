/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	{
			"NAME": "cloudscale",
			"TYPE": "float",
			"DEFAULT": 0.95,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "cloudcover",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "cloudlight",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "clouddark",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "cloudalpha",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "skytint",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "skycolor1",
			"TYPE": "color",
			"DEFAULT": [
				0.2,
				0.4,
				0.6,
				1.0
			]
		},
		{
			"NAME": "skycolor2",
			"TYPE": "color",
			"DEFAULT": [
				0.4,
				0.7,
				1.0,
				1.0
			]
		},
		{
			"NAME": "detail",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "warp",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

// Ported and adapted from "2D Clouds" by drift: https://www.shadertoy.com/view/4tdSWr


vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

// const float cloudscale = 1.1;
// const float cloudcover = 0.72;
// const float clouddark = 0.5;
// const float cloudlight = 0.13;
// const float cloudalpha = 8.0;
// const float skytint = 0.5;
// const vec3 skycolour1 = vec3(0.2, 0.4, 0.6);
// const vec3 skycolour2 = vec3(0.4, 0.7, 1.0);

const int maxdetail = 8;
const float speed = 0.03;
const mat2 m = mat2( 1.6,  1.2, -1.2,  1.6 );

vec2 hash( vec2 p ) {
	p = vec2(dot(p,vec2(127.1,311.7)), dot(p,vec2(269.5,183.3)));
	return -1.0 + 2.0*fract(sin(p)*43758.5453123);
}

float noise( in vec2 p ) {
    const float K1 = 0.366025404; // (sqrt(3)-1)/2;
    const float K2 = 0.211324865; // (3-sqrt(3))/6;
	vec2 i = floor(p + (p.x+p.y)*K1);	
    vec2 a = p - i + (i.x+i.y)*K2;
    vec2 o = (a.x>a.y) ? vec2(1.0,0.0) : vec2(0.0,1.0); //vec2 of = 0.5 + 0.5*vec2(sign(a.x-a.y), sign(a.y-a.x));
    vec2 b = a - o + K2;
	vec2 c = a - 1.0 + 2.0*K2;
    vec3 h = max(0.5-vec3(dot(a,a), dot(b,b), dot(c,c) ), 0.0 );
	vec3 n = h*h*h*h*vec3( dot(a,hash(i+0.0)), dot(b,hash(i+o)), dot(c,hash(i+1.0)));
    return dot(n, vec3(70.0));	
}

float fbm(vec2 n) {
	float total = 0.0, amplitude = 0.1;
	for (int i=0; i<maxdetail; i++){
    	if (i > int(detail)) {break;}
		total += noise(n) * amplitude;
		n = m * n;
		amplitude *= 0.4;
	}
	return total;
}

// -----------------------------------------------

void mainImage( out vec4 fragColor, in vec2 fragCoord ) {
    
    float cloudscale = (1.0-cloudscale+.001) * 40.0;
    float cloudcover = cloudcover*8.0;
    float clouddark = 1.-clouddark;
    float cloudalpha = cloudalpha *80.;
    vec3 skycolour1 = skycolor1.rgb;
    vec3 skycolour2 = skycolor2.rgb;
    float detail = detail * float (maxdetail);
    
    vec2 p = fragCoord.xy / iResolution.xy;
	vec2 uv = (.5,.5)-p*(vec2(iResolution.x/iResolution.y,1.0));    
    float time = iGlobalTime * speed;
    float q = fbm(uv * cloudscale * warp);
    
    //ridged noise shape
	float r = 0.0;
	uv *= cloudscale;
    uv -= q - time;
    float weight = 0.8;
    for (int i=0; i<maxdetail; i++){
    	if (i > int(detail)) {break;}
		r += abs(weight*noise( uv ));
        uv = m*uv + time;
		weight *= 0.7;
    }
    
    //noise shape
	float f = 0.0;
    uv = uv = (.5,.5)-p*(vec2(iResolution.x/iResolution.y,1.0));
	uv *= cloudscale;
    uv -= q - time;
    weight = 0.7;
    for (int i=0; i<maxdetail; i++){
    	if (i > int(detail)) {break;}
		f += weight*noise( uv );
        uv = m*uv + time;
		weight *= 0.6;
    }
    
    f *= (r + f);
    
    //noise colour
    float c = 0.0;
    time = iGlobalTime * speed * 2.0;
    uv = uv = (.5,.5)-p*(vec2(iResolution.x/iResolution.y,1.0));
	uv *= cloudscale*2.0;
    uv -= q - time;
    weight = 0.4;
    for (int i=0; i<maxdetail; i++){
    	if (i > int(detail)) {break;}
		c += weight*noise( uv );
        uv = m*uv + time;
		weight *= 0.6;
    }
    
    //noise ridge colour
    float c1 = 0.0;
    time = iGlobalTime * speed * 3.0;
    uv = uv = (.5,.5)-p*(vec2(iResolution.x/iResolution.y,1.0));
	uv *= cloudscale*3.0;
    uv -= q - time;
    weight = 0.4;
    for (int i=0; i<maxdetail; i++){
    	if (i > int(detail)) {break;}
		c1 += abs(weight*noise( uv ));
        uv = m*uv + time;
		weight *= 0.6;
    }
	
    c += c1;
    
    vec3 skycolour = mix(skycolour2, skycolour1, p.y);
    vec3 cloudcolour = vec3(1.1, 1.1, 0.9) * clamp((clouddark + cloudlight*c), 0.0, 1.0);
   
    f = cloudcover + cloudalpha*f*r;
    
    vec3 result = mix(skycolour, clamp(skytint * skycolour + cloudcolour, 0.0, 1.0), clamp((f + c)/2., 0.0, 1.0));
    
	fragColor = vec4( result, 1.0 );
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
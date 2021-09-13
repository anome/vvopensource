
/*{
	"DESCRIPTION": "dunas ISF by Uará Lab, forked from http://glslsandbox.com/e#65191.0",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "fall_off",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "bg_r",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "bg_g",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "bg_b",
			"TYPE": "float",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "sun_r",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "sun_g",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "sun_b",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

/*
 * Original shader from: https://www.shadertoy.com/view/tdSBWc
 */




// shadertoy emulation
#define iTime TIME
#define iResolution RENDERSIZE
const vec4 iMouse = vec4(0.);

// --------[ Original ShaderToy begins here ]---------- //
// Fork of "Two Suns" by Plento. https://shadertoy.com/view/tdSBWc
// 2020-06-01 01:08:54

// Plento

#define R iResolution.xy
#define m vec2(R.x/R.y*(iMouse.x/R.x-.5),iMouse.y/R.y-.5)
#define ss(a, b, t) smoothstep(a, b, t)

vec2 hash22( vec2 x ){
    const vec2 k = vec2( 0.3183099, 0.3678794 );
    x = x*k + k.yx;
    return -1.0 + 2.0*fract( 16.0 * k*fract( x.x*x.y*(x.x+x.y)) );
}

mat2 rot(float a) {
    return mat2(cos(a), -sin(a), sin(a), cos(a));
}

float ridges(vec2 p ){
    vec2 i = floor( p );
    vec2 f = fract( p );
	
	vec2 u = ss(0., 1., f);
    
    vec2 a = hash22(i);
    vec2 b = hash22( i + vec2(1.,0.));
    vec2 c = hash22( i + vec2(.0,1.));
    vec2 d = hash22( i + vec2(1));
    
    
    float nse = mix( mix( dot( a, f ), 
                     dot( b, f - vec2(1.0,0.0) ), u.x),
                mix( dot( c, f - vec2(0.0,1.0) ), 
                     dot( d, f - vec2(1) ), u.x), u.y);
    nse = abs(nse);
    return nse;
}



float octnse(vec2 p, int oct){
    float a = 1.;
    float n = 0.;
    
    for(int i = 0; i<2; i++){
        if (i >= oct) break;
        n += ridges(p) * a;		
        
        p*=2.;
        a *= .5;
    }
    
    return n;
}

float map(vec3 rp){
    rp.y-=1.;
    rp.y += octnse(rp.xz*.06, 2)*12.;
    return rp.y;
}

vec3 normal( in vec3 pos ){
    vec2 e = vec2(0.002, -0.002);
    return normalize(
        e.xyy * map(pos + e.xyy) + 
        e.yyx * map(pos + e.yyx) + 
        e.yxy * map(pos + e.yxy) + 
        e.xxx * map(pos + e.xxx));
}

float march(vec3 rd, vec3 ro){
 	float t = 0., d = 0.;   
    for(int i = 0; i < 68; i++){
    	d = map(ro + rd*t); 	   
        
        if(abs(d) < .0025 || t > 64.)break;
        t += d * .65;
    }
    return t;
}

vec3 color(vec3 p, vec3 rd, vec3 n, vec2 u, float t){
    vec3 lp = p+vec3(0.2, 16.0, -4.0);
    vec3 ld = normalize(lp-p);
   	
    float lgd = length(lp - p);
    float faloff = exp(-lgd*fall_off);
    
    float fres = smoothstep(-.5, .5, dot(rd, n));
    float dif = max(dot(n, ld), .0);
    
    vec3 col = vec3(ss(.2, .27, abs(fract(p.z*.75)-.5)));
    
    vec3 lights = 2.5*fres*vec3(1., 0.55, 0.);
    lights += 6.6*dif*vec3(1., 1., 1.) * faloff;
    
    col *= lights;
    
    vec2 bgc = vec2(atan(rd.x, rd.z), max(rd.y, 0.));
    float nse = octnse(vec2(0.47, 0.)+bgc.xy*2., 3);
    float stamp = ss(0.005, 0.01, (rd.y + nse*.26 - .05));
    vec3 bg2D = vec3(bg_r, bg_g, bg_b)*stamp;
    vec3 sun = vec3(sun_r, sun_g, sun_b)*exp(-6.*(length(vec2(0., 0.0)-rd.xy)-.1));
    bg2D += sun*stamp;
    
    col = mix(vec3(0.96, 0.7, 0.5), col, exp(-t*t*t*0.00003));
    col += sun*.6*vec3(1., 0.6, 1.);
    col = mix(bg2D, col, .5);
    
	return col;   
}

const float rad = 25.;
const float spd = .2;

void mainImage( out vec4 f, in vec2 u ){
    vec2 uv = vec2(u.xy - 0.5*R.xy)/R.y;
    vec3 rd = normalize(vec3(uv, .5));
    vec3 ro = vec3(0., 3.5, rad);
    
    rd.xz *= rot(iTime*spd+m.y*12.+2.);
    rd.xy *= rot(cos(iTime*1.3)*.07);
    
    ro.x += cos(iTime*spd+m.y*12.+2.) * rad;
    ro.z += sin(iTime*spd+m.y*12.+2.) * rad;
    
    float t = march(rd, ro);
    
    vec3 n = normal(ro + rd*t);
    vec3 col = color(ro + rd*t, rd, n, u, t);
    
    col *= ss(0.5, 0.1, abs(uv.y));
    
    f = vec4(sqrt(clamp(col, .0, 1.)), 1.);
    
}
// --------[ Original ShaderToy ends here ]---------- //

void main(void)
{
    mainImage(gl_FragColor, gl_FragCoord.xy);
}

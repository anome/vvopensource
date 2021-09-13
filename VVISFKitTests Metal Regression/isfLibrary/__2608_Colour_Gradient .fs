/*{
	"CREDIT": "by joshpbatty",
	"DESCRIPTION": "B&W Colour Gradeint ",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	{
		"NAME": "mirror",
		"TYPE": "bool",
		"DEFAULT": 1.0
	},
	{
		"NAME": "waveforms",
		"TYPE": "long",
		"VALUES": [
			0,
			1,
			2,
			3,
			4
		],
		"LABELS": [
			"sine",
			"triangle",
			"sawtooth",
			"square",
			"random"
		],
		"DEFAULT": 1
	},
   	{
		"NAME": "animate_speed",
		"TYPE": "float",
		"DEFAULT": 0.1,
		"MIN": 0.0,
		"MAX": 1.0
	},
   	{
		"NAME": "dc",
		"TYPE": "float",
		"DEFAULT": 0.5,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
		"NAME": "amp",
		"TYPE": "float",
		"DEFAULT": 0.5,
		"MIN": 0.0,
		"MAX": 1.0
	},
 	{
		"NAME": "freq",
		"TYPE": "float",
		"DEFAULT": 0.5,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
		"NAME": "phase",
		"TYPE": "float",
		"DEFAULT": 0.3,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
		"NAME": "num_bands",
		"TYPE": "float",
		"DEFAULT": 1,
		"MIN": 0,
		"MAX": 4
	}
  ]
}*/

#define TWO_PI 6.2831853072
#define PI 3.14159265359
#define HALF_PI 1.57079632679

//--------- LFO's --------------
// FLOATS
float tri(float x) {
    return asin(sin(x))/(PI/2.);
}
float puls(float x) {
    return (floor(sin(x))+0.5)*2.;
}
float saw(float x) {
    return (fract((x/2.)/PI)-0.5)*2.;
}
float noise(float x) {
    return (fract(sin((x*2.) *(12.9898+78.233)) * 43758.5453)-0.5)*2.;
}
// VEC3
vec3 tri(vec3 x) {
    return vec3(asin(sin(x))/(PI/2.));
}
vec3 puls(vec3 x) {
    return vec3((floor(sin(x))+0.5)*2.);
}
vec3 saw(vec3 x) {
    return vec3((fract((x/2.)/PI)-0.5)*2.);
}
vec3 noise(vec3 x) {
    return vec3((fract(sin((x*2.) *(12.9898+78.233)) * 43758.5453)-0.5)*2.);
}


float lfo(int type, float x){
    if(type == 0) return sin(x);
    else if(type == 1) return tri(x);
    else if(type == 2) return saw(x);
    else if(type == 3) return puls(x);
    else if(type == 4) return noise(x);
    else return 0.0;
}
vec3 lfo(int type, vec3 x){
    if(type == 0) return cos(x);
    else if(type == 1) return tri(x);
    else if(type == 2) return saw(x);
    else if(type == 3) return puls(x);
    else if(type == 4) return noise(x);
    else return vec3(0.0);
}

float remap( float value, float inMin, float inMax, float outMin, float outMax )
{
    return ( (value - inMin) / ( inMax - inMin ) * ( outMax - outMin ) ) + outMin; 
}

//--------- Colour Palette
vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
//    return a + b*lfo(params.palette_lfo_type, (6.28318*(c*t+d)));
    return a + b*lfo(waveforms, (6.28318*(c*t+d)));
}


void main() {
	
	
	vec2 p = isf_FragNormCoord.xy; // fragCoord.xy / iResolution.xy;
    p = p*2.-1.;
    //vec2 tx = (gl_FragCoord.xy / iResolution.xy) - 0.5;
    
    if(mirror){
		p.y = abs(p.y);
	}
	
    // animate
    float t = TIME;
    p.y += (animate_speed*3.0)*t;
    
    //float num_bands = 1.;
    // compute colors
    float idx = 0.10;//ceil(p.x*num_bands) / num_bands;
	
	
	/*
    vec3 col = pal( p.x, vec3(dc),
                   (idx*lfo(params.amp_lfo_type,t*params.amp_lfo_speed)*params.amp_lfo_amp) + vec3(amp) + (t*(params.amp_cycle_speed*0.1)),
                   (idx*lfo(params.freq_lfo_type,t*params.freq_lfo_speed)*params.freq_lfo_amp) + vec3(freq) + (t*(params.freq_cycle_speed*0.1)) ,
                   (idx*lfo(params.phase_lfo_type,t*params.phase_lfo_speed)*params.phase_lfo_amp) + vec3(phase) + (t*(params.phase_cycle_speed*0.1)));
    */
    
    
	// vec3 col = pal( p.x, vec3(dc),
 //              (idx*lfo(1,vec3(amp))),
 //              (idx*lfo(1,vec3(freq))) ,
 //              (idx*lfo(1,vec3(phase))));
               
   	vec3 col = pal( p.y, vec3(dc),
               (vec3(amp)),
               (vec3(idx+freq)) ,
               (vec3(idx*phase)));
    
    // band
    //float f = fract(p.y*params.num_bands);
    // borders
    //col *= smoothstep( 0.49, 0.47, abs(f-0.5) );
    // shadowing
    //col *= 0.5 + 0.5*sqrt(4.0*f*(1.0-f));
    // dithering
    //col += (1.0/255.0)*texture( iChannel0, gl_FragCoord.xy/iChannelResolution[0].xy ).xyz;
    
    gl_FragColor = vec4( col, 1.0 );
    
    //gl_FragColor = vec4(1.0,abs(sin(TIME)),0.0,1.0);
}
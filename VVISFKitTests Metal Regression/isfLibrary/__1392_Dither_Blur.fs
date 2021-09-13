/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"NAME": "iChannel0",
			"TYPE": "image"
		},
      	{
			"LABEL": "AMOUNT",
			"NAME": "AMOUNT",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "MAXRADIUS",
			"NAME": "MAXRADIUS",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "JITTER",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
	]
}*/


// Based on "lens: bokeh blur, circular 3pass"
// by hornet: https://www.shadertoy.com/view/Xd33Dl


vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

const vec2 blurdir = vec2( 1.0, 0.0 );

float blurdist_px = float(RENDERSIZE.x*MAXRADIUS);
const int NUM_SAMPLES = 1;

vec3 srgb2lin(vec3 c) { return c*c; }
vec3 lin2srgb(vec3 c) { return sqrt(c); }

//note: uniform pdf rand [0;1]

float hash12n(vec2 p)
{
	p  = fract(p * vec2(5.3987, 5.4421));
    p += dot(p.yx, p.xy + vec2(21.5351, 14.3137));
	return fract(p.x * p.y * 95.4307);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 suv = fragCoord / iResolution.xy; 
    vec2 uv = fragCoord / iResolution.xy;
    
    float blur = 3. * AMOUNT * blurdist_px;
    blur *= .1; //empiric constant...
    
    float da = 6.283;
    float a = 1.0;
    
    if (JITTER) { a = da * hash12n(uv+(iGlobalTime)); }
    else { a = da * hash12n(uv); }
    
    vec3 sumcol = vec3(0.0);
 
 for (int i=0;i<10;++i)
    	{
        vec2 ofs = vec2( cos(a), sin(a) ) / iResolution.xy * blur*float(i+1)*.31;
    	vec2 p = uv+(ofs/10.);
       	sumcol = sumcol+srgb2lin(texture2D(iChannel0, p, -10.0).rgb);
        a += da;
    	
    	
    }
    
    sumcol = sumcol / 10.;
    sumcol = max( sumcol, 0.0 );

    
    
  fragColor = vec4 (lin2srgb( sumcol ), 1.0);
}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
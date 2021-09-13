/*{
	"DESCRIPTION": "xLights Audio2",
	"CREDIT": "adapted from https://www.shadertoy.com/view/ldl3W8 for xLights",
	"ISFVSN": "1",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	    {
	        "NAME": "inputImage",
	        "TYPE": "image"
	    },
		{
			"NAME": "scale",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 5.0
		},
		{
			"NAME": "borderWidth",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MIN": 0.1,
			"MAX": 1.0
		},
		{
		    "LABEL": "Mode: ",
			    "LABELS":
				[
				    "One                             ",
				    "Two                             "
				],
			"NAME": "mode",
			"TYPE": "long",
			"VALUES":
				[
				    0,
				    1
				],
			"DEFAULT": 0
		},
		{
		"LABEL": "Color Mode: ",
			"LABELS":
			[
			    "Shader Default                  ",
				"xLights Color Palette (3 used)  "
			],
		"NAME": "colorMode",
		"TYPE": "long",
		"VALUES":
		    [
			    0,
			    1
			],
		"DEFAULT": 0
		},
		{
		    "NAME": "color1",
			"TYPE": "color",
			"DEFAULT":
			[
				1.0,
				1.0,
				0.0,
				1.000
			]
		},
		{
		    "NAME": "color2",
		    "TYPE": "color",
		    "DEFAULT": 
		    [
			    0.0,
				1.0,
				1.0,
				1.0
			]
		},
		{
		    "NAME": "color3",
			"TYPE": "color",
			"DEFAULT": 
			[
				1.0,
				0.0,
				1.0,
				1.0
			]
		}			
	]
}*/

#define ANIMATE

// apparently the '%' operator is not supported until OpenGL ES 3.0
// and the ISF website is on an earlier version
int myModulo( int a, int b )
{
    return a - (a/b) * b;
}

vec2 hash2( vec2 p )
{
	// texture based white noise
	//return textureLod( iChannel0, (p+0.5)/256.0, 0.0 ).xy;
	
    // procedural white noise	
	return fract(sin(vec2(dot(p,vec2(127.1,311.7)),dot(p,vec2(269.5,183.3))))*43758.5453);
}


vec3 voronoi( in vec2 x )
{
    vec2 n = floor(x);
    vec2 f = fract(x);

    //----------------------------------
    // first pass: regular voronoi
    //----------------------------------
	vec2 mg, mr;

    float md = 8.0;
    for( int j=-1; j<=1; j++ )
    for( int i=-1; i<=1; i++ )
    {
        vec2 g = vec2(float(i),float(j));
		vec2 o = hash2( n + g );
		#ifdef ANIMATE
        o = 0.5 + 0.5*sin( TIME + 6.2831*o );
        #endif	
        vec2 r = g + o - f;
        float d = dot(r,r);

        if( d<md )
        {
            md = d;
            mr = r;
            mg = g;
        }
    }

    //----------------------------------
    // second pass: distance to borders
    //----------------------------------
    if ( mode == 0 )
    {
        md = 8.0;
        for( int j=-2; j<=2; j++ )
        {
            for( int i=-2; i<=2; i++ )
            {
                vec2 g = mg + vec2(float(i),float(j));
		        vec2 o = hash2( n + g );
		        #ifdef ANIMATE
                o = 0.5 + 0.5*sin( TIME + 6.2831*o );
                #endif	
                vec2 r = g + o - f;

                if ( dot(mr-r,mr-r)>0.00001 )
                    md = min( md, dot( 0.5*(mr+r), normalize(r-mr) ) );
            }
        }
    }

    return vec3( md, mr );
}


void main()
{
    vec2 p = scale * gl_FragCoord.xy / RENDERSIZE.xx;
    float audioIntensity = texture2D( inputImage, vec2( 0.5, 0.5 ) ).x;
    vec3 c = voronoi( 4.0 * p );
    
	// isolines
    vec3 multColor = ( myModulo( int( 16.0 * audioIntensity * c.x ), 2 ) == 0 ) ? color2.rgb : color3.rgb;
    vec3 col = c.x*(0.5 + 0.5*sin(64.0*c.x)) * multColor;
    
    // borders	
    col = mix( color1.rgb, col, smoothstep( 0.04, 0.07, c.x / borderWidth ) );

    gl_FragColor = vec4( col, 1. );
}

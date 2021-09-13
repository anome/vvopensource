/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	{
			"NAME": "Zoom",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "ShiftX",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "ShiftY",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "ShiftXFine",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "ShiftYFine",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "Iterations",
			"TYPE": "float",
			"DEFAULT": 256.0,
			"MIN": 0.0,
			"MAX": 512.0
		},
		{
			"NAME": "Bailout",
			"TYPE": "float",
			"DEFAULT": 500000.0,
			"MIN": 100.0,
			"MAX": 1000000.0
		},
		{
			"NAME": "Red",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "Green",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
	{
			"NAME": "Blue",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

// Original shader: "test 124657451894"
// by Natthan: https://www.shadertoy.com/view/Xs3Xz2

vec3 iResolution = vec3(RENDERSIZE, 1.);

float iGlobalTime = TIME;

void mainImage( out vec4 f, in vec2 p )
{
    float n = 0. ;
    
    // p.xy=p.yx;
    
    vec2 c = vec2(-ShiftX*4.-ShiftXFine/100.,-ShiftY*2.-ShiftYFine/100.) + 5. * vec2 (p.x/iResolution.x-.5, p.y/iResolution.y-.5)*pow(.01,1.-1.+Zoom*3.0), z=c*n;
   
    for( int i=0; i<512; i++ )
    {
    	
    	if (i>int(Iterations)) { break; }
    	
        z = vec2( z.x*z.x - z.y*z.y, 2.*z.x*z.y) + c;
 
        if( dot(z,z)>Bailout ) break;
 
        n++;
        
        vec3 f = vec3(step(0.5, mod(float(i),1.)));
    }
   
    //f = 1.0 + 1.0*sin( vec4((1.0-Red)*10.,(1.0-Green)*10.,(1.0-Blue)*10.,0) + .05*(n - log2(log2(dot(z,z)))) );
    
   
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
/*{
	"CREDIT": "by gosub7777777",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "anchorPoint",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			],
			"MIN": [-2.0,-2.0],
			"MAX": [2.0,2.0]
		},
		{
			"NAME": "anchorPoint2",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			],
			"MIN": [-1.0,-1.0],
			"MAX": [1.0,1.0]
		}
	]
}*/

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec3 col = vec3(0.0);
        
    vec2 p = (-RENDERSIZE.xy + 2.0*fragCoord.xy)/RENDERSIZE.y;
    float time = TIME;

    float zoo = zoom;
    float coa = cos( 0.15*(1.0-zoo)*time );
    float sia = sin( 0.15*(1.0-zoo)*time );
    zoo = pow( zoo,8.0);
    vec2 xy = vec2( p.x*coa-p.y*sia, p.x*sia+p.y*coa);
    vec2 c = anchorPoint+anchorPoint2*zoo + p * zoo;

    const float B = 256.0;
    float l = 0.0;
    vec2 z  = vec2(0.0);
    for( int i=0; i<200; i++ )
    {
        // z = z*z + c		
        z = vec2( z.x*z.x - z.y*z.y, 2.0*z.x*z.y ) + c;

        if( dot(z,z)>(B*B) ) break;

        l += 1.0;
    }

    // ------------------------------------------------------
    // smooth interation count
    //float sl = l - log(log(length(z))/log(B))/log(2.0);

    // equivalent optimized smooth interation count
    float sl = l - log2(log2(dot(z,z))) + 4.0; 
    // ------------------------------------------------------

    //float al = smoothstep( -0.1, 0.0, sin(0.5*6.2831*iTime ) );
    //l = mix( l, sl, al );

	float center = length(p);

   	col += clamp(vec3((sin(sl*0.2)*0.5+0.5)) , 0.0,1.0);
   	col += clamp(smoothstep(0.02,0.01,center) * vec3(1.0,0.0,0.0), 0.0,1.0);
   	
	fragColor = vec4( col, 1.0 );
}

void main() {
	mainImage(gl_FragColor, gl_FragCoord.xy);
}
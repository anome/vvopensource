/*{
	"CREDIT": "by gosub7777777",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "colorA",
			"TYPE": "color",
			"DEFAULT": [
				0.9,
				0.2,
				0.1,
				1.0
			]
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.2,
				0.6,
				0.9
			]
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.001,
			"MAX": 1.0
		},
		{
			"NAME": "zoom2",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.001,
			"MAX": 1.0
		},
		{
			"NAME": "zoom3",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.001,
			"MAX": 1.0
		},
		{
			"NAME": "rotation",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -3.1415,
			"MAX": 3.1415
		},
		{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			],
			"MIN": [-2.0,-2.0],
			"MAX": [2.0,2.0]
		},
		{
			"NAME": "fallOff",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0,
			"MAX": 8
		},
		{
			"NAME": "colorFreq",
			"TYPE": "float",
			"DEFAULT": 40.0,
			"MIN": 0,
			"MAX": 40
		},
		{
			"NAME": "colorSpeed",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0,
			"MAX": 10
		},
		{
			"NAME": "flowerRotationSepeed",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0,
			"MAX": 4
		},
		{
			"NAME": "lineAmp",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "lineFreq",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0,
			"MAX": 20
		}
		
	]
}*/

#define ITERATION 200.0

vec2 rotate(vec2 p, vec2 pivot, float a){
	float s = sin(a);
	float c = cos(a);
	p -= pivot;
	p = vec2(p.x*c-p.y*s, p.x*s+p.y*c);
	p += pivot;
	return p;
}

vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
    return a + b*cos( 6.28318*(c*t+d) );
}

vec3 fractal(vec2 uv, vec2 center, float zoom){
	vec2 c = center + uv * zoom;
	c = rotate(c, center, rotation);
	
	float r = 300.0;
	float r2 = r*r;
	
	float flowerRotation = TIME * flowerRotationSepeed;
	
	vec2 z;
	vec2 zPrevious;
	float iter;
	for(float i= 0.0; i < ITERATION; i++){
		zPrevious = rotate(z,vec2(0,0),flowerRotation);
		z = vec2(z.x*z.x-z.y*z.y, 2.0*z.x*z.y) + c;
		iter = i;
		if(dot(z,zPrevious) > r2) break;
	}
	
	if(iter > ITERATION) return vec3(0.0);
	float dist = length(z);
	float fracIter1 = (dist - r) / (r2 - r);
	float fracIter = log2( log(dist) / log(r));
	// float fracIter = log2(log2(dot(z,z)));
	
	
	//iter -= fracIter;
	
	float m = sqrt(iter / ITERATION);
	//vec3 col = pal( m * colorFreq + TIME*0.1  , vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,0.7,0.4),vec3(0.0,0.15,0.20) );
	vec3 col = mix(colorA.rgb, colorB.rgb, sin(m * colorFreq + TIME*colorSpeed) *0.5 + 0.5 );
	
	float depth = sin(dot(zPrevious,z)*0.0000000002)*0.5 +0.5; 
	
	fracIter = smoothstep(fallOff,0.0,fracIter);	
	col *= vec3(fracIter);
	float angle = atan(z.x,z.y);
	float waves = 1.0-sin(angle * 2.0 + flowerRotation * 4.0)*lineAmp; 
	col *= waves;
	// col = vec3(depth);
	return col;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec3 col = vec3(0.0);
        
    vec2 uv = (-RENDERSIZE.xy + 2.0*fragCoord.xy)/RENDERSIZE.y;
    float time = TIME;
    
    //Specular
	//uv.x = abs(uv.x);
	
    col = fractal(uv, center, zoom * zoom2 * zoom3);
    
    fragColor = vec4( col, 1.0 );
}
void main() {
	mainImage(gl_FragColor, gl_FragCoord.xy);
}
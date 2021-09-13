/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"LABEL": "OrginX",
			"NAME": "OriginX",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
			{
			"LABEL": "OrginY",
			"NAME": "OriginY",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Angle1",
			"NAME": "Angle1",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
	{
			"LABEL": "Radius1",
			"NAME": "Radius1",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Angle2",
			"NAME": "Angle2",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
	{
			"LABEL": "Radius2",
			"NAME": "Radius2",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
	{
			"LABEL": "ROTATE",
			"NAME": "ROTATE",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "DotSize",
			"NAME": "DotSize",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MIN": 0.0,
			"MAX": 1.0
		},
	{
			"LABEL": "LineWidth",
			"NAME": "LineWidth",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;
float twoPi = 6.28318531;

// Based on "Time Coordinates" by burito (Daniel Burke): https://www.shadertoy.com/view/Xd2XWR
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// Special thanks IÃ±igo Quilez!

// THIS SECTION DEFINES THE GEOMETRIC PRIMITIVE FUNCTIONS --------------------------------------

// Generalized rotation formula ----------------------------------------------------------------

	vec2 rot(vec2 p, float a) // 
	{
    	float c = cos(a);
    	float s = sin(a);
    	return vec2(p.x*c + p.y*s,
	             -p.x*s + p.y*c);
	}

// Draw filled circlesbased on CARTESIAN coordinates -------------------------------------------
	
	float circleFill(vec2 pos, float radius) 
	{
    	return clamp(((.99-(length(pos)-radius))-0.99)*500.0, 0.0, 1.0); // Mutiplication factor term affects sharpness of blend. Higher values are more distinct (default 500)  
	}
	
// END DEFINITION OF PRIMITIVE FUNCTIONS -----------------------------



void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    vec2 p = 1.0 - 2.0 * uv; // Coordinate system is centered on screen, Cartesian style. Up/Right is positive, Down/Left is negative. Origin = p
    
    p = vec2(p.x+OriginX, p.y+OriginY);
    
    p = rot(p,(1.0-ROTATE)*twoPi); // This uses the rot transform to rotate the origin 'p'. All function calls currently reference 'p'.
    
    p.x *= iResolution.x / iResolution.y; // Aspect correction
 
    vec3 colour = vec3(0.); // Initialize colour variable
    vec3 white = vec3(1.,1.,1.); // Initialize geometry colour
    float DotSize = DotSize/50.; // Scaling factor determines range of Dot Sizes (Default 50.)
    
	float Angle1 = Angle1 * twoPi;
	float Angle2 = Angle2 * twoPi;
	
	float Radius1 = Radius1 * 2.;
	float Radius2 = Radius2 * 2.;
	
	
	vec2 a = vec2 (Radius1 * sin (Angle1), Radius1 * cos(Angle1));
	vec2 b = vec2 (Radius2 * sin (Angle2), Radius2 * cos(Angle2));
	
    vec2 pa = -p - a;
    vec2 ba = b - a;
    	
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    float d = length( pa - ba*h );
    
    float c = clamp(((.99+LineWidth/100. - d)-0.99)*500., 0.0, 1.0);
    	
    c += circleFill(p+a, DotSize);
    c += circleFill(p+b, DotSize);
    
    c = clamp(c, 0.0, 1.0); // Final calculation. Modifying "c" term can make transparent layering.
    
    colour = white * c; // Colours all geometry with pre-defined "white" colour. White can be anything.
    
    fragColor = vec4(colour, 0.0);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
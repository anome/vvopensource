/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Stylize"
	],
	"INPUTS": [
		{
			"NAME": "iChannel0",
			"TYPE": "image"
		},
		{
			"NAME": "CELLSIZE",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": 3.0,
			"MAX": 200
		}
      	]
}*/

// Based on: https://www.shadertoy.com/view/4dX3DM
// Added CELLSIZE parameter to adjust pixel size

vec3 iResolution = vec3(RENDERSIZE, 1.0);

int CELL_SIZE = int (CELLSIZE);
float CELL_SIZE_FLOAT = float(CELL_SIZE);
int RED_COLUMNS = int(CELL_SIZE_FLOAT/3.0);
int GREEN_COLUMNS = CELL_SIZE-RED_COLUMNS;

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{

	vec2 p = floor(fragCoord.xy / CELL_SIZE_FLOAT)*CELL_SIZE_FLOAT;
	int offsetx = int(mod(fragCoord.x,CELL_SIZE_FLOAT));
	int offsety = int(mod(fragCoord.y,CELL_SIZE_FLOAT));

	vec4 sum = texture2D(iChannel0, p / iResolution.xy);
	
	fragColor = vec4(0.0,0.0,0.0,1.0);
	if (offsety < CELL_SIZE-1) {		
		if (offsetx < RED_COLUMNS) fragColor.r = sum.r;
		else if (offsetx < GREEN_COLUMNS) fragColor.g = sum.g;
		else fragColor.b = sum.b;
	}
	
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
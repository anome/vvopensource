/*{
	"CREDIT": "by Carter Rosenberg",
	"CATEGORIES": [
		"Utility", "Geometry Adjustment"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "edge_overlap",
			"LABEL": "Edge Overlap",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.1
		},
		{
			"NAME": "edge_softness",
			"LABEL": "Edge Softness",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 0.5,
			"DEFAULT": 0.0
		},
		{
			"NAME": "center_line",
			"LABEL": "Center Line",
			"TYPE": "float",
			"MIN": 0.4,
			"MAX": 0.6,
			"DEFAULT": 0.5
		},
		{
			"NAME": "p",
			"LABEL": "Power Curve",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 5.0,
			"DEFAULT": 1.0
		}
	]
}*/


//	basically based on http://paulbourke.net/texture_colour/edgeblend/


void main() {
	vec2		loc;
	float		blendLevel = 0.0;
	loc = vv_FragNormCoord;

	//	here's how this works
	//	push anything on the left side to the left by center_line and anything on the right to the right by center_line
	//	then apply the gain curve
	
	float dist = abs(center_line - loc.x);
	blendLevel = (dist < edge_softness + center_line) ? pow(dist / (edge_softness + center_line), p) : 1.0;

	loc.x = (loc.x < center_line) ? loc.x + (center_line - 0.5 + edge_overlap) : loc.x - (center_line - 0.5 + edge_overlap);

	if ((loc.x < 0.0)||(loc.y < 0.0)||(loc.x > 1.0)||(loc.y > 1.0))	{
		gl_FragColor = vec4(0.0);
	}
	else	{
		vec4 pix = IMG_NORM_PIXEL(inputImage,loc);
		pix.rgb = pix.rgb * blendLevel;
		gl_FragColor = pix;
	}
}

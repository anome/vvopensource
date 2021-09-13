/*{
	"DESCRIPTION": "trails",
	"CREDIT": "SHELTRON visuals",
	"CATEGORIES": [
		"Distortion Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "edge_radius",
			"TYPE": "float",
			"MIN": 1,
			"MAX": 20,
			"DEFAULT": 5
		},
		{
			"NAME": "decay",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 0.5,
			"DEFAULT": 0.5
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"MIN": 0.9,
			"MAX": 1.1,
			"DEFAULT": 1.0
		},
		{
			"NAME": "offset",
			"TYPE": "float",
			"MIN": -0.01,
			"MAX": 0.01,
			"DEFAULT": 0.0
		},
		{
			"NAME": "rotation",
			"TYPE": "float",
			"MIN": -30,
			"MAX": 30,
			"DEFAULT": 0.0
		},
		{
			"NAME": "rot180",
			"TYPE": "bool"
		},
		{
			"NAME": "invert",
			"TYPE": "bool"
		},
		{
			"NAME": "edge",
			"TYPE": "bool"
		},
		{
			"NAME": "edge_intensity",
			"TYPE": "float",
			"MIN": 0,
			"MAX": 1,
			"DEFAULT": 1
		}
	],
	"PERSISTENT_BUFFERS": [
		"buffer"
	],
	"PASSES": [
		{
			"TARGET":"buffer"
		},
		{
			
		}
	]
	
}*/



float gray(vec4 n)
{
	return (n.r + n.g + n.b)/3.0;
}

vec4 edges(vec2 uv)
{
	vec2 d = edge_radius * 1.0/RENDERSIZE;
	
	vec2 left_coord  = clamp(vec2(uv + vec2(-d.x , 0)),0.0,1.0);
	vec2 right_coord = clamp(vec2(uv + vec2(d.x , 0)),0.0,1.0);
	vec2 above_coord = clamp(vec2(uv + vec2(0,d.y)),0.0,1.0);
	vec2 below_coord = clamp(vec2(uv + vec2(0,-d.y)),0.0,1.0);

	vec2 lefta_coord = clamp(vec2(uv + vec2(-d.x , d.x)),0.0,1.0);
	vec2 rightb_coord = clamp(vec2(uv + vec2(d.x , -d.x)),0.0,1.0);
	
	vec4 colorL = IMG_NORM_PIXEL(buffer, left_coord);
	vec4 colorR = IMG_NORM_PIXEL(buffer, right_coord);
	vec4 colorA = IMG_NORM_PIXEL(buffer, above_coord);
	vec4 colorB = IMG_NORM_PIXEL(buffer, below_coord);

	vec4 colorLA = IMG_NORM_PIXEL(buffer, lefta_coord);
	vec4 colorRB = IMG_NORM_PIXEL(buffer, rightb_coord);

	vec4 final = edge_intensity * (colorR + colorB + colorRB - colorL - colorA - colorLA);

	return final;
}

mat2 rotmat( float deg ) {
	float theta = radians(deg);
	float s = sin(theta);
	float c = cos(theta);

	return mat2(c, -s, s, c);
}

void main()
{
	vec2 uv =  gl_FragCoord.xy /  RENDERSIZE;

	if (PASSINDEX == 0)	{
 		vec2 warp = (uv - 0.5) * zoom ;
		
		warp *= rotmat(rotation + (rot180? 180. : 0.));
		// warp += offset;
		warp += 0.5;
		
		
		gl_FragColor = texture2D(buffer, warp); 
		
		if (edge)
			gl_FragColor += edges(warp);
			
		if ( invert )
			gl_FragColor = 1. - gl_FragColor;

		float scale = 4.;
		
		vec2 texCoord = RENDERSIZE * ((uv - 0.5) * scale + 0.5)/RENDERSIZE.x ;
		gl_FragColor += IMG_NORM_PIXEL(inputImage,texCoord );
		gl_FragColor *= (1. - decay);
;
	}
	else if (PASSINDEX == 1)	{	
		gl_FragColor = IMG_THIS_PIXEL(buffer);
	}
}

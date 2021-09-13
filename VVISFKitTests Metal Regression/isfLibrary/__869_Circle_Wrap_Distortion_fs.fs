/*{
	"DESCRIPTION": "Wraps the video into a circular shape",
	"CREDIT": "VIDVOX",
	"CATEGORIES": [
		"Distortion Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "arcStart",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "arcWidth",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "radiusStart",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "radiusSize",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "radiusWarp",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -4.0,
			"MAX": 4.0
		},
		{
			"NAME": "inputCenter",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		},
		{
			"NAME": "mirror",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "correctAspect",
			"TYPE": "bool",
			"DEFAULT": true
		}
	]
	
}*/


const float tau = 6.28318530718;
const float pi = 3.14159265359;
const float halfpi = 1.57079632679;


void main()	{
	vec4		inputPixelColor;
	if (radiusSize > 0.0)	{
	
		vec2		loc = vv_FragNormCoord.xy;
		vec2		center = inputCenter / RENDERSIZE.xy;
		//	account for aspect ratio so we stay circle
		if (correctAspect)	{
			loc.x = loc.x * RENDERSIZE.x/RENDERSIZE.y;
			center.x = center.x * RENDERSIZE.x/RENDERSIZE.y;
		}
		//	translate from polar coords back to cart for this point
		//		the effect translates (x,y) to (r,theta)
		float		r = clamp(distance(loc,center) - radiusStart,0.0,2.0);
		float		theta = 0.5+(atan(loc.x-center.x,loc.y-center.y))/tau;
		
		theta -= arcStart;
		if ((theta > -arcStart) && (theta <= arcWidth))	{
			theta = mod(theta, 1.0);
			theta = theta / arcWidth;

			//theta = mod(theta / arcWidth,1.0);
			if (r > 0.0)	{
				//theta = mod(theta, 1.0);
				r = clamp(r,0.0,2.0);
				loc = vec2(theta, (r * 2.0)/radiusSize);	
				
				float	bumpWarp = pow(2.0,radiusWarp);
				loc.y = pow(loc.y,bumpWarp);
				
				if ((loc.x < 0.0)||(loc.x > 1.0)||(loc.y < 0.0)||(loc.y > 1.0))
					inputPixelColor = vec4(0.0);
				else	{
					loc.y = 1.0 - loc.y;
					if (mirror)	{
						loc.x = (loc.x < 0.5) ? loc.x * 2.0 : 2.0 - loc.x * 2.0;
					}
					inputPixelColor = IMG_NORM_PIXEL(inputImage, loc);
				}
			}
		}
	}
	gl_FragColor = inputPixelColor;
}

/*{
  "CREDIT": "by solkatt",
  "CATEGORIES": [
    "Color Effect"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "brightColor",
      "TYPE": "color",
      "DEFAULT": [
        1,
        0.9,
        0.8,
        1
      ]
    },
    {
      "NAME": "darkColor",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        1
      ]
    },
    {
      "NAME": "invert",
      "TYPE": "bool"
    },
	{
		"NAME": "shapeWidth",
		"LABEL": "Shape Width",
		"TYPE": "float",
		"DEFAULT": 0.5
	},
	{
		"NAME": "shapeHeight",
		"LABEL": "Shape Height",
		"TYPE": "float",
		"DEFAULT": 0.5
	}
  ]
}*/

const vec4		lumcoeff = vec4(0.299, 0.587, 0.114, 0.0);

void main() {
	vec2		thisPoint = isf_FragNormCoord;
	vec4		color = IMG_THIS_PIXEL(inputImage);
	vec2 		center = vec2(0.5);
	float width = 0.5 - shapeWidth * 0.5;
	float height = 0.5 - shapeHeight * 0.5;
	
	bool insideX = (thisPoint.x < center.x - width || thisPoint.x >= center.x + width);
	bool insideY = (thisPoint.y < center.y - height || thisPoint.y >= center.y + height);
	float inside = (insideX || insideY) ? 1.0 : 0.;
	if(invert)
		inside = 1.0 - inside;
		
	if(inside == 1.0) {
		float luminance = dot(color,lumcoeff);
		color = mix(darkColor, brightColor, luminance);
		
	}
	gl_FragColor = color;
}

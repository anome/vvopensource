/*{
  "CREDIT": "by wilstonoreo",
  "DESCRIPTION": "",
  "CATEGORIES": ["LiCHTPiRATEN"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "tileImage",
      "TYPE": "image"
    },
        {
            "NAME": "num_horz",
            "TYPE": "float",
            "DEFAULT": 50.0,
            "MIN": 1.0,
            "MAX": 100.0
        },
        {
            "NAME": "num_vert",
            "TYPE": "float",
            "DEFAULT": 50.0,
            "MIN": 1.0,
            "MAX": 100.0
        },
        {
            "NAME": "polar",
            "TYPE": "bool",
            "DEFAULT": false
        }
  ]
}*/

const float PI = 3.14159;

void main() {
  vec2 num = vec2(num_horz,num_vert);
  vec2 v = vv_FragNormCoord ;
  vec2 texCoords;
  float l = length(v-0.5);
  if (polar)
  {
    v.x = l;
    v.y = v.y = atan(vv_FragNormCoord.y - 0.5,vv_FragNormCoord.x-0.5);

    texCoords.x = v.x * l * num.x;
    texCoords.y = v.y * l * num.y/PI;
  } else
  {
    texCoords = v*num;
  }

	// Reduce brightness in pixels away from the square center
	vec4 brightness = IMG_NORM_PIXEL(tileImage,fract(texCoords));
  
  if (polar)
  {
    vec2 p;
  float theta = atan(v.x,v.y) ;
  float r = v.x;
  p.s = fract(0.5 * (1.0 + r* sin(theta)*num.x));
  p.t = fract(0.5 * (1.0 + r * cos(theta)*num.x));

  //  gl_FragColor = vec4(p,0.0,1.0);
    gl_FragColor = brightness * IMG_NORM_PIXEL(inputImage,floor(texCoords));
   
  } else
  {
    gl_FragColor = brightness * IMG_NORM_PIXEL(inputImage,floor(texCoords)/num);
  }
}
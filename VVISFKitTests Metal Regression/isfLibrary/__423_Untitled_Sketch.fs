/*
{
  "CATEGORIES" : [
    "Automatically Converted",
    "GLSLSandbox"
  ],
  "INPUTS" : [
  {
      "NAME": "v1",
      "LABEL": "v1",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.5
    },
    {
      "NAME": "v2",
      "LABEL": "v2",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.5
    },
    {
      "NAME": "v3",
      "LABEL": "v3",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.5
    },
    {
      "NAME": "v4",
      "LABEL": "v4",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.628318,
      "DEFAULT": 0.628318
    },
      {
      "NAME": "v5",
      "LABEL": "v5",
      "TYPE": "float",
      "MIN": 0.3,
      "MAX": 0.9,
      "DEFAULT": 0.6
    },
     {
      "NAME": "v6",
      "LABEL": "v6",
      "TYPE": "float",
      "MIN": 0.005,
      "MAX": 0.05,
      "DEFAULT": 0.005
    },
    {
		"NAME": "v7",
		"TYPE": "long",
		"VALUES": [
			2,
			1,
			3
		],
		"LABELS": [
			"cos",
			"sin",
			"abs"
		],
		"DEFAULT": 1
		}
    
    


  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#36696.0"
}
*/

void main( void ) {
	vec2 p = (gl_FragCoord.xy*2.0-RENDERSIZE)/min(RENDERSIZE.x,RENDERSIZE.y);
	float ratio = (RENDERSIZE.x/2.)/(RENDERSIZE.y);
	vec3 destColor = vec3(v5);
	float f = 0.1;
	for(float i = 0.0; i < 10.0; i++){
        	float s = sin(i * v4) * v2 * sin(TIME);
        	float c = cos(i * v4) * v3 * sin(TIME);
		f += v6 / abs(length(p + vec2(c, s)) - v1);
	}
	gl_FragColor = vec4(destColor*f, 1.0);
}
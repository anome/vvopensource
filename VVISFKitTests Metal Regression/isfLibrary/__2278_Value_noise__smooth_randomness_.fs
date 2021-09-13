/*{
	"CREDIT": "by isadoratelesdecastro",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
      "NAME": "offset",
      "TYPE": "point2D",
      "DEFAULT": [ 0.5, 0.5 ],
      "MAX": [ 10, 10 ],
      "MIN": [ -10, -10 ]
    },
    {
      "NAME": "colorA",
      "TYPE": "color",
      "DEFAULT": [
        0.0,
        1.0,
        0.0,
        1.0
      ]
    },
    {
      "NAME": "colorB",
      "TYPE": "color",
      "DEFAULT": [
        0.0,
        1.0,
        0.0,
        1.0
      ]
    },
    {
      "NAME": "range",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 100.0
    },
     {
      "NAME": "range2",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 100.0
    }
	]
}*/
float random (in vec2 st) 
{
    return fract(sin(dot(st.xy, offset))*43758.5453123*TIME*0.00001);
}

float smoothRandomness(in vec2 st)
{
	vec2 i = floor(st);
	vec2 f = fract(st);
	
	// Four corners in 2D of a tile
    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));

    // Smooth Interpolation

    // Cubic Hermine Curve.  Same as SmoothStep()
    //vec2 u = f*f*(3.0-2.0*f);
    vec2 u = smoothstep(0.,1.,f);

    // Mix 4 coorners porcentages
    return mix	(a, b, u.x) +  
    			(c - a)* u.y * (1.0 - u.x) + 
    			(d - b) * u.x * u.y;
}

void main() 
{
	vec2 st = gl_FragCoord.xy / RENDERSIZE.xy;
	
	// Scale the coordinate system to see
    // some noise in action
    vec2 pos = vec2(st*range);
    
    // Use the noise function
    float n = smoothRandomness(pos);
    
    //composition
    vec3 newColor = vec3(n)*colorA.rgb;
    
     vec2 pos2 = vec2(st*range2);
     
    // Use the noise function
    float m = smoothRandomness(pos2);
    
    //composition
    vec3 newColor2 = vec3(m)/colorB.rgb;
    

    gl_FragColor = vec4(newColor/newColor2, 1.0);
}










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
      "MIN": [ 0, 0 ]
    },
		{
      "NAME": "offset2",
      "TYPE": "point2D",
      "DEFAULT": [ 0.5, 0.5 ],
      "MAX": [ 100, 100 ],
      "MIN": [ 0, 0 ]
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
    },
    {
      "NAME": "range3",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 100.0
    }
	]
}*/
float random (in vec2 st) 
{
    return fract(sin(dot(st.xy, offset))*43758.5453123);
}

vec2 random2(vec2 st)
{
    st = vec2( dot(st,vec2(197.1,311.7)), dot(st,vec2(269.5,183.3)) );
    return -1.0 + 2.0*fract(sin(st)*43758.5453123);
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

float gradientNoise(vec2 st) 
{
    vec2 i = floor(st);
    vec2 f = fract(st);

	// Cubic Hermine Curve.  Same as SmoothStep()
    vec2 u = f*f*(3.0-2.0*f);
    //vec2 u = st*st*st*(st*(st*6.-15.)+10.);

	
    return mix( mix( dot( random2(i + vec2(0.0,0.0) ), f - vec2(0.0,0.0) ), 
                     dot( random2(i + vec2(1.0,0.0) ), f - vec2(1.0,0.0) ), u.x),
                mix( dot( random2(i + vec2(0.0,1.0) ), f - vec2(0.0,1.0) ), 
                     dot( random2(i + vec2(1.0,1.0) ), f - vec2(1.0,1.0) ), u.x), u.y);
}

void callSmoothRandom() 
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

void main()
{
	
	vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    st.x += (RENDERSIZE.x*TIME*0.004)/(RENDERSIZE.y);
    vec3 colorBack = vec3(0.0);
    vec3 colorFront = vec3(0.0);
    vec3 color = vec3(0.0);


    vec2 pos = vec2(st*range);

    colorBack = vec3( gradientNoise(pos*offset-TIME*0.1)*range2)*colorB.rgb;
    colorFront = vec3( gradientNoise(pos*offset2))*range3*colorA.rgb;
    color = colorBack+colorFront;

    gl_FragColor = vec4(color,1.0);
}









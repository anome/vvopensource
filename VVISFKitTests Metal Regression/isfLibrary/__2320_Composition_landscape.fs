/*{
	"CREDIT": "by isadoratelesdecastro",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": 
	[
	 {
      "NAME": "BlendingLimit",
      "TYPE": "float",
      "DEFAULT": 0.6,
      "MIN": 0.0,
      "MAX": 1.0
    },
	{
		"NAME": "rampControl",
		"TYPE": "float",
		"DEFAULT": 0.5,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
      	"NAME": "scaleSky_A",
      	"TYPE": "point2D",
      	"DEFAULT": [ 4.0, 9.0 ],
      	"MAX": [ 10, 10 ],
      	"MIN": [ 0, 0 ]
    },
    {
      	"NAME": "scaleSky_B",
      	"TYPE": "point2D",
      	"DEFAULT": [ 2, 3 ],
      	"MAX": [ 10, 10 ],
      	"MIN": [ 0, 0 ]
    },
    {
      "NAME": "range",
      "TYPE": "float",
      "DEFAULT": 1.59,
      "MIN": 0.0,
      "MAX": 2.0
    },
     {
      "NAME": "range2",
      "TYPE": "float",
      "DEFAULT": 1.68,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "range3",
      "TYPE": "float",
      "DEFAULT": 1.8,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "colorSky_A",
      "TYPE": "color",
      "DEFAULT": 
      [
        0.3,
        0.6,
        0.8,
        1.0
      ]
    },
    {
      "NAME": "colorSky_B",
      "TYPE": "color",
      "DEFAULT": 
      [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
    {
      "NAME": "rangeWater_A",
      "TYPE": "float",
      "DEFAULT": 13.0,
      "MIN": 0.0,
      "MAX": 100.0
    },
    {
      "NAME": "rangeWater_B",
      "TYPE": "float",
      "DEFAULT": 5.4,
      "MIN": 0.0,
      "MAX": 100.0
    },
    {
      "NAME": "rangeWater_C",
      "TYPE": "float",
      "DEFAULT": 40.77,
      "MIN": 0.0,
      "MAX": 100.0
    },
    {
      "NAME": "colorWater_A",
      "TYPE": "color",
      "DEFAULT": 
      [
        0.15,
        0.35,
        0.5,
        1.0
      ]
    },
    {
      "NAME": "colorWater_B",
      "TYPE": "color",
      "DEFAULT": 
      [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
    {
      	"NAME": "offset_Water",
      	"TYPE": "point2D",
      	"DEFAULT": [ 1.0, 4.0 ],
      	"MAX": [ 10, 10 ],
      	"MIN": [ 0, 0 ]
    },
    {
      	"NAME": "offset_Water2",
      	"TYPE": "point2D",
      	"DEFAULT": [ 1.0, 3.0 ],
      	"MAX": [ 10, 10 ],
      	"MIN": [ 0, 0 ]
    }
	]
}*/
float random (in vec2 st) 
{
    return fract(sin(dot(st.xy, vec2(197.1,311.7)))*43758.5453123);
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

vec2 random2(vec2 st)
{
    st = vec2( dot(st,vec2(197.1,311.7)), dot(st,vec2(269.5,183.3)) );
    return -1.0 + 2.0*fract(sin(st)*43758.5453123);
}

float gradientNoise(vec2 st) 
{
    vec2 i = floor(st);
    vec2 f = fract(st);

	// Cubic Hermine Curve.  Same as SmoothStep()
    vec2 u = f*f*(3.0-2.0*f);

	
    return mix( mix( dot( random2(i + vec2(0.0,0.0) ), f - vec2(0.0,0.0) ), 
                     dot( random2(i + vec2(1.0,0.0) ), f - vec2(1.0,0.0) ), u.x),
                mix( dot( random2(i + vec2(0.0,1.0) ), f - vec2(0.0,1.0) ), 
                     dot( random2(i + vec2(1.0,1.0) ), f - vec2(1.0,1.0) ), u.x), u.y);
}
/*
vec4 callRamp()
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec3 pct = vec3(uv.y);
	
	return mix(colorA_Gradient, colorB_Gradient, uv.y*rampControl);
}
*/
vec3 callSky(vec2 st)
{
	
    st.x += (RENDERSIZE.x*TIME*0.004)/(RENDERSIZE.y);
    vec3 colorBack = vec3(1.0);
    vec3 colorFront = vec3(1.0);
    vec3 color = vec3(1.0);

    vec2 pos = vec2(st*range);
	
    colorBack = vec3( (gradientNoise(pos*scaleSky_A-TIME*0.2)*range2)+0.5)/**colorSky_B.rgb*/;
    colorFront = vec3( (gradientNoise(pos*scaleSky_B)*range3)+0.5);
    color = mix(colorBack, colorFront, 0.5);
    color = mix(colorSky_A.rgb, colorSky_B.rgb, color);
    return color;
}

vec4 callWater()
{
	
	vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
	
    st.x += (RENDERSIZE.x*TIME*0.004)/(RENDERSIZE.y);
    vec3 colorBack 	= vec3(0.0);
    vec3 colorFront = vec3(0.0);
    vec3 color 	= vec3(0.0);

    vec2 pos 	= vec2(st*rangeWater_A);
		colorBack 	= vec3( gradientNoise(pos*offset_Water-TIME*0.4)+0.5)/*colorWater_A.rgb*/;
    	colorFront 	= vec3( gradientNoise(pos*offset_Water2+TIME*0.1)+0.5)/*colorWater_B.rgb*/;
    	color = mix(colorBack, colorFront, 0.5);
    	color = mix(colorWater_A.rgb, colorWater_B.rgb, color);
    
    	return vec4(color,1.0);

}
void main()
{
	vec2 mySt = gl_FragCoord.xy/RENDERSIZE.xy;
	//vec4 ramp = callRamp();
	vec4 sky1 = vec4(callSky(mySt), 1.0);
	//vec4 sky2 = ramp+sky1;
	vec4 water = callWater();
	
	float depth = smoothstep(1.0,0.5,mySt.y);
    depth *= smoothstep(0.0,1.0,mySt.y);
    
	float blend = smoothstep(BlendingLimit,0.0,mySt.y);

	//vec4 finalColor = mix((water*ramp*sky1), sky2, pow(mySt.y, 2.));
	//vec4 finalColor = mix((water*ramp*sky1), sky2, mySt.y);
	//vec4 finalColor = (ramp+sky)+water;
	//float limit = step(0.5, mySt.y);
	
	//gl_FragColor = vec4(mix(water.xyz+depth,sky1.xyz+depth*0.5,limit), 1.0);
	
	vec4 merge = mix(sky1, water, blend);
	gl_FragColor = merge;
}








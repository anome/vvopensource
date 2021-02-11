/*{
    "CREDIT": "by Anomes",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "quality",
            "TYPE": "long",
            "VALUES": [
                0,
                1,
                2
            ],
            "LABELS": [
                "Low",
                "Medium",
                "High"
            ],
            "DEFAULT": 0,
        },
        {
            "NAME": "strength",
            "TYPE": "float",
            "MIN": -100.0,
            "MAX": 100.0,
            "DEFAULT": 0.25
        },
        {
            "NAME": "center",
            "TYPE": "point2D",
            "DEFAULT": [0.5,0.5]
        }
    ],
}*/




float random(vec3 scale,float seed)
{
    return fract(  sin(dot(gl_FragCoord.xyz+seed,scale))*43758.5453  +  seed  );
}


void main()
{
    float iterations = float(quality+1)*25.;
    vec2 pos = gl_FragCoord.xy / RENDERSIZE;
    vec4 color = vec4(0.);
    float total = 0.;
    vec2 toCenter = center - pos;
    float offset = random(  vec3(12.9898,78.233,151.7182), 0.  );
    for(float t=0.; t<=iterations; t++)
    {
        float percent = (t+offset)/iterations;
        float weight = 4.0*(percent-percent*percent);
         // MODIFICATION for SPIRV: variable named 'sample' is forbiden. Renamed 'sampleVariable'
        vec4 sampleVariable = IMG_NORM_PIXEL(inputImage, pos+toCenter*percent*strength);
        sampleVariable.rgb *= sampleVariable.a;
        color += sampleVariable*weight;
        total += weight;
    }
    gl_FragColor = color/total;
    gl_FragColor.rgb /= max(gl_FragColor.a,0.00001);
}


/*{
  "CREDIT": "by isak.burstrom",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "XXX"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "floatInput",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 3
    },
    {
      "NAME": "dist",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 1
    }
  ]
}*/

void main() {
	float unit =  1.0 / min (RENDERSIZE.x, RENDERSIZE.y);
    float t = floatInput * 2.;
    vec4 o;
	vec2 uv = (2.0*gl_FragCoord.xy - RENDERSIZE.xy) * unit;
    vec3 ro = vec3(0., 0., 3.);
    vec3 rd = normalize(vec3(uv.x, uv.y, -1.));
    float d = 0.;
    vec3 p;
    o=vec4(0.);
    
    for(int i=0;i<25;i++)
    {
        p = ro + rd * d + vec3(0.,0.,-3.*t);
        float s = length(mod(p, 6.)-3.)-.5;
        d+= dist * s;
        if (s<.01)
            o += vec4(dot(mod(p,6.)-3., -vec3(-.607, -.1, -.607)) * vec3(0.2, 0.1, .3), 1.);
        
    }
	gl_FragColor = o;
}
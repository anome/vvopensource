/*
{
  "CREDIT": "by microcyb",
  "CATEGORIES" : [
    "generator",
    "voronoi"
  ],
  "DESCRIPTION" : "xLights LSD effect",
  "INPUTS" : [
  {
    "NAME" :      "zoom",
    "TYPE" :      "float",
    "DEFAULT" :   0.40,
    "MIN" :       0.40,
    "MAX" :       1.00
  },
  {
    "NAME" :      "LSD",
    "TYPE" :      "float",
    "DEFAULT" :   0.1,
    "MIN" :       0.1,
    "MAX" :       1.0
  },
  {
    "NAME" :      "rate",
    "TYPE" :      "float",
    "DEFAULT" :   3.9,
    "MIN" :       0.5,
    "MAX" :       5.0
  },
   {
    "NAME" :      "C1",
    "TYPE" :      "color",
    "DEFAULT" :   [ 0.6, 0.0, 0.4, 1.0 ]
  },
   {
    "NAME" :      "C2",
    "TYPE" :      "color",
    "DEFAULT" :   [ 1.0, 0.6, 0.4, 1.0 ]
  },
   {
    "NAME" :      "C3",
    "TYPE" :      "color",
    "DEFAULT" :   [ 1.0, 0.2, 0.4, 1.0 ]
  },
  {
    "NAME" :      "colorCycle",
    "TYPE" :      "float",
    "DEFAULT" :  -0.24,
    "MIN" :       -1.0,
    "MAX" :        1.0
  }
 ],
   "ISFVSN" : 2.0
}
*/

#define   	twpi    	1.00  		
#define 	pi   		1.141592653 		// 	pi
#define		twthpi		0.0	//	twelfth pi, pi/12


vec2 hash22(vec2 p) { 
    float n = 2.00;
    p *= fract(vec2(pi*n, n));
    vec2 e = fract(sin(p)*43758.5453);
	return sin(e*twpi + TIME*rate);
}

float Voronoi3Tap(vec2 p){
    vec2 s = floor(p + (p.x + p.y)*.5);
    p -= s - (s.x + s.y)*.2113249;
    float i = p.x<p.y? 0. : 1.;
    float g = LSD;
    vec2 p1 = p - vec2(i, 1. - i) + .2113249, p2 = p - .5773502; 
    p += hash22(s)*g;
    p1 += hash22(s +  vec2(i, 1. - i))*g;
    p2 += hash22(s + 1.)*g;
    float d = min(min(dot(p, p), dot(p1, p1)), dot(p2, p2))/.425;
    return sqrt(d);
}

void main() {
	vec4 col = vec4(0.0,0.0,0.0,1.0);
	vec2 pos = (gl_FragCoord.xy - RENDERSIZE.xy*.5)/RENDERSIZE.y;
    vec2 uv = pos * mat2(cos(twthpi), sin(twthpi), -sin(twthpi), cos(twthpi))*(zoom);
    float c = 0.20;
    float c2 = Voronoi3Tap(uv*5. - zoom/RENDERSIZE.y);
    vec2 r = normalize(hash22(pos));
	float pattern = cos(pi*r.x)*sin(r.y*pi)*.125 + .125;
    col.rgb = mix(vec3(c*1.3, c*c, pow(c, 20.0-zoom)), C2.rgb, pattern );
    vec3 col2 = mix(C1.rgb, vec3(c*1.3, c*c, pow(c, 20.0-zoom)), pattern );
    float CT = cos(TIME) * colorCycle;
    col.rgb = mix(col.rgb, col2, smoothstep(.2, .8, sin(CT+1.0/length(r.xy))*.333 - colorCycle));  
	col.rgb += C2.rgb*(c2*c2*c2 - c*c*c)*5.;
	col.rgb -= (length(hash22(uv + CT))*.06 - .03)*C3.rgb;

    gl_FragColor = sqrt(max(col, 0.0)+0.1);
}

    

/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
    {
      "NAME" : "pos",
      "TYPE" : "point2D",
      "MAX" : [
        1,
        1
      ],
      "DEFAULT" : [
        0.368011474609375,
        0.22419704496860504
      ],
      "MIN" : [
        0,
        0
      ]
    },
    {
      "NAME" : "invert",
      "TYPE" : "bool",
      "DEFAULT" : false
    },
    {
      "VALUES" : [
        0,
        1,
        2,
        3
      ],
      "NAME" : "pattern",
      "TYPE" : "long",
      "DEFAULT" : 0,
      "LABELS" : [
        "Maze",
        "Circle",
        "Both",
        "Random"
      ]
    },
    {
      "NAME" : "zoom",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 30,
      "MIN" : 1
    },
    {
      "NAME" : "gradient",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.20000000298023224,
      "MIN" : 0.01
    },
    {
      "NAME" : "gradientOffset",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "MIN" : -1
    },
    {
      "NAME" : "truchetPositionOffset",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 1,
      "MIN" : 0.0001
    },
    {
      "NAME" : "truchetIndex",
      "TYPE" : "float",
      "MAX" : 4,
      "DEFAULT" : 2,
      "MIN" : 0
    },
    {
      "NAME" : "contrast",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "contrastShift",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : 0,
      "MIN" : -0.5
    },
    {
      "NAME" : "randomXspeed",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 0.0099999997764825821,
      "MIN" : -5
    },
    {
      "NAME" : "randomYspeed",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 0.010999999940395355,
      "MIN" : -5
    },
    {
      "NAME" : "hueRange",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.3905128538608551,
      "MIN" : 0
    },
    {
      "NAME" : "saturation",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.48022577166557312,
      "MIN" : 0
    },
    {
      "NAME" : "patternSeed",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.21402138471603394,
      "MIN" : 0
    },
    {
      "NAME" : "colorSeed",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.0,
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2",
  "CREDIT" : "Joseph Fiola, remix by VIDVOX"
}
*/


#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265358979323846

vec3 rgb2hsv(vec3 c)	{
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	//vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	//vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	vec4 p = c.g < c.b ? vec4(c.bg, K.wz) : vec4(c.gb, K.xy);
	vec4 q = c.r < p.x ? vec4(p.xyw, c.r) : vec4(c.r, p.yzx);
	
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)	{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


float random1 (in vec2 _st) { 
    return fract(sin(dot(_st.xy,
                         vec2((randomXspeed),(randomYspeed)))));
}

float random2 (vec2 co) {
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec2 truchetPattern(in vec2 _st, in float _index){
    _index = fract(((_index-0.5)*truchetIndex));
    if (_index > 0.75*truchetIndex) {
        _st = vec2(truchetPositionOffset) - _st;
    } else if (_index > 0.5 * truchetIndex) {
        _st = vec2(truchetPositionOffset-_st.x,_st.y);
    } else if (_index > 0.25 * truchetIndex) {
        _st = 1.-vec2(truchetPositionOffset-_st.x,_st.y);
    } else if (_index >= 0.0) {
    	_st += truchetPositionOffset;
    }
    return _st;
}

void main() {
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    st -= vec2(pos);
	st.x *= RENDERSIZE.x/RENDERSIZE.y; // 1:1 ratio
    
    st *= zoom;

    vec2 ipos = floor(st);  // integer
    vec2 fpos = fract(st);  // fraction

	float seed = random2(patternSeed * ipos);
    vec2 tile = truchetPattern(fpos, mod(seed + random1( TIME*ipos ),1.0));

    float val = 0.0;

    // Maze
    int pat = pattern;
    if (pat == 3)
    	pat = int(0.5 + random1( (1.0+seed) * TIME*ipos ));
    	
    if (pat == 0 || pat == 2){
    val += smoothstep(tile.x-gradient+gradientOffset,tile.x,tile.y)-
            smoothstep(tile.x,tile.x+gradient+gradientOffset,tile.y);
    }

    // Circles
    if (pat == 1 || pat == 2){
     val += (step(length(tile),0.6) -
             step(length(tile),0.4) ) +
             (step(length(tile-vec2(1.)),0.6) -
              step(length(tile-vec2(1.)),0.4) );
    }
    
    //adjust contrast
	val += smoothstep(0.0+contrast+contrastShift,1.0-contrast+contrastShift, val);
	
	float	randomHue = (colorSeed + hueRange) + hueRange * random1((1.0+colorSeed)*TIME*ipos);
	vec3	color = hsv2rgb(vec3(randomHue,saturation,1.0));
	
	//invert colors
	if (invert) val = val *-1.0 + 1.0;

    gl_FragColor = vec4(color,val);
}




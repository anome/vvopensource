/*{
	"DESCRIPTION": "z",
	"CREDIT": "by z",
	"ISFVSN": "2.0",
	"CATEGORIES": [
		"TEST-GLSL FX"
	],
	"INPUTS": [
		{
			"NAME": "threshold",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0
		},
		{
			"NAME": "time_",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MAX": 1000.0
		},
		{
			"NAME": "spotSeed",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MAX": 1000.0
		},
		{
			"NAME": "colorShift_",
			"TYPE": "float",
			"DEFAULT": 0.02,
			"MAX": 1.0
		},
		{
			"NAME": "spotRadius",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 2.0
		},
		{
			"NAME": "spotDetails",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 50.0
		},
		{
			"NAME": "spotAmplitude",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 5.0
		},
		{
			"NAME": "blur",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MAX": 1.5
		}
	]
}*/

#define pointsNumber 10

float WaveletNoise(vec2 p, float z, float k) {
    float d=0.,s=1.,m=0., a;
    for(float i=0.; i<4.; i++) {
        vec2 q = p*s, g=fract(floor(q)*vec2(123.34,233.53));
    	g += dot(g, g+23.234);
		a = fract(g.x*g.y)*1e3;// +z*(mod(g.x+g.y, 2.)-1.); // add vorticity
        q = (fract(q)-.5)*mat2(cos(a),-sin(a),sin(a),cos(a));
        d += sin(q.x*10.+z)*smoothstep(.25, .0, dot(q,q))/s;
        p = p*mat2(.54,-.84, .84, .54)+i;
        m += 1./s;
        s *= k; 
    }
    return d/m + 0.5;
}

vec3 colorShift = vec3(0., colorShift_, colorShift_ * 2.);

float noise(float x, float y) {
    return WaveletNoise(vec2(x, y), 1., 0.5);
}

struct Point
{
  float mass;
  vec3 posX; // for rgb
  vec3 posY;
};

void main()
{
    // Point points[3] = Point[3](
    //   Point(1.0,  vec2(-19.0, 4.5)),
    //   Point(-3.0, vec2(2.718, 2.0)),
    //   Point(29.5, vec2(3.142, 3.333))
    // );

    Point points[pointsNumber];
    for (int i = 0; i < pointsNumber; i++) {
        float mass = 0.1;
        vec3 cs = colorShift;
        
        vec3 posX; // for rgb
        vec3 posY;
        posX.r = .25+.5*noise(
            10. + 100. * float(i),
            1.+ 600. + (time_ + TIME) * 0.1 + cs.r
        );
        posY.r = .25+.5*noise(
            1. + 400. * float(i),
            10.+ 200. + (time_ + TIME) * 0.1 + cs.r
        );
        posX.g = .25+.5*noise(
            10. + 100. * float(i),
            1.+ 600. + (time_ + TIME) * 0.1 + cs.g
        );
        posY.g = .25+.5*noise(
            1. + 400. * float(i),
            10.+ 200. + (time_ + TIME) * 0.1 + cs.g
        );
        posX.b = .25+.5*noise(
            10. + 100. * float(i),
            1.+ 600. + (time_ + TIME) * 0.1 + cs.b
        );
        posY.b = .25+.5*noise(
            1. + 400. * float(i),
            10.+ 200. + (time_ + TIME) * 0.1 + cs.b
          );
          
        points[i] = Point(mass * 100., posX, posY);
    }

    vec2 xy = gl_FragCoord.xy / vec2(min(RENDERSIZE.x, RENDERSIZE.y));
    xy.x += 1. / 2.;
    xy.x -= (RENDERSIZE.x / RENDERSIZE.y) / 2.;
    // vec2 xy = isf_FragNormCoord;
    
    float field = 9999.;
    for (int i = 0; i < pointsNumber; i++) {
        float field2 = 50.*distance(vec2(points[i].posX.r, points[i].posY.r), xy);
        if(field2 < field) field = field2;
    }
    
    // if(field < 20.) {
    //     field *= 0.;
    // }
    if (field > 10.) {gl_FragColor = vec4(1.); return;}
    if (field < 1.) {gl_FragColor = vec4(0,0,0,1); return;}
   
    
    // vec3 field = vec3(1.);
    // for (int i = 0; i < pointsNumber; i++) {
    //     field.r *= points[i].mass * 1. / distance(vec2(points[i].posX.r, points[i].posY.r), xy);
    //     field.g *= points[i].mass * 1. / distance(vec2(points[i].posX.g, points[i].posY.g), xy);
    //     field.b *= points[i].mass * 1. / distance(vec2(points[i].posX.b, points[i].posY.b), xy);
    // }
    
    // vec2 spotDistort;
    // spotDistort.x = WaveletNoise(xy + vec2(0., 100. + spotSeed), 1., spotDetails);
    // spotDistort.y = WaveletNoise(xy + vec2(100., 0. + spotSeed), 1., spotDetails);
    // spotDistort *= spotAmplitude;
    // float k = mix(1., -1., smoothstep(spotRadius - blur, spotRadius, distance(vec2(0.5), xy + spotDistort)));
    // field = field * k;

    vec3 color = vec3(step(.9-field/10., fract(field-TIME)));
    
    // color = mix(color, 1. - color, smoothstep(spotRadius - blur, spotRadius, distance(vec2(0.5), xy)));

    // if (distance(point1, xy) < 0.01) color = vec3(1., 0., 0.);
    // if (distance(point2, xy) < 0.01) color = vec3(1., 0., 0.);
    // if (distance(point3, xy) < 0.01) color = vec3(1., 0., 0.);
    
	gl_FragColor = vec4(color, 1.);
}

/*{
	"DESCRIPTION": "demonstrates an invert image filter",
	"CREDIT": "by VIDVOX",
	"ISFVSN": "2.0",
	"CATEGORIES": [
		"TEST-GLSL FX"
	],
	"INPUTS": [
		{
			"NAME": "threshold",
			"TYPE": "float",
			"DEFAULT": 0.4
		},
		{
			"NAME": "pointsNumber",
			"TYPE": "float",
			"DEFAULT": 6.0,
			"MAX": 100
		}
	]
}*/

#define pointsNumberMax 100

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

vec3 colorShift = vec3(0., .01, .02);

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

    Point points[pointsNumberMax];
    for (int i = 0; i < pointsNumberMax; i++) {
        if (float(i) >= pointsNumber) {
            break;
        }
        vec3 posX; // for rgb
        vec3 posY;
        posX.r = noise(
            10. + 100. * float(i),
            1.+ 600. + TIME + colorShift.r
        );
        posY.r = noise(
            1. + 400. * float(i),
            10.+ 200. + TIME + colorShift.r
        );
        posX.g = noise(
            10. + 100. * float(i),
            1.+ 600. + TIME + colorShift.g
        );
        posY.g = noise(
            1. + 400. * float(i),
            10.+ 200. + TIME + colorShift.g
        );
        posX.b = noise(
            10. + 100. * float(i),
            1.+ 600. + TIME + colorShift.b
        );
        posY.b = noise(
            1. + 400. * float(i),
            10.+ 200. + TIME + colorShift.b
          );
          
        float mass = noise(
            10. + 400. * float(i),
            1.+ 800. + TIME + colorShift.b
          );
        points[i] = Point(mass * 100., posX, posY);
    }

    vec2 xy = gl_FragCoord.xy / RENDERSIZE.y;
    xy.x += 0.5;
    xy.x -= 0.5 * RENDERSIZE.x / RENDERSIZE.y;
    
    vec3 field = vec3(0.);
    for (int i = 0; i < pointsNumberMax; i++) {
        if (float(i) >= pointsNumber) {
            break;
        }
        field.r += 0.0001 * points[i].mass / 
            pow(distance(vec2(points[i].posX.r, points[i].posY.r), xy), 2.);
        field.g += 0.0001 * points[i].mass / 
            pow(distance(vec2(points[i].posX.g, points[i].posY.g), xy), 2.);
        field.b += 0.0001 * points[i].mass / 
            pow(distance(vec2(points[i].posX.b, points[i].posY.b), xy), 2.);
    }
    
    vec3 abberation = vec3(0., .01, .02);
    float th = pointsNumber * threshold / 4.;// (pointsNumber * pointsNumber);
    float dth = pointsNumber * 0.04;
    vec3 color = vec3(smoothstep(th-dth, th+dth, vec3(field
    )));
    
    color = mix(color, 1. - color, smoothstep(0.4, 0.5, distance(vec2(0.5), xy)));

    // if (distance(point1, xy) < 0.01) color = vec3(1., 0., 0.);
    // if (distance(point2, xy) < 0.01) color = vec3(1., 0., 0.);
    // if (distance(point3, xy) < 0.01) color = vec3(1., 0., 0.);
    
	gl_FragColor = vec4(color, 1.);
}
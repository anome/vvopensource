/*
{
  "CATEGORIES" : [
    "TEST-GLSL FX"
  ],
  "DESCRIPTION" : "z",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "speed",
      "TYPE" : "float",
      "DEFAULT" : 1.0,
      "MIN" : 0.0,
      "MAX" : 10.0
    },
    {
      "NAME" : "loop_duration",
      "TYPE" : "float",
      "DEFAULT" : 0.2,
      "MAX" : 10.0
    },
    {
      "NAME" : "threshold",
      "TYPE" : "float",
      "DEFAULT" : 0.0,
      "MIN" : -1.0
    },
    {
      "NAME" : "seed",
      "TYPE" : "float",
      "MAX" : 1000,
      "DEFAULT" : 0.0
    },
    {
      "NAME" : "spotSeed",
      "TYPE" : "float",
      "MAX" : 1000,
      "DEFAULT" : 0.0
    },
    {
      "NAME" : "colorShift_",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.019999999552965164
    },
    {
      "NAME" : "spotRadius",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.5
    },
    {
      "NAME" : "spotDetails",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 0.5
    },
    {
      "NAME" : "spotAmplitude",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 0.5
    },
    {
      "NAME" : "blur",
      "TYPE" : "float",
      "MAX" : 1.5,
      "DEFAULT" : 0.10000000149011612
    },
    {
      "NAME" : "image",
      "TYPE" : "image",
      "LABEL" : "image"
    }
  ],
  "CREDIT" : "by z"
}
*/

#define pointsNumber 8

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

	vec2 loop = vec2(
		loop_duration * sin(TIME * speed / loop_duration),
		loop_duration * cos(TIME * speed / loop_duration)
	);

    Point points[pointsNumber];
    for (int i = 0; i < pointsNumber; i++) {
        float mass = noise(
            10. + 400. * float(i) + loop.x,
            1.+ 800. + colorShift.b + loop.y + seed
          );
        mass -= 0.5;
        vec3 cs = colorShift;
        
        vec3 posX; // for rgb
        vec3 posY;
        posX.r = noise(
            10. + 100. * float(i) + loop.x,
            1.+ 600. + cs.r + loop.y + seed
        );
        posY.r = noise(
            1. + 400. * float(i) + loop.x,
            10.+ 200. + cs.r + loop.y + seed
        );
        posX.g = noise(
            10. + 100. * float(i) + loop.x,
            1.+ 600. + cs.g + loop.y + seed
        );
        posY.g = noise(
            1. + 400. * float(i) + loop.x,
            10.+ 200. + cs.g + loop.y + seed
        );
        posX.b = noise(
            10. + 100. * float(i) + loop.x,
            1.+ 600. + cs.b + loop.y + seed
        );
        posY.b = noise(
            1. + 400. * float(i) + loop.x,
            10.+ 200. + cs.b + loop.y + seed
          );
          
        points[i] = Point(mass * 100., posX, posY);
    }

    vec2 xy = gl_FragCoord.xy / vec2(min(RENDERSIZE.x, RENDERSIZE.y));
    xy.x += 1. / 2.;
    xy.x -= (RENDERSIZE.x / RENDERSIZE.y) / 2.;
    // vec2 xy = isf_FragNormCoord;
    
    vec3 field = vec3(0.);
    for (int i = 0; i < pointsNumber; i++) {
        field.r += 0.0001 * points[i].mass / 
            pow(distance(vec2(points[i].posX.r, points[i].posY.r), xy), 2.);
        field.g += 0.0001 * points[i].mass / 
            pow(distance(vec2(points[i].posX.g, points[i].posY.g), xy), 2.);
        field.b += 0.0001 * points[i].mass / 
            pow(distance(vec2(points[i].posX.b, points[i].posY.b), xy), 2.);
    }
    
    // vec3 field = vec3(1.);
    // for (int i = 0; i < pointsNumber; i++) {
    //     field.r *= points[i].mass * 1. / distance(vec2(points[i].posX.r, points[i].posY.r), xy);
    //     field.g *= points[i].mass * 1. / distance(vec2(points[i].posX.g, points[i].posY.g), xy);
    //     field.b *= points[i].mass * 1. / distance(vec2(points[i].posX.b, points[i].posY.b), xy);
    // }
    
    vec2 spotDistort;
    spotDistort.x = WaveletNoise(xy + vec2(0., 100. + spotSeed) + loop, 1., spotDetails);
    spotDistort.y = WaveletNoise(xy + vec2(100., 0. + spotSeed) + loop, 1., spotDetails);
    spotDistort *= spotAmplitude;
    float k = mix(1., -1., smoothstep(spotRadius - blur, spotRadius, distance(vec2(0.5), xy + spotDistort)));
    field = field * k;

    vec3 abberation = vec3(0., .01, .02);
    vec3 color = vec3(smoothstep(threshold-blur, threshold+blur, vec3(field
    )));
    
    // color = mix(color, 1. - color, smoothstep(spotRadius - blur, spotRadius, distance(vec2(0.5), xy)));

    // if (distance(point1, xy) < 0.01) color = vec3(1., 0., 0.);
    // if (distance(point2, xy) < 0.01) color = vec3(1., 0., 0.);
    // if (distance(point3, xy) < 0.01) color = vec3(1., 0., 0.);
    
    vec3 img = IMG_NORM_PIXEL(image, xy).rgb;
    color = img + color - 2. * color * img;
    
	gl_FragColor = vec4(color, 1.);
}

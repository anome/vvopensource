/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [],
  "INPUTS": [
    {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ],
      "NAME": "mouse",
      "TYPE": "point2D"
    },
    {
      "MAX": -1.0,
      "MIN": -3.13,
      "DEFAULT": -3.0,
      "NAME": "cubesize",
      "TYPE": "float"
    },
    {
      "MAX": 20,
      "MIN": 3,
      "DEFAULT": 4.35,
      "NAME": "vanishingpoint",
      "TYPE": "float"
    },
    {
      "MAX": 0.9,
      "MIN": 0.1,
      "DEFAULT": 0.15,
      "NAME": "brightness",
      "TYPE": "float"
    },
    {
      "MAX": 2,
      "MIN": 0.25,
      "DEFAULT": 0.56,
      "NAME": "rate",
      "TYPE": "float"
    },
    
    {
      "MAX": 2.5,
      "MIN": -2.25,
      "DEFAULT": 0.1,
      "NAME": "stretchx",
      "TYPE": "float"
    },
    {
      "MAX": 2.5,
      "MIN": -0.35,
      "DEFAULT": -0.5,
      "NAME": "stretchy",
      "TYPE": "float"
    },
    {
      "MAX": 1.15,
      "MIN": -0.02,
      "DEFAULT": -0.08,
      "NAME": "stretchz",
      "TYPE": "float"
    },
    
    {
      "MAX": 1.0,
      "MIN": 0.0,
      "DEFAULT": 0.5,
      "NAME": "RED",
      "TYPE": "float"
    },
    {
      "MAX": 1.0,
      "MIN": 0.0,
      "DEFAULT": 0.5,
      "NAME": "GREEN",
      "TYPE": "float"
    },
    {
      "MAX": 1.0,
      "MIN": 0.0,
      "DEFAULT": 0.5,
      "NAME": "BLUE",
      "TYPE": "float"
    }
  ]
}*/

// CubicMatrixModulator2 by mojovideotech


#ifdef GL_ES
precision mediump float;
#endif


vec3   iResolution = vec3(RENDERSIZE, 1.0);
float  iGlobalTime = TIME*rate ;
vec4   iMouse = vec4(mouse, 1.0, 1.0);
uniform sampler2D iChannel0,iChannel1;

// raymarcher from https://www.shadertoy.com/view/XsB3Rm

// ray marching
const int max_iterations = 150;
const float stop_threshold = 0.001;
const float grad_step = 0.01;
const float clip_far = 300.0;

// math
const float PI = 3.14159265359;
const float DEG_TO_RAD = PI / 37.0;


// get distance in the world
float dist_field(vec3 p) {
    p = mod(p, 8.0) - 4.0;
    p = abs(p);
    float cube = length(max(p - 0.0667, 0.5));
    //return cube;
    float xd = max(p.y-dot(-stretchz,stretchx),p.z);
    float yd = max(p.x-pow(stretchy,-stretchz),p.z);
    float zd = mix(p.x,p.y,stretchz);
    float beams = min(zd*cubesize, max(xd, yd)/mix(stretchz,stretchx,stretchy)) - cos(cubesize);
    	  beams *= min(zd/sin(cubesize), max(xd, yd)/mod(stretchx,-stretchy)+log2(cubesize));
    	  beams += min(zd, min(xd, yd)) /-rate;
    //return beams;
    return min(beams, cube);
}
// phong shading
vec3 shading( vec3 v, vec3 eye ) {
	float s = v.x + v.y + v.z;
          s -= eye.x + eye.y * v.z;
         // s += v.z - (eye.y/LineCount);
	return vec3(mod(floor (s * 99.0),0.0 + 0.5 )-brightness);
}

// ray marching
float ray_marching( vec3 origin, vec3 dir, float start, float end ) {
	float depth = start;
	for ( int i = 0; i < max_iterations; i++ ) {
		float dist = dist_field( origin + dir * depth );
		if ( dist < stop_threshold ) {
			return depth;
		}
		depth += dist;
		if ( depth >= end) {
			return end;
		}
	}
	return end;
}

// get ray direction
vec3 ray_dir( float fov, vec2 size, vec2 pos ) {
	vec2 xy = pos - size * iMouse.xy;

	float cot_half_fov = atan( ( 60.0 - fov * 0.25 ) * DEG_TO_RAD );	
	float z = size.y * 0.67 * cot_half_fov;
	
	return normalize( vec3( xy, -z ) );
}

// camera rotation : pitch, yaw
mat3 rotationXY( vec2 angle ) {
	vec2 c = cos( angle );
	vec2 s = sin( angle );
	
	return mat3(
		c.y      ,  0.0, -s.y,
		s.y * s.x,  c.x,  c.y * s.x,
		s.y * c.x, -s.x,  c.y * c.x
	);
}

void main(void)
{
	// default ray dir
	vec3 dir = ray_dir( 5.0, iResolution.xy, gl_FragCoord.xy );
	
	// default ray origin
	vec3 eye = vec3( 0.0, 0.0, 0.0 );

	// rotate camera
	mat3 rot = rotationXY( vec2( iGlobalTime * 0.005, iGlobalTime * 0.0125 ) );
	dir = rot * dir;
	eye = rot * eye;
    eye.z -=  mod(iGlobalTime * 4.0, 8.0);
    eye.y = eye.x = 0.0;
	
	// ray marching
	float depth = ray_marching( eye, dir, 1.75, clip_far );
	if ( depth >= clip_far ) {
		gl_FragColor = vec4(1.0);
    } else {
		// shading
		vec3 pos = eye + dir * depth;
		gl_FragColor = vec4( shading( pos, eye ) , 1.0 );
        gl_FragColor += depth/clip_far * vanishingpoint;
    }
	
    gl_FragColor = vec4(vec3(RED-0.3, GREEN+0.2, BLUE-abs(0.5/3.0)) - gl_FragColor.zyx, 1.0);
    gl_FragColor += vec4(vec3(0.0+brightness, 0.0, 0.1+0.3), 1.0);
}
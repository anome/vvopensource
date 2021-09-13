/*{
  "CREDIT": "by wilstonoreo",
  "DESCRIPTION": "",
  "CATEGORIES": ["LiCHTPiRATEN"
  ],
  "INPUTS": [
        {
            "NAME": "time",
            "TYPE": "float",
            "DEFAULT": 0.0
              },
        {
            "NAME": "cam_roll",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN" : 0.0,
            "MAX" : 180.0
        },    
        {
            "NAME": "cam_pitch",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN" : 0.0,
            "MAX" : 180.0
        },    
        {
            "NAME": "cam_yaw",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN" : 0.0,
            "MAX" : 180.0
        }
  ]
}*/

#ifdef GL_ES
precision mediump float;
#endif

// a raymarching experiment by kabuto


const float PI = 3.14159265358979323846264;

const int MAXITER = 80;

/// Convert degrees to radians
float deg2rad(in float deg)
{
  return deg * PI / 180.0;
}


/// Calculates the rotation matrix of a rotation around X axis with an angle in radians
mat3 rotateAroundX( in float angle )
{
  float s = sin(angle);
  float c = cos(angle);
  return mat3(1.0,0.0,0.0,
              0.0,  c, -s,
              0.0,  s,  c);
}

/// Calculates the rotation matrix of a rotation around Y axis with an angle in radians
mat3 rotateAroundY( in float angle )
{
  float s = sin(angle);
  float c = cos(angle);
  return mat3(  c,0.0,  s,
              0.0,1.0,0.0,
               -s,0.0,  c);
}

/// Calculates the rotation matrix of a rotation around Z axis with an angle in radians
mat3 rotateAroundZ( in float angle )
{
  float s = sin(angle);
  float c = cos(angle);
  return mat3(  c, -s,0.0,
                s,  c,0.0,
              0.0,0.0,1.0);
}

vec3 field(vec3 p) {
	p *= .1;
	float f = .1;
	for (int i = 0; i < 5; i++) {
		p = p.yzx*mat3(.8,.6,0,-.6,.8,0,0,0,1);
		p += vec3(.123,.456,.789)*float(i);
		p = abs(fract(p)-0.5);
		p *= 2.0;
		f *= 2.0;
	}
	p *= p;
	return sqrt(p+p.yzx)/f-.002;
}



float spherical_direction(out vec3 rd)
{
	vec2 uv = vv_FragNormCoord.xy;
  float theta = (uv.t) * PI,
        phi = (uv.s - 0.5)* 2.0 * PI;
  rd = vec3(sin(theta) * sin(phi), sin(theta) * cos(phi), cos(theta));
  rd *= rotateAroundZ(deg2rad(90.0+cam_yaw))*rotateAroundY(deg2rad(cam_roll))*rotateAroundX(deg2rad(cam_pitch));

  return 1.0;
}


void main( void ) {
	vec3 dir;
	spherical_direction(dir);
	
	vec3 pos = vec3(0.0,0.5+time,0.2);
	vec3 color = vec3(0);
	for (int i = 0; i < MAXITER; i++) {
		vec3 f2 = field(pos);
		float f = min(min(f2.x,f2.y),f2.z);
		
		pos += dir*f;
		color += float(MAXITER-i)/(f2+.001);
	}
	vec3 color3 = vec3(1.-1./(1.+color*(.09/float(MAXITER*MAXITER))));
	color3 *= color3;
	gl_FragColor = vec4(color3.zyx,1.);
}


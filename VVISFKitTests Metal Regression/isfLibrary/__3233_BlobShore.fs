/*
{
  "CATEGORIES" : [
    "Generators"
  ],
  "VSN" : ".01",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "u_scale1",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 17.484375,
      "MIN" : 1
    },
    {
      "NAME" : "u_scale2",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 11.591263771057129,
      "MIN" : 1
    },
    {
      "NAME" : "u_scale3",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 5.0493607521057129,
      "MIN" : 1
    },
    {
      "NAME" : "u_apply_thresh",
      "TYPE" : "bool",
      "DEFAULT" : true
    },
    {
      "NAME" : "u_thresh1",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.51025718450546265,
      "MIN" : 0
    },
    {
      "NAME" : "u_thresh2",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.36108702421188354,
      "MIN" : 0
    },
    {
      "NAME" : "u_thresh3",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 1,
      "MIN" : 0
    },
    {
      "NAME" : "u_offset",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.3082573413848877,
      "MIN" : 0
    },
    {
      "NAME" : "u_color1",
      "TYPE" : "color",
      "DEFAULT" : [
        0.99705451726913452,
        0.92930144071578979,
        0,
        1
      ]
    },
    {
      "NAME" : "u_color2",
      "TYPE" : "color",
      "DEFAULT" : [
        0.2166106253862381,
        0.94258779287338257,
        0.087304338812828064,
        1
      ]
    },
    {
      "NAME" : "u_color3",
      "TYPE" : "color",
      "DEFAULT" : [
        0.91921168565750122,
        0.33995956182479858,
        0,
        1
      ]
    }
  ],
  "DESCRIPTION" : "Cellular noise",
  "CREDIT" : "Bokk of Shaders"
}
*/

vec2 random2(vec2 p) {
	return fract(sin(vec2(dot(p,vec2(127.1,311.7)),dot(p,vec2(269.5,183.3))))*43758.5453);
}

// cellular noise from Book Of Shaders
// https://thebookofshaders.com/edit.php?log=161127231150
float cellular(vec2 p,bool t,float thresh) {
	vec2 i_st = floor(p);
	vec2 f_st = fract(p);
	float m_dist = 10.0;
	for (int j=-1; j<=1; j++ ) {
		for (int i=-1; i<=1; i++ ) {
			vec2 neighbor = vec2(float(i),float(j));
			vec2 point = random2(i_st + neighbor);
			point = 0.5 + 0.5*sin(6.2831*point);
			vec2 diff = neighbor + point - f_st;
			float dist = length(diff);
			if( dist < m_dist ) {
				m_dist = dist;
			}
		}
	}
	//m_dist *= 2.0+sin((p.x+TIME*0.2)*3.0*3.1415)+cos((p.y+TIME*0.2)*3.0*3.1415);
	//m_dist *= 2.0+sin(p.x*3.1415)+cos(p.y*3.1415);
	m_dist *= 2.0+p.y+2.0+sin((p.y+TIME*0.5)*1.0*3.1415);
	if ( t ) {
		float p = 3.0/RENDERSIZE.y;
		m_dist = smoothstep(thresh-p,thresh+p,m_dist);
	}
	return 1.0-m_dist;
}

// --------------------------
// MAIN
vec2 normalizedCoord(float r)
{
	vec2 st = (isf_FragNormCoord - 0.5) * 2.0 * r;
	st.x *= (RENDERSIZE.x/RENDERSIZE.y);
	return st;
}

void main() {
	vec2 st = normalizedCoord(1.0);
	vec3 source = vec3(isf_FragNormCoord.x);
//	vec3 source = vec3(abs(st.x));
	float src = source.x;

	
	float c1 = cellular( st*u_scale1+u_offset*-1.0, u_apply_thresh, u_thresh1 );
	float c2 = cellular( st*u_scale2+u_offset*0.0, u_apply_thresh, u_thresh2 );
	float c3 = cellular( st*u_scale3+u_offset*1.0, u_apply_thresh, u_thresh3 );
	
	vec3 color = vec3(0);
	if ( c3 > 0.0)
		color = u_color3.xyz;
	else if ( c2 > 0.0)
		color = u_color2.xyz;
	else if ( c1 > 0.0)
		color = u_color1.xyz;
	
	//color = vec3(c1,c2,c3);
	
    gl_FragColor = vec4(color, 1.0);
}

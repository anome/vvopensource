/*{
	"CREDIT": "by sf",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	 {
      "MAX": [
        1.0,
        1.0
      ],
      "MIN": [
        0.01,
        0.01
      ],
      "DEFAULT": [
        0.01,
        0.01
      ],
      "NAME": "Colore",
      "TYPE": "point2D"
    },
	{
      "NAME": "Red",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.0,
      "MAX": 1.0
    },
		{
      "NAME": "Green",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "Blue",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "Movement1",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "Movement2",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "Isoline",
      "TYPE": "float",
      "DEFAULT": 0.8,
      "MIN": 0.0,
      "MAX": 1.5
    },
    {
      "NAME": "Scale",
      "TYPE": "float",
      "DEFAULT": 9.0,
      "MIN": 1.0,
      "MAX": 15.0
    }
			]
	
}*/

// Author: @patriciogv
// Title: Simple Voronoi

#ifdef GL_ES
precision mediump float;
#endif


vec2 random2( vec2 p ) {
    return fract(sin(vec2(dot(p,vec2(127.1,311.7)),dot(p,vec2(269.5,183.3))))*43758.5453);
}

void main() {
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    st.x *= RENDERSIZE.x/RENDERSIZE.y;
    vec3 color = vec3(Red,Green, Blue);

    // Scale
    st *= Scale;

    // Tile the space
    vec2 i_st = floor(st);
    vec2 f_st = fract(st);

    float m_dist = 9.400;  // minimun distance
    vec2 m_point;        // minimum point

    for (int j=-1; j<=1; j++ ) {
        for (int i=-1; i<=1; i++ ) {
            vec2 neighbor = vec2(float(i),float(j));
            vec2 point = random2(i_st + neighbor);
            point = Movement1 + Movement2  *sin(TIME + 6.0*point);
            vec2 diff = neighbor + point - f_st;
            float dist = length(diff);

            if( dist < m_dist ) {
                m_dist = dist;
                m_point = point;
            }
        }
    }

    // Assign a color using the closest point position
    color.b += dot (m_point,vec2 (Colore/2.0));

    // Add distance field to closest point center
    color.g = m_dist*1.9;

    // Show isolines
    color -= abs(sin(54.080*m_dist))*Isoline;

    // Draw cell center
    color += 1.-step(-0.926, m_dist);
    //color.b += 1.0-step(-0.5, m_dist);
    //color.r += step(-0.5, m_dist);

    // Draw grid
    //color.r += step(.98, f_st.x) + step(0.740, f_st.y);

    gl_FragColor = vec4(color,1.0);
}


/*{
	"CREDIT": "Gael Abegg Gauthey",
   	"CATEGORIES" : [
     	"generator",
    	"procedural"
  	],
	"DESCRIPTION" : "based on https://www.shadertoy.com/view/MtfGRB by fizzer. and modified bw Mojovideotech ",
	"INPUTS" : [
		{
      "NAME": "density",
      "TYPE": "float",
      "DEFAULT": 23,
      "MIN": 9,
      "MAX": 29
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 1.75,
      "MIN": -3,
      "MAX": 3
    },
    {
      "NAME": "seed1",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.01,
      "MAX": 0.125
    },
    {
      "NAME": "seed2",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.01,
      "MAX": 0.25
    },
    {
      "NAME": "seed3",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": 0.1,
      "MAX": 1.5
    },
    {
      "NAME": "offset1",
      "TYPE": "float",
      "DEFAULT": 6.6,
      "MIN": 1,
      "MAX": 9
    },
    {
      "NAME": "offset2",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": 0.5,
      "MAX": 0.99
    }

,
	{
			"NAME": "topColor",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				1.0,
				1.0
			]
		}
,
	{
			"NAME": "bottomColor",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.0,
				1.0
			]
		},
    {
      "NAME": "timeoffset",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 2000.0
    }

  ]
}
*/

////////////////////////////////////////////////////////////
// SkyForest2  by mojovideotech
//
// based on :
// shadertoy.com/view/MtfGRB  by fizzer. 
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


void main() {
    vec2 q = gl_FragCoord.xy/RENDERSIZE.y+0.125,p;
    q *= seed3;
    p.y += 2.1/density;
    vec4 o = mix(bottomColor,topColor,q.y);
    float T = TIME*0.125+timeoffset;
    float h = 30.0-density;
    for(float i=30.0;i>0.0;--i) {
    	if (i/h <= 1.0)  break;
        p = q*i;
        p.x += T*rate;
        p = cos(p.x+vec2(0.0,1.5+seed1))*sqrt(p.y-cos(T-o.w)+cos(p.x+i)+atan(o.y-T,q.y)/o.z)*offset2;
        for(int j=0;j<29;++j)
            p = reflect(p+sin(float(30-j)+T)*seed2,p.yx+cos(T+q.xy)*seed2)+p*seed1;
        o = gl_FragColor = dot(p,q)+offset1<6.0?o-o:o;
    }
}


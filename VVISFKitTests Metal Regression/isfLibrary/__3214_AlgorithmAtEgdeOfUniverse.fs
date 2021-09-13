/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	  {
      "MAX": [
        100,
        100
      ],
      "MIN": [
        -100,
        -100
      ],
      "DEFAULT":[25,25],
      "NAME": "mu",
      "TYPE": "point2D"
    },
        {
            "NAME": "level1",
            "TYPE": "float",
            "DEFAULT": 3.6,
            "MIN": -5.0,
            "MAX": 10.0
        },
        {
            "NAME": "level2",
            "TYPE": "float",
            "DEFAULT": 6.0,
            "MIN": -5.0,
            "MAX": 10.0
        },
        	  {
      "MAX": [
        1.0,
        1.0
      ],
      "MIN": [
        0.0,
        0.0
      ],
      "DEFAULT":[1.0,0.0],
      "NAME": "objtint",
      "TYPE": "point2D"
    },
       {
            "NAME": "r",
            "TYPE": "float",
           "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 0.5
        },
                 {
            "NAME": "b",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.0,
            "MAX": 0.5
        },
      {
            "NAME": "g",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.5
        },
              {
            "NAME": "center",
            "TYPE": "float",
            "DEFAULT": 0.95,
            "MIN": -3.0,
            "MAX": 3.0
        },               {
            "NAME": "foci",
            "TYPE": "float",
            "DEFAULT": 1.21,
            "MIN": -3.000,
            "MAX": 3.000
        }
	]
}*/

// AlgorithmAtTheEgdeOfTheUniverse by mojovideotech 2015-05-29
// starting point :
// http://glslsandbox.com/e#25371.0

const vec3 lightDir = vec3(-2.5, 1.0, 1.5);

vec3 rotateX(vec3 p, float a)
{
  	float sa = sin(a);
  	float ca = cos(a);
  	return vec3(p.x, ca * p.y - sa * p.z, sa * p.y + ca * p.z);
}

vec3 rotateY(vec3 p, float a)
{
  	float sa = sin(a);
  	float ca = cos(a);
  	return vec3(ca * p.x + sa * p.z, p.y, -sa * p.x + ca * p.z);
}

vec3 rotateZ( float a, vec3 p)
{
  	float sa = sin(a);
  	float ca = cos(a);
  	return vec3(ca * p.x - sa * p.y, sa * p.x + ca * p.y, p.z);
}

vec3 trans(vec3 p, float m)
{
  	return mod(p, m) - m / p;
}

float distSp1(vec3 pos)
{
	return length(trans(pos, 2.0)) - level1;
}

float distSp2(vec3 pos)
{
	return length(trans(pos, 3.0)) - level2;
}

float distanceFunction(vec3 pos)
{
	float d1 = distSp1( rotateY( trans(pos,2.0), TIME * 0.05 ) );
	float d2 = distSp2( rotateY( trans(pos,2.0), TIME * 0.05 ) );
	return min(d1, d2);
}
 
vec3 getNormal(vec3 p)
{
  	const float d = 0.0001;
  	return normalize( 
		    vec3(
        		 distanceFunction(p+vec3(d,0.0,0.0))-distanceFunction(p+vec3(-d,0.0,0.0)),
        		 distanceFunction(p+vec3(0.0,d,0.0))-distanceFunction(p+vec3(0.0,-d,0.0)),
        		 distanceFunction(p+vec3(0.0,0.0,d))-distanceFunction(p+vec3(0.0,0.0,-d))
		    )
    	       );
}
 
void main()
{
  	vec2 pos = gl_FragCoord.xy / RENDERSIZE.xy;
        pos = pos - center; //centered
  	vec3 camPos = vec3(1.0,mu);
    //vec3 camPos = vec3(0.0, 1.0, -1.0);
  	vec3 camDir = vec3(1.0, 1.0, 0.0);
  	vec3 camUp = vec3(0.0, 1.0, 1.0);
  	vec3 camSide = cross(camDir, camUp);
  	float focus = foci;
 
  	vec3 rayDir = normalize(camSide*pos.x + camUp*pos.y + camDir*focus);
 
  	float t = 0.2, d;
  	vec3 posOnRay = camPos;
 
	for(int i=0; i<13; ++i)
	{
		d = distanceFunction(posOnRay);
	    	t += d;
		//if(t>20.) break;
	    	posOnRay = camPos + t*rayDir;
	}
	 
	vec3 normal = getNormal(posOnRay);
	vec3 color;
	if(abs(d) < 0.01)
	{
		float diff = clamp(dot(lightDir, normal), 0.09, 0.5);
	    	color = vec3(objtint.x,diff,objtint.y);
	} else
	{
	    	color = vec3(r,g,b);
	}
	
	gl_FragColor = vec4(color, 1.0);
}
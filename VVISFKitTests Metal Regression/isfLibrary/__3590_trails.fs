/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	{
		"NAME" : "heightmap",
		"TYPE" : "image"
	},
        {
			"NAME": "torus_w",
			"TYPE": "float",
			"DEFAULT": 1.0
		},{
			"NAME": "bump_scale",
			"TYPE": "float",
			"DEFAULT": 0.0
		},{
			"NAME": "colormap_scale",
			"TYPE": "float",
			"DEFAULT": 1.0
		},{
			"NAME": "colormap_offs",
			"TYPE": "float",
			"DEFAULT": 0.0
		},{
			"NAME": "distort_domain",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MAX" : 40
		},{
			"NAME": "distort_scale",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MAX" : 40
		},
		{
			"NAME": "r_x",
			"TYPE": "float",
			"DEFAULT": 0.0
		},
		{
			"NAME": "r_y",
			"TYPE": "float",
			"DEFAULT": 0.0
		},
		{
			"NAME": "r_z",
			"TYPE": "float",
			"DEFAULT": 0.0
		},
		{
			"NAME" : "_stripe",
			"TYPE" : "bool"
		},
		{
			"NAME" : "_rotation",
			"TYPE" : "float",
			"DEFAULT" : 0,
			"MIN" : 0,
			"MAX" : "6.28"
		}
			
	]
}*/



vec3   iResolution = vec3(RENDERSIZE, 1.0);
float  iGlobalTime = TIME;

const int MAX_ITER = 20;

float length8( vec2 p ) {
	return pow(pow(p.x, 8.) + pow(p.y, 8.), 1./8.);
}

float length2( vec2 p ) {
	return pow(pow(p.x, 2.) + pow(p.y, 2.), 1./2.);
}



mat3 RotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat3(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c);
}


float height(float x) {
	float x_cord = - log(1. - x/2.) / log(40.);
	vec2 tex_space =  (vec2(x,0.5)/ 1.5 + 0.5);

	return (1. + log(texture2D(heightmap, tex_space).r)) / bump_scale;
}

float sdTorus( vec3 p, vec2 t )
{
  // vec2 q = vec2(length(p.xz)-t.x,p.y);
  // return length(q)-t.y;
  
	vec2 q = abs(vec2(max(abs(p.x), abs(p.z))-t.x, p.y));
	return max(q.x, q.y)-t.y;
  
}

vec2 sdSphere( vec3 p, float s )
{

  float h = height(fract(atan(p.x, p.y)));
  return vec2(length(p)-s - h, h);
}

vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
	t = t * colormap_scale + colormap_offs;
    return a + b*cos( 6.28318*(c*t+d) );
}


vec3 palette(float f ) {
	//return pal(f, vec3(0.8,0.5,0.4),vec3(0.2,0.4,0.2),vec3(2.0,1.0,1.0),vec3(0.0,0.25,0.25) );
	return pal(f, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,0.10,0.20) );
}


vec3 intersect(in vec3 ro, in vec3 rd)
{

    float t = 0.;
    vec2 d = vec2(0.);
    vec3 p ;

    for( int i = 0; i < MAX_ITER; i ++)
    {
    	p = ro + rd * t;
    	
    	p = p * RotationMatrix(vec3(r_x, r_y, r_z), _rotation);
        d = sdSphere(p, 0.5) * 0.75;
        
        // d = sdTorus(p, vec2(torus_w, bump_scale));
        if( d.x < 0.001 )
        {
            return palette(d.y) * (1. - float(i)/float(MAX_ITER));
        }
		t += d;

    }
    return vec3(-1.);

}



void main()
{

	vec3 cameraOrigin = vec3(0., 0., -torus_w);

	vec2 screenPos = -1.0 + 2.0 * gl_FragCoord.xy / iResolution.xy;
	screenPos.x *= iResolution.x / iResolution.y;
	vec3 rayDir = normalize(vec3(screenPos, 1.));
	

	vec3 color = intersect(cameraOrigin, rayDir);
	

	// vingette
	// color *= 1.0 - pow(length(screenPos), 2.);
	color *=1.2;
	
	gl_FragColor = vec4(color, 1.0);
} 
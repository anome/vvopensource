/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
        {
			"NAME": "torus_w",
			"TYPE": "float",
			"DEFAULT": 1.0
		},{
			"NAME": "torus_h",
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

const int MAX_ITER = 30;

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

vec2 rotate(in vec2 v, in float a) {
	return vec2(cos(a)*v.x + sin(a)*v.y, -sin(a)*v.x + cos(a)*v.y);
}

float sdTorus( vec3 p, vec2 t )
{
  // vec2 q = vec2(length(p.xz)-t.x,p.y);
  // return length(q)-t.y;
  
	vec2 q = abs(vec2(max(abs(p.x), abs(p.z))-t.x, p.y));
	return max(q.x, q.y)-t.y;
  
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

vec2 DE(in vec3 p)
{
	// p = mod(p, -1.);

	vec3 axe = vec3(r_y, r_x, r_z);

	p += sin(p*distort_domain)/distort_scale;
	p = p * RotationMatrix(axe, _rotation);
	
	float color =  mod(3.0 * (RotationMatrix(axe, -_rotation) * p).y , 1.) < 0.5 ? 1.0 : 0.0 ;
	
	return vec2(sdTorus(p.yxz, vec2(torus_w, torus_h)), color);
}

vec3 gradient( in vec3 pos )
{
	vec3 eps = vec3( 0.0001, 0.0, 0.0 );
	vec3 nor = vec3(
	    DE(pos+eps.xyy).x - DE(pos-eps.xyy).x,
	    DE(pos+eps.yxy).x - DE(pos-eps.yxy).x,
	    DE(pos+eps.yyx).x - DE(pos-eps.yyx).x );
	return normalize(nor);
}

float height(float x,  float y) {
	return sin(x ) * sin (y);
}

bool intersect(in vec3 ro, in vec3 rd)
{

    const float delt = 0.01;
    const float mint = 0.001;
    const float maxt = 10.0;
    
    for( float t = mint; t < maxt; t += delt )
    {
        vec3 p = ro + rd*t;
        if( p.y < height( p.x, p.z ) )
        {
            return true;
        }
    }
    return false;

}



void main()
{

	vec3 cameraDir = vec3(1., 0., 0.);
	vec3 cameraOrigin = vec3(-3., 0., 0.);

	vec2 screenPos = -1.0 + 2.0 * gl_FragCoord.xy / iResolution.xy;
	screenPos.x *= iResolution.x / iResolution.y;
	vec3 rayDir = normalize(vec3(1.0, screenPos));
	
	bool hit = intersect(cameraOrigin, rayDir);
	
	vec3 color = vec3(0.0);
	if ( hit ) {
		color = vec3(0.5);
	}
	

	// vingette
	// color *= 1.0 - pow(length(screenPos), 2.);
	color *=1.2;
	
	gl_FragColor = vec4(color, 1.0);
} 
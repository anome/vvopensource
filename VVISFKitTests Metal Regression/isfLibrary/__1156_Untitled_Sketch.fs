/*
{
  "CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "generator",
    "voronoi"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
  {
			"LABEL": "Mouse X",
			"NAME": "mX",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Mouse Y",
			"NAME": "mY",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	],
	"ISFVSN" : 2.0
}*/




vec3 iResolution = vec3(RENDERSIZE, 1.);
float iTime = TIME;
vec2 iMouse = vec2(mX*RENDERSIZE.x, mY*RENDERSIZE.y);


const float pi = 3.1415926535897932384;

// Edge lengths of the rectunglar plate. Note that in reality only 
// for the case a=b degenerate eigenmodes appear, leading to the 
// superimposition as implemented here.
float a;
float b = 1.0;

// Chladni eigenmodes
float chladni( float m, float n, vec2 uv )
{	
	// cos()*cos() for modes of a plate fixed at its center
	// sin()*sin() for modes of a plate fixed at its border (boring)
	return cos(n*pi*uv.x/a)*cos(m*pi*uv.y/b);
}

// Eigenfrequencies (not used)
float omega( float m, float n )
{
	const float rho = 1.0;
	const float eta = 1.0;	
	return pi * sqrt( (rho/eta) * (m/a)*(m/a) + (n/b)*(n/b) );
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	// Domain [0,1]x[0,1]
	vec2 uv = fragCoord.xy / iResolution.y;
	a = 1.; //iResolution.x / iResolution.y;
	
	// Knot numbers
	vec2 mn = 21.0 * iMouse / iResolution.xy; //vec2(4.0,2.0);
	
	// Superposition coefficients
	float alpha = iTime;
	mat2 R = mat2( cos(alpha), sin(alpha), -sin(alpha), cos(alpha) );
	vec2 c = R * vec2(1.0,-1.0);	
	//c = vec2(1.0,-1.0); // Default coefficients
	
	// Superposition of eigenmodes
	float u = c.x*chladni(mn.x,mn.y,uv) + c.y*chladni(mn.y,mn.x,uv);
	
	// Shift-scale from [-1,+1] to [0,1]		
	u = (0.5+u/2.0);
	
	// Visualize knot lines (i.e. zero-crossings)
	u = step( abs(u-0.5), 0.05 );
	
	fragColor = vec4(u*vec3(1.0),1.0);
}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}


/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.01,
		"MIN" : 		-5.0,
		"MAX" : 		5.0
	},
	{
		"NAME" : 		"rot",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.01,
		"MIN" : 		-5.0,
		"MAX" : 		5.0
	}
	]
}*/



vec2 bipolar(vec2 p,float a, float b){
     
    float alpha = a*a - dot(p,p);
    float beta = a*a + dot(p,p);
    float gamma = sqrt(alpha*alpha - 4.0*p.y*p.y*a*a);
    float sigma = atan( 2.0*a*p.y ,alpha + b*gamma );
    float tau = 0.5*log((beta + 2.0*a*p.x)/(beta - 2.0*a*p.x));
    
    return vec2(sigma,tau);
}

void main()
{
	float time = TIME*rate;
    vec2 p = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy) / RENDERSIZE.y;
	
    //rotate
   float rotationRate = rot;
   float s = sin(rotationRate*time);
   float c = cos(rotationRate*time);
   p = mat2(c,s,-s,c)*p;
    
    vec2 bp = bipolar(p,0.3, 1.0 + sin(time));
    float osc = bp.x + bp.y;
	
    vec3 color = vec3(sin(15.0*osc + 10.0*time));
   
 	gl_FragColor  = vec4(color,1.0);
}
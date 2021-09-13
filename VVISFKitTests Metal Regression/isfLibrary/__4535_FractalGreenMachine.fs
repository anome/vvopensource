
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
	
}*/

void main()	{
#define ROTATION_SPEED 0.0251
#define MERGE_SPEED 3.0

float iTime = TIME;
#define iResolution RENDERSIZE
#define R(p,a,r)mix(a*dot(p,a),p,cos(r))+sin(r)*cross(p,a)
//void mainImage(out vec4 O, vec2 C){
    vec4 O=vec4(0);
    vec3 r=RENDERSIZE.xyx , p;  
    r.z = 1.6;
    vec2 C = isf_FragNormCoord.xy;
    float  g=0., e = 0., s = 0.;
    for(float i=0.; i<99.; i++){
        if(e<.003) { 
            O.xyz += mix(
                r/r,
                cos(p*.2+g*1.2)*.5+.5,
                .8
            )*27./i/i;
        }
            else{
                O.xyz+=p;
            }
        p=vec3(g*(C-.5*r.xy)/r.y,g-.5);
        p=R(p,normalize(vec3(1,2,3)),TIME*ROTATION_SPEED); //rotation speed
        s=2.;
        p.y=abs(p.y-1.);
        for(int j=0; j<8; j++){
            p.xz=abs(p.xz),
            p.z>p.x?p=p.zyx:p,
            p.z=1.2+clamp(-.8,.3,cos(TIME*MERGE_SPEED))*.1
                -abs(p.z-.8+clamp(-.6,.6,sin(TIME*MERGE_SPEED*0.5))*.1),
            p.y>p.x?p=p.yxz:p,
            p.x-=2.,
            p.y>p.x?p=p.yxz:p,
            p.y+=.1,
            p=3.*p-vec3(6,1,1),
            s*=3.;
        g+=e=length(p)/s;
    }
    O=pow(O,vec4(1.5,1,1.8,1));
    gl_FragColor = O;
}

}

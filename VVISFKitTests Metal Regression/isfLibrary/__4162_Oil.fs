
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/
#ifdef GL_ES
precision lowp float;
#endif

#define u_time TIME;

float hash(float n){return fract(sin(n)*43758.5453);}
float noise(in vec3 x){
    vec3 p=floor(x);
    vec3 f=fract(x);
    f=f*f*(3.-2.*f);
    lowp float n=p.x+p.y*57.+113.*p.z;
    return mix(mix(mix(hash(n+0.),hash(n+1.),f.x),
    mix(hash(n+57.),hash(n+58.),f.x),f.y),
    mix(mix(hash(n+113.),hash(n+114.),f.x),
    mix(hash(n+170.),hash(n+171.),f.x),f.y),f.z);
}
vec3 noise3(in vec3 x){
    return vec3(noise(x+vec3(123.456,.567,.37)),
    noise(x+vec3(.11,47.43,19.17)),
    noise(x));
}
float bias(float x,float b){return x/((1./b-2.)*(1.-x)+1.);}

float gain(float x,float g){
    lowp float t=(1./g-2.)*(1.-(2.*x));
    return x<.5?(x/(t+1.)):(t-x)/(t-1.);
}

mat3 rotation(float angle,vec3 axis){
    lowp float s=sin(-angle);
    lowp float c=cos(-angle);
    lowp float oc=1.-c;
    vec3 sa=axis*s;
    vec3 oca=axis*oc;
    return mat3(oca.x*axis+vec3(c,-sa.z,sa.y),oca.y*axis+vec3(sa.z,c,-sa.x),oca.z*axis+vec3(-sa.y,sa.x,c));
}
vec3 fbm(vec3 x,float H,float L,int oc){
    vec3 v=vec3(0);
    lowp float f=1.;
    for(int i=0;i<8;i++){
        lowp float w=pow(f,-H);
        v+=noise3(x)*w;
        x*=L;
        f*=L;
    }
    return v;
}

vec3 smf(vec3 x,float H,float L,int oc,float off){
    vec3 v=vec3(1);
    lowp float f=1.;
    for(int i=0;i<8;i++){
        v*=off+f*(noise3(x)*2.-1.);
        f*=H;
        x*=L;
    }
    return v;
}

void main(){
    vec2 u_resolution = vec2(1600, 1400);
    lowp vec2 uv=gl_FragCoord.xy/u_resolution.xy*2.;
    uv.x*=u_resolution.x/u_resolution.y;
    lowp float time=u_time;
    lowp float slow=time*.005;
    lowp float ts=time*.37;
    lowp vec3 p=vec3(uv*.2,slow);//coordinate + slight change over time
    // float change=gain(fract(ts),.00008)+floor(ts);//flick to a different view
    // vec3 p=vec3(uv*.2,slow+change);//coordinate + slight change over time
    lowp vec3 axis=4.*fbm(p,.5,2.,8);//random fbm axis of rotation
    lowp vec3 colorVec=.5*5.*fbm(p*.3,.5,2.,7);//random base color
    p+=colorVec;
    lowp float mag=80e3;//still clips a bit
    lowp vec3 colorMod=mag*smf(p,.7,2.,8,.2);//multifractal saturation
    colorVec+=colorMod;
    colorVec=rotation(length(axis)+slow,normalize(axis))*colorVec*.08;
    colorVec=pow(colorVec,vec3(1./2.));//gamma
    gl_FragColor=vec4(colorVec,1.);
}

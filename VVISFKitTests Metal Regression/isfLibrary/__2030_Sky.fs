/*{
	"DESCRIPTION": "Fractal Land",
 	"CREDIT": "by Kali",
	"CATEGORIES": [
		"raymarching, fractal"
	],
	"INPUTS": [
		{
			"NAME": "Image1",
			"TYPE": "image"
		},	
		{
			"NAME": "Image2",
			"TYPE": "image"
		},
		{
			"NAME": "FOV",
			"TYPE": "float",
			"DEFAULT": 0.9,
			"MIN": 0.5,
			"MAX": 2.0
		},
		{
			"NAME": "RAYS",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "IMAGESIZE",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 3.0
		},
		{
			"NAME": "STRETCH",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "POS_X",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "POS_Y",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "WAVESAMPLITUDE",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "BRIGHT",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 2.0
		},
		{
			"NAME": "SPEED",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "RBOW_SPEED",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "RBOW_POS_X",
			"TYPE": "float",
			"DEFAULT": -0.2,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "RBOW_POS_Y",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "RBOW_SIZE",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": 1.0,
			"MAX": 10.0
		},
		{
			"NAME": "RBOW_STRETCH",
			"TYPE": "float",
			"DEFAULT": 0.75,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "FRACTAL",
			"TYPE": "float",
			"DEFAULT": 35.0,
			"MIN": 25.0,
			"MAX": 42.0
		}
		,
		{
			"NAME": "COLORS",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 2.0
		}

	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);


float iGlobalTime = TIME; //---  PUT iGlobalTime = 23. instead of iGlobalTime = TIME to see the rainbow and rainbow image freezed


//vec4 iMouse = vec4(mX*RENDERSIZE.x, mY*RENDERSIZE.y, mZ*RENDERSIZE.x, mW*RENDERSIZE.y);

// "Fractal Cartoon" - former "DE edge detection" by Kali

// Cartoon-like effect using eiffies's edge detection found here: 
// https://www.shadertoy.com/view/4ss3WB
// I used my own method previously but was too complicated and not compiling everywhere.
// Thanks to the suggestion by WouterVanNifterick. 

// There are no lights and no AO, only color by normals and dark edges.

// update: Nyan Cat cameo, thanks to code from mu6k: https://www.shadertoy.com/view/4dXGWH


#define SHOWONLYEDGES
#define NYAN 
#define WAVES
#define BORDER

#define RAY_STEPS 100

#define BRIGHTNESS 1.2
#define GAMMA 1.4
#define SATURATION .65


#define detail .001
#define t iGlobalTime*.5*SPEED


const vec3 origin=vec3(-1.,.7,0.);
float det=0.0;
mat2 rota;


// 2D rotation function
mat2 rot(float a) {
	return mat2(cos(a),sin(a),-sin(a),cos(a));	
}

// "Amazing Surface" fractal
vec4 formula(vec4 p) {
		p.xz = abs(p.xz+1.)-abs(p.xz-1.)-p.xz;
		p.y-=.25;
		p.xy*=rota;
		p=p*2./clamp(dot(p.xyz,p.xyz),.2,1.);
	return p;
}

// Distance function
float de(vec3 pos) {
#ifdef WAVES
	pos.y+=sin(pos.z-t*6.)*.15*WAVESAMPLITUDE; //waves!
#endif
	float hid=0.;
	vec3 tpos=pos;
	tpos.z=abs(3.-mod(tpos.z,6.));
	vec4 p=vec4(tpos,1.);
	for (int i=0; i<4; i++) {p=formula(p);}
	float fr=(length(max(vec2(0.),p.yz-1.5))-1.)/p.w;
	float ro=max(abs(pos.x+1.)-.3,pos.y-.35);
		  ro=max(ro,-max(abs(pos.x+1.)-.1,pos.y-.5));
	pos.z=abs(.25-mod(pos.z,.5));
		  ro=max(ro,-max(abs(pos.z)-.2,pos.y-.3));
		  ro=max(ro,-max(abs(pos.z)-.01,-pos.y+.32));
	float d=min(fr,ro);
	return d;
}


// Camera path
vec3 path(float ti) {
	ti*=1.5;
	vec3  p=vec3(sin(ti),(1.-sin(ti*2.))*.5,-ti*5.)*.5;
	return p;
}

// Calc normals, and here is edge detection, set to variable "edge"

float edge=0.;
vec3 normal(vec3 p) { 
	vec3 e = vec3(0.0,det*5.,0.0);

	float d1=de(p-e.yxx),d2=de(p+e.yxx);
	float d3=de(p-e.xyx),d4=de(p+e.xyx);
	float d5=de(p-e.xxy),d6=de(p+e.xxy);
	float d=de(p);
	edge=abs(d-0.5*(d2+d1))+abs(d-0.5*(d4+d3))+abs(d-0.5*(d6+d5));//edge finder
	edge=min(1.,pow(edge,.55)*15.);
	return normalize(vec3(d1-d2,d3-d4,d5-d6));
}


// Used Nyan Cat code by mu6k, with some mods

vec4 rainbow(vec2 p)
{
	float q = max(p.x,-0.1);
	float s = sin(p.x*7.0+t*50.0)*0.08;
	p.y+=s;
	p.y*=1.1;
	
	vec4 c;
	if (p.x>0.0) c=vec4(0,0,0,0); else
	if (0.0/6.0<p.y&&p.y<1.0/6.0) c= vec4(255,43,14,255)/255.0; else
	if (1.0/6.0<p.y&&p.y<2.0/6.0) c= vec4(255,168,6,255)/255.0; else
	if (2.0/6.0<p.y&&p.y<3.0/6.0) c= vec4(255,244,0,255)/255.0; else
	if (3.0/6.0<p.y&&p.y<4.0/6.0) c= vec4(51,234,5,255)/255.0; else
	if (4.0/6.0<p.y&&p.y<5.0/6.0) c= vec4(8,163,255,255)/255.0; else
	if (5.0/6.0<p.y&&p.y<6.0/6.0) c= vec4(122,85,255,255)/255.0; else
	if (abs(p.y)-.05<0.0001) c=vec4(0.,0.,0.,1.); else
	if (abs(p.y-1.)-.05<0.0001) c=vec4(0.,0.,0.,1.); else
		c=vec4(0,0,0,0);
	c.a*=.8-min(.8,abs(p.x*.08));
	c.xyz=mix(c.xyz,vec3(length(c.xyz)),.15);
	return c;
}

vec4 nyan(vec2 p)
{
	vec2 uv = p;
	vec4 color = IMG_NORM_PIXEL(Image2,uv);
	return color;
}


// Raymarching and 2D graphics

vec3 raymarch(in vec3 from, in vec3 dir) 

{
	rota=rot(radians(FRACTAL));
	edge=0.;
	vec3 p, norm;
	float d=110.;
	float totdist=0.;
	for (int i=0; i<RAY_STEPS; i++) {
		if (d>det && totdist<25.0) {
			// p=from+totdist*dir;
			// d=de(p);
			// det=detail*exp(.13*totdist);
			totdist+=d; 
		} else break;
	}
	vec3 col=vec3(0.);
	// p-=(det-d)*dir;
	// norm=normal(p);
	// norm.xz*=rot(COLORS);
// #ifdef SHOWONLYEDGES
	// col=1.-vec3(edge); // show wireframe version
// #else
	// col=(1.-abs(norm))*max(0.,1.-edge*.8); // set normal as color with dark edges
// #endif		
	totdist=clamp(totdist,0.,26.);
	dir.y-=0.02;
	float sunsize=4./pow(RAYS,.4); // responsive sun size
	float an=atan(dir.x,dir.y)+iGlobalTime*1.5; // angle for drawing and rotating sun
	float s=0.*pow(clamp(1.0-length(dir.xy)*sunsize-abs(.2-mod(an,.4)),0.,1.),.1); // sun
	float sb=0.*pow(clamp(1.0-length(dir.xy)*(sunsize-.2)-abs(.2-mod(an,.4)),0.,1.),.1); // sun border
	float sg=pow(clamp(1.0-length(dir.xy)*(sunsize-4.5)-.5*abs(.2-mod(an,.4)),0.,1.),3.); // sun rays
	float y=mix(.45,1.2,pow(smoothstep(0.,1.,.75-dir.y),2.))*(1.-sb*.5); // gradient sky
	
	// set up background with sky and sun
	vec2 imagecoords=(dir.xy-vec2(0.,.13/IMAGESIZE))*2.*IMAGESIZE*vec2(1.,STRETCH)+.5+vec2(POS_X,POS_Y);
	vec4 image=IMG_NORM_PIXEL(Image1,imagecoords);
	vec3 backg=vec3(0.5,0.,1.)*((1.-s)*(1.-sg)*y+(1.-sb)*sg*vec3(1.,.8,0.15)*3.);
		 backg+=vec3(1.,.9,.1)*s;
		 backg=max(backg,sg*vec3(1.,.9,.5));
		 backg=mix(backg,image.xyz,image.w);
	
	col=mix(vec3(1.,.9,.7),col,exp(-.004*totdist*totdist));// distant fading to sun color
	if (totdist>25.) col=backg; // hit background
	col=pow(col,vec3(GAMMA))*BRIGHTNESS;
	col=mix(vec3(length(col)),col,SATURATION);
// #ifdef SHOWONLYEDGES
	col=1.-vec3(length(col));
// #else
	// col*=vec3(1.,.9,.85);
// #ifdef NYAN
	dir.yx*=rot(dir.x);
	vec2 ncatpos=(dir.xy+vec2(-2.+mod(-t*RBOW_SPEED+1.3,4.),-.27));
	vec4 ncat=nyan(ncatpos*RBOW_SIZE*vec2(-1.,RBOW_STRETCH)+vec2(.7,.5)+vec2(RBOW_POS_X,RBOW_POS_Y));
	vec4 rain=rainbow(ncatpos*10.+vec2(.8,.5));
	if (totdist>8.) col=mix(col,max(vec3(.2),rain.xyz),rain.a*.9);
	if (totdist>8.) col=mix(col,ncat.xyz,ncat.a*.9);
// #endif
// #endif
	return col;
}

// get camera position
vec3 move(inout vec3 dir) {
	vec3 go=path(t);
	vec3 adv=path(t+.7);
//	float hd=de(adv);
	vec3 advec=normalize(adv-go);
	float an=adv.x-go.x; an*=min(1.,abs(adv.z-go.z))*sign(adv.z-go.z)*.7;
	dir.xy*=mat2(cos(an),sin(an),-sin(an),cos(an));
    an=advec.y*1.;
	dir.yz*=mat2(cos(an),sin(an),-sin(an),cos(an));
	an=atan(advec.x,advec.z);
	dir.xz*=mat2(cos(an),sin(an),-sin(an),cos(an));
	return go;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy*2.-1.;
	vec2 oriuv=uv;
	uv.y*=iResolution.y/iResolution.x;
	//vec2 mouse=(iMouse.xy/iResolution.xy-.5)*3.;
	//if (iMouse.z<1.) mouse=vec2(0.,-0.05);
	float fov=FOV;
	uv.y+=.05;
	vec3 dir=normalize(vec3(uv*fov,1.));
	vec3 from=origin+move(dir);
	vec3 color=raymarch(from,dir); 
	vec3 image=IMG_PIXEL(Image1,fragCoord.xy).xyz;
	fragColor = vec4(color*BRIGHT,1.);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
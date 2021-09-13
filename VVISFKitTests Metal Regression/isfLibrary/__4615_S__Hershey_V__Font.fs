/*
	{
	"DESCRIPTION": "Simplex Hershey Vector Font",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "ISF Import by: Old Salt",
	"VSN": "1.0",
	"INPUTS":
		[
		]
	}
*/
// Import from: https://www.shadertoy.com/view/MsyGzz
// A complete vector font compiled into GLSL via a C program,
// Original from: http://paulbourke.net/dataformats/hershey/


#define PI 3.14159265359
#define opU(a,b) max(a,b)

float res=0.;
vec2 cpos=vec2(0.);
float width;


float antiAlias(float x) {return (x-(1.0-2.0/RENDERSIZE.y))*(RENDERSIZE.y/2.);}

float line( in vec2 p, in vec2 a, in vec2 b )
{
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    float d = length( pa - ba*h );

    return  1.00 + width - d;
}
// CHAR: 32 : 
void char_32(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    cpos.x+=16.0*s.x;
}


// CHAR: 33 :!
void char_33(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,21.0)*s,vec2(5.0,7.0)*s));
    res=opU(res,line(p,vec2(5.0,2.0)*s,vec2(4.0,1.0)*s));
    res=opU(res,line(p,vec2(4.0,1.0)*s,vec2(5.0,0.0)*s));
    res=opU(res,line(p,vec2(5.0,0.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(5.0,2.0)*s));
    cpos.x+=10.0*s.x;
}


// CHAR: 34 :"
void char_34(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,14.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(12.0,14.0)*s));
    cpos.x+=16.0*s.x;
}


// CHAR: 35 :#
void char_35(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(11.0,25.0)*s,vec2(4.0,-7.0)*s));
    res=opU(res,line(p,vec2(17.0,25.0)*s,vec2(10.0,-7.0)*s));
    res=opU(res,line(p,vec2(4.0,12.0)*s,vec2(18.0,12.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(17.0,6.0)*s));
    cpos.x+=21.0*s.x;
}


// CHAR: 36 :$
void char_36(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(8.0,25.0)*s,vec2(8.0,-4.0)*s));
    res=opU(res,line(p,vec2(12.0,25.0)*s,vec2(12.0,-4.0)*s));
    res=opU(res,line(p,vec2(17.0,18.0)*s,vec2(15.0,20.0)*s));
    res=opU(res,line(p,vec2(15.0,20.0)*s,vec2(12.0,21.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(8.0,21.0)*s));
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(5.0,20.0)*s));
    res=opU(res,line(p,vec2(5.0,20.0)*s,vec2(3.0,18.0)*s));
    res=opU(res,line(p,vec2(3.0,18.0)*s,vec2(3.0,16.0)*s));
    res=opU(res,line(p,vec2(3.0,16.0)*s,vec2(4.0,14.0)*s));
    res=opU(res,line(p,vec2(4.0,14.0)*s,vec2(5.0,13.0)*s));
    res=opU(res,line(p,vec2(5.0,13.0)*s,vec2(7.0,12.0)*s));
    res=opU(res,line(p,vec2(7.0,12.0)*s,vec2(13.0,10.0)*s));
    res=opU(res,line(p,vec2(13.0,10.0)*s,vec2(15.0,9.0)*s));
    res=opU(res,line(p,vec2(15.0,9.0)*s,vec2(16.0,8.0)*s));
    res=opU(res,line(p,vec2(16.0,8.0)*s,vec2(17.0,6.0)*s));
    res=opU(res,line(p,vec2(17.0,6.0)*s,vec2(17.0,3.0)*s));
    res=opU(res,line(p,vec2(17.0,3.0)*s,vec2(15.0,1.0)*s));
    res=opU(res,line(p,vec2(15.0,1.0)*s,vec2(12.0,0.0)*s));
    res=opU(res,line(p,vec2(12.0,0.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(5.0,1.0)*s));
    res=opU(res,line(p,vec2(5.0,1.0)*s,vec2(3.0,3.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 37 :%
void char_37(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(21.0,21.0)*s,vec2(3.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(10.0,19.0)*s));
    res=opU(res,line(p,vec2(10.0,19.0)*s,vec2(10.0,17.0)*s));
    res=opU(res,line(p,vec2(10.0,17.0)*s,vec2(9.0,15.0)*s));
    res=opU(res,line(p,vec2(9.0,15.0)*s,vec2(7.0,14.0)*s));
    res=opU(res,line(p,vec2(7.0,14.0)*s,vec2(5.0,14.0)*s));
    res=opU(res,line(p,vec2(5.0,14.0)*s,vec2(3.0,16.0)*s));
    res=opU(res,line(p,vec2(3.0,16.0)*s,vec2(3.0,18.0)*s));
    res=opU(res,line(p,vec2(3.0,18.0)*s,vec2(4.0,20.0)*s));
    res=opU(res,line(p,vec2(4.0,20.0)*s,vec2(6.0,21.0)*s));
    res=opU(res,line(p,vec2(6.0,21.0)*s,vec2(8.0,21.0)*s));
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(10.0,20.0)*s));
    res=opU(res,line(p,vec2(10.0,20.0)*s,vec2(13.0,19.0)*s));
    res=opU(res,line(p,vec2(13.0,19.0)*s,vec2(16.0,19.0)*s));
    res=opU(res,line(p,vec2(16.0,19.0)*s,vec2(19.0,20.0)*s));
    res=opU(res,line(p,vec2(19.0,20.0)*s,vec2(21.0,21.0)*s));
    res=opU(res,line(p,vec2(17.0,7.0)*s,vec2(15.0,6.0)*s));
    res=opU(res,line(p,vec2(15.0,6.0)*s,vec2(14.0,4.0)*s));
    res=opU(res,line(p,vec2(14.0,4.0)*s,vec2(14.0,2.0)*s));
    res=opU(res,line(p,vec2(14.0,2.0)*s,vec2(16.0,0.0)*s));
    res=opU(res,line(p,vec2(16.0,0.0)*s,vec2(18.0,0.0)*s));
    res=opU(res,line(p,vec2(18.0,0.0)*s,vec2(20.0,1.0)*s));
    res=opU(res,line(p,vec2(20.0,1.0)*s,vec2(21.0,3.0)*s));
    res=opU(res,line(p,vec2(21.0,3.0)*s,vec2(21.0,5.0)*s));
    res=opU(res,line(p,vec2(21.0,5.0)*s,vec2(19.0,7.0)*s));
    res=opU(res,line(p,vec2(19.0,7.0)*s,vec2(17.0,7.0)*s));
    cpos.x+=24.0*s.x;
}


// CHAR: 38 :&
void char_38(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(23.0,12.0)*s,vec2(23.0,13.0)*s));
    res=opU(res,line(p,vec2(23.0,13.0)*s,vec2(22.0,14.0)*s));
    res=opU(res,line(p,vec2(22.0,14.0)*s,vec2(21.0,14.0)*s));
    res=opU(res,line(p,vec2(21.0,14.0)*s,vec2(20.0,13.0)*s));
    res=opU(res,line(p,vec2(20.0,13.0)*s,vec2(19.0,11.0)*s));
    res=opU(res,line(p,vec2(19.0,11.0)*s,vec2(17.0,6.0)*s));
    res=opU(res,line(p,vec2(17.0,6.0)*s,vec2(15.0,3.0)*s));
    res=opU(res,line(p,vec2(15.0,3.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(7.0,0.0)*s,vec2(5.0,1.0)*s));
    res=opU(res,line(p,vec2(5.0,1.0)*s,vec2(4.0,2.0)*s));
    res=opU(res,line(p,vec2(4.0,2.0)*s,vec2(3.0,4.0)*s));
    res=opU(res,line(p,vec2(3.0,4.0)*s,vec2(3.0,6.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(4.0,8.0)*s));
    res=opU(res,line(p,vec2(4.0,8.0)*s,vec2(5.0,9.0)*s));
    res=opU(res,line(p,vec2(5.0,9.0)*s,vec2(12.0,13.0)*s));
    res=opU(res,line(p,vec2(12.0,13.0)*s,vec2(13.0,14.0)*s));
    res=opU(res,line(p,vec2(13.0,14.0)*s,vec2(14.0,16.0)*s));
    res=opU(res,line(p,vec2(14.0,16.0)*s,vec2(14.0,18.0)*s));
    res=opU(res,line(p,vec2(14.0,18.0)*s,vec2(13.0,20.0)*s));
    res=opU(res,line(p,vec2(13.0,20.0)*s,vec2(11.0,21.0)*s));
    res=opU(res,line(p,vec2(11.0,21.0)*s,vec2(9.0,20.0)*s));
    res=opU(res,line(p,vec2(9.0,20.0)*s,vec2(8.0,18.0)*s));
    res=opU(res,line(p,vec2(8.0,18.0)*s,vec2(8.0,16.0)*s));
    res=opU(res,line(p,vec2(8.0,16.0)*s,vec2(9.0,13.0)*s));
    res=opU(res,line(p,vec2(9.0,13.0)*s,vec2(11.0,10.0)*s));
    res=opU(res,line(p,vec2(11.0,10.0)*s,vec2(16.0,3.0)*s));
    res=opU(res,line(p,vec2(16.0,3.0)*s,vec2(18.0,1.0)*s));
    res=opU(res,line(p,vec2(18.0,1.0)*s,vec2(20.0,0.0)*s));
    res=opU(res,line(p,vec2(20.0,0.0)*s,vec2(22.0,0.0)*s));
    res=opU(res,line(p,vec2(22.0,0.0)*s,vec2(23.0,1.0)*s));
    res=opU(res,line(p,vec2(23.0,1.0)*s,vec2(23.0,2.0)*s));
    cpos.x+=26.0*s.x;
}


// CHAR: 39 :'
void char_39(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,19.0)*s,vec2(4.0,20.0)*s));
    res=opU(res,line(p,vec2(4.0,20.0)*s,vec2(5.0,21.0)*s));
    res=opU(res,line(p,vec2(5.0,21.0)*s,vec2(6.0,20.0)*s));
    res=opU(res,line(p,vec2(6.0,20.0)*s,vec2(6.0,18.0)*s));
    res=opU(res,line(p,vec2(6.0,18.0)*s,vec2(5.0,16.0)*s));
    res=opU(res,line(p,vec2(5.0,16.0)*s,vec2(4.0,15.0)*s));
    cpos.x+=10.0*s.x;
}


// CHAR: 40 :(
void char_40(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(11.0,25.0)*s,vec2(9.0,23.0)*s));
    res=opU(res,line(p,vec2(9.0,23.0)*s,vec2(7.0,20.0)*s));
    res=opU(res,line(p,vec2(7.0,20.0)*s,vec2(5.0,16.0)*s));
    res=opU(res,line(p,vec2(5.0,16.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(4.0,7.0)*s));
    res=opU(res,line(p,vec2(4.0,7.0)*s,vec2(5.0,2.0)*s));
    res=opU(res,line(p,vec2(5.0,2.0)*s,vec2(7.0,-2.0)*s));
    res=opU(res,line(p,vec2(7.0,-2.0)*s,vec2(9.0,-5.0)*s));
    res=opU(res,line(p,vec2(9.0,-5.0)*s,vec2(11.0,-7.0)*s));
    cpos.x+=14.0*s.x;
}


// CHAR: 41 :)
void char_41(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(3.0,25.0)*s,vec2(5.0,23.0)*s));
    res=opU(res,line(p,vec2(5.0,23.0)*s,vec2(7.0,20.0)*s));
    res=opU(res,line(p,vec2(7.0,20.0)*s,vec2(9.0,16.0)*s));
    res=opU(res,line(p,vec2(9.0,16.0)*s,vec2(10.0,11.0)*s));
    res=opU(res,line(p,vec2(10.0,11.0)*s,vec2(10.0,7.0)*s));
    res=opU(res,line(p,vec2(10.0,7.0)*s,vec2(9.0,2.0)*s));
    res=opU(res,line(p,vec2(9.0,2.0)*s,vec2(7.0,-2.0)*s));
    res=opU(res,line(p,vec2(7.0,-2.0)*s,vec2(5.0,-5.0)*s));
    res=opU(res,line(p,vec2(5.0,-5.0)*s,vec2(3.0,-7.0)*s));
    cpos.x+=14.0*s.x;
}


// CHAR: 42 :*
void char_42(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(8.0,9.0)*s));
    res=opU(res,line(p,vec2(3.0,18.0)*s,vec2(13.0,12.0)*s));
    res=opU(res,line(p,vec2(13.0,18.0)*s,vec2(3.0,12.0)*s));
    cpos.x+=16.0*s.x;
}


// CHAR: 43 :+
void char_43(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(13.0,18.0)*s,vec2(13.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,9.0)*s,vec2(22.0,9.0)*s));
    cpos.x+=26.0*s.x;
}


// CHAR: 44 :,
void char_44(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(5.0,0.0)*s));
    res=opU(res,line(p,vec2(5.0,0.0)*s,vec2(4.0,1.0)*s));
    res=opU(res,line(p,vec2(4.0,1.0)*s,vec2(5.0,2.0)*s));
    res=opU(res,line(p,vec2(5.0,2.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(6.0,-1.0)*s));
    res=opU(res,line(p,vec2(6.0,-1.0)*s,vec2(5.0,-3.0)*s));
    res=opU(res,line(p,vec2(5.0,-3.0)*s,vec2(4.0,-4.0)*s));
    cpos.x+=10.0*s.x;
}


// CHAR: 45 :-
void char_45(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,9.0)*s,vec2(22.0,9.0)*s));
    cpos.x+=26.0*s.x;
}


// CHAR: 46 :.
void char_46(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,2.0)*s,vec2(4.0,1.0)*s));
    res=opU(res,line(p,vec2(4.0,1.0)*s,vec2(5.0,0.0)*s));
    res=opU(res,line(p,vec2(5.0,0.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(5.0,2.0)*s));
    cpos.x+=10.0*s.x;
}


// CHAR: 47 :/
void char_47(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(20.0,25.0)*s,vec2(2.0,-7.0)*s));
    cpos.x+=22.0*s.x;
}


// CHAR: 48 :0
void char_0(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(6.0,20.0)*s));
    res=opU(res,line(p,vec2(6.0,20.0)*s,vec2(4.0,17.0)*s));
    res=opU(res,line(p,vec2(4.0,17.0)*s,vec2(3.0,12.0)*s));
    res=opU(res,line(p,vec2(3.0,12.0)*s,vec2(3.0,9.0)*s));
    res=opU(res,line(p,vec2(3.0,9.0)*s,vec2(4.0,4.0)*s));
    res=opU(res,line(p,vec2(4.0,4.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(9.0,0.0)*s));
    res=opU(res,line(p,vec2(9.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(14.0,1.0)*s));
    res=opU(res,line(p,vec2(14.0,1.0)*s,vec2(16.0,4.0)*s));
    res=opU(res,line(p,vec2(16.0,4.0)*s,vec2(17.0,9.0)*s));
    res=opU(res,line(p,vec2(17.0,9.0)*s,vec2(17.0,12.0)*s));
    res=opU(res,line(p,vec2(17.0,12.0)*s,vec2(16.0,17.0)*s));
    res=opU(res,line(p,vec2(16.0,17.0)*s,vec2(14.0,20.0)*s));
    res=opU(res,line(p,vec2(14.0,20.0)*s,vec2(11.0,21.0)*s));
    res=opU(res,line(p,vec2(11.0,21.0)*s,vec2(9.0,21.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 49 :1
void char_1(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(6.0,17.0)*s,vec2(8.0,18.0)*s));
    res=opU(res,line(p,vec2(8.0,18.0)*s,vec2(11.0,21.0)*s));
    res=opU(res,line(p,vec2(11.0,21.0)*s,vec2(11.0,0.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 50 :2
void char_2(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,16.0)*s,vec2(4.0,17.0)*s));
    res=opU(res,line(p,vec2(4.0,17.0)*s,vec2(5.0,19.0)*s));
    res=opU(res,line(p,vec2(5.0,19.0)*s,vec2(6.0,20.0)*s));
    res=opU(res,line(p,vec2(6.0,20.0)*s,vec2(8.0,21.0)*s));
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(12.0,21.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(14.0,20.0)*s));
    res=opU(res,line(p,vec2(14.0,20.0)*s,vec2(15.0,19.0)*s));
    res=opU(res,line(p,vec2(15.0,19.0)*s,vec2(16.0,17.0)*s));
    res=opU(res,line(p,vec2(16.0,17.0)*s,vec2(16.0,15.0)*s));
    res=opU(res,line(p,vec2(16.0,15.0)*s,vec2(15.0,13.0)*s));
    res=opU(res,line(p,vec2(15.0,13.0)*s,vec2(13.0,10.0)*s));
    res=opU(res,line(p,vec2(13.0,10.0)*s,vec2(3.0,0.0)*s));
    res=opU(res,line(p,vec2(3.0,0.0)*s,vec2(17.0,0.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 51 :3
void char_3(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,21.0)*s,vec2(16.0,21.0)*s));
    res=opU(res,line(p,vec2(16.0,21.0)*s,vec2(10.0,13.0)*s));
    res=opU(res,line(p,vec2(10.0,13.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(15.0,12.0)*s));
    res=opU(res,line(p,vec2(15.0,12.0)*s,vec2(16.0,11.0)*s));
    res=opU(res,line(p,vec2(16.0,11.0)*s,vec2(17.0,8.0)*s));
    res=opU(res,line(p,vec2(17.0,8.0)*s,vec2(17.0,6.0)*s));
    res=opU(res,line(p,vec2(17.0,6.0)*s,vec2(16.0,3.0)*s));
    res=opU(res,line(p,vec2(16.0,3.0)*s,vec2(14.0,1.0)*s));
    res=opU(res,line(p,vec2(14.0,1.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(5.0,1.0)*s));
    res=opU(res,line(p,vec2(5.0,1.0)*s,vec2(4.0,2.0)*s));
    res=opU(res,line(p,vec2(4.0,2.0)*s,vec2(3.0,4.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 52 :4
void char_4(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(3.0,7.0)*s));
    res=opU(res,line(p,vec2(3.0,7.0)*s,vec2(18.0,7.0)*s));
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(13.0,0.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 53 :5
void char_5(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(15.0,21.0)*s,vec2(5.0,21.0)*s));
    res=opU(res,line(p,vec2(5.0,21.0)*s,vec2(4.0,12.0)*s));
    res=opU(res,line(p,vec2(4.0,12.0)*s,vec2(5.0,13.0)*s));
    res=opU(res,line(p,vec2(5.0,13.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(14.0,13.0)*s));
    res=opU(res,line(p,vec2(14.0,13.0)*s,vec2(16.0,11.0)*s));
    res=opU(res,line(p,vec2(16.0,11.0)*s,vec2(17.0,8.0)*s));
    res=opU(res,line(p,vec2(17.0,8.0)*s,vec2(17.0,6.0)*s));
    res=opU(res,line(p,vec2(17.0,6.0)*s,vec2(16.0,3.0)*s));
    res=opU(res,line(p,vec2(16.0,3.0)*s,vec2(14.0,1.0)*s));
    res=opU(res,line(p,vec2(14.0,1.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(5.0,1.0)*s));
    res=opU(res,line(p,vec2(5.0,1.0)*s,vec2(4.0,2.0)*s));
    res=opU(res,line(p,vec2(4.0,2.0)*s,vec2(3.0,4.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 54 :6
void char_6(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(16.0,18.0)*s,vec2(15.0,20.0)*s));
    res=opU(res,line(p,vec2(15.0,20.0)*s,vec2(12.0,21.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(10.0,21.0)*s));
    res=opU(res,line(p,vec2(10.0,21.0)*s,vec2(7.0,20.0)*s));
    res=opU(res,line(p,vec2(7.0,20.0)*s,vec2(5.0,17.0)*s));
    res=opU(res,line(p,vec2(5.0,17.0)*s,vec2(4.0,12.0)*s));
    res=opU(res,line(p,vec2(4.0,12.0)*s,vec2(4.0,7.0)*s));
    res=opU(res,line(p,vec2(4.0,7.0)*s,vec2(5.0,3.0)*s));
    res=opU(res,line(p,vec2(5.0,3.0)*s,vec2(7.0,1.0)*s));
    res=opU(res,line(p,vec2(7.0,1.0)*s,vec2(10.0,0.0)*s));
    res=opU(res,line(p,vec2(10.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(14.0,1.0)*s));
    res=opU(res,line(p,vec2(14.0,1.0)*s,vec2(16.0,3.0)*s));
    res=opU(res,line(p,vec2(16.0,3.0)*s,vec2(17.0,6.0)*s));
    res=opU(res,line(p,vec2(17.0,6.0)*s,vec2(17.0,7.0)*s));
    res=opU(res,line(p,vec2(17.0,7.0)*s,vec2(16.0,10.0)*s));
    res=opU(res,line(p,vec2(16.0,10.0)*s,vec2(14.0,12.0)*s));
    res=opU(res,line(p,vec2(14.0,12.0)*s,vec2(11.0,13.0)*s));
    res=opU(res,line(p,vec2(11.0,13.0)*s,vec2(10.0,13.0)*s));
    res=opU(res,line(p,vec2(10.0,13.0)*s,vec2(7.0,12.0)*s));
    res=opU(res,line(p,vec2(7.0,12.0)*s,vec2(5.0,10.0)*s));
    res=opU(res,line(p,vec2(5.0,10.0)*s,vec2(4.0,7.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 55 :7
void char_7(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(17.0,21.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(3.0,21.0)*s,vec2(17.0,21.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 56 :8
void char_8(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(5.0,20.0)*s));
    res=opU(res,line(p,vec2(5.0,20.0)*s,vec2(4.0,18.0)*s));
    res=opU(res,line(p,vec2(4.0,18.0)*s,vec2(4.0,16.0)*s));
    res=opU(res,line(p,vec2(4.0,16.0)*s,vec2(5.0,14.0)*s));
    res=opU(res,line(p,vec2(5.0,14.0)*s,vec2(7.0,13.0)*s));
    res=opU(res,line(p,vec2(7.0,13.0)*s,vec2(11.0,12.0)*s));
    res=opU(res,line(p,vec2(11.0,12.0)*s,vec2(14.0,11.0)*s));
    res=opU(res,line(p,vec2(14.0,11.0)*s,vec2(16.0,9.0)*s));
    res=opU(res,line(p,vec2(16.0,9.0)*s,vec2(17.0,7.0)*s));
    res=opU(res,line(p,vec2(17.0,7.0)*s,vec2(17.0,4.0)*s));
    res=opU(res,line(p,vec2(17.0,4.0)*s,vec2(16.0,2.0)*s));
    res=opU(res,line(p,vec2(16.0,2.0)*s,vec2(15.0,1.0)*s));
    res=opU(res,line(p,vec2(15.0,1.0)*s,vec2(12.0,0.0)*s));
    res=opU(res,line(p,vec2(12.0,0.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(5.0,1.0)*s));
    res=opU(res,line(p,vec2(5.0,1.0)*s,vec2(4.0,2.0)*s));
    res=opU(res,line(p,vec2(4.0,2.0)*s,vec2(3.0,4.0)*s));
    res=opU(res,line(p,vec2(3.0,4.0)*s,vec2(3.0,7.0)*s));
    res=opU(res,line(p,vec2(3.0,7.0)*s,vec2(4.0,9.0)*s));
    res=opU(res,line(p,vec2(4.0,9.0)*s,vec2(6.0,11.0)*s));
    res=opU(res,line(p,vec2(6.0,11.0)*s,vec2(9.0,12.0)*s));
    res=opU(res,line(p,vec2(9.0,12.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(15.0,14.0)*s));
    res=opU(res,line(p,vec2(15.0,14.0)*s,vec2(16.0,16.0)*s));
    res=opU(res,line(p,vec2(16.0,16.0)*s,vec2(16.0,18.0)*s));
    res=opU(res,line(p,vec2(16.0,18.0)*s,vec2(15.0,20.0)*s));
    res=opU(res,line(p,vec2(15.0,20.0)*s,vec2(12.0,21.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(8.0,21.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 57 :9
void char_9(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(16.0,14.0)*s,vec2(15.0,11.0)*s));
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(13.0,9.0)*s));
    res=opU(res,line(p,vec2(13.0,9.0)*s,vec2(10.0,8.0)*s));
    res=opU(res,line(p,vec2(10.0,8.0)*s,vec2(9.0,8.0)*s));
    res=opU(res,line(p,vec2(9.0,8.0)*s,vec2(6.0,9.0)*s));
    res=opU(res,line(p,vec2(6.0,9.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(3.0,14.0)*s));
    res=opU(res,line(p,vec2(3.0,14.0)*s,vec2(3.0,15.0)*s));
    res=opU(res,line(p,vec2(3.0,15.0)*s,vec2(4.0,18.0)*s));
    res=opU(res,line(p,vec2(4.0,18.0)*s,vec2(6.0,20.0)*s));
    res=opU(res,line(p,vec2(6.0,20.0)*s,vec2(9.0,21.0)*s));
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(10.0,21.0)*s));
    res=opU(res,line(p,vec2(10.0,21.0)*s,vec2(13.0,20.0)*s));
    res=opU(res,line(p,vec2(13.0,20.0)*s,vec2(15.0,18.0)*s));
    res=opU(res,line(p,vec2(15.0,18.0)*s,vec2(16.0,14.0)*s));
    res=opU(res,line(p,vec2(16.0,14.0)*s,vec2(16.0,9.0)*s));
    res=opU(res,line(p,vec2(16.0,9.0)*s,vec2(15.0,4.0)*s));
    res=opU(res,line(p,vec2(15.0,4.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(10.0,0.0)*s));
    res=opU(res,line(p,vec2(10.0,0.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(5.0,1.0)*s));
    res=opU(res,line(p,vec2(5.0,1.0)*s,vec2(4.0,3.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 58 ::
void char_58(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,14.0)*s,vec2(4.0,13.0)*s));
    res=opU(res,line(p,vec2(4.0,13.0)*s,vec2(5.0,12.0)*s));
    res=opU(res,line(p,vec2(5.0,12.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(5.0,14.0)*s));
    res=opU(res,line(p,vec2(5.0,2.0)*s,vec2(4.0,1.0)*s));
    res=opU(res,line(p,vec2(4.0,1.0)*s,vec2(5.0,0.0)*s));
    res=opU(res,line(p,vec2(5.0,0.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(5.0,2.0)*s));
    cpos.x+=10.0*s.x;
}


// CHAR: 59 :;
void char_59(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,14.0)*s,vec2(4.0,13.0)*s));
    res=opU(res,line(p,vec2(4.0,13.0)*s,vec2(5.0,12.0)*s));
    res=opU(res,line(p,vec2(5.0,12.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(5.0,14.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(5.0,0.0)*s));
    res=opU(res,line(p,vec2(5.0,0.0)*s,vec2(4.0,1.0)*s));
    res=opU(res,line(p,vec2(4.0,1.0)*s,vec2(5.0,2.0)*s));
    res=opU(res,line(p,vec2(5.0,2.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(6.0,-1.0)*s));
    res=opU(res,line(p,vec2(6.0,-1.0)*s,vec2(5.0,-3.0)*s));
    res=opU(res,line(p,vec2(5.0,-3.0)*s,vec2(4.0,-4.0)*s));
    cpos.x+=10.0*s.x;
}


// CHAR: 60 :<
void char_60(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(20.0,18.0)*s,vec2(4.0,9.0)*s));
    res=opU(res,line(p,vec2(4.0,9.0)*s,vec2(20.0,0.0)*s));
    cpos.x+=24.0*s.x;
}


// CHAR: 61 :=
void char_61(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,12.0)*s,vec2(22.0,12.0)*s));
    res=opU(res,line(p,vec2(4.0,6.0)*s,vec2(22.0,6.0)*s));
    cpos.x+=26.0*s.x;
}


// CHAR: 62 :>
void char_62(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,18.0)*s,vec2(20.0,9.0)*s));
    res=opU(res,line(p,vec2(20.0,9.0)*s,vec2(4.0,0.0)*s));
    cpos.x+=24.0*s.x;
}


// CHAR: 63 :?
void char_63(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(3.0,16.0)*s,vec2(3.0,17.0)*s));
    res=opU(res,line(p,vec2(3.0,17.0)*s,vec2(4.0,19.0)*s));
    res=opU(res,line(p,vec2(4.0,19.0)*s,vec2(5.0,20.0)*s));
    res=opU(res,line(p,vec2(5.0,20.0)*s,vec2(7.0,21.0)*s));
    res=opU(res,line(p,vec2(7.0,21.0)*s,vec2(11.0,21.0)*s));
    res=opU(res,line(p,vec2(11.0,21.0)*s,vec2(13.0,20.0)*s));
    res=opU(res,line(p,vec2(13.0,20.0)*s,vec2(14.0,19.0)*s));
    res=opU(res,line(p,vec2(14.0,19.0)*s,vec2(15.0,17.0)*s));
    res=opU(res,line(p,vec2(15.0,17.0)*s,vec2(15.0,15.0)*s));
    res=opU(res,line(p,vec2(15.0,15.0)*s,vec2(14.0,13.0)*s));
    res=opU(res,line(p,vec2(14.0,13.0)*s,vec2(13.0,12.0)*s));
    res=opU(res,line(p,vec2(13.0,12.0)*s,vec2(9.0,10.0)*s));
    res=opU(res,line(p,vec2(9.0,10.0)*s,vec2(9.0,7.0)*s));
    res=opU(res,line(p,vec2(9.0,2.0)*s,vec2(8.0,1.0)*s));
    res=opU(res,line(p,vec2(8.0,1.0)*s,vec2(9.0,0.0)*s));
    res=opU(res,line(p,vec2(9.0,0.0)*s,vec2(10.0,1.0)*s));
    res=opU(res,line(p,vec2(10.0,1.0)*s,vec2(9.0,2.0)*s));
    cpos.x+=18.0*s.x;
}


// CHAR: 64 :@
void char_64(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(18.0,13.0)*s,vec2(17.0,15.0)*s));
    res=opU(res,line(p,vec2(17.0,15.0)*s,vec2(15.0,16.0)*s));
    res=opU(res,line(p,vec2(15.0,16.0)*s,vec2(12.0,16.0)*s));
    res=opU(res,line(p,vec2(12.0,16.0)*s,vec2(10.0,15.0)*s));
    res=opU(res,line(p,vec2(10.0,15.0)*s,vec2(9.0,14.0)*s));
    res=opU(res,line(p,vec2(9.0,14.0)*s,vec2(8.0,11.0)*s));
    res=opU(res,line(p,vec2(8.0,11.0)*s,vec2(8.0,8.0)*s));
    res=opU(res,line(p,vec2(8.0,8.0)*s,vec2(9.0,6.0)*s));
    res=opU(res,line(p,vec2(9.0,6.0)*s,vec2(11.0,5.0)*s));
    res=opU(res,line(p,vec2(11.0,5.0)*s,vec2(14.0,5.0)*s));
    res=opU(res,line(p,vec2(14.0,5.0)*s,vec2(16.0,6.0)*s));
    res=opU(res,line(p,vec2(16.0,6.0)*s,vec2(17.0,8.0)*s));
    res=opU(res,line(p,vec2(12.0,16.0)*s,vec2(10.0,14.0)*s));
    res=opU(res,line(p,vec2(10.0,14.0)*s,vec2(9.0,11.0)*s));
    res=opU(res,line(p,vec2(9.0,11.0)*s,vec2(9.0,8.0)*s));
    res=opU(res,line(p,vec2(9.0,8.0)*s,vec2(10.0,6.0)*s));
    res=opU(res,line(p,vec2(10.0,6.0)*s,vec2(11.0,5.0)*s));
    res=opU(res,line(p,vec2(18.0,16.0)*s,vec2(17.0,8.0)*s));
    res=opU(res,line(p,vec2(17.0,8.0)*s,vec2(17.0,6.0)*s));
    res=opU(res,line(p,vec2(17.0,6.0)*s,vec2(19.0,5.0)*s));
    res=opU(res,line(p,vec2(19.0,5.0)*s,vec2(21.0,5.0)*s));
    res=opU(res,line(p,vec2(21.0,5.0)*s,vec2(23.0,7.0)*s));
    res=opU(res,line(p,vec2(23.0,7.0)*s,vec2(24.0,10.0)*s));
    res=opU(res,line(p,vec2(24.0,10.0)*s,vec2(24.0,12.0)*s));
    res=opU(res,line(p,vec2(24.0,12.0)*s,vec2(23.0,15.0)*s));
    res=opU(res,line(p,vec2(23.0,15.0)*s,vec2(22.0,17.0)*s));
    res=opU(res,line(p,vec2(22.0,17.0)*s,vec2(20.0,19.0)*s));
    res=opU(res,line(p,vec2(20.0,19.0)*s,vec2(18.0,20.0)*s));
    res=opU(res,line(p,vec2(18.0,20.0)*s,vec2(15.0,21.0)*s));
    res=opU(res,line(p,vec2(15.0,21.0)*s,vec2(12.0,21.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(9.0,20.0)*s));
    res=opU(res,line(p,vec2(9.0,20.0)*s,vec2(7.0,19.0)*s));
    res=opU(res,line(p,vec2(7.0,19.0)*s,vec2(5.0,17.0)*s));
    res=opU(res,line(p,vec2(5.0,17.0)*s,vec2(4.0,15.0)*s));
    res=opU(res,line(p,vec2(4.0,15.0)*s,vec2(3.0,12.0)*s));
    res=opU(res,line(p,vec2(3.0,12.0)*s,vec2(3.0,9.0)*s));
    res=opU(res,line(p,vec2(3.0,9.0)*s,vec2(4.0,6.0)*s));
    res=opU(res,line(p,vec2(4.0,6.0)*s,vec2(5.0,4.0)*s));
    res=opU(res,line(p,vec2(5.0,4.0)*s,vec2(7.0,2.0)*s));
    res=opU(res,line(p,vec2(7.0,2.0)*s,vec2(9.0,1.0)*s));
    res=opU(res,line(p,vec2(9.0,1.0)*s,vec2(12.0,0.0)*s));
    res=opU(res,line(p,vec2(12.0,0.0)*s,vec2(15.0,0.0)*s));
    res=opU(res,line(p,vec2(15.0,0.0)*s,vec2(18.0,1.0)*s));
    res=opU(res,line(p,vec2(18.0,1.0)*s,vec2(20.0,2.0)*s));
    res=opU(res,line(p,vec2(20.0,2.0)*s,vec2(21.0,3.0)*s));
    res=opU(res,line(p,vec2(19.0,16.0)*s,vec2(18.0,8.0)*s));
    res=opU(res,line(p,vec2(18.0,8.0)*s,vec2(18.0,6.0)*s));
    res=opU(res,line(p,vec2(18.0,6.0)*s,vec2(19.0,5.0)*s));
    cpos.x+=27.0*s.x;
}


// CHAR: 65 :A
void char_A(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(1.0,0.0)*s));
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(17.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,7.0)*s,vec2(14.0,7.0)*s));
    cpos.x+=18.0*s.x;
}


// CHAR: 66 :B
void char_B(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(13.0,21.0)*s));
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(16.0,20.0)*s));
    res=opU(res,line(p,vec2(16.0,20.0)*s,vec2(17.0,19.0)*s));
    res=opU(res,line(p,vec2(17.0,19.0)*s,vec2(18.0,17.0)*s));
    res=opU(res,line(p,vec2(18.0,17.0)*s,vec2(18.0,15.0)*s));
    res=opU(res,line(p,vec2(18.0,15.0)*s,vec2(17.0,13.0)*s));
    res=opU(res,line(p,vec2(17.0,13.0)*s,vec2(16.0,12.0)*s));
    res=opU(res,line(p,vec2(16.0,12.0)*s,vec2(13.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(13.0,11.0)*s));
    res=opU(res,line(p,vec2(13.0,11.0)*s,vec2(16.0,10.0)*s));
    res=opU(res,line(p,vec2(16.0,10.0)*s,vec2(17.0,9.0)*s));
    res=opU(res,line(p,vec2(17.0,9.0)*s,vec2(18.0,7.0)*s));
    res=opU(res,line(p,vec2(18.0,7.0)*s,vec2(18.0,4.0)*s));
    res=opU(res,line(p,vec2(18.0,4.0)*s,vec2(17.0,2.0)*s));
    res=opU(res,line(p,vec2(17.0,2.0)*s,vec2(16.0,1.0)*s));
    res=opU(res,line(p,vec2(16.0,1.0)*s,vec2(13.0,0.0)*s));
    res=opU(res,line(p,vec2(13.0,0.0)*s,vec2(4.0,0.0)*s));
    cpos.x+=21.0*s.x;
}


// CHAR: 67 :C
void char_C(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(18.0,16.0)*s,vec2(17.0,18.0)*s));
    res=opU(res,line(p,vec2(17.0,18.0)*s,vec2(15.0,20.0)*s));
    res=opU(res,line(p,vec2(15.0,20.0)*s,vec2(13.0,21.0)*s));
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(9.0,21.0)*s));
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(7.0,20.0)*s));
    res=opU(res,line(p,vec2(7.0,20.0)*s,vec2(5.0,18.0)*s));
    res=opU(res,line(p,vec2(5.0,18.0)*s,vec2(4.0,16.0)*s));
    res=opU(res,line(p,vec2(4.0,16.0)*s,vec2(3.0,13.0)*s));
    res=opU(res,line(p,vec2(3.0,13.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(4.0,5.0)*s));
    res=opU(res,line(p,vec2(4.0,5.0)*s,vec2(5.0,3.0)*s));
    res=opU(res,line(p,vec2(5.0,3.0)*s,vec2(7.0,1.0)*s));
    res=opU(res,line(p,vec2(7.0,1.0)*s,vec2(9.0,0.0)*s));
    res=opU(res,line(p,vec2(9.0,0.0)*s,vec2(13.0,0.0)*s));
    res=opU(res,line(p,vec2(13.0,0.0)*s,vec2(15.0,1.0)*s));
    res=opU(res,line(p,vec2(15.0,1.0)*s,vec2(17.0,3.0)*s));
    res=opU(res,line(p,vec2(17.0,3.0)*s,vec2(18.0,5.0)*s));
    cpos.x+=21.0*s.x;
}


// CHAR: 68 :D
void char_D(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(11.0,21.0)*s));
    res=opU(res,line(p,vec2(11.0,21.0)*s,vec2(14.0,20.0)*s));
    res=opU(res,line(p,vec2(14.0,20.0)*s,vec2(16.0,18.0)*s));
    res=opU(res,line(p,vec2(16.0,18.0)*s,vec2(17.0,16.0)*s));
    res=opU(res,line(p,vec2(17.0,16.0)*s,vec2(18.0,13.0)*s));
    res=opU(res,line(p,vec2(18.0,13.0)*s,vec2(18.0,8.0)*s));
    res=opU(res,line(p,vec2(18.0,8.0)*s,vec2(17.0,5.0)*s));
    res=opU(res,line(p,vec2(17.0,5.0)*s,vec2(16.0,3.0)*s));
    res=opU(res,line(p,vec2(16.0,3.0)*s,vec2(14.0,1.0)*s));
    res=opU(res,line(p,vec2(14.0,1.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(4.0,0.0)*s));
    cpos.x+=21.0*s.x;
}


// CHAR: 69 :E
void char_E(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(17.0,21.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(12.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,0.0)*s,vec2(17.0,0.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 70 :F
void char_F(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(17.0,21.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(12.0,11.0)*s));
    cpos.x+=18.0*s.x;
}


// CHAR: 71 :G
void char_G(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(18.0,16.0)*s,vec2(17.0,18.0)*s));
    res=opU(res,line(p,vec2(17.0,18.0)*s,vec2(15.0,20.0)*s));
    res=opU(res,line(p,vec2(15.0,20.0)*s,vec2(13.0,21.0)*s));
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(9.0,21.0)*s));
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(7.0,20.0)*s));
    res=opU(res,line(p,vec2(7.0,20.0)*s,vec2(5.0,18.0)*s));
    res=opU(res,line(p,vec2(5.0,18.0)*s,vec2(4.0,16.0)*s));
    res=opU(res,line(p,vec2(4.0,16.0)*s,vec2(3.0,13.0)*s));
    res=opU(res,line(p,vec2(3.0,13.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(4.0,5.0)*s));
    res=opU(res,line(p,vec2(4.0,5.0)*s,vec2(5.0,3.0)*s));
    res=opU(res,line(p,vec2(5.0,3.0)*s,vec2(7.0,1.0)*s));
    res=opU(res,line(p,vec2(7.0,1.0)*s,vec2(9.0,0.0)*s));
    res=opU(res,line(p,vec2(9.0,0.0)*s,vec2(13.0,0.0)*s));
    res=opU(res,line(p,vec2(13.0,0.0)*s,vec2(15.0,1.0)*s));
    res=opU(res,line(p,vec2(15.0,1.0)*s,vec2(17.0,3.0)*s));
    res=opU(res,line(p,vec2(17.0,3.0)*s,vec2(18.0,5.0)*s));
    res=opU(res,line(p,vec2(18.0,5.0)*s,vec2(18.0,8.0)*s));
    res=opU(res,line(p,vec2(13.0,8.0)*s,vec2(18.0,8.0)*s));
    cpos.x+=21.0*s.x;
}


// CHAR: 72 :H
void char_H(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(18.0,21.0)*s,vec2(18.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(18.0,11.0)*s));
    cpos.x+=22.0*s.x;
}


// CHAR: 73 :I
void char_I(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    cpos.x+=8.0*s.x;
}


// CHAR: 74 :J
void char_J(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(12.0,5.0)*s));
    res=opU(res,line(p,vec2(12.0,5.0)*s,vec2(11.0,2.0)*s));
    res=opU(res,line(p,vec2(11.0,2.0)*s,vec2(10.0,1.0)*s));
    res=opU(res,line(p,vec2(10.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(6.0,0.0)*s));
    res=opU(res,line(p,vec2(6.0,0.0)*s,vec2(4.0,1.0)*s));
    res=opU(res,line(p,vec2(4.0,1.0)*s,vec2(3.0,2.0)*s));
    res=opU(res,line(p,vec2(3.0,2.0)*s,vec2(2.0,5.0)*s));
    res=opU(res,line(p,vec2(2.0,5.0)*s,vec2(2.0,7.0)*s));
    cpos.x+=16.0*s.x;
}


// CHAR: 75 :K
void char_K(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(18.0,21.0)*s,vec2(4.0,7.0)*s));
    res=opU(res,line(p,vec2(9.0,12.0)*s,vec2(18.0,0.0)*s));
    cpos.x+=21.0*s.x;
}


// CHAR: 76 :L
void char_L(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,0.0)*s,vec2(16.0,0.0)*s));
    cpos.x+=17.0*s.x;
}


// CHAR: 77 :M
void char_M(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(12.0,0.0)*s));
    res=opU(res,line(p,vec2(20.0,21.0)*s,vec2(12.0,0.0)*s));
    res=opU(res,line(p,vec2(20.0,21.0)*s,vec2(20.0,0.0)*s));
    cpos.x+=24.0*s.x;
}


// CHAR: 78 :N
void char_N(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(18.0,0.0)*s));
    res=opU(res,line(p,vec2(18.0,21.0)*s,vec2(18.0,0.0)*s));
    cpos.x+=22.0*s.x;
}


// CHAR: 79 :O
void char_O(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(7.0,20.0)*s));
    res=opU(res,line(p,vec2(7.0,20.0)*s,vec2(5.0,18.0)*s));
    res=opU(res,line(p,vec2(5.0,18.0)*s,vec2(4.0,16.0)*s));
    res=opU(res,line(p,vec2(4.0,16.0)*s,vec2(3.0,13.0)*s));
    res=opU(res,line(p,vec2(3.0,13.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(4.0,5.0)*s));
    res=opU(res,line(p,vec2(4.0,5.0)*s,vec2(5.0,3.0)*s));
    res=opU(res,line(p,vec2(5.0,3.0)*s,vec2(7.0,1.0)*s));
    res=opU(res,line(p,vec2(7.0,1.0)*s,vec2(9.0,0.0)*s));
    res=opU(res,line(p,vec2(9.0,0.0)*s,vec2(13.0,0.0)*s));
    res=opU(res,line(p,vec2(13.0,0.0)*s,vec2(15.0,1.0)*s));
    res=opU(res,line(p,vec2(15.0,1.0)*s,vec2(17.0,3.0)*s));
    res=opU(res,line(p,vec2(17.0,3.0)*s,vec2(18.0,5.0)*s));
    res=opU(res,line(p,vec2(18.0,5.0)*s,vec2(19.0,8.0)*s));
    res=opU(res,line(p,vec2(19.0,8.0)*s,vec2(19.0,13.0)*s));
    res=opU(res,line(p,vec2(19.0,13.0)*s,vec2(18.0,16.0)*s));
    res=opU(res,line(p,vec2(18.0,16.0)*s,vec2(17.0,18.0)*s));
    res=opU(res,line(p,vec2(17.0,18.0)*s,vec2(15.0,20.0)*s));
    res=opU(res,line(p,vec2(15.0,20.0)*s,vec2(13.0,21.0)*s));
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(9.0,21.0)*s));
    cpos.x+=22.0*s.x;
}


// CHAR: 80 :P
void char_P(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(13.0,21.0)*s));
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(16.0,20.0)*s));
    res=opU(res,line(p,vec2(16.0,20.0)*s,vec2(17.0,19.0)*s));
    res=opU(res,line(p,vec2(17.0,19.0)*s,vec2(18.0,17.0)*s));
    res=opU(res,line(p,vec2(18.0,17.0)*s,vec2(18.0,14.0)*s));
    res=opU(res,line(p,vec2(18.0,14.0)*s,vec2(17.0,12.0)*s));
    res=opU(res,line(p,vec2(17.0,12.0)*s,vec2(16.0,11.0)*s));
    res=opU(res,line(p,vec2(16.0,11.0)*s,vec2(13.0,10.0)*s));
    res=opU(res,line(p,vec2(13.0,10.0)*s,vec2(4.0,10.0)*s));
    cpos.x+=21.0*s.x;
}


// CHAR: 81 :Q
void char_Q(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(7.0,20.0)*s));
    res=opU(res,line(p,vec2(7.0,20.0)*s,vec2(5.0,18.0)*s));
    res=opU(res,line(p,vec2(5.0,18.0)*s,vec2(4.0,16.0)*s));
    res=opU(res,line(p,vec2(4.0,16.0)*s,vec2(3.0,13.0)*s));
    res=opU(res,line(p,vec2(3.0,13.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(4.0,5.0)*s));
    res=opU(res,line(p,vec2(4.0,5.0)*s,vec2(5.0,3.0)*s));
    res=opU(res,line(p,vec2(5.0,3.0)*s,vec2(7.0,1.0)*s));
    res=opU(res,line(p,vec2(7.0,1.0)*s,vec2(9.0,0.0)*s));
    res=opU(res,line(p,vec2(9.0,0.0)*s,vec2(13.0,0.0)*s));
    res=opU(res,line(p,vec2(13.0,0.0)*s,vec2(15.0,1.0)*s));
    res=opU(res,line(p,vec2(15.0,1.0)*s,vec2(17.0,3.0)*s));
    res=opU(res,line(p,vec2(17.0,3.0)*s,vec2(18.0,5.0)*s));
    res=opU(res,line(p,vec2(18.0,5.0)*s,vec2(19.0,8.0)*s));
    res=opU(res,line(p,vec2(19.0,8.0)*s,vec2(19.0,13.0)*s));
    res=opU(res,line(p,vec2(19.0,13.0)*s,vec2(18.0,16.0)*s));
    res=opU(res,line(p,vec2(18.0,16.0)*s,vec2(17.0,18.0)*s));
    res=opU(res,line(p,vec2(17.0,18.0)*s,vec2(15.0,20.0)*s));
    res=opU(res,line(p,vec2(15.0,20.0)*s,vec2(13.0,21.0)*s));
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(9.0,21.0)*s));
    res=opU(res,line(p,vec2(12.0,4.0)*s,vec2(18.0,-2.0)*s));
    cpos.x+=22.0*s.x;
}


// CHAR: 82 :R
void char_R(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(13.0,21.0)*s));
    res=opU(res,line(p,vec2(13.0,21.0)*s,vec2(16.0,20.0)*s));
    res=opU(res,line(p,vec2(16.0,20.0)*s,vec2(17.0,19.0)*s));
    res=opU(res,line(p,vec2(17.0,19.0)*s,vec2(18.0,17.0)*s));
    res=opU(res,line(p,vec2(18.0,17.0)*s,vec2(18.0,15.0)*s));
    res=opU(res,line(p,vec2(18.0,15.0)*s,vec2(17.0,13.0)*s));
    res=opU(res,line(p,vec2(17.0,13.0)*s,vec2(16.0,12.0)*s));
    res=opU(res,line(p,vec2(16.0,12.0)*s,vec2(13.0,11.0)*s));
    res=opU(res,line(p,vec2(13.0,11.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(11.0,11.0)*s,vec2(18.0,0.0)*s));
    cpos.x+=21.0*s.x;
}


// CHAR: 83 :S
void char_S(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(17.0,18.0)*s,vec2(15.0,20.0)*s));
    res=opU(res,line(p,vec2(15.0,20.0)*s,vec2(12.0,21.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(8.0,21.0)*s));
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(5.0,20.0)*s));
    res=opU(res,line(p,vec2(5.0,20.0)*s,vec2(3.0,18.0)*s));
    res=opU(res,line(p,vec2(3.0,18.0)*s,vec2(3.0,16.0)*s));
    res=opU(res,line(p,vec2(3.0,16.0)*s,vec2(4.0,14.0)*s));
    res=opU(res,line(p,vec2(4.0,14.0)*s,vec2(5.0,13.0)*s));
    res=opU(res,line(p,vec2(5.0,13.0)*s,vec2(7.0,12.0)*s));
    res=opU(res,line(p,vec2(7.0,12.0)*s,vec2(13.0,10.0)*s));
    res=opU(res,line(p,vec2(13.0,10.0)*s,vec2(15.0,9.0)*s));
    res=opU(res,line(p,vec2(15.0,9.0)*s,vec2(16.0,8.0)*s));
    res=opU(res,line(p,vec2(16.0,8.0)*s,vec2(17.0,6.0)*s));
    res=opU(res,line(p,vec2(17.0,6.0)*s,vec2(17.0,3.0)*s));
    res=opU(res,line(p,vec2(17.0,3.0)*s,vec2(15.0,1.0)*s));
    res=opU(res,line(p,vec2(15.0,1.0)*s,vec2(12.0,0.0)*s));
    res=opU(res,line(p,vec2(12.0,0.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(5.0,1.0)*s));
    res=opU(res,line(p,vec2(5.0,1.0)*s,vec2(3.0,3.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 84 :T
void char_T(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(1.0,21.0)*s,vec2(15.0,21.0)*s));
    cpos.x+=16.0*s.x;
}


// CHAR: 85 :U
void char_U(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,6.0)*s));
    res=opU(res,line(p,vec2(4.0,6.0)*s,vec2(5.0,3.0)*s));
    res=opU(res,line(p,vec2(5.0,3.0)*s,vec2(7.0,1.0)*s));
    res=opU(res,line(p,vec2(7.0,1.0)*s,vec2(10.0,0.0)*s));
    res=opU(res,line(p,vec2(10.0,0.0)*s,vec2(12.0,0.0)*s));
    res=opU(res,line(p,vec2(12.0,0.0)*s,vec2(15.0,1.0)*s));
    res=opU(res,line(p,vec2(15.0,1.0)*s,vec2(17.0,3.0)*s));
    res=opU(res,line(p,vec2(17.0,3.0)*s,vec2(18.0,6.0)*s));
    res=opU(res,line(p,vec2(18.0,6.0)*s,vec2(18.0,21.0)*s));
    cpos.x+=22.0*s.x;
}


// CHAR: 86 :V
void char_V(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(1.0,21.0)*s,vec2(9.0,0.0)*s));
    res=opU(res,line(p,vec2(17.0,21.0)*s,vec2(9.0,0.0)*s));
    cpos.x+=18.0*s.x;
}


// CHAR: 87 :W
void char_W(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(2.0,21.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(12.0,21.0)*s,vec2(17.0,0.0)*s));
    res=opU(res,line(p,vec2(22.0,21.0)*s,vec2(17.0,0.0)*s));
    cpos.x+=24.0*s.x;
}


// CHAR: 88 :X
void char_X(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(3.0,21.0)*s,vec2(17.0,0.0)*s));
    res=opU(res,line(p,vec2(17.0,21.0)*s,vec2(3.0,0.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 89 :Y
void char_Y(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(1.0,21.0)*s,vec2(9.0,11.0)*s));
    res=opU(res,line(p,vec2(9.0,11.0)*s,vec2(9.0,0.0)*s));
    res=opU(res,line(p,vec2(17.0,21.0)*s,vec2(9.0,11.0)*s));
    cpos.x+=18.0*s.x;
}


// CHAR: 90 :Z
void char_Z(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(17.0,21.0)*s,vec2(3.0,0.0)*s));
    res=opU(res,line(p,vec2(3.0,21.0)*s,vec2(17.0,21.0)*s));
    res=opU(res,line(p,vec2(3.0,0.0)*s,vec2(17.0,0.0)*s));
    cpos.x+=20.0*s.x;
}


// CHAR: 91 :[
void char_91(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,25.0)*s,vec2(4.0,-7.0)*s));
    res=opU(res,line(p,vec2(5.0,25.0)*s,vec2(5.0,-7.0)*s));
    res=opU(res,line(p,vec2(4.0,25.0)*s,vec2(11.0,25.0)*s));
    res=opU(res,line(p,vec2(4.0,-7.0)*s,vec2(11.0,-7.0)*s));
    cpos.x+=14.0*s.x;
}


// CHAR: 92 :\;
void char_92(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(0.0,21.0)*s,vec2(14.0,-3.0)*s));
    cpos.x+=14.0*s.x;
}


// CHAR: 93 :]
void char_93(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(9.0,25.0)*s,vec2(9.0,-7.0)*s));
    res=opU(res,line(p,vec2(10.0,25.0)*s,vec2(10.0,-7.0)*s));
    res=opU(res,line(p,vec2(3.0,25.0)*s,vec2(10.0,25.0)*s));
    res=opU(res,line(p,vec2(3.0,-7.0)*s,vec2(10.0,-7.0)*s));
    cpos.x+=14.0*s.x;
}


// CHAR: 94 :^
void char_94(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(6.0,15.0)*s,vec2(8.0,18.0)*s));
    res=opU(res,line(p,vec2(8.0,18.0)*s,vec2(10.0,15.0)*s));
    res=opU(res,line(p,vec2(3.0,12.0)*s,vec2(8.0,17.0)*s));
    res=opU(res,line(p,vec2(8.0,17.0)*s,vec2(13.0,12.0)*s));
    res=opU(res,line(p,vec2(8.0,17.0)*s,vec2(8.0,0.0)*s));
    cpos.x+=16.0*s.x;
}


// CHAR: 95 :_
void char_95(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(0.0,-2.0)*s,vec2(16.0,-2.0)*s));
    cpos.x+=16.0*s.x;
}


// CHAR: 96 :`
void char_96(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(6.0,21.0)*s,vec2(5.0,20.0)*s));
    res=opU(res,line(p,vec2(5.0,20.0)*s,vec2(4.0,18.0)*s));
    res=opU(res,line(p,vec2(4.0,18.0)*s,vec2(4.0,16.0)*s));
    res=opU(res,line(p,vec2(4.0,16.0)*s,vec2(5.0,15.0)*s));
    res=opU(res,line(p,vec2(5.0,15.0)*s,vec2(6.0,16.0)*s));
    res=opU(res,line(p,vec2(6.0,16.0)*s,vec2(5.0,17.0)*s));
    cpos.x+=10.0*s.x;
}


// CHAR: 97 :a
void char_a(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(15.0,14.0)*s,vec2(15.0,0.0)*s));
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(3.0,6.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(4.0,3.0)*s));
    res=opU(res,line(p,vec2(4.0,3.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(15.0,3.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 98 :b
void char_b(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(15.0,11.0)*s));
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(16.0,8.0)*s));
    res=opU(res,line(p,vec2(16.0,8.0)*s,vec2(16.0,6.0)*s));
    res=opU(res,line(p,vec2(16.0,6.0)*s,vec2(15.0,3.0)*s));
    res=opU(res,line(p,vec2(15.0,3.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(4.0,3.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 99 :c
void char_c(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(3.0,6.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(4.0,3.0)*s));
    res=opU(res,line(p,vec2(4.0,3.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(15.0,3.0)*s));
    cpos.x+=18.0*s.x;
}


// CHAR: 100 :d
void char_d(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(15.0,21.0)*s,vec2(15.0,0.0)*s));
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(3.0,6.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(4.0,3.0)*s));
    res=opU(res,line(p,vec2(4.0,3.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(15.0,3.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 101 :e
void char_e(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(15.0,8.0)*s));
    res=opU(res,line(p,vec2(15.0,8.0)*s,vec2(15.0,10.0)*s));
    res=opU(res,line(p,vec2(15.0,10.0)*s,vec2(14.0,12.0)*s));
    res=opU(res,line(p,vec2(14.0,12.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(3.0,6.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(4.0,3.0)*s));
    res=opU(res,line(p,vec2(4.0,3.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(15.0,3.0)*s));
    cpos.x+=18.0*s.x;
}


// CHAR: 102 :f
void char_f(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(10.0,21.0)*s,vec2(8.0,21.0)*s));
    res=opU(res,line(p,vec2(8.0,21.0)*s,vec2(6.0,20.0)*s));
    res=opU(res,line(p,vec2(6.0,20.0)*s,vec2(5.0,17.0)*s));
    res=opU(res,line(p,vec2(5.0,17.0)*s,vec2(5.0,0.0)*s));
    res=opU(res,line(p,vec2(2.0,14.0)*s,vec2(9.0,14.0)*s));
    cpos.x+=12.0*s.x;
}


// CHAR: 103 :g
void char_g(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(15.0,14.0)*s,vec2(15.0,-2.0)*s));
    res=opU(res,line(p,vec2(15.0,-2.0)*s,vec2(14.0,-5.0)*s));
    res=opU(res,line(p,vec2(14.0,-5.0)*s,vec2(13.0,-6.0)*s));
    res=opU(res,line(p,vec2(13.0,-6.0)*s,vec2(11.0,-7.0)*s));
    res=opU(res,line(p,vec2(11.0,-7.0)*s,vec2(8.0,-7.0)*s));
    res=opU(res,line(p,vec2(8.0,-7.0)*s,vec2(6.0,-6.0)*s));
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(3.0,6.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(4.0,3.0)*s));
    res=opU(res,line(p,vec2(4.0,3.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(15.0,3.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 104 :h
void char_h(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,10.0)*s,vec2(7.0,13.0)*s));
    res=opU(res,line(p,vec2(7.0,13.0)*s,vec2(9.0,14.0)*s));
    res=opU(res,line(p,vec2(9.0,14.0)*s,vec2(12.0,14.0)*s));
    res=opU(res,line(p,vec2(12.0,14.0)*s,vec2(14.0,13.0)*s));
    res=opU(res,line(p,vec2(14.0,13.0)*s,vec2(15.0,10.0)*s));
    res=opU(res,line(p,vec2(15.0,10.0)*s,vec2(15.0,0.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 105 :i
void char_i(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(3.0,21.0)*s,vec2(4.0,20.0)*s));
    res=opU(res,line(p,vec2(4.0,20.0)*s,vec2(5.0,21.0)*s));
    res=opU(res,line(p,vec2(5.0,21.0)*s,vec2(4.0,22.0)*s));
    res=opU(res,line(p,vec2(4.0,22.0)*s,vec2(3.0,21.0)*s));
    res=opU(res,line(p,vec2(4.0,14.0)*s,vec2(4.0,0.0)*s));
    cpos.x+=8.0*s.x;
}


// CHAR: 106 :j
void char_j(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,21.0)*s,vec2(6.0,20.0)*s));
    res=opU(res,line(p,vec2(6.0,20.0)*s,vec2(7.0,21.0)*s));
    res=opU(res,line(p,vec2(7.0,21.0)*s,vec2(6.0,22.0)*s));
    res=opU(res,line(p,vec2(6.0,22.0)*s,vec2(5.0,21.0)*s));
    res=opU(res,line(p,vec2(6.0,14.0)*s,vec2(6.0,-3.0)*s));
    res=opU(res,line(p,vec2(6.0,-3.0)*s,vec2(5.0,-6.0)*s));
    res=opU(res,line(p,vec2(5.0,-6.0)*s,vec2(3.0,-7.0)*s));
    res=opU(res,line(p,vec2(3.0,-7.0)*s,vec2(1.0,-7.0)*s));
    cpos.x+=10.0*s.x;
}


// CHAR: 107 :k
void char_k(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(14.0,14.0)*s,vec2(4.0,4.0)*s));
    res=opU(res,line(p,vec2(8.0,8.0)*s,vec2(15.0,0.0)*s));
    cpos.x+=17.0*s.x;
}


// CHAR: 108 :l
void char_l(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,21.0)*s,vec2(4.0,0.0)*s));
    cpos.x+=8.0*s.x;
}


// CHAR: 109 :m
void char_m(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,14.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,10.0)*s,vec2(7.0,13.0)*s));
    res=opU(res,line(p,vec2(7.0,13.0)*s,vec2(9.0,14.0)*s));
    res=opU(res,line(p,vec2(9.0,14.0)*s,vec2(12.0,14.0)*s));
    res=opU(res,line(p,vec2(12.0,14.0)*s,vec2(14.0,13.0)*s));
    res=opU(res,line(p,vec2(14.0,13.0)*s,vec2(15.0,10.0)*s));
    res=opU(res,line(p,vec2(15.0,10.0)*s,vec2(15.0,0.0)*s));
    res=opU(res,line(p,vec2(15.0,10.0)*s,vec2(18.0,13.0)*s));
    res=opU(res,line(p,vec2(18.0,13.0)*s,vec2(20.0,14.0)*s));
    res=opU(res,line(p,vec2(20.0,14.0)*s,vec2(23.0,14.0)*s));
    res=opU(res,line(p,vec2(23.0,14.0)*s,vec2(25.0,13.0)*s));
    res=opU(res,line(p,vec2(25.0,13.0)*s,vec2(26.0,10.0)*s));
    res=opU(res,line(p,vec2(26.0,10.0)*s,vec2(26.0,0.0)*s));
    cpos.x+=30.0*s.x;
}


// CHAR: 110 :n
void char_n(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,14.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,10.0)*s,vec2(7.0,13.0)*s));
    res=opU(res,line(p,vec2(7.0,13.0)*s,vec2(9.0,14.0)*s));
    res=opU(res,line(p,vec2(9.0,14.0)*s,vec2(12.0,14.0)*s));
    res=opU(res,line(p,vec2(12.0,14.0)*s,vec2(14.0,13.0)*s));
    res=opU(res,line(p,vec2(14.0,13.0)*s,vec2(15.0,10.0)*s));
    res=opU(res,line(p,vec2(15.0,10.0)*s,vec2(15.0,0.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 111 :o
void char_o(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(3.0,6.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(4.0,3.0)*s));
    res=opU(res,line(p,vec2(4.0,3.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(15.0,3.0)*s));
    res=opU(res,line(p,vec2(15.0,3.0)*s,vec2(16.0,6.0)*s));
    res=opU(res,line(p,vec2(16.0,6.0)*s,vec2(16.0,8.0)*s));
    res=opU(res,line(p,vec2(16.0,8.0)*s,vec2(15.0,11.0)*s));
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(8.0,14.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 112 :p
void char_p(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,14.0)*s,vec2(4.0,-7.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(15.0,11.0)*s));
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(16.0,8.0)*s));
    res=opU(res,line(p,vec2(16.0,8.0)*s,vec2(16.0,6.0)*s));
    res=opU(res,line(p,vec2(16.0,6.0)*s,vec2(15.0,3.0)*s));
    res=opU(res,line(p,vec2(15.0,3.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(4.0,3.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 113 :q
void char_q(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(15.0,14.0)*s,vec2(15.0,-7.0)*s));
    res=opU(res,line(p,vec2(15.0,11.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(11.0,14.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(6.0,13.0)*s));
    res=opU(res,line(p,vec2(6.0,13.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(3.0,6.0)*s));
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(4.0,3.0)*s));
    res=opU(res,line(p,vec2(4.0,3.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(11.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,0.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(15.0,3.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 114 :r
void char_r(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,14.0)*s,vec2(4.0,0.0)*s));
    res=opU(res,line(p,vec2(4.0,8.0)*s,vec2(5.0,11.0)*s));
    res=opU(res,line(p,vec2(5.0,11.0)*s,vec2(7.0,13.0)*s));
    res=opU(res,line(p,vec2(7.0,13.0)*s,vec2(9.0,14.0)*s));
    res=opU(res,line(p,vec2(9.0,14.0)*s,vec2(12.0,14.0)*s));
    cpos.x+=13.0*s.x;
}


// CHAR: 115 :s
void char_s(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(14.0,11.0)*s,vec2(13.0,13.0)*s));
    res=opU(res,line(p,vec2(13.0,13.0)*s,vec2(10.0,14.0)*s));
    res=opU(res,line(p,vec2(10.0,14.0)*s,vec2(7.0,14.0)*s));
    res=opU(res,line(p,vec2(7.0,14.0)*s,vec2(4.0,13.0)*s));
    res=opU(res,line(p,vec2(4.0,13.0)*s,vec2(3.0,11.0)*s));
    res=opU(res,line(p,vec2(3.0,11.0)*s,vec2(4.0,9.0)*s));
    res=opU(res,line(p,vec2(4.0,9.0)*s,vec2(6.0,8.0)*s));
    res=opU(res,line(p,vec2(6.0,8.0)*s,vec2(11.0,7.0)*s));
    res=opU(res,line(p,vec2(11.0,7.0)*s,vec2(13.0,6.0)*s));
    res=opU(res,line(p,vec2(13.0,6.0)*s,vec2(14.0,4.0)*s));
    res=opU(res,line(p,vec2(14.0,4.0)*s,vec2(14.0,3.0)*s));
    res=opU(res,line(p,vec2(14.0,3.0)*s,vec2(13.0,1.0)*s));
    res=opU(res,line(p,vec2(13.0,1.0)*s,vec2(10.0,0.0)*s));
    res=opU(res,line(p,vec2(10.0,0.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(7.0,0.0)*s,vec2(4.0,1.0)*s));
    res=opU(res,line(p,vec2(4.0,1.0)*s,vec2(3.0,3.0)*s));
    cpos.x+=17.0*s.x;
}


// CHAR: 116 :t
void char_t(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,21.0)*s,vec2(5.0,4.0)*s));
    res=opU(res,line(p,vec2(5.0,4.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(10.0,0.0)*s));
    res=opU(res,line(p,vec2(2.0,14.0)*s,vec2(9.0,14.0)*s));
    cpos.x+=12.0*s.x;
}


// CHAR: 117 :u
void char_u(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,14.0)*s,vec2(4.0,4.0)*s));
    res=opU(res,line(p,vec2(4.0,4.0)*s,vec2(5.0,1.0)*s));
    res=opU(res,line(p,vec2(5.0,1.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(7.0,0.0)*s,vec2(10.0,0.0)*s));
    res=opU(res,line(p,vec2(10.0,0.0)*s,vec2(12.0,1.0)*s));
    res=opU(res,line(p,vec2(12.0,1.0)*s,vec2(15.0,4.0)*s));
    res=opU(res,line(p,vec2(15.0,14.0)*s,vec2(15.0,0.0)*s));
    cpos.x+=19.0*s.x;
}


// CHAR: 118 :v
void char_v(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(2.0,14.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(14.0,14.0)*s,vec2(8.0,0.0)*s));
    cpos.x+=16.0*s.x;
}


// CHAR: 119 :w
void char_w(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(3.0,14.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(11.0,14.0)*s,vec2(15.0,0.0)*s));
    res=opU(res,line(p,vec2(19.0,14.0)*s,vec2(15.0,0.0)*s));
    cpos.x+=22.0*s.x;
}


// CHAR: 120 :x
void char_x(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(3.0,14.0)*s,vec2(14.0,0.0)*s));
    res=opU(res,line(p,vec2(14.0,14.0)*s,vec2(3.0,0.0)*s));
    cpos.x+=17.0*s.x;
}


// CHAR: 121 :y
void char_y(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(2.0,14.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(14.0,14.0)*s,vec2(8.0,0.0)*s));
    res=opU(res,line(p,vec2(8.0,0.0)*s,vec2(6.0,-4.0)*s));
    res=opU(res,line(p,vec2(6.0,-4.0)*s,vec2(4.0,-6.0)*s));
    res=opU(res,line(p,vec2(4.0,-6.0)*s,vec2(2.0,-7.0)*s));
    res=opU(res,line(p,vec2(2.0,-7.0)*s,vec2(1.0,-7.0)*s));
    cpos.x+=16.0*s.x;
}


// CHAR: 122 :z
void char_z(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(14.0,14.0)*s,vec2(3.0,0.0)*s));
    res=opU(res,line(p,vec2(3.0,14.0)*s,vec2(14.0,14.0)*s));
    res=opU(res,line(p,vec2(3.0,0.0)*s,vec2(14.0,0.0)*s));
    cpos.x+=17.0*s.x;
}


// CHAR: 123 :{
void char_123(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(9.0,25.0)*s,vec2(7.0,24.0)*s));
    res=opU(res,line(p,vec2(7.0,24.0)*s,vec2(6.0,23.0)*s));
    res=opU(res,line(p,vec2(6.0,23.0)*s,vec2(5.0,21.0)*s));
    res=opU(res,line(p,vec2(5.0,21.0)*s,vec2(5.0,19.0)*s));
    res=opU(res,line(p,vec2(5.0,19.0)*s,vec2(6.0,17.0)*s));
    res=opU(res,line(p,vec2(6.0,17.0)*s,vec2(7.0,16.0)*s));
    res=opU(res,line(p,vec2(7.0,16.0)*s,vec2(8.0,14.0)*s));
    res=opU(res,line(p,vec2(8.0,14.0)*s,vec2(8.0,12.0)*s));
    res=opU(res,line(p,vec2(8.0,12.0)*s,vec2(6.0,10.0)*s));
    res=opU(res,line(p,vec2(7.0,24.0)*s,vec2(6.0,22.0)*s));
    res=opU(res,line(p,vec2(6.0,22.0)*s,vec2(6.0,20.0)*s));
    res=opU(res,line(p,vec2(6.0,20.0)*s,vec2(7.0,18.0)*s));
    res=opU(res,line(p,vec2(7.0,18.0)*s,vec2(8.0,17.0)*s));
    res=opU(res,line(p,vec2(8.0,17.0)*s,vec2(9.0,15.0)*s));
    res=opU(res,line(p,vec2(9.0,15.0)*s,vec2(9.0,13.0)*s));
    res=opU(res,line(p,vec2(9.0,13.0)*s,vec2(8.0,11.0)*s));
    res=opU(res,line(p,vec2(8.0,11.0)*s,vec2(4.0,9.0)*s));
    res=opU(res,line(p,vec2(4.0,9.0)*s,vec2(8.0,7.0)*s));
    res=opU(res,line(p,vec2(8.0,7.0)*s,vec2(9.0,5.0)*s));
    res=opU(res,line(p,vec2(9.0,5.0)*s,vec2(9.0,3.0)*s));
    res=opU(res,line(p,vec2(9.0,3.0)*s,vec2(8.0,1.0)*s));
    res=opU(res,line(p,vec2(8.0,1.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(7.0,0.0)*s,vec2(6.0,-2.0)*s));
    res=opU(res,line(p,vec2(6.0,-2.0)*s,vec2(6.0,-4.0)*s));
    res=opU(res,line(p,vec2(6.0,-4.0)*s,vec2(7.0,-6.0)*s));
    res=opU(res,line(p,vec2(6.0,8.0)*s,vec2(8.0,6.0)*s));
    res=opU(res,line(p,vec2(8.0,6.0)*s,vec2(8.0,4.0)*s));
    res=opU(res,line(p,vec2(8.0,4.0)*s,vec2(7.0,2.0)*s));
    res=opU(res,line(p,vec2(7.0,2.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(5.0,-1.0)*s));
    res=opU(res,line(p,vec2(5.0,-1.0)*s,vec2(5.0,-3.0)*s));
    res=opU(res,line(p,vec2(5.0,-3.0)*s,vec2(6.0,-5.0)*s));
    res=opU(res,line(p,vec2(6.0,-5.0)*s,vec2(7.0,-6.0)*s));
    res=opU(res,line(p,vec2(7.0,-6.0)*s,vec2(9.0,-7.0)*s));
    cpos.x+=14.0*s.x;
}

// CHAR: 124 :|
void char_124(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(4.0,25.0)*s,vec2(4.0,-7.0)*s));
    cpos.x+=8.0*s.x;
}

// CHAR: 125 :}
void char_125(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(5.0,25.0)*s,vec2(7.0,24.0)*s));
    res=opU(res,line(p,vec2(7.0,24.0)*s,vec2(8.0,23.0)*s));
    res=opU(res,line(p,vec2(8.0,23.0)*s,vec2(9.0,21.0)*s));
    res=opU(res,line(p,vec2(9.0,21.0)*s,vec2(9.0,19.0)*s));
    res=opU(res,line(p,vec2(9.0,19.0)*s,vec2(8.0,17.0)*s));
    res=opU(res,line(p,vec2(8.0,17.0)*s,vec2(7.0,16.0)*s));
    res=opU(res,line(p,vec2(7.0,16.0)*s,vec2(6.0,14.0)*s));
    res=opU(res,line(p,vec2(6.0,14.0)*s,vec2(6.0,12.0)*s));
    res=opU(res,line(p,vec2(6.0,12.0)*s,vec2(8.0,10.0)*s));
    res=opU(res,line(p,vec2(7.0,24.0)*s,vec2(8.0,22.0)*s));
    res=opU(res,line(p,vec2(8.0,22.0)*s,vec2(8.0,20.0)*s));
    res=opU(res,line(p,vec2(8.0,20.0)*s,vec2(7.0,18.0)*s));
    res=opU(res,line(p,vec2(7.0,18.0)*s,vec2(6.0,17.0)*s));
    res=opU(res,line(p,vec2(6.0,17.0)*s,vec2(5.0,15.0)*s));
    res=opU(res,line(p,vec2(5.0,15.0)*s,vec2(5.0,13.0)*s));
    res=opU(res,line(p,vec2(5.0,13.0)*s,vec2(6.0,11.0)*s));
    res=opU(res,line(p,vec2(6.0,11.0)*s,vec2(10.0,9.0)*s));
    res=opU(res,line(p,vec2(10.0,9.0)*s,vec2(6.0,7.0)*s));
    res=opU(res,line(p,vec2(6.0,7.0)*s,vec2(5.0,5.0)*s));
    res=opU(res,line(p,vec2(5.0,5.0)*s,vec2(5.0,3.0)*s));
    res=opU(res,line(p,vec2(5.0,3.0)*s,vec2(6.0,1.0)*s));
    res=opU(res,line(p,vec2(6.0,1.0)*s,vec2(7.0,0.0)*s));
    res=opU(res,line(p,vec2(7.0,0.0)*s,vec2(8.0,-2.0)*s));
    res=opU(res,line(p,vec2(8.0,-2.0)*s,vec2(8.0,-4.0)*s));
    res=opU(res,line(p,vec2(8.0,-4.0)*s,vec2(7.0,-6.0)*s));
    res=opU(res,line(p,vec2(8.0,8.0)*s,vec2(6.0,6.0)*s));
    res=opU(res,line(p,vec2(6.0,6.0)*s,vec2(6.0,4.0)*s));
    res=opU(res,line(p,vec2(6.0,4.0)*s,vec2(7.0,2.0)*s));
    res=opU(res,line(p,vec2(7.0,2.0)*s,vec2(8.0,1.0)*s));
    res=opU(res,line(p,vec2(8.0,1.0)*s,vec2(9.0,-1.0)*s));
    res=opU(res,line(p,vec2(9.0,-1.0)*s,vec2(9.0,-3.0)*s));
    res=opU(res,line(p,vec2(9.0,-3.0)*s,vec2(8.0,-5.0)*s));
    res=opU(res,line(p,vec2(8.0,-5.0)*s,vec2(7.0,-6.0)*s));
    res=opU(res,line(p,vec2(7.0,-6.0)*s,vec2(5.0,-7.0)*s));
    cpos.x+=14.0*s.x;
}


// CHAR: 126 :~
void char_126(vec2 uv,vec2 s){
    vec2 p=uv-cpos;
    res=opU(res,line(p,vec2(3.0,6.0)*s,vec2(3.0,8.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(4.0,11.0)*s));
    res=opU(res,line(p,vec2(4.0,11.0)*s,vec2(6.0,12.0)*s));
    res=opU(res,line(p,vec2(6.0,12.0)*s,vec2(8.0,12.0)*s));
    res=opU(res,line(p,vec2(8.0,12.0)*s,vec2(10.0,11.0)*s));
    res=opU(res,line(p,vec2(10.0,11.0)*s,vec2(14.0,8.0)*s));
    res=opU(res,line(p,vec2(14.0,8.0)*s,vec2(16.0,7.0)*s));
    res=opU(res,line(p,vec2(16.0,7.0)*s,vec2(18.0,7.0)*s));
    res=opU(res,line(p,vec2(18.0,7.0)*s,vec2(20.0,8.0)*s));
    res=opU(res,line(p,vec2(20.0,8.0)*s,vec2(21.0,10.0)*s));
    res=opU(res,line(p,vec2(3.0,8.0)*s,vec2(4.0,10.0)*s));
    res=opU(res,line(p,vec2(4.0,10.0)*s,vec2(6.0,11.0)*s));
    res=opU(res,line(p,vec2(6.0,11.0)*s,vec2(8.0,11.0)*s));
    res=opU(res,line(p,vec2(8.0,11.0)*s,vec2(10.0,10.0)*s));
    res=opU(res,line(p,vec2(10.0,10.0)*s,vec2(14.0,7.0)*s));
    res=opU(res,line(p,vec2(14.0,7.0)*s,vec2(16.0,6.0)*s));
    res=opU(res,line(p,vec2(16.0,6.0)*s,vec2(18.0,6.0)*s));
    res=opU(res,line(p,vec2(18.0,6.0)*s,vec2(20.0,7.0)*s));
    res=opU(res,line(p,vec2(20.0,7.0)*s,vec2(21.0,10.0)*s));
    res=opU(res,line(p,vec2(21.0,10.0)*s,vec2(21.0,12.0)*s));
    cpos.x+=24.0*s.x;
}

// END DATA


void main()
	{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	uv.y/=RENDERSIZE.x/RENDERSIZE.y;
	res=0.;
	vec2 s=vec2(0.0025);    
	vec2 p = gl_FragCoord.xy / gl_FragCoord.xy;
#ifdef XL_SHADER
	uv*=1000.;
#endif

	// colors:
	vec3 col=vec3(0.4,0.25,0.0);  // letters
	vec3 outline=vec3(1.,1.,1.); // letter outline
	vec3 background=vec3(0.6, 0.0, 1.0);  // background

	cpos=vec2(0.261 ,0.32);
		width=0.004;
		char_H(uv,s);
		char_e(uv,s);
		char_l(uv,s);
		char_l(uv,s);
		char_o(uv,s);
		char_44(uv,s);
		char_32(uv,s);

		char_W(uv,s);
		char_o(uv,s);
		char_r(uv,s);
		char_l(uv,s);
		char_d(uv,s);
		char_33(uv,s);

		cpos=vec2(0.176 ,0.19);
		width=0.002;
		char_S(uv,s);
		char_i(uv,s);
		char_m(uv,s);
		char_p(uv,s);
		char_l(uv,s);
		char_e(uv,s);
		char_x(uv,s);
		char_32(uv,s);

		char_H(uv,s);
		char_e(uv,s);
		char_r(uv,s);
		char_s(uv,s);
		char_h(uv,s);
		char_e(uv,s);
		char_y(uv,s);

		cpos=vec2(0.279 ,0.09);
		width=0.002;
		char_V(uv,s);
		char_e(uv,s);
		char_c(uv,s);
		char_t(uv,s);
		char_o(uv,s);
		char_r(uv,s);
		char_32(uv,s);

		char_F(uv,s);
		char_o(uv,s);
		char_n(uv,s);
		char_t(uv,s);
		
	float aares=clamp(antiAlias(res), 0.0, 1.);
	outline*=smoothstep(1.,0.4,aares)*smoothstep(0.,0.1,aares);
//	gl_FragColor = vec4(mix(background,vec3((col)+outline),aares),1.0);

	vec3 oCol = background;
	if (length(aares) > 0.001) oCol = vec3((col)+outline)*aares;
	gl_FragColor = vec4(oCol,1.0);
	}
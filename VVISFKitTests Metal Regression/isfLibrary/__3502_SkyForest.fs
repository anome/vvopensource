/*{
	"CREDIT": "by mojovideotech",
   	"CATEGORIES" : [
     	"generator",
    	"procedural"
  	],
	"DESCRIPTION" : "based on https://www.shadertoy.com/view/MtfGRB by fizzer. ",
	"INPUTS" : []
}*/

////////////////////////////////////////////////////////////
// SkyForest  by mojovideotech
//
// based on :
// shadertoy.com/view/MtfGRB  by fizzer. 
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


void main() {
    vec2 q = gl_FragCoord.xy/RENDERSIZE.x+0.125,p;
    q *= 0.85;
    p.y -= 0.9;
    vec4 o = mix(vec4(0.6,0.0,0.6,1.0),vec4(1.0,0.0,0.0,1.0),q.y);
    float T = TIME*0.125;
    for(float i=19.0;i>0.0;--i) {
        p = q*i;
        p.x += T;
        p = cos(p.x+vec2(0.0,1.5))*sqrt(p.y-cos(T-o.w)+cos(p.x+i)+atan(o.y-T,q.y)/o.z)*0.95;
        for(int j=0;j<35;++j)
            p = reflect(p+sin(float(30-j)+T)*0.125,p.yx+cos(T+q.xy)*0.025)+p*0.125;
        o = gl_FragColor = dot(p,q)<3.0?o-o:o;
    }
}
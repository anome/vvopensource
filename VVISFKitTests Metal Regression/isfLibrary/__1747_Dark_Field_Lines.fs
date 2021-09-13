/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"NAME": "iChannel0",
			"TYPE": "image"
		}
      	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

/*
	Dark Field Lines
	04/2016
	by seb chevrel
    https://www.shadertoy.com/view/lstSR7
*/

#define PI 3.1415926535898
#define TWO_PI 6.28318530717959

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	float ar=iResolution.y/iResolution.x;
	vec2 uv = (fragCoord.xy / iResolution.xx) - vec2(0.5,0.5*ar);
    
    // polar uv
    float r = atan(uv.y,uv.x);
    float nr = (r+PI)/TWO_PI;
    float t = length(uv);
    
    // audio info
    float wave=texture2D(iChannel0,vec2(nr,0.75)).x;
    #define EXP 2.5
    float bass=pow(texture2D(iChannel0,vec2(0.0,0.25)).x,EXP);
    float bass2=pow(texture2D(iChannel0,vec2(0.15,0.25)).x,EXP);
    float mid=pow(texture2D(iChannel0,vec2(0.3,0.25)).x,EXP);
    float mid2=pow(texture2D(iChannel0,vec2(0.5,0.25)).x,EXP);
    float mid3=pow(texture2D(iChannel0,vec2(0.6,0.25)).x,EXP);
    float hi=pow(texture2D(iChannel0,vec2(0.75,0.25)).x,EXP);
    float hi2=pow(texture2D(iChannel0,vec2(0.9,0.25)).x,EXP);
    
    // background                
    vec3 color=(1.0-pow(t,0.3))*vec3(1,1,1)*0.8;
    
    //white glow
    color+=(1.0-smoothstep(0.0,bass*0.25,t))* vec3(1,1,1);
    
    // rotate in polar space
    float time=iGlobalTime;
    r+=time*0.1;
    
    // 1/t polar transform
    uv=vec2(cos(r),sin(r))/pow(t,0.8);
    uv+= vec2( time*0.5, time*0.7);
      
    // layers
    float cellSize=0.5; float halfCellSize=cellSize/2.0;
    //color +=((mod(uv.x,cellSize) < halfCellSize) ^^ (mod(uv.y,cellSize) < halfCellSize)==true) ? vec3(1.0) : vec3(0.0);
    color +=((mod(uv.x,cellSize) < halfCellSize) ==true) ? vec3(0.01) : vec3(0.0);  
    color += clamp(sin(uv.x*20.0),0.0,1.0)*mid2 *vec3(1,1,1);
    color -= clamp(sin(uv.y*20.0),0.0,1.0)*mid *vec3(1,1,1);
    color += clamp(sin(uv.x*40.0)*sin(uv.y*40.0),0.0,1.0)*hi *vec3(1,1,1);
    color += clamp(cos(-uv.x*1.0)*cos(-uv.y*1.0),-1.0,1.0)*bass2 *vec3(1,1,1);
    float s=mid2*0.2+mid3*0.1; color -= (1.0-smoothstep(s,s+0.05,t))*0.8;
    

    fragColor=vec4( vec3(color), 1.0);
    
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
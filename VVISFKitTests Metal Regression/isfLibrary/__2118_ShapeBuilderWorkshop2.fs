/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	
	]
}*/

// ShapeBuilderWorkshop2 by mojovideotech
// more experiments with various shape building methods


    float circle(in vec2 pos, in float radius)
{
	return 1.-smoothstep(radius-(radius*0.005),radius+(radius*0.005),dot(pos,pos)*2.0);
}

void main()
{
	vec2 st =  gl_FragCoord.xy/RENDERSIZE.xy;
	  st.x *= RENDERSIZE.x/RENDERSIZE.y;
	vec2 su = (2.0*gl_FragCoord.xy-RENDERSIZE)/RENDERSIZE.y;
	vec2 sv = ((2.0 * gl_FragCoord.xy ) - RENDERSIZE) / RENDERSIZE.y ;

    int N=9;
    float aa=atan(su.x,su.y)+.2;
    float b=6.28319/float(N);
    float ff = (smoothstep(.61,.6, cos(floor(.5+aa/b)*b-aa)*length(su.xy)));

    vec2 pos = su;
    float r = length(pos)*2.0;
    float a = atan(pos.y,pos.x);
    float f = smoothstep(-.5,1., cos(a*11.)-0.6)*0.5+0.7;
    float color = ( 1.-smoothstep(f,f+0.03,r) );

    float color1 = pow(distance(sv,vec2(0.5)),distance(sv,vec2(-0.3)));
    
    float color2 = distance(st,vec2(0.5));
    
    float color3 = (circle(su,0.667));

	gl_FragColor = vec4(mix(vec3(color2,color,color3),vec3(color,ff,color1),0.3),1.0);
}


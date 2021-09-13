/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	]
}*/

// ShapeBuilderWorkshop by mojovideotch
// experimenting with various methods of building simple shapes in [0.0-1.0] space


    float circle(in vec2 pos, in float radius)
{
	return 1.-smoothstep(radius-(radius*0.005),radius+(radius*0.005),dot(pos,pos)*2.0);
}

void main() {

	vec2 c = (( gl_FragCoord.xy )-0.5*RENDERSIZE)/ RENDERSIZE.y ;
    vec2 p = (2.0*gl_FragCoord.xy-RENDERSIZE)/RENDERSIZE.y;
    vec2 uv = 3.0 * vec2(gl_FragCoord.x / RENDERSIZE.x-.5, gl_FragCoord.y / RENDERSIZE.y -.5);

    float s=1.0001-length(c.xy*c.xy*c.xy*c.xy);
    float color = (smoothstep(0.996,0.9961,s));
    
    float ss = 1.-length(pow(uv.xy,vec2(0.35)));    
    float color2 = (circle(p,0.667));
          color2 += (smoothstep(0.0,0.005,ss));
    
    float sc = 1.-length(pow(uv.xy,vec2(0.44)));
          sc -= 1.-length(pow(c.xy,vec2(0.19)));
    float color3 = (smoothstep(0.0,0.01,sc));
          
    
    gl_FragColor = vec4( color2, color, color3, 1.0 );
	
}


  /*
  	vec2 cn = -1.0 + 2.0 * gl_FragCoord.xy/ RENDERSIZE.xy;
    vec2 cc = vec2 (cn.x, gl_FragCoord.y/RENDERSIZE.x);
      vec2 f = gl_FragCoord.xy/ RENDERSIZE;
  
  vec2 c =  gl_FragCoord.xy / vec2 (0.5 - RENDERSIZE.xy)+0.5;
    vec3 sky = mix( vec3(.0,.0,.5), vec3(.0,.5,1.), c.y*.5+.5);
    float circle = smoothstep(.8,.9,c.y-length(c.xy));
    vec4 col = vec4(mix( sky, vec3(0.6), circle),1.);
    
vec4 col = vec4(mix( vec3(.0,.0,.5), vec3(.0,.5,1.), c.y*.5+.5),1.);

	uv.y*=RENDERSIZE.y/RENDERSIZE.x;

float pct = distance(st,vec2(0.5));
	vec3 color = vec3(circle(st,0.5))-vec3(0.5,0.5,0.0);
	
	vec2 toCenter = vec2(0.5)-st;
    float oct = length(toCenter);
         color *= vec3(oct);

       vec2 toCenter = vec2(1.)-f;
       float cs = length(toCenter);
       
vec2 st = gl_FragCoord.xy/u_resolution.xy;
  st.x *= u_resolution.x/u_resolution.y;
  vec3 color = vec3(0.0);
  
vec2 c = gl_FragCoord.xy/RENDERSIZE.xy;
c.x *= RENDERSIZE.x / RENDERSIZE.y;
c = c * 2.0 - 1.0;

    float col  = distance(p,vec2(0.005));

gl_FragColor = vec4( vec3(color3), 1.0

gl_FragColor -= vec4( 0.0,0.0,col, 0.1 );

  */
/*{
	"CREDIT": "by mojovideotech",
   "CATEGORIES" : [
    "Generator",
    "Quad Tree",
    "eyeballs"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
   	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	13.0,
		"MIN" : 		2.0,
		"MAX" : 		16.0
	},
	{
   		"NAME" : 		"grid",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	false
   	}
  ],
   "ISFVSN" : 2.0
}
*/


#define     twpi    6.283185307179586   // two pi, 2*pi

vec2 hash22(vec2 p) { return fract(vec2(22271.0, 72221.0)*sin(dot(p, vec2(16061.0, 10601.0)))); }

void main() 
{
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy*0.5)/RENDERSIZE.y;
    vec2 oP = uv*(18.0-scale) + vec2(0.5, TIME*0.5);
    vec3 bg = vec3(0.05)*vec3(0.8, 0.5, 1.0);
    float pat = clamp(sin((oP.x - oP.y)*twpi*RENDERSIZE.y/50.5) + 0.75, 0.0, 1.0);
    vec2 hoP = hash22(oP);
    bg *= (hoP.x*0.15 + 1.0)*(pat*0.35 + 0.65);
    vec3 col = bg;
    vec4 d = vec4(1e5);
    float dim = 1.0;
    vec2 rndTh[3];
    rndTh[0] = vec2(0.333, 0.667);
    rndTh[1] = vec2(0.667, 0.667);
    rndTh[2] = vec2(1.0, 0.667);
    for(int k=0; k<3; k++){
    	vec2 ip = floor(oP*dim);        
        vec2 rnd = hash22(ip);
        if(rnd.x<rndTh[k].x){
            vec2 p = oP - (ip + 0.5)/dim;
            rnd = fract(rnd*181.0 + float(k*191 + 1));
            vec2 off = (rnd - 0.5)*0.33/dim;
            d.x = length(p - off) - 0.3/dim;
 			const float lwg = 0.015;
            d.y = abs(max(abs(p.x), abs(p.y)) - 0.5/dim) - lwg/2.0;
            float fo = 4.0/RENDERSIZE.y;
            vec3 pCol = mix(col, vec3(0.8, 0.5, 1.0), 0.5 - pat*0.5)*max(1.0 - d.y/(lwg/2.0), 0.0);
            if (grid) {
                col = mix(col, vec3(0), (1.0 - smoothstep(0.0, fo*3.0, d.y - 0.02))*0.03);
                col = mix(col, pCol, (1.0 - smoothstep(0.0, fo, d.y))*0.05);
            	}
            fo = 10.0/RENDERSIZE.y/sqrt(dim);
            pCol = vec3(1.0, 0.1, 0.3);
            pCol = mix(pCol.xzy, pCol, step(0.5, fract(rnd.y*113.97 + 0.51)));
            float sh = max(0.8 - d.x*dim*2.0, 0.0); 
            col = mix(col, vec3(0), (1.0 - smoothstep(0.0, fo*5.0, d.x))*0.35);
            col = mix(col, vec3(0), 1.0 - smoothstep(0.0, fo, d.x));
            col = mix(col, pCol*sh, 1.0 - smoothstep(0.0, fo, d.x + 0.015));
			col = mix(col, vec3(0), 1.0 - smoothstep(0.0, fo, d.x + 0.25/1.4/dim));
			col = mix(col, pCol*0.6*(pat*0.5 + 0.5), 1.0 - smoothstep(0.0, fo, d.x + 0.25/1.4/dim + 0.015));
			col = mix(col, vec3(0), 1.0 - smoothstep(0.0, fo, d.x + 0.375/1.4/dim));
			col = mix(col, bg*0.8, 1.0 - smoothstep(0.0, fo, d.x + 0.375/1.4/dim + 0.015));
            break;
        }
        dim *= 2.0;        
    }    
    col = mix(col.xzy, col, sign(uv.y)*uv.y*uv.y*2.0 + 0.5);
    col *= max(1.05 - length(uv)*0.25, 0.5);
    gl_FragColor = vec4(sqrt(max(col, 0.0)), 1);
}

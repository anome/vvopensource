/*{
	"CREDIT": "by isakburstrom",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "mod1",
			"TYPE": "float"
		},
		{
			"NAME": "mod2",
			"TYPE": "float"
		},
		{
			"NAME": "mod3",
			"TYPE": "float"
		},
		{
			"NAME": "mod4",
			"TYPE": "float"
		}
	],
  "PASSES": [
    {
      "TARGET": "buffer",
      "persistent": true
    },
    {}
  ]
}*/

vec4 read(vec2 fragCoord, vec2 pos) {
    return IMG_NORM_PIXEL(buffer, fragCoord + pos);
}

void main() {
	if (PASSINDEX == 0)	{
	    vec4 fragColor = IMG_NORM_PIXEL(buffer, isf_FragNormCoord.xy);
	    vec2 fragCoord = isf_FragNormCoord.xy;
		vec4 u = vec4(0.0,0.0,0.0,0.0);
    	float spillAmount = clamp(0.01,0.0,0.7);
        
        float samp1 = length(IMG_NORM_PIXEL(buffer, isf_FragNormCoord.xy));
        float samp2 = length(IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy));

        float ang1 = (samp1)*3.14*2.0;
        ang1 = ang1 + samp1*4.0;
        float x1 = cos(ang1);
        float y1 = sin(ang1);
        
        float ang2 =samp2*3.14*2.0;
        
        //rotate!
        ang2 = ang2 + samp2;
        
        float x2 = cos(ang2);
        float y2 = sin(ang2);        
        
        u += read(fragCoord,vec2(x1,y1)*spillAmount*2.0)*3.0;
        u += read(fragCoord,vec2(x2,y2)*spillAmount*8.0)*5.0;
		u += read(fragCoord,vec2(0.0,0.0));
        
        u/=9.0;
        gl_FragColor = u;
	} else {
		gl_FragColor = mix(IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy), IMG_NORM_PIXEL(buffer, isf_FragNormCoord.xy), mod1);
	}
}
/*
void main() {
	if (PASSINDEX == 1)	{
		vec2 gid = vec2(isf_FragNormCoord[0], isf_FragNormCoord[1]);
		
	    vec4 l = IMG_NORM_PIXEL(buffer, gid - vec2(-1, 0));
	    vec4 r = IMG_NORM_PIXEL(buffer, gid - vec2(1, 0));
	    vec4 t = IMG_NORM_PIXEL(buffer, gid - vec2(0, 1));
	    vec4 b = IMG_NORM_PIXEL(buffer, gid - vec2(0, -1));
	    vec4 c = IMG_NORM_PIXEL(buffer, gid);
	    
	    vec4 m = max(c, max(l, max(r, max(t, b))));
	    vec4 result = m * 0.95 + IMG_NORM_PIXEL(inputImage, gid) * 0.05;
		
	} else {
		gl_FragColor = IMG_NORM_PIXEL(buffer, isf_FragNormCoord.xy);
	}
}*/
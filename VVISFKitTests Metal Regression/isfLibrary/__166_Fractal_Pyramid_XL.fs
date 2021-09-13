/*{
    "CATEGORIES": [
        "Fractal",
        "xLights"
    ],
    "CREDIT": "Conversion to ISF and modifications by PierreB",
    "DESCRIPTION": "Fractal Pyramid XL",
    "INPUTS": [
        {
            "DEFAULT": 0,
            "LABEL": "Color Mode",
            "LABELS": [
                "Shader Default                  ",
                "xLights Color Pallete (2 used)  "
            ],
            "NAME": "colorMode",
            "TYPE": "long",
            "VALUES": [
                0,
                1
            ]
        },
        {
            "DEFAULT": 0,
            "NAME": "reverse",
            "TYPE": "bool"
        },
        {
            "DEFAULT": 3,
            "MAX": 5,
            "MIN": 1,
            "NAME": "fractalSize",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.2,
            "MAX": 1,
            "MIN": 0.1,
            "NAME": "speed",
            "TYPE": "float"
        },
        {
            "DEFAULT": [
                0.2,
                0.6980392156862745,
                0.9019607843137255,
                1
            ],
            "LABEL": "Color 1",
            "NAME": "color1",
            "TYPE": "color"
        },
        {
            "DEFAULT": [
                1,
                0,
                1,
                1
            ],
            "LABEL": "Color 2",
            "NAME": "color2",
            "TYPE": "color"
        },
        {
            "DEFAULT": 8,
            "NAME": "fractalMultiplier",
            "TYPE": "long",
            "MAX": 12,
            "MIN": 4
        }
    ],
    "ISFVSN": "2"
}
*/

// Original: https://www.shadertoy.com/view/tsXBzS
// Original Creator: bradjamesgrant

vec3 palette(float d){
	vec3 col1 = vec3(0.2,0.7,0.9);
	vec3 col2 = vec3(1.,0.,1.);
	if(colorMode == 1) {
		col1 = vec3(color1);
		col2 = vec3(color2);
	}
	
	return mix(col1,col2,d);
}

vec2 rotate(vec2 p,float a){
	float c = cos(a);
    float s = sin(a);
    return p*mat2(c,s,-s,c);
}

float map(vec3 p){
    for( int i = 0; i< 12; ++i){
        if (i >= fractalMultiplier){break;}
        float t = TIME*speed;
        p.xz =rotate(p.xz,t);
        p.xy =rotate(p.xy,t*1.89);
        p.xz = abs(p.xz);
        p.xz-=.5;
	}
	return dot(sign(p),p)/5.;
}

vec4 rm (vec3 ro, vec3 rd){
    float t = 0.;
    vec3 col = vec3(0.);
    float d;
    for(float i =0.; i<64.; i++){
		vec3 p = ro + rd*t;
        d = map(p)*.5;
        if(d<0.02){
            break;
        }
        if(d>100.){
        	break;
        }

        col+=palette(length(p)*.1)/(400.*(d));
        t+=d;
    }
    return vec4(col,1./(d*100.));
}

void main()	{
    vec2 uv = (gl_FragCoord.xy-(RENDERSIZE.xy/2.))/RENDERSIZE.x;
	vec3 ro = vec3(0.,0.,-50.);
    
    ro.xz = rotate(ro.xz, TIME);
    
    vec3 cf = normalize(-ro);
    vec3 cs = normalize(cross(cf,vec3(0.,1.,0.)));
    vec3 cu = normalize(cross(cf,cs));
    
    float revmultiplier = 1.;
    if(reverse) {
        revmultiplier*=-1.;
    }
    
    vec3 uuv = ro+cf*fractalSize + uv.x*cs*revmultiplier + uv.y*cu;
    
    vec3 rd = normalize(uuv-ro);
    
    vec4 col = rm(ro,rd);
	
	//	gl_FragColor = col;
	gl_FragColor = vec4(col.rgb, 1);
	

}

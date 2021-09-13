/*{
	"CREDIT": "by nestor",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "angle1",
			"TYPE": "float",
			"DEFAULT": -0.5,
			"MIN": -3.14,
			"MAX": 3.14
		},
		{
			"NAME": "angle2",
			"TYPE": "float",
			"DEFAULT": -0.0,
			"MIN": -3.14,
			"MAX": 3.14
		},
			{
			"NAME": "angle3",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": -3.14,
			"MAX": 3.14
		},
			{
			"NAME": "lineScale",
			"TYPE": "float",
			"DEFAULT": 255,
			"MIN": 0,
			"MAX": 2048
		},
		{
			"NAME": "somePt",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0
			]
		},
		{
			"NAME": "rampMode",
			"TYPE" : "bool"
		},
		{
			"NAME": "circular",
			"TYPE" : "bool"
		}
	]
}*/


mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

vec2 rotated(vec2 _uv, float _angle){
    _uv -= vec2(0.5,0.5);
    _uv = rotate2d( _angle ) * _uv;
    _uv += vec2(0.5,0.5);
    return _uv;
}

float cosColor(float f, float m){
    return 0.5*cos(f*m)+0.5;
}

float sinColor(float f, float m){
    return 0.5*sin(f*m)+0.5;
}
float tanColor(float f){
    float s = 0.5*sin(f)+0.5;
    float c = 0.5*cos(f)+0.5;
    return s*c;
}

float fractColor(float f, float m){
    return fract(f*m);
}

float circ(vec2 xy){
    float s = 0.5*sin(xy.x)+0.5;
    float c = 0.5*cos(xy.y)+0.5;
    return s*c;
}

float random (vec2 st) {
    return fract(sin(dot(st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}

void main( void  )
{
	mat3 yuv2rgb = mat3(1.0, 0.0, 1.13983, 
                    1.0, -0.39465, -0.58060, 
                    1.0, 2.03211, 0.0);
                    
                    mat3 yuv2fck = mat3(1.0, 0.0, angle1, 
                    1.0, -0.39465, angle2, 
                    1.0, 2.03211, angle3);
	vec2 uv = isf_FragNormCoord.xy ;
       uv.x += -1.;
    
    vec2 uv2 = isf_FragNormCoord.xy ;
    
    uv2.x += 1.;
    
    vec2 uv3 = isf_FragNormCoord.xy ;

    
    //uv -= vec2(0.5f,0.5f);
    //uv = rotate2d( sin(iGlobalTime)*3.1416 ) * uv;
    //uv += vec2(0.5f,0.5f);
    
    uv = rotated(uv, angle1);
    uv2 = rotated(uv2, angle2);
    uv3 = rotated(uv3, angle3);
float d, d2,d3;
   if(circular){
   	d = length(vec2(uv - somePt));
    d2 = length(vec2(uv2 - somePt));
    d3 = length(vec2(uv3 - somePt));
   }else{
   	d = length(vec2(vec2(0.5, uv.y) - somePt));
    d2 = length(vec2(vec2(0.5, uv2.y) - somePt));
    d3 = length(vec2(vec2(0.5, uv3.y) - somePt));
   }
    //float d = pow( length( vec2( vec2(0.5, uv.y) - somePt) ),sin(uv.x)+0.5*2.0 );
    
    
    
    
    float c;
    
    //float c = cosColor(d*255.0);
    if(rampMode){
    	 c = fractColor(d*d3*(1.0-d2), lineScale+(random(uv)));
    }else{
    	 c = cosColor(d*d3*(1.0-d2), lineScale+(random(uv)));
    }
   
    //float c2 = cosColor(d*d3*d2, 120.);
    //float c2 = cosColor(pow(d2,d),240.);
    //float c3 = cosColor(d3, 270.);
    
    float c2 = sinColor(d*d2*(1.0-d3), lineScale+(random(uv))) ;
    
    //float c = max(cos(d*255.0), 0.0);
    //float c2 = max(cos(d2*127.0), 0.0) ;
    
    float cc = (c*c2);
    //cc = step(0.5, cc);
    vec3 color = yuv2fck * vec3(c, c2, c); 
    //gl_FragColor = vec4(color, 1.0);
    
    gl_FragColor = vec4(c2/c);
	//gl_FragColor = vec4(0.5, 1.0, 0.27, 1.0);
    //fragColor = vec4(uv,0.5+0.5*sin(iGlobalTime),1.0);
}

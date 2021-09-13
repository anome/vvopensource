/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		}
	]
}*/


mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

vec2 rotated(vec2 _uv, float _angle){
    _uv -= vec2(0.5f,0.5f);
    _uv = rotate2d( _angle ) * _uv;
    _uv += vec2(0.5f,0.5f);
    return _uv;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    vec2 uv2 = fragCoord.xy / iResolution.xy;
    
    vec2 somePt = vec2(0.5, 0);
    
    //uv -= vec2(0.5f,0.5f);
    //uv = rotate2d( sin(iGlobalTime)*3.1416 ) * uv;
    //uv += vec2(0.5f,0.5f);
    
    uv = rotated(uv, sin(iGlobalTime)*3.1416);
    uv2 = rotated(uv2, cos(iGlobalTime)*0.25);
    
    
    
    float d = length(vec2(vec2(uv.x,0) - somePt));
    float d2 = length(vec2(vec2(uv2.x,0) - somePt));
    
    float c = cos(d*255.0);
    float c2 = cos(d2*255.0);
    float cc = (c+c2)/2.0;
    
    fragColor = vec4(cc, cc-c, cc-c2, 1.0);
	
    //fragColor = vec4(uv,0.5+0.5*sin(iGlobalTime),1.0);
}

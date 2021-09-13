
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Bubbles",
		"Landscape"
	],
	"INPUTS": [

		{
			"NAME": "colorInput1",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.8,
				0.7,
				1.0
			]
		},
		{
			"NAME": "colorInput2",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.8,
				0.95,
				1.0
			]
		},

		{
			"NAME": "yd",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 1.0,
			"MAX": 10.0
		},
		
		{
			"NAME": "cDistFactor",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 0.01,
			"MAX": 0.5
		},
		
		{
			"NAME": "size",
			"TYPE": "float",
			"DEFAULT": 300.0,
			"MIN": 100.0,
			"MAX": 500.0
		},
		

		{
			"NAME": "sizexy",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/1.5",
			"HEIGHT": "$HEIGHT/1.5"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/


#define time TIME
#define R RENDERSIZE

#define GAMMA 0.8

// Blend a premultiplied rbga color onto rgb
vec3 premulBlend(vec4 src, vec3 dst){
    return dst * (1.0 - src.a) + src.rgb;
}

// Blend a premultiplied rbga color onto rgba (accurate alpha handling)
vec4 premulBlend(vec4 src, vec4 dst){
    vec4 res;
    res.rgb = dst.rgb * (1.0 - src.a) + src.rgb;
    res.a = 1.0 - (1.0 - src.a) * (1.0 - dst.a); 
    return res;
}


// Borrowed from BigWIngs (random 1 -> 4)
vec4 N14(float t) {
  return fract(sin(t*vec4(123., 104., 145., 24.))*vec4(657., 345., 879., 154.));
}

#define HASHSCALE 443.8975
vec2 hash22(vec2 p){
	vec3 p3 = fract(vec3(p.xyx) * HASHSCALE);
    p3 += dot(p3, p3.yzx+19.19);
    return fract(vec2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y)); 
}

// Compute a randomized Bokeh spot inside a grid cell
float spot(vec2 uv, vec2 id, float decimation){
    float accum = 0.0;
    for (float x = -1.0; x <= 1.0; x += 1.0){
        for (float y = -1.0; y <= 1.0; y += 1.0){
            vec2 offset = vec2(x, y);
            vec2 spotId = id + offset;
            
            float k = mod(spotId.x, size * sizexy.x) * 5.0 + mod(spotId.y, size * sizexy.y) * 6.5;
            
            vec2 rnd = hash22( spotId );
            
            vec2 spotUV = uv - offset + rnd.yx * 0.5;

            float dst = length(spotUV);

            float radSeed = sin(time * 0.01 + rnd.x * 50.0);
            float rad =  (abs(radSeed) - decimation) / (1.0 - decimation);

            float intensity = smoothstep(rad, rad - 0.15, dst);
            
            accum += intensity;
        }
    }
    return accum;
}

// Computes a random layer of bokeh spots
float layer(vec2 uv, float decimation){
    vec2 id = floor(uv);
    vec2 spotUV = (uv - id) - vec2(0.5, 0.5) ;
    float intensity = spot(spotUV, id, decimation);
    return intensity;
}


// Computes the bokeh background
vec3 background(vec2 uv){
    //accumulates several layers of bokeh
    float intensity = layer(uv * 1.5 + vec2( time * 0.12, 0.0), 0.75) * 0.1;
    //intensity += layer(uv * 1.0  + vec2(layer1 + iTime * 0.1, 134.0), 0.75) * 0.1;
    intensity += layer(uv * 0.5 + vec2(0.0 + time * 0.2, 300.0), 0.95) * 0.15;  
    intensity += layer(uv * 3.0 + vec2(time * 0.3, 99.0), 0.95) * 0.05;
    
    float cDist = max(0.0, 1.0 - length(uv) * cDistFactor);
    
    intensity = cDist + intensity;
    
    // vary color with intensity and uv y
    vec3 chroma = mix(colorInput1.rgb, colorInput2.rgb, uv.y * 0.5  + intensity * 0.5);
    
    return chroma * intensity;
}


void main()	{
	// Normalized pixel coordinates (from 0 to 1)
    vec2 uv = (2.0*gl_FragCoord.xy - R.xy) / R.x;
    uv *= yd;
  
    vec3 col = background(uv);
    // 
    col -= uv.y * 0.15;
    // increase gamma
    col = pow(col, vec3(GAMMA));
	// out col
	gl_FragColor = vec4(col, 1.0);
}

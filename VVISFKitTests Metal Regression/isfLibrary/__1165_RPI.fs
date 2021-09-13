/*{
  "DESCRIPTION": "trails",
  "CREDIT": "INKA",
  "CATEGORIES": [
    "Distortion Effect",
    "INKA"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "overlayImage",
      "TYPE": "image"
    },
    {
      "NAME": "hue",
      "TYPE": "float"
    },
    {
      "NAME": "saturation",
      "TYPE": "float"
    },
    {
      "NAME": "zoom",
      "TYPE": "float"
    },
    {
      "NAME": "overlay",
      "TYPE": "float"
    },
    {
      "NAME": "brightness",
      "TYPE": "float",
      "DEFAULT": 1
    },
    {
      "NAME": "bleedthrough",
      "TYPE": "float"    },
    {
      "NAME": "rotation",
      "TYPE": "float"
    },
	{
		"NAME": "distortion",
		"TYPE": "float",
		"MIN": 0.0,
		"MAX": 5.0,
		"DEFAULT": 2.0
	},
	{
		"NAME": "spots",
		"TYPE": "float"
	},
    {
      "NAME" : "spin",
      "TYPE" : "float",
      "DEFAULT": 0.35
    },
    {
      "NAME" : "invert",
      "TYPE" : "bool"
    },
    {
      "NAME" : "random",
      "TYPE" : "float",
      "DEFAULT": 0.35
    },
	{
		"NAME": "period",
		"TYPE": "float",
		"MIN": 0.0,
		"MAX": 1.0,
		"DEFAULT": 0.3
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

#define PI 3.141592654


float gray(vec4 n)
{
	return (n.r + n.g + n.b)/3.0;
}


vec3 rgb2hsv(vec3 c)	{
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	vec4 p = c.g < c.b ? vec4(c.bg, K.wz) : vec4(c.gb, K.xy);
	vec4 q = c.r < p.x ? vec4(p.xyw, c.r) : vec4(c.r, p.yzx);
	
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)	{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


mat2 rotmat( float deg ) {
	float theta = radians(deg);
	float s = sin(theta);
	float c = cos(theta);

	return mat2(c, -s, s, c);
}


vec2 pb(in vec2 uv, in float per){
    uv.y += (period * PI * 2.) / per;
    vec2 result = (cos(uv.y * per)) * normalize(vec2(1., cos((uv.y) * per)));
    return result;
}

#define NOISEVEC vec3(443.8975,397.2973, 491.1871)

//  1 out, 2 in...
float noise(vec2 p)
{
    vec3 p3 = fract(vec3(p.xyx) * NOISEVEC);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}

vec4 blend(vec4 bottom, vec4 top) {
	vec4		DB = vec4(bottom.a)*bottom;
	DB.a = bottom.a;
	float		TA = top.a;
	vec4		DT = vec4(TA) * top;
	DT.a = TA;
	vec4		returnMe = abs(DT-DB);
	returnMe.a = 1.0;
	return returnMe;
	
}


void main()
{
	vec2 uv =  gl_FragCoord.xy / RENDERSIZE.xy;
	vec4 color;
	float noiseVal = noise(uv * 0.5 + TIME);
	vec3 noiseCol = vec3(noiseVal);
	vec2 wave = pb(uv, 5.) * distortion * .00108;

	if (PASSINDEX == 0)	{
		vec4 original = IMG_THIS_PIXEL(inputImage);
		
 		vec2 warp = (uv - 0.5) * (1.0 + zoom * 0.01);
 		
		warp *= rotmat(rotation * .25);
		warp += 0.5;
		warp += wave;
		
 		color = IMG_NORM_PIXEL(buffer, warp); 
 		
		vec2 pos = warp + vec2(color.y - color.x, color.x - color.z) * ((spin * 20.) / RENDERSIZE.xy);
		
 		vec4 colOut = IMG_NORM_PIXEL(buffer, pos);
 		
	    colOut.rgb = rgb2hsv(colOut.rgb);
	    
	    colOut.r += hue * 0.01;
	    colOut.g += saturation * 0.01;
	    colOut.b += (-.005 * random) + (-0.002 + 0.004 * brightness);
	    
	    float lumaOut = colOut.b;
	  
	    colOut.rgb = hsv2rgb(colOut.rgb);
	    colOut.rgb += noiseCol.rgb * 0.01 * random;
	    
	    
	    float lumaOriginal = gray(original);
	    
		if(bleedthrough > 0.) {
			if(lumaOriginal > 1.0 - bleedthrough) {
				colOut = original;
			}
		}
		
		if(spots > 0. && noiseVal > 1.0 - spots * 0.25) {
			colOut = lumaOut > lumaOriginal ? original : IMG_NORM_PIXEL(overlayImage, uv);
		}

		gl_FragColor = colOut;
	}
	else if (PASSINDEX == 1)	{	
		//color = IMG_THIS_PIXEL(buffer);
		
	    color = IMG_NORM_PIXEL(buffer, uv);
		
	    uv += wave * 10.;
	    
	    if(distance(vec2(0.5), uv) < 0.25) {
	    	
	    	vec4 colorFG = IMG_NORM_PIXEL(overlayImage, uv);
	    	if (gray(colorFG) > 1.0 - overlay)
	    		color = blend(color, colorFG);
	    }
	    
		//gl_FragColor = col;
			
		gl_FragColor = color;
	}
}

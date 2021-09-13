/*{
	"CREDIT": "by isak.burstrom",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}, {
			"NAME": "noiseImage",
			"TYPE": "image"
		}, {
			"NAME": "colorVal",
			"TYPE": "float"
		}, {
			"NAME": "process",
			"TYPE": "float"
		}, {
			"NAME": "noiseAmount",
			"TYPE": "float"
		},{
			"NAME": "colorFlip",
			"TYPE": "float"
		}, {
			"NAME": "distortion",
			"TYPE": "float"
		}, {
			"NAME": "kaleido",
			"TYPE": "float"
		}, {
			"NAME": "glitch",
			"TYPE": "float"
		}, {
			"NAME": "bright",
			"TYPE": "float"
		}, {
			"NAME": "zoom",
			"TYPE": "float"
		}, {
			"NAME": "pinchAmnt",
			"TYPE": "float"
		}, {
			"NAME": "feedback",
			"TYPE": "float"
		}, {
			"NAME": "tunnelize",
			"TYPE": "float"
		}, {
			"NAME": "rotation",
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



#define NOISEVEC vec3(443.8975,397.2973, 491.1871)
#define PI 3.141592654
#define TAU 6.28318530718

float aspect = RENDERSIZE.x / RENDERSIZE.y;
vec2 center = vec2(0.5);

float noiseGenerate(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * NOISEVEC);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 hsv2rgb( in vec3 c ) {
    vec3 rgb = clamp( abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
	rgb = rgb*rgb*(3.0-2.0*rgb); // cubic smoothing	
	return c.z * mix( vec3(1.0), rgb, c.y);
}



vec4 blend(vec4 top, vec4 bottom) {
	return vec4(abs(top.rgb - bottom.rgb), 1.0);
}

vec4 vignette(vec2 uv, vec4 color, float amount) {
	vec2 coord = (uv - 0.5) * (aspect) * 1.;
	float rf = sqrt(dot(coord, coord)) * amount;
	float rf2_1 = rf * rf + 1.0;
	float e = 1.0 / (rf2_1 * rf2_1);
	color *= vec4(e);
	return color;
}


vec2 pb(in vec2 uv, in float per){
    uv.y += (TIME) / per;
    vec2 result = (cos(uv.y * per)) * normalize(vec2(1., cos((uv.y) * per)));
    return result * distortion * 0.05;
}
	
#define TWO_PI (PI*2.0)


float pattern(vec2 uv, float smooth) {
	float contrast = sin(TIME * 0.01);
	float pattern = sin(TIME * 0.02);
	float offset = 0.;
	vec2 center = gl_FragCoord.xy;
	uv -= vec2(0.5);
	uv.x *= aspect;
	uv *= 2. + pattern * 4.;

	for(float i = 0.; i < 2.0; i++) {
	  	float a = i * 4. * (TWO_PI * pattern / 10.);
		contrast += cos(TWO_PI*(uv.y * cos(a) + uv.x * sin(a) + offset)) +cos(TWO_PI*(uv.y * cos(a) + uv.x * sin(-a) + offset));
	}
	
	return (1.0 - smoothstep(.4, .5, contrast));
}

float gray(vec4 color) {
	return (color.r + color.g + color.b) / 3.;
}


vec4 tunnel(vec2 uv) {
	uv = uv - 0.5;
	float n = 1.0 / floor(5.);
	vec4 color = vec4(0.0);
	
	for (int i=0; i<int(5); i++) {
		float p = fract(fract(TIME * 0.5) * -1. + float(i) * n);
		float c = sin(p * PI);
		float z = (p * 5.) * tunnelize;
		vec2 uv = 0.5 + uv * z;

		vec4 pixel = IMG_NORM_PIXEL(inputImage, uv);
		
		if ((uv.x < 0.0) || (uv.y < 0.0) || (uv.x > 1.0) || (uv.y > 1.0)) {
			pixel = vec4(0.0);
		}
		
		float pixelLuma = (pixel.r + pixel.g + pixel.b) / 3.0;
		pixel = pixel / max(1., pow(5., p));
		if(pixelLuma > (-0.5 + tunnelize * 1.7) * 0.8 && gray(pixel) > gray(color)) {
			color = pixel;
		}
	
	}
	return color;
}

vec2 rotate(vec2 uv, float amnt) {
	uv = uv - 0.5;
    float rot = amnt * TAU; //radians(amnt * 3.0);
    mat2 m = mat2(cos(rot), -sin(rot), sin(rot), cos(rot));
    return m * uv + 0.5;
}


// CENTER VAL AROUND POINT
float centerVal(float around, float size, float x) {
	return around - size + (size * 2. * x);
}

vec2 kaleidoscope(vec2 uv, float sides) {
	sides = floor(8. * (0.25 + sides));
	float r = distance(center, uv);
	float a = atan ((uv.y-center.y), (uv.x-center.x));
	a = mod(a, TAU/sides);
	a = abs(a - TAU/sides/2.);
	uv.x = r * cos(a + TIME * 0.1);
	uv.y = r * sin(a + TIME * 0.1);
	return center + uv;
}



void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    if(PASSINDEX == 0) {
	    float noise = noiseGenerate(uv + TIME * 0.00001);
		
		// ZOOM
		vec2 uvFG = 0.5 + (uv - 0.5) * (centerVal(0.5, 0.1, zoom));
		
		// KALEIDO
		if(kaleido > 0.) {
			uvFG = kaleidoscope(uvFG, kaleido);
		}
		
		// MIRROR EDGE
		uvFG = mod(uvFG + center / 2., 1.0);
		uvFG = 2.0 * abs(uvFG - center);
		
		//GLITCH
		vec2 uvDIST = vec2(0.), uvWAVE = vec2(0.);
		
		if(glitch > 0.) {
			float tresh =  pow(fract(TIME * 1236.0453), 2.0) * 0.4 * glitch;
			vec2 block = floor(gl_FragCoord.xy / vec2(16));
			vec2 uv_noise = block / vec2(64);
			uv_noise += floor(vec2(TIME) * vec2(1204.0, 3543.0)) / vec2(64) / RENDERSIZE;
			
			if (noiseGenerate(uv_noise) < tresh)
				uvDIST += (fract(uv_noise) - 0.5) * .03 * glitch;
		}

		// WAVE
		if(distortion > 0.) {
			uvWAVE = pb(uvFG, 11. * distortion);	
		}
		
		// ROTATION
		uvFG = rotate(uvFG, rotation);
		
		vec3 hsl = vec3(colorVal, colorFlip, colorFlip * 0.4);
		vec4 colorNOISE = IMG_THIS_PIXEL(noiseImage);
		vec4 colorBG = vec4(hsv2rgb(hsl), 1.0) - colorNOISE + centerVal(0., 0.3 * noiseAmount, noise);
	    
		if(bright > 0.) {
			colorBG += bright * pattern(uvFG, bright) * .45;
		}
		
		vec4 colorFG = IMG_PIXEL(inputImage, (uvFG + uvDIST + uvWAVE) * RENDERSIZE);
		
	   // TUNNEL
	   if(tunnelize > 0.) 
			colorFG = mix(colorFG, tunnel(uvFG + uvDIST + uvWAVE), min(1., tunnelize * 2.));
			
	    vec4 color = blend(colorBG, colorFG);
	    
	    uvFG = center + (uv + uvDIST - 0.5) * centerVal(1.0, 0.01, zoom);
 		vec4 colorFeedback = IMG_PIXEL(buffer, uvFG * RENDERSIZE);
 		
 		if(feedback > 0.) {
 			float lumaColor = gray(color);
	 			
			if(lumaColor < feedback || noise > 1.0 - feedback * 0.5 && gray(colorFeedback) < lumaColor ) {
				color = colorFeedback;
			}

			color *= 1.0 - feedback * 0.004;
	    }
			
		
		gl_FragColor = color;
    } else {
    	vec4 color = IMG_PIXEL(buffer, uv * RENDERSIZE);
		gl_FragColor = color;
    }
	
       
}
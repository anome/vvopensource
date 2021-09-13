/*{
  "DESCRIPTION": "Your shader description",
  "CREDIT": "by you",
  "CATEGORIES": [
    "XXX"
  ],
  "INPUTS": [
    {
      "NAME": "VID_1",
      "TYPE": "image"
    },
    {
      "NAME": "VID_2",
      "TYPE": "image"
    }, {
		"NAME": "colorizeVideo",
		"TYPE": "float"
	}, {
		"NAME": "colorizeHue",
		"TYPE": "float"
	}, {
		"NAME": "colorProcess",
		"TYPE": "float"
	},{
		"NAME": "noiseAmount",
		"TYPE": "float"
	}, {
		"NAME": "strobeAmount",
		"TYPE": "float"
	}, {
		"NAME": "rotation",
		"TYPE": "float"
	}, {
		"NAME": "VID_1_OPACITY",
		"TYPE": "float"
	}, {
		"NAME": "VID_2_OPACITY",
		"TYPE": "float"
	}, {
		"NAME": "zoom",
		"TYPE": "float"
	}, {
		"NAME": "glitch",
		"TYPE": "float"
	}, {
		"NAME": "feedback",
		"TYPE": "float"
	}, {
		"NAME": "tunnelize",
		"TYPE": "float"
	}, {
		"NAME": "volumeamount",
		"TYPE": "float"
	}, {
		"NAME": "VOLUME",
		"TYPE": "float"
	},{
		"NAME": "VOLUME_SMOOTH",
		"TYPE": "float"
	},{
		"NAME": "echoTrace",
		"TYPE": "float"
	},{
		"NAME": "kaleido",
		"TYPE": "float"
	},{
		"NAME": "distortion",
		"TYPE": "float"
	},{
		"NAME": "feedbackInvert",
		"TYPE": "float"
	}
  ],
	"PASSES": [
    {
      "TARGET": "VID_BUFFER"
    }, {
    	
    }
	]
}*/

/*

float colorizeVideo = BOTTOM_KNOB_1;
float colorizeHue = BOTTOM_KNOB_2;
float colorProcess = BOTTOM_KNOB_3;
float noiseAmount = BOTTOM_KNOB_5;
float strobeAmount = BOTTOM_KNOB_6;
float rotation = BOTTOM_KNOB_7;
float volumeamount = BOTTOM_KNOB_8;

float VID_1_OPACITY = TOP_KNOB_1;
float VID_2_OPACITY = TOP_KNOB_2;
float tunnelize = TOP_KNOB_3;
float zoom = TOP_KNOB_4;
float kaleido = TOP_KNOB_5;
float distortion = TOP_KNOB_6;
float glitch = TOP_KNOB_7;
float echoTrace = TOP_KNOB_7;
float feedback = TOP_KNOB_8;
//float blendmode = floor(BOTTOM_KNOB_7 * 4.);
*/

// SOUND

// CONSTANTS
vec4 lumcoeff = vec4(0.299, 0.587, 0.114, 0.0);
vec2 center = vec2(0.5);
float PI = 3.141592654;
float TAU = 6.283185307;
vec3 NOISEVEC = vec3(443.8975,397.2973, 491.1871);

vec3 hsv2rgb( in vec3 c ) {
	vec3 rgb = clamp( abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0 );
	rgb = rgb*rgb*(3.0-2.0*rgb); // cubic smoothing
	return c.z * mix( vec3(1.0), rgb, c.y);
}

vec4 blend(vec4 top, vec4 bottom) {
  return vec4(abs(top.rgb - bottom.rgb), 1.0);
}


float gray(vec4 c) {
	return (c.r + c.g + c.b) / 3.;
}

float centerVal(float around, float size, float x) {
	return around - size + (size * 2. * x);
}

vec2 videoWave(in vec2 uv, in float per){
    uv.y += (TIME * 4.) / per;
    vec2 result = (cos(uv.y * per)) * normalize(vec2(1., cos((uv.y) * per)));
    return result * distortion * 0.1;
}

float noiseGenerate(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * NOISEVEC);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}

vec4 vignette(vec2 uv, vec4 color, float amount) {
	vec2 coord = (uv - 0.5) * (RENDERSIZE.x / RENDERSIZE.y) * 1.;

	float rf = sqrt(dot(coord, coord)) * amount;
	float rf2_1 = rf * rf + 1.0;
	float e = 1.0 / (rf2_1 * rf2_1);

	color *= vec4(e);

	return vec4(vec3(color.rgb), 1.0);
}


vec4 tunnel(vec2 uv) {
	uv = uv - 0.5;
	float n = 1.0 / floor(5.);
  	float k = fract(TIME * 0.2) * -1.;
	vec4 color = vec4(0.0);

	for (int i=0; i<int(5); i++) {
		float p = fract(k + float(i) * n);
		float z = (p * 5.);
		vec2 uv = 0.5 + uv * z;
		vec4 pixel = IMG_NORM_PIXEL(VID_2, uv);
		if ((uv.x < 0.0) || (uv.y < 0.0) || (uv.x > 1.0) || (uv.y > 1.0)) {
			pixel = vec4(0.0);
	  }

		float pixelLuma = gray(pixel);
		pixel = pixel / max(1., pow(5., cos(PI * .5 + p * PI)));
    color = mix(color, pixel, clamp(gray(pixel) - (-0.5 + tunnelize * 1.6) * 0.4, 0., 1.));
	}
	return color;
}

vec2 rotate(vec2 uv, float amnt) {
	uv = uv - 0.5;
  float rot = (1.0 + amnt) * PI; //radians(amnt * 3.0);
  mat2 m = mat2(cos(rot), -sin(rot), sin(rot), cos(rot));
  return m * uv + 0.5;
}

vec2 kaleidoscope(vec2 uv, float sides) {
	sides = floor(8. * (0.25 + sides));
	float r = distance(center, uv);
	float a = atan ((uv.y-center.y), (uv.x-center.x));
	a = mod(a, TAU/sides);
	a = abs(a - TAU/sides/2.);
	uv.x = r * cos(a + TIME * 0.07);
	uv.y = r * sin(a + TIME * 0.07);
	return center + uv;
}

vec4 strobe(vec4 color, float speed) {
	float eq = mod(floor(TIME * speed), 2.0);
	float strobe = step( 0.0 , eq ) * (1.0 - step( 1.0 , eq));
	return vec4(color.rgb + vec3(strobe) * .3, color.a);
}

vec2 pinch(vec2 uv, float amount) {
	float k = -1. * amount;
	float r2 = (uv.x-0.5) * (uv.x-0.5) + (uv.y-0.5) * (uv.y-0.5);
	float f = 1.0 + r2 * (k + 0.01 * sqrt(r2));
	return f * (uv.xy - 0.5) + 0.5;
}

float remap( float value, float inMin, float inMax, float outMin, float outMax ) {
    return ( (value - inMin) / ( inMax - inMin ) * ( outMax - outMin ) ) + outMin; 
}

vec4 feedbackGLSL(vec4 color, vec2 uv) {
	float offset_x = remap(0.5,0.0,1.0,-0.05,0.05);
	float offset_y = remap(0.5,0.0,1.0,-0.05,0.05);
	vec2 uv2 = uv * 2.0 - vec2(1.0 - offset_x, 1.0 - offset_y);
	uv2.x *= RENDERSIZE.x / RENDERSIZE.y;
	vec2 uv_b = uv2 * 0.5;
	vec4 col;
	if(length(uv_b) < feedbackInvert) {
		vec2 coord = uv2;
		coord *= 1.09 - (0.1 + (remap(zoom * 0.5, 0.0,1.0,-0.20,0.30) * 0.5)); + sin(TIME * 0.1) * 0.2;
		coord.x /= RENDERSIZE.x / RENDERSIZE.y;
		coord = (coord + 1.0)/2.0;
		col = IMG_NORM_PIXEL(VID_BUFFER, coord, 0.1) + 0.01;
		col *= color;
	    col = 1.0 - col;
	} else {
		col = color;
	}
	
	return col; //Insane feedback! 
}


void main() {
	// MAC
  	vec2 uv = isf_FragNormCoord.xy;
  	float volume_nosmooth = clamp(VOLUME * volumeamount * 20., 0., 2.);
	float volume = clamp(VOLUME_SMOOTH * volumeamount * 20., 0., 2.);
	
    float noise = noiseGenerate(uv + TIME * 0.1);

    //GLITCH
    vec2 uvDIST  = vec2(0.), uvWAVE = vec2(0.);

    if(glitch > 0.) {
      float tresh =  pow(fract(TIME * 1236.0453), 2.0) * 0.4 * glitch;
      vec2 block = floor(gl_FragCoord.xy / vec2(12));
      vec2 uv_noise = block / vec2(64);
      uv_noise += floor(vec2(TIME) * vec2(1204.0, 3543.0)) / vec2(64) / RENDERSIZE;

      if (noiseGenerate(uv_noise) < tresh)
        uvDIST += (fract(uv_noise) - 0.5) * .03 * glitch;
    }

    // WAVE
    if(distortion > 0.) {
      uvWAVE = videoWave(uv, 7. + 7. * distortion);
    }

  	// MIRROR EDGE
	vec2 uvZOOM = (uv - 0.5) * (0.5 + (zoom * 2. + volume * 0.5 ) * 0.5);
	uvZOOM = mod(uvZOOM + center / 2., 1.0);
  	uvZOOM = 2.0 * abs(uvZOOM - center);

		// ROTATION
    float rotationOffset = 0.;
    if(kaleido > 0.) {
      rotationOffset = -0.5;
	}
	vec2 uvFG = rotate(uvZOOM, rotationOffset + rotation);

	// KALEIDO
	if(kaleido > 0.) {
		uvFG = kaleidoscope(uvFG, kaleido);
	}

    // TEXTURES
    vec3 hsl = vec3(colorizeHue, colorizeVideo, colorizeVideo * 0.75);
    vec4 colorVID_2 = IMG_NORM_PIXEL(VID_2, uvFG + uvDIST + uvWAVE) * VID_1_OPACITY;
    vec4 colorVID_1 = IMG_NORM_PIXEL(VID_1, uvZOOM) * VID_2_OPACITY;
	vec4 colorBG_COL = vec4(hsv2rgb(hsl), 1.0) ;

    // TUNNEL
    if(tunnelize > 0.) {
      colorVID_2 = mix(colorVID_2, tunnel(uvFG + uvDIST + uvWAVE), min(1., tunnelize * 2.));
    }

    // BLEND
    vec4 color = blend(colorBG_COL, colorVID_2);

    // BLEND VID 1 AND VID 2
    color = blend(color, colorVID_1) + centerVal(0., 0.1 * noiseAmount, noise);

    // B&W
    if(colorizeHue > 0.8) {
      float luminance = dot(color, lumcoeff);
      vec4 colorBW = mix(vec4(0.0, 0.0, 0.0, 1.0), vec4(1.0), luminance);
    	color = mix(color, colorBW, (colorizeHue - 0.8) / 0.2);
    }

  	// VIGNETTE
    color = vignette(uv, color, 0.85 * colorProcess);

    //LEVELS
    float gamma = colorProcess * .125 + volume * .3;
    float contrast = 0.5;
    float exposure =  colorProcess * .125 + volume * .2;
    float blackLevel = 0. + colorProcess * .01;
  	vec4 inputRange = min(max(color - vec4(gamma), vec4(0.0)) / (vec4(1.0 - exposure) - vec4(gamma)), vec4(1.0));
  	inputRange = pow(inputRange, vec4(1.0 / (1.5 - contrast)));
  	color = mix(vec4(blackLevel), vec4(1.0), inputRange);
  	
  	//INVERT 
  	if(feedbackInvert > 0.) {
  		color = feedbackGLSL(color, uv);
  	}

    // STROBE
    if(strobeAmount > 0.) {
      color = strobe(color, strobeAmount * 15.);
    }


    // ECHO TRACE
    if(echoTrace > 0.) {
    	color = mix(color, IMG_NORM_PIXEL(VID_BUFFER, uv), (gray(color) < (echoTrace * 0.6) - volume) ? 1.0 : 0.0);
    }

    // FEEDBACK
    if(feedback > 0.) {
	vec4 colorFeedback = IMG_NORM_PIXEL(VID_BUFFER, vec2(0.5) + (uv + uvDIST - 0.5) * centerVal(1.0, 0.01, zoom));
	float lumaColor = gray(color);
	float lumaFeedback = gray(colorFeedback);
	float treshold = 1.0 - (volume_nosmooth * feedback);
	
	if(lumaFeedback >= lumaColor - (1.0 - treshold) && lumaColor > treshold) {
		colorFeedback = color;
	}
	
	colorFeedback = colorFeedback + vec4(vec3(-0.01), 0.0);
		color = mix(color, colorFeedback, feedback);
	}

    gl_FragColor = color;
}

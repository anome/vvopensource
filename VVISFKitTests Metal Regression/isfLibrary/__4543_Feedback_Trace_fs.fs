/*{
  "DESCRIPTION": "",
  "CREDIT": "VIDVOX",
  "CATEGORIES": [
    "Stylize"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "feedbackAmount",
      "LABEL": "Feedback Amount",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "mixMode",
      "TYPE": "long",
      "VALUES": [
        0,
        1,
        2,
        3
      ],
      "LABELS": [
        "Additive",
        "Mix",
        "Max",
        "Threshold"
      ],
      "DEFAULT": 1
    }
  ],
  "PASSES": [
    {
      "TARGET": "buffer",
      "WIDTH": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "persistent": true
    }
  ]
}*/


vec3 rgb2hsv(vec3 c)	{
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	//vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	//vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
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


void main()	{
	vec4		inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	vec4		bufferPixelColor = IMG_THIS_NORM_PIXEL(buffer);
	float		logFeedBack = sign(feedbackAmount) * log(1.0 + 10.0 * abs(feedbackAmount)) / log(10.0);
	
	vec4		result = vec4(0.0,0.0,0.0,1.0);
	
	if (mixMode == 0)	{
		result = abs(inputPixelColor + bufferPixelColor * logFeedBack);
	}
	else if (mixMode == 1)	{
		result = abs(inputPixelColor * (1.0 - logFeedBack) + bufferPixelColor * logFeedBack);
	}
	else if (mixMode == 2)	{
		result = max(inputPixelColor, abs(bufferPixelColor * logFeedBack));
	}
	else if (mixMode == 3)	{
		vec4	feedbackHSV = vec4(1.0);
		feedbackHSV.rgb = rgb2hsv(bufferPixelColor.rgb);
		result.rgb = rgb2hsv(inputPixelColor.rgb);
		if (abs(result.b - sign(feedbackAmount) * feedbackHSV.b) < abs(feedbackAmount))	{
			result.rgb = feedbackHSV.rgb;
		}
		else if (abs(result.r - sign(feedbackAmount) * feedbackHSV.r) < abs(feedbackAmount))	{
			result.rgb = feedbackHSV.rgb;
		}
		result.rgb = hsv2rgb(result.rgb);
	}
	
	gl_FragColor = result;
}


/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	    {
	        "NAME": "enabled",
	        "TYPE": "bool",
	        "DEFAULT": true
	    },
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "exposure",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "contrast",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "highlights",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "shadows",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "hueRotation",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 1,
			"MIN": 0
		},
		{
			"NAME": "saturation",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "lightness",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 1,
			"MIN": 0
		},
		{
			"NAME": "temperature",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 1,
			"MIN": 0
		},
		{
			"NAME": "tint",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 1,
			"MIN": 0
		}
	]
	
}*/

vec3 refWhite, refWhiteRGB;

vec3 d, s;

const mediump vec3 luminanceWeighting = vec3(0.2125, 0.7154, 0.0721);

mat3 matRGBtoXYZ = mat3(
	0.4124564390896922, 0.21267285140562253, 0.0193338955823293,
	0.357576077643909, 0.715152155287818, 0.11919202588130297,
	0.18043748326639894, 0.07217499330655958, 0.9503040785363679
);


mat3 matXYZtoRGB = mat3(
	3.2404541621141045, -0.9692660305051868, 0.055643430959114726,
	-1.5371385127977166, 1.8760108454466942, -0.2040259135167538,
	-0.498531409556016, 0.041556017530349834, 1.0572251882231791
);

mat3 matAdapt = mat3(
	0.8951, -0.7502, 0.0389,
	0.2664, 1.7135, -0.0685,
	-0.1614, 0.0367, 1.0296
);

mat3 matAdaptInv = mat3(
	0.9869929054667123, 0.43230526972339456, -0.008528664575177328,
	-0.14705425642099013, 0.5183602715367776, 0.04004282165408487,
	0.15996265166373125, 0.0492912282128556, 0.9684866957875502
);

// illuminants
//vec3 A = vec3(1.09850, 1.0, 0.35585);
vec3 D50 = vec3(0.96422, 1.0, 0.82521);
vec3 D65 = vec3(0.95047, 1.0, 1.08883);
vec3 CCT2K = vec3(1.274335, 1.0, 0.145233);
vec3 CCT4K = vec3(1.009802, 1.0, 0.644496);
vec3 CCT20K = vec3(0.995451, 1.0, 1.886109);

// -- HELPERS ---

vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 rgb2xyz(vec3 rgb){
	vec3 xyz, XYZ;
	xyz = matRGBtoXYZ * rgb;
	XYZ = matAdapt * xyz;
	XYZ *= d/s;
	xyz = matAdaptInv * XYZ;

	return xyz;
}

vec3 xyz2rgb(vec3 xyz){
	vec3 rgb, RGB;
	RGB = matAdapt * xyz;
	rgb *= s/d;
	xyz = matAdaptInv * RGB;
	rgb = matXYZtoRGB * xyz;
	return rgb;
}

float lum(vec3 c){
	return 0.299*c.r + 0.587*c.g + 0.114*c.b;
}

vec3 clipColor(vec3 c){
	float l = lum(c);
	float n = min(min(c.r, c.g), c.b);
	float x = max(max(c.r, c.g), c.b);
	if (n < 0.0) c = (c-l)*l / (l-n) + l;
	if (x > 1.0) c = (c-l) * (1.0-l) / (x-l) + l;
	return c;
}

vec3 setLum(vec3 c, float l){
	float d = l - lum(c);
	c.r = c.r + d;
	c.g = c.g + d;
	c.b = c.b + d;
	return clipColor(c);
}


void main()	{
    vec2 size = IMG_SIZE(inputImage);
    float ratio = size.y/size.x;
    vec2 uv = isf_FragNormCoord * vec2(1.0, ratio);
	vec4 image = IMG_NORM_PIXEL(inputImage, uv);
	
	if (!enabled) {
	    gl_FragColor = image;
	    return;
	}
	
	// Exposure
	float exposureScaled = (exposure * 2.0) - 1.0;
	image = vec4(image.rgb * pow(2.0, exposureScaled), image.w);
	
	// Hue, Saturation, Lightness
	vec3 imageHSV = rgb2hsv(image.xyz);
	float hueRotationScaled = ((hueRotation * 2.0) - 1.0) * 0.5;
	float saturationScaled = (saturation * 2.0);
	imageHSV.x = fract(imageHSV.x + hueRotationScaled);
	imageHSV.y *= saturationScaled;
	imageHSV.z = min(1.0, imageHSV.z + (lightness * 2.) - 1.);
	image = vec4(hsv2rgb(imageHSV), image.w);
	
	// Highlights, Shadows
	float luminance = dot(image.rgb, luminanceWeighting);
	float shadowsScaled = shadows * 2.0;
	float highlightsScaled = highlights * 2.0;
	
	mediump float shadow = clamp((pow(luminance, 1.0/shadowsScaled) + (-0.76)*pow(luminance, 2.0/shadowsScaled)) - luminance, 0.0, 1.0);
    mediump float highlight = clamp((1.0 - (pow(1.0-luminance, 1.0/(2.0-highlightsScaled)) + (-0.8)*pow(1.0-luminance, 2.0/(2.0-highlightsScaled)))) - luminance, -1.0, 0.0);
    lowp vec3 result = vec3(0.0, 0.0, 0.0) + ((luminance + shadow + highlight) - 0.0) * ((image.rgb - vec3(0.0, 0.0, 0.0))/(luminance - 0.0));
    
    // blend toward white if highlights is more than 1
    mediump float contrastedLuminance = ((luminance - 0.5) * 1.5) + 0.5;
    mediump float whiteInterp = contrastedLuminance*contrastedLuminance*contrastedLuminance;
    mediump float whiteTarget = clamp(highlightsScaled, 1.0, 2.0) - 1.0;
    result = mix(result, vec3(1.0), whiteInterp*whiteTarget);

    // blend toward black if shadows is less than 1
    mediump float invContrastedLuminance = 1.0 - contrastedLuminance;
    mediump float blackInterp = invContrastedLuminance*invContrastedLuminance*invContrastedLuminance;
    mediump float blackTarget = 1.0 - clamp(shadowsScaled, 0.0, 1.0);
    result = mix(result, vec3(0.0), blackInterp*blackTarget);
    image = vec4(result, image.w);
    
    // Temperature and Tint
    float temperatureScaled = (temperature * 2.0) - 1.0;
    float tintScaled = (tint * 2.0) - 1.0;
    
    vec3 to, from;

    if (temperatureScaled < 0.0) {
        to = CCT20K;
        from = D65;
    } else {
        to = CCT4K;
        from = D65;
    }
    
    vec3 base = image.rgb;
	float l = lum(base);
	// mask by luminance
	float temp = abs(temperatureScaled) * (1.0 - pow(l, 2.72));

    // from
	refWhiteRGB = from;
	// to
	refWhite = vec3(mix(from.x, to.x, temp), mix(1.0, 0.9, tintScaled), mix(from.z, to.z, temp));

    // mix based on alpha for local adjustments
	refWhite = mix(refWhiteRGB, refWhite, image.a);

	d = matAdapt * refWhite;
	s = matAdapt * refWhiteRGB;
    vec3 xyz = rgb2xyz(base);
	vec3 rgb = xyz2rgb(xyz);
	// brightness compensation
	vec3 res = rgb * (1.0 + (temp + tintScaled) / 10.0);
    image = vec4(res, image.w);


    // Contrast
	float contrastScaled = (contrast * 2.0);
	image = vec4(((image.rgb - vec3(0.5)) * contrastScaled + vec3(0.5)), image.w);
	
	gl_FragColor = image;
}

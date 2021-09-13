/*{
    "CATEGORIES": [
        "XXX"
    ],
    "CREDIT": "",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "DEFAULT": 0.125,
            "MAX": 1,
            "MIN": 0,
            "NAME": "width",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.00,
            "MAX": 1,
            "MIN": 0,
            "NAME": "height",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.0,
            "MAX": 1,
            "MIN": 0,
            "NAME": "x_shift",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.125,
            "MAX": 1,
            "MIN": 0,
            "NAME": "y_wobble",
            "TYPE": "float"
        },        
        {
            "DEFAULT": [
                1,
                0,
                0,
                1
            ],
            "NAME": "color1",
            "TYPE": "color"
        },
        {
            "DEFAULT": 1,
            "MAX": 1,
            "MIN": 0,
            "NAME": "colorize",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.75,
            "MAX": 2,
            "MIN": 0,
            "NAME": "gain_level",
            "TYPE": "float"
        },
        {
            "DEFAULT": -0.1,
            "MAX": 1,
            "MIN": -1,
            "NAME": "hue_shift",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.881,
            "MAX": 1,
            "MIN": 0,
            "NAME": "seed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.2819,
            "MAX": 1,
            "MIN": 0,
            "NAME": "gain_seed",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}
*/


//	inspired by https://www.youtube.com/watch?v=yD_kCKiSkoI

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

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec4 rand4(vec4 co)	{
	vec4	returnMe = vec4(0.0);
	returnMe.r = rand(co.rg);
	returnMe.g = rand(co.gb);
	returnMe.b = rand(co.ba);
	returnMe.a = rand(co.rb);
	return returnMe;
}

float brightness(vec4 c)	{
	return (c.r+c.g+c.b)*c.a/3.0;
}


void main() {
	vec4		out_color = vec4(0.0);
	vec2		c = isf_FragNormCoord;
	float		_width = (width == 0.0) ? 1.0 / RENDERSIZE.x : width;
	float		_height = (height == 0.0) ? 1.0 / RENDERSIZE.y : height;
	float		mod_x = _width * floor(mod(x_shift + c.x, 1.0) / _width);
	float		y_seed = y_wobble * (sin(18.137*mod_x + seed + 0.97) + cos(11.347 * mod_x + seed + 0.19)+1.0)/2.0;
	float		row_hash = rand(seed+vec2(y_seed + mod_x, _height*ceil(c.y/_height)));
	float		row_width = row_hash * _width;
	c.x = row_width * floor(c.x / row_width);
	//	duo tone
	//	block errors
	//	row errors
	//	over saturation
	out_color = IMG_NORM_PIXEL(inputImage,c);
	if (colorize > 0.0)	{
		vec4		mod_color = (brightness(out_color))*color1;
		out_color = mix(out_color,mod_color,colorize);
	}
	if (gain_level > 0.0)	{
		float		gain_rand = rand(gain_seed+vec2(_width * floor(c.x / _width),_height*ceil(c.y/_height)));
		vec4		mod_color = out_color;
		mod_color.rgb = rgb2hsv(mod_color.rgb);
		mod_color.g += (gain_rand * gain_level);
		mod_color.b += (gain_rand * gain_level);
		mod_color.r += (gain_rand * hue_shift);
		//mod_color.r += gain_level;
		mod_color.rgb = hsv2rgb(mod_color.rgb);
		out_color = mix(out_color,mod_color,gain_rand);
	}

	gl_FragColor = out_color;
}
/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Feeling Sleepy",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "rate",
      "TYPE" : "float",
      "MAX" : 0.1,
      "DEFAULT" : -0.02,
      "LABEL" : "SPEED",
      "MIN" : -0.1
    },
    {
      "NAME" : "colorIN",
      "TYPE" : "color",
      "DEFAULT" : [
        1,
        0.20000000298023224,
        0.10000000149011612,
        1
      ],
      "LABEL" : "Color"
    },
    {
      "NAME" : "Count",
      "TYPE" : "float",
      "MAX" : 25,
      "DEFAULT" : 18.131168365478516,
      "LABEL" : "Ray Count",
      "MIN" : 3
    },
    {
      "NAME" : "posY",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : -0.33067807555198669,
      "LABEL" : "Position Y",
      "MIN" : -0.5
    },
    {
      "NAME" : "posX",
      "TYPE" : "float",
      "MAX" : 0.75,
      "DEFAULT" : 0,
      "LABEL" : "Position X",
      "MIN" : -0.75
    },
    {
      "NAME" : "width",
      "TYPE" : "float",
      "MAX" : 0.45,
      "DEFAULT" : 0.2891484797000885,
      "LABEL" : "Ray Width",
      "MIN" : 0.01
    },
		{
			"NAME" : "soft",
			"TYPE" : "float",
			"MAX" : 0.99,
			"DEFAULT" : 0.2891484797000885,
			"LABEL" : "Ray Softness",
			"MIN" : 0.01
		}
  ],
  "CREDIT" : "howie.tv"
}
*/
// its a  howie.tv thing

#define M_PI 3.1415926535897932384626433832795


// rgb2hsv
vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// hsv2rgb
vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float rays(vec2 uv, float c, float w, float s)
	{
		uv.x = fract(uv.x*c)-0.5;
	 	return smoothstep(-w , -w*s, uv.x) * smoothstep(w, w*s, uv.x);
	}

void main()
{

		float ratio = RENDERSIZE.y/RENDERSIZE.x;
		vec2 uv = vec2( isf_FragNormCoord.x -0.5, (isf_FragNormCoord.y -0.5) * ratio);
				
		float count = floor(Count);
        uv.y -= posY;
        uv.x -= posX;

		vec3 hvs = rgb2hsv(colorIN.rgb);
		vec3 colorA = hsv2rgb(vec3(hvs.x +0.1, hvs.y, hvs.z));
		vec3 colorB = hsv2rgb(vec3(hvs.x -0.1, hvs.y, hvs.z));

		 float angle = atan(uv.y, uv.x);
		angle = angle/((M_PI*4.0)*0.5) + TIME*rate;
		float radius = length(uv);
		uv = vec2(angle, radius*1.5);
		vec3 col = mix(colorA,colorB, abs(uv.y)/ratio);

		uv.x += sin((TIME*-rate)+(uv.y*1.5))*0.1;

        float m = rays(uv,count,width,soft);
				m = pow(m,3.0);
				vec3 color = col*m;

	gl_FragColor = vec4(color,1.0);
}

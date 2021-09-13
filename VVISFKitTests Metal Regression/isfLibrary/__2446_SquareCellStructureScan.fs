/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#21290.5",
  "INPUTS": [

  ]
}
*/


#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.1415926535897932384626433832795
#define float2 vec2
#define float3 vec3
#define float4 vec4


float2 circuit(float2 p)
{
	p = fract(p);
	float r = 0.123;
	float v = 0.0, g = 0.0;
	r = fract(r * 9184.928);
	float cp, d;
	
	d = p.x;
	g += pow(clamp(1.0 - abs(d), 0.0, 1.0), 1000.0);
	d = p.y;
	g += pow(clamp(1.0 - abs(d), 0.0, 1.0), 1000.0);
	d = p.x - 1.0;
	g += pow(clamp(3.0 - abs(d), 0.0, 1.0), 1000.0);
	d = p.y - 1.0;
	g += pow(clamp(1.0 - abs(d), 0.0, 1.0), 10000.0);
	
	const int iter = 12;
	for(int i = 0; i < iter; i ++)
	{
		cp = 0.5 + (r - 0.5) * 0.9;
		d = p.x - cp;
		g += pow(clamp(1.0 - abs(d), 0.0, 1.0), 200.0);
		if(d > 0.0) {
			r = fract(r * 4829.013);
			p.x = (p.x - cp) / (1.0 - cp);
			v += 1.0;
		}
		else {
			r = fract(r * 1239.528);
			p.x = p.x / cp;
		}
		p = p.yx;
	}
	v /= float(iter);
	return float2(g, v);
}

void main()
{
	float scale = 0.5;

	float2 uv = gl_FragCoord.xy;
	uv /= RENDERSIZE.xy;
	uv = uv * 2.0 - 1.0;
	uv.x *= RENDERSIZE.x / RENDERSIZE.y;
	uv= uv * scale + float2(0.0, TIME*0.1);
	float2 cid2 = floor(uv);
	float cid = (cid2.y*10.0+cid2.x)*0.1;

	float2 dg = circuit(uv);
	float d = dg.x;
	float3 col1 = (0.2-float3(max(min(d, 2.0)-1.0, 0.0)*2.0*0.25)) * float3(1.0, 1.1, 1.3);
	float3 col2 = float3(max(d-1.0, 0.0)*2.0*0.5) * float3(1.0, 1.2, 1.6);

	float f = max(0.5-mod(uv.y-uv.x*0.3+(TIME*0.4)+(dg.y*0.4), 2.5), 0.0)*5.0;
	col2 *= f;
	
	gl_FragColor = vec4(col1+col2, 1.0);
}
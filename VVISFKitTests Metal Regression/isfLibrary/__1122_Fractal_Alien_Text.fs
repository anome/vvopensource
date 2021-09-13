/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Fractal Alien Text",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "scrollSpeed",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.3,
      "LABEL" : "Scroll Speed",
      "MIN" : 0
    },
    {
      "NAME" : "scrollAngle",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : 0.1,
      "LABEL" : "Scroll Angle",
      "MIN" : -0.5
    },
    {
      "NAME" : "baseSaturation",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 1,
      "MIN" : 0
    }
  ],
  "CREDIT" : "airtight"
}
*/

//original https://www.shadertoy.com/view/4lscz8

#define PI 3.1415926535

//from https://github.com/keijiro/ShaderSketches/blob/master/Text.glsl

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

float letter(vec2 coord, float size)
{
    vec2 gp = floor(coord / size * 7.); // global
    vec2 rp = floor(fract(coord / size) * 7.); // repeated
    vec2 odd = fract(rp * 0.5) * 2.;
    float rnd = fract(sin(dot(gp, vec2(12.9898, 78.233))) * 43758.5453);
    float c = max(odd.x, odd.y) * step(0.5, rnd); // random lines
    c += min(odd.x, odd.y); // corder and center points
    c *= rp.x * (6. - rp.x); // cropping
    c *= rp.y * (6. - rp.y);
    return clamp(c, 0., 1.);
}

float random2d(vec2 n) { 
    return fract(sin(dot(n, vec2(129.9898, 4.1414))) * 4358.5453);
}

vec2 getCellIJ(vec2 uv, float gridDims){
    return floor(uv * gridDims)/ gridDims;
}

vec2 rotate2D(vec2 position, float theta)
{
    mat2 m = mat2( cos(theta), -sin(theta), sin(theta), cos(theta) );
    return m * position;
}

void main()
{

    vec2 uv = isf_FragNormCoord.xy;    
    //correct aspect ratio
    uv.x *= RENDERSIZE.x/RENDERSIZE.y;

    float t = TIME;
    float dims = 2.0;
    //int maxSubdivisions = 3;
    
    uv = rotate2D(uv,PI*scrollAngle);
    uv.y -= TIME * scrollSpeed - abs(scrollAngle);
    
    float cellRand;
    vec2 ij;
    
   	for(int i = 0; i <= 3; i++) { 
        ij = getCellIJ(uv, dims);
        cellRand = random2d(ij);
        dims *= 2.0;
        //decide whether to subdivide cells again
        float cellRand2 = random2d(ij + 454.4543);
        if (cellRand2 > 0.3){
        	break; 
        }
    }
   
    //draw letters    
    float b = letter(uv, 1.0 / (dims));
	
    //fade in
    float scrollPos = TIME*scrollSpeed + 0.5 + scrollAngle;
    float showPos = -ij.y + cellRand;
    float fade = smoothstep(showPos ,showPos + 0.05, scrollPos );
    b *= fade;
    
    //hide some
    //if (cellRand < 0.1) b = 0.0;
    float h = cellRand;
    vec3 randColor = hsv2rgb(vec3(h, baseSaturation, b));
    
    gl_FragColor = vec4(randColor, b);
    
}

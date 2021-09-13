/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"equirectangular"
	],
	"INPUTS": [
	{
		"NAME" : 		"grid",
		"TYPE" : 		"float",
		"DEFAULT" : 	16.0,
		"MIN" : 		2.0,
		"MAX" : 		64.0
	},
	{
		"NAME" : 		"spiral",
		"TYPE" : 		"float",
		"DEFAULT" : 	9.0,
		"MIN" : 		2.0,
		"MAX" : 		32.0
	},
	{
		"NAME" : 		"gline",
		"TYPE" : 		"float",
		"DEFAULT" :     6.0,
		"MIN" : 		1.0,
		"MAX" : 		24.0
	},
	{
		"NAME" : 		"sline",
		"TYPE" : 		"float",
		"DEFAULT" :     2.0,
		"MIN" : 		0.0,
		"MAX" : 		20.0
	},
	{
		"NAME" : 		"rot1",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		-1.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"rot2",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.0,
		"MIN" : 		-1.0,
		"MAX" : 		 1.0
	}
	]
}*/


////////////////////////////////////////////////////////////
// EquirecRemap  by mojovideotech
//
// based on :
// shadertoy.com/\4dBBWV
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////




#define 	pi   	3.141592653589793 	// pi

float map(float v, float low1, float high1, float low2, float high2) {
	return (v-low1)/(high1-low1)*(high2-low2);
}

vec2 xyzToLonLat(vec3 v) {
    vec3 p = normalize(v);
    float lat = map(asin(p.y), pi*0.5, -pi*0.5, 0.0, 1.0);
    float lon = map(atan(p.x, -p.z), pi, -pi, 0.0, 1.0);
    return vec2(lon, lat);
}

vec3 lonLatToXYZ(vec2 lonLat) {
  float lon = map(lonLat.x, 0.0, 1.0, -pi, pi);
  float lat = map(lonLat.y, 0.0, 1.0, -pi*0.5, pi*0.5);
  float x = sin(lat)*sin(lon);
  float y = cos(lat);
  float z = sin(lat)*cos(lon);
  return vec3(x,y,z);
}

vec3 xRot(vec3 v, float theta) {
  float x = v.x;
  float y = v.y*cos(theta) - v.z*sin(theta);
  float z = v.y*sin(theta) + v.z*cos(theta);
  return vec3(x,y,z);
}

vec3 yRot(vec3 v, float theta) {
  float x = v.z*sin(theta) + v.x*cos(theta);
  float y = v.y;
  float z = v.z*cos(theta) - v.x*sin(theta);
  return vec3(x,y,z);
}

vec3 zRot(vec3 v, float theta) {
  float x = v.x*cos(theta) - v.y*sin(theta);
  float y = v.x*sin(theta) + v.y*cos(theta);
  float z = v.z;
  return vec3(x,y,z);
}

vec2 equiRemap(vec2 lonLat, vec2 delta) {
    vec3 v = lonLatToXYZ(lonLat);
	v = zRot(v,delta.x);
    v = xRot(v,delta.y);
    return xyzToLonLat(v);
}

void main()
{
    float graticuleSize = 1.0/floor(grid);
    float graticuleWeight = 0.001*gline;
	vec2 lonLat = gl_FragCoord.xy / RENDERSIZE.xy;
    lonLat = equiRemap(lonLat, vec2(rot1*pi,rot2*pi));
	gl_FragColor = vec4(lonLat,0.0,1.0);
    vec2 graticuleDist = mod(lonLat + vec2(graticuleWeight*0.5),vec2(graticuleSize));
    if (graticuleDist.x < graticuleWeight || graticuleDist.y < graticuleWeight) { gl_FragColor += vec4(0.5); }
    vec2 p0 = vec2(0.0,0.0);
    vec2 p1 = vec2(floor(spiral),1.0);
    float slope = (p1.y-p0.y)/(p1.x-p0.x);
    float theta = atan(p1.y-p0.y,p1.x-p0.x);
    float lineHalfWeight = 1.0/(30.0-sline)*(1.0+cos(theta));
    bool b0 = atan(mod(lonLat.y,slope)-p0.y,lonLat.x-p0.x+lineHalfWeight) < theta;
    bool b1 = atan(mod(lonLat.y,slope)-p0.y,lonLat.x-p0.x-lineHalfWeight) > theta;
    if (b0 && b1 ){ gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0); }
    
}

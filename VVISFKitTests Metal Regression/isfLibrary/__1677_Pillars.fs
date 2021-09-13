/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "skyroad",
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/Xd33zr by jameswilddev.  Hipster comment about Kirby Air Ride\nWas meant to be something else, ended up here.",
  "INPUTS" : [
	  {
		"NAME": "BUILDING_SIDE_GAPS",
		"TYPE": "float",
		"MIN": 0.0,
		"MAX": 10.0,
		"DEFAULT": 1.8
	  },
	  {
		"NAME": "BUILDING_WIDTH",
		"TYPE": "float",
		"MIN": 0.0,
		"MAX": 1.0,
		"DEFAULT": 0.5
	  },
	  {
		"NAME": "CHECKER_DENSITY",
		"TYPE": "float",
		"MIN": 0.0,
		"MAX": 10.0,
		"DEFAULT": 4.6
	  },
	  {
		"NAME": "SEA_LEVEL",
		"TYPE": "float",
		"MIN": -10.0,
		"MAX": 10.0
	  }
  ]
}
*/


// NDC, expanded to match the aspect ratio.
vec2 aspectNdc;

vec2 onPlane() {
    float dist = abs(aspectNdc.x) / BUILDING_SIDE_GAPS;
    
    return vec2((1.0 / abs(dist)) + fract((TIME * 0.3) / 1.0) * 1.0, aspectNdc.y / (dist));
}

float onPlaneX() {
    float dist = abs(aspectNdc.x) / BUILDING_SIDE_GAPS;
    
    return (1.0 / abs(dist)) + ((TIME * 0.3) / 1.0) * 1.0;
}

float trueBuildingId() {
    float temp = floor(onPlaneX() - BUILDING_WIDTH);
    return temp;
}

float buildingTop() {
    return ceil(sin(trueBuildingId() * 101.0) * 8.0 + 8.0);
}

float buildingBottom() {
    return floor(sin(trueBuildingId() * 131.0) * 8.0 - 8.0);
}

float buildingLocation() {
    return onPlane().x / 1.0;
}

float buildingId() {
    return floor(buildingLocation());
}

float withinBuilding() {
    return fract(buildingLocation());
}

bool onSideOfBuilding() {
    return withinBuilding() > BUILDING_WIDTH;
}

vec3 checker(vec2 uv) {
    uv = fract(uv);
    return uv.x > 0.5 != uv.y > 0.5 ? vec3(1.0, 1.0, 0.6) : vec3(0.6, 0.6, 1.0);
}

vec2 checkerFront() {
    vec2 offset = aspectNdc;
	return CHECKER_DENSITY*(offset * ((buildingId() + 1.0) - fract(TIME * 0.3)));
}

vec2 checkerSide() {
    return onPlane() * CHECKER_DENSITY;
}

bool onFrontOfBuilding() {
    return abs(checkerFront().x) > (BUILDING_SIDE_GAPS * CHECKER_DENSITY)+ (CHECKER_DENSITY * BUILDING_WIDTH);
}

#define SKY_ZENITH vec3(0.1, 0.3, 0.6)
#define SKY_HORIZON vec3(0.8, 0.8, 1.0)

vec3 sky() {
    return mix(SKY_ZENITH, SKY_HORIZON, 1.0 / (1.0 + abs(aspectNdc.y * 3.0)));
}

#define CHECKER_SIDE_COLOR 1.0
#define CHECKER_FRONT_COLOR 0.6
#define SEA_REFLECTION_COLOR vec3(0.6, 0.7, 0.8)
#define SEA_BASE_COLOR vec3(0.05, 0.1, 0.12)

vec3 color() {    
    vec2 uv = onSideOfBuilding() ? checkerFront() : checkerSide();
    
    bool inSky = (onFrontOfBuilding() && onSideOfBuilding()) || uv.y > buildingTop() || uv.y < buildingBottom();
    vec3 preColor = inSky? sky() : mix(checker(uv) * (onSideOfBuilding() ? CHECKER_SIDE_COLOR : CHECKER_FRONT_COLOR), sky(), 1.0 / (abs(aspectNdc.x * 5.0) + 1.0));
    return preColor;
}

void main() {



	aspectNdc = (gl_FragCoord.xy - (RENDERSIZE.xy / 2.0)) / (min(RENDERSIZE.x, RENDERSIZE.y) / 2.0);
    
    
    
	gl_FragColor = vec4(pow(color(), vec3(1.0 / 2.2)), 1.0);
}

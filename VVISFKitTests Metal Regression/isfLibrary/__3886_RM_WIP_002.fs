#define GLSLIFY 1
/*{
    "CATEGORIES": [
        "XXX"
    ],
    "CREDIT": "by gosub7777777",
    "DESCRIPTION": "",
    "INPUTS": [
        {
            "DEFAULT": 1.0,
            "MAX": 100.0,
            "MIN": 0,
            "NAME": "seed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.0,
            "MAX": 10.0,
            "MIN": 1.0,
            "NAME": "numOffSeeds",
            "TYPE": "float"
        },
        {
            "DEFAULT": 5.0,
            "MAX": 10.0,
            "MIN": 0,
            "NAME": "glowSize",
            "TYPE": "float"
        },
        {
            "DEFAULT": 20.0,
            "MAX": 100.0,
            "MIN": 0,
            "NAME": "lineFreq",
            "TYPE": "float"
        },
        {
            "DEFAULT": 2.0,
            "MAX": 10.0,
            "MIN": 0,
            "NAME": "lineSpeed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.5,
            "MAX": 1.0,
            "MIN": 0,
            "NAME": "lineWidthA",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.5,
            "MAX": 1.0,
            "MIN": 0,
            "NAME": "lineWidthB",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "MAX": 1.0,
            "MIN": 0,
            "NAME": "smoothSize",
            "TYPE": "float"
        },
        {
            "DEFAULT": 4.0,
            "MAX": 20.0,
            "MIN": 1.0,
            "NAME": "fov",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "FLOAT": false,
            "PERSISTENT": true,
            "TARGET": "first",
            "WIDTH": "$WIDTH",
			"HEIGHT": "$HEIGHT"
        },
        {
            "TARGET": "final",
            "WIDTH": "$WIDTH",
			"HEIGHT": "$HEIGHT"
        }
    ]
}
*/

// float numOffSeeds = 4.0;
float globalSeed;
vec2 globalUV;
vec2 globalRenderSize;
vec2 globalNormUV; 

// #pragma glslify: fxaa = require(glsl-fxaa) 
// RAYTRACING
vec2 doModel(vec3 p);
float lensLength_0   = fov;

float calcAO( in vec3 pos, in vec3 nor )
{
	float occ = 0.0;
    float sca = 1.0;
    for( int i=0; i<5; i++ )
    {
        float hr = 0.01 + 0.12 * float( i ) / 4.0;
        vec3 aopos =  nor * hr + pos;
        float dd = doModel( aopos ).x;
        occ += -(dd-hr)*sca;
        sca *= 0.95;
    }
    return clamp( 1.0 - 3.0*occ, 0.0, 1.0 );    
}

// Originally sourced from https://www.shadertoy.com/view/ldfSWs
// Thank you Iñigo :)

vec2 calcRayIntersection(vec3 rayOrigin, vec3 rayDir, float maxd, float precis) {
  float latest = precis * 2.0;
  float dist   = +0.0;
  float type   = -1.0;
  vec2  res    = vec2(-1.0, -1.0);

  for (int i = 0; i < 90; i++) {
    if (latest < precis || dist > maxd) break;

    vec2 result = doModel(rayOrigin + rayDir * dist);

    latest = result.x;
    type   = result.y;
    dist  += latest;
  }

  if (dist < maxd) {
    res = vec2(dist, type);
  }

  return res;
}

vec2 calcRayIntersection(vec3 rayOrigin, vec3 rayDir) {
  return calcRayIntersection(rayOrigin, rayDir, 20.0, 0.001);
}

// #pragma glslify: calcAO = require('glsl-sdf-ops/ao', map = doModel )
// Originally sourced from https://www.shadertoy.com/view/ldfSWs
// Thank you Iñigo :)

vec3 calcNormal(vec3 pos, float eps) {
  const vec3 v1 = vec3( 1.0,-1.0,-1.0);
  const vec3 v2 = vec3(-1.0,-1.0, 1.0);
  const vec3 v3 = vec3(-1.0, 1.0,-1.0);
  const vec3 v4 = vec3( 1.0, 1.0, 1.0);

  return normalize( v1 * doModel( pos + v1*eps ).x +
                    v2 * doModel( pos + v2*eps ).x +
                    v3 * doModel( pos + v3*eps ).x +
                    v4 * doModel( pos + v4*eps ).x );
}

vec3 calcNormal(vec3 pos) {
  return calcNormal(pos, 0.002);
}

// #pragma glslify: softshadow = require('glsl-sdf-ops/softshadow', map = doModel )

vec2 squareFrame(vec2 screenSize) {
  vec2 position = 2.0 * (gl_FragCoord.xy / screenSize.xy) - 1.0;
  position.x *= screenSize.x / screenSize.y;
  return position;
}

vec2 squareFrame(vec2 screenSize, vec2 coord) {
  vec2 position = 2.0 * (coord.xy / screenSize.xy) - 1.0;
  position.x *= screenSize.x / screenSize.y;
  return position;
}

mat3 calcLookAtMatrix(vec3 origin, vec3 target, float roll) {
  vec3 rr = vec3(sin(roll), cos(roll), 0.0);
  vec3 ww = normalize(target - origin);
  vec3 uu = normalize(cross(ww, rr));
  vec3 vv = normalize(cross(uu, ww));

  return mat3(uu, vv, ww);
}

vec3 getRay(mat3 camMat, vec2 screenPos, float lensLength) {
  return normalize(camMat * vec3(screenPos, lensLength));
}

vec3 getRay(vec3 origin, vec3 target, vec2 screenPos, float lensLength) {
  mat3 camMat = calcLookAtMatrix(origin, target, 0.0);
  return getRay(camMat, screenPos, lensLength);
}

// PRIMITIVES
float sdPlane( vec3 p, vec4 n )
{
  // n must be normalized
  return dot(p,n.xyz) + n.w;
}

float sdBox( vec3 p, vec3 b )
{
  vec3 d = abs(p) - b;
  return min(max(d.x,max(d.y,d.z)),0.0) +
         length(max(d,0.0));
}

float udRoundBox( vec3 p, vec3 b, float r )
{
  return length(max(abs(p)-b,0.0))-r;
}

float sdTorus( vec3 p, vec2 t )
{
  vec2 q = vec2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}

float sdCapsule( vec3 p, vec3 a, vec3 b, float r )
{
    vec3 pa = p - a, ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}

float sdTriPrism( vec3 p, vec2 h )
{
    vec3 q = abs(p);
    return max(q.z-h.y,max(q.x*0.866025+p.y*0.5,-p.y)-h.x*0.5);
}

float sdHexPrism( vec3 p, vec2 h )
{
    vec3 q = abs(p);
    return max(q.z-h.y,max((q.x*0.866025+q.y*0.5),q.y)-h.x);
}

float sdSphere( vec3 p, float s )
{
  return length( p ) - s;
}

float dot2( in vec3 v ) { return dot(v,v); }
float udTriangle( vec3 p, vec3 a, vec3 b, vec3 c )
{
    vec3 ba = b - a; vec3 pa = p - a;
    vec3 cb = c - b; vec3 pb = p - b;
    vec3 ac = a - c; vec3 pc = p - c;
    vec3 nor = cross( ba, ac );

    return sqrt(
    (sign(dot(cross(ba,nor),pa)) +
     sign(dot(cross(cb,nor),pb)) +
     sign(dot(cross(ac,nor),pc))<2.0)
     ?
     min( min(
     dot2(ba*clamp(dot(ba,pa)/dot2(ba),0.0,1.0)-pa),
     dot2(cb*clamp(dot(cb,pb)/dot2(cb),0.0,1.0)-pb) ),
     dot2(ac*clamp(dot(ac,pc)/dot2(ac),0.0,1.0)-pc) )
     :
     dot(nor,pa)*dot(nor,pa)/dot2(nor) );
}

float sdCone( in vec3 p, in vec3 c )
{
    vec2 q = vec2( length(p.xz), p.y );
    float d1 = -p.y-c.z;
    float d2 = max( dot(q,c.xy), p.y);
    return length(max(vec2(d1,d2),0.0)) + min(max(d1,d2), 0.);
}

float sdCappedCylinder( vec3 p, vec2 h )
{
  vec2 d = abs(vec2(length(p.xz),p.y)) - h;
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

float random(vec2 co)
{
    float a = 12.9898;
    float b = 78.233;
    float c = 43758.5453;
    float dt= dot(co.xy ,vec2(a,b));
    float sn= mod(dt,3.14);
    return fract(sin(sn) * c);
}

// OPS
float opU_1( float d1, float d2 )
{
    return min(d1,d2);
}

vec2 opU_1( vec2 d1, vec2 d2 ){
	return ( d1.x < d2.x ) ? d1 : d2;
}

float smin(float a, float b, float k) {
  float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
  return mix(b, a, h) - k * h * (1.0 - h);
}

// #pragma glslify: opUSPow = require(glsl-smooth-min/pow) 
// #pragma glslify: opUSExp = require(glsl-smooth-min/exp) 

vec2 smin(vec2 a, vec2 b, float k){
    // return vec2(opUS(a.x, b.x, k), opUS(a.y, b.y, k));
    return vec2(smin(a.x, b.x, k), smin(a.y, b.y, k));
}

vec2 opU_0(vec2 a, vec2 b){
    return smin(a,b,0.8 * smoothSize);
}

// DEFINE RENDER MODEL
vec2 doModel(vec3 p) {
    
    vec3 roomSize = vec3(1.0 / lensLength_0);
    vec3 roomPos = vec3(0.0);
    roomSize.z = 1.0;
    roomSize.x *= globalRenderSize.x / globalRenderSize.y;
    roomPos.z = roomSize.z - 0.1;

    float roomD = sdBox( p - roomPos, roomSize);;
    roomSize.xy *= 3.0;
    // float roomD2 = sdBox( p - roomPos, roomSize);
    float roomD2 = sdPlane( p - vec3(  0.0, 0.0,  0.0 ), normalize(vec4( 0.0, 0.0, -1.0, 0.0 )));
    roomD =  max(-roomD,roomD2);

    float height = sin(TIME * 0.4)*0.3 + .5;

    float mID1 = 1.;
    float mID2 = 2.;
    float mID3 = 3.;

    vec3 objPos = vec3(  .0, sin(TIME * 2.34 + globalSeed * 123.0123 )*0.01,  .0 );
    objPos.z = roomSize.z * 0.5; 

	vec2 res = vec2(0.0, mID1);
    
    objPos = p - objPos;
    objPos.y += sin(objPos.x * 20.0 + TIME * 5.0) * 0.1; 
    // objPos.x += sin(objPos.z * 1.2 + TIME * 5.0) * 0.1; 

    if(globalSeed >= 0.75){
	    res = vec2( sdSphere( objPos, 0.2 ), mID1 );
    }else if(globalSeed >= 0.5){
        res = vec2( sdBox( objPos, vec3( .25 )), mID1 );
    }else if(globalSeed >= 0.25){
        res = vec2( sdTorus( objPos, vec2( 0.20, 0.05 )), mID1 );
    }else if(globalSeed >= 0.0){
        res = vec2( sdCone( objPos, vec3( 0.8, 0.6, 0.3 )), mID1 );
    }
    // res = opU( res, vec2( udRoundBox( p - vec3( 1.0, sin(TIME * 0.64)*0.4 + .5,  1.0 ), vec3( .15 ), 0.1 )               , mID1 ));
    // res = opU( res, vec2( sdTorus(    p - vec3(  .0, sin(TIME * 0.344)*0.4 + .5, 1.0 ), vec2( 0.20, 0.05 ))              , mID2 ));
    // res = opU( res, vec2( sdCapsule(  p,  vec3(-1.3, sin(TIME * 0.754)*0.5 + .5,  -.1 ), vec3( -1.0, 0.20, 0.2), 0.1 )    , mID1 ));
    // res = opU( res, vec2( sdTriPrism( p - vec3(-1.0, sin(TIME * 0.234)*0.4 + .5, -1.0 ), vec2(0.25,0.05) )                , mID1 ));
    // res = opU( res, vec2( sdCylinder( p - vec3( 1.0, sin(TIME * 0.54324)*0.5 + .5, -1.0 ), vec2(0.1,0.2) )                  , mID3 ));
    // res = opU( res, vec2( sdCone(     p - vec3( 0.0, sin(TIME * 0.2344)*0.4 + .5, -1.0 ), vec3( 0.8, 0.6, 0.3 ))           , mID3 ));
    // res = opU( res, vec2( sdHexPrism( p - vec3(-1.0, sin(TIME * 0.534)*0.43 + .5,  1.0 ), vec2( 0.25, 0.05 ))              , mID1 ));

    float kappa = smoothSize;
    res = smin(res, vec2( roomD , mID3 ), kappa);
    // res = vec2(min(roomD,res.x), mID3 );

  	return res;
}

// COMPUTE LIGHTING
vec3 lighting( vec3 pos, vec3 nor, vec3 ro, vec3 rd) {

	float outBuff = 1.0;

	float occ = calcAO( pos, nor );
    outBuff = exp( occ * 3.);

	return vec3(outBuff);
}

// COMPUTE MATERIAL COLOR
vec3 material( float materialID, vec3 pos, vec3 nor, vec3 ro, vec3 rd  ){
	// return sin( vec3(1.0,0.2,0.5) * ( materialID * 10.0 ) ) * 0.5 + 0.5;
    // if(materialID >= 0.55){
    //     return vec3(0.0,1.0,0.0);
    // }
    vec3 color = vec3(0.9,0.002,0.001);

    float line = fract(pos.x * 10.) + fract(pos.z * 10.);
    line = distance(vec2(0), pos.xz );
    line = sin(line * lineFreq - TIME * lineSpeed)  * 0.5 + 0.5;
    // line *= sin( pos.y * 100.0) * 0.5 + 0.5;
    
    line =  smoothstep(line,  lineWidthA, lineWidthB );

    return color * line;

    // return vec3( fract(materialID*100.0));
}

vec3 renderRays(){
    vec3 color  = vec3(0.0);

	vec3 ro = vec3(0, 0.0, -1.0);
    vec3 rayTarget    = vec3(0, 0, 0);

    vec2  screenPos    = squareFrame(globalRenderSize.xy);

    vec3 rd = getRay(ro, rayTarget, screenPos, lensLength_0);
	
    vec2 res = calcRayIntersection( ro, rd );

    vec3 pos = ro + rd * res.x;
    vec3 nor = calcNormal( pos, 0.00001 );
    color = material( res.y, pos, nor, ro, rd ) * lighting( pos, nor, ro, rd );
    // color = material( res.y );
    
    // color = lighting( pos, nor, ro, rd );
    float fog = 1.0 - (res.x * 0.5 - 0.5);
    color = mix( color, vec3(0.0), fog);
    // color = normalize(color);
    // color *= fog;

    // color = vec3(fog);
	
    // gamma correction
    color = pow( color, vec3( 0.23 ));

    // return vec3(fog);
    return color;
}

vec4 generateRayMarchingScene(){
    vec3 rr = renderRays();
    return vec4(rr, 1.0);
}

void main() {
    globalUV = gl_FragCoord.xy;
    globalNormUV = gl_FragCoord.xy/RENDERSIZE.xy;
    globalRenderSize = RENDERSIZE.xy;
    
    if(PASSINDEX == 0){
        float ns = floor(numOffSeeds);
        vec2 grid = floor(globalNormUV * ns)/ns;       
        // globalSeed = (grid.x/ns + grid.y);
        globalSeed = random(grid + seed);
        // globalNormUV.x = fract(globalNormUV.x * ns);
        // globalRenderSize.x /= ns; 
        // globalRenderSize.y /= ns; 
        // globalUV = fract(globalNormUV * ns) * globalRenderSize;
    
        // renderSize.x = renderSize.x / ns ;
        // pixRS.x = pixRS.x / (ns * 18.0);

        // vec3 rays = renderRays(); 
        // vec3 rays = vec3(1.0,1.0,0.0);
        
        vec3 rays = generateRayMarchingScene().rgb;
	    gl_FragColor.rgb = rays;

        // gl_FragColor.rgb = vec3(globalSeed);
        // gl_FragColor.rgb = vec3(globalNormUV.xy, 0.0);
    }

    if(PASSINDEX == 1){
        // gl_FragColor.rgb = pass0.rgb;
        // gl_FragColor.rgb = fxaa(blurBuffer, gl_FragCoord.xy, RENDERSIZE).rgb;
    
	    // gl_FragColor.rgb = blur(uv, glowSize);

	    // gl_FragColor.rgb = pass0.rgb + blur(uv, glowSize);
        
        // vec3 outImg = fxaa(first, gl_FragCoord.xy, RENDERSIZE).rgb + blur(first, uv, glowSize);        	    
        // gl_FragColor.rgb = outImg;

        gl_FragColor.rgb = IMG_PIXEL(first, globalUV).rgb;
    }

	gl_FragColor.a = 1.;
}
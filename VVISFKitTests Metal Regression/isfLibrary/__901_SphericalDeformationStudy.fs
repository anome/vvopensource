/*{
	"CREDIT": "by mojovideotech",
    "CATEGORIES" : [
    "3d",
    "equirectangular",
    "Automatically Converted"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/lt3XW7 by soma_arc.  Deformation of spherical images. Rotation -> Translation -> Scaling\nReference:\nSquares that Look Round: Transforming Spherical Images by Saul Schleimer and Henry Segerman\nhttp://archive.bridgesmathart.org/2016/bridges2016-15.html\n",
  "INPUTS" : [

  ]
}
*/


/*
Created by soma_arc - 2016
This work is licensed under Creative Commons Attribution-ShareAlike 3.0 Unported.
*/


vec2 rand2n(vec2 co, float sampleIndex) {
    vec2 seed = co * (sampleIndex + 1.0);
    seed+=vec2(-1,1);
    // implementation based on: lumina.sourceforge.net/Tutorials/Noise.html
    return vec2(fract(sin(dot(seed.xy ,vec2(12.9898,78.233))) * 43758.5453),
                fract(cos(dot(seed.xy ,vec2(4.898,7.23))) * 23421.631));
}

const float NO_ANSWER = -999999.;

const float PI = 3.14159265359;
const float fourPI = 12.566368;
const float EPSILON = 0.01;

const vec3 BLACK = vec3(0);
const vec3 WHITE = vec3(1);
const vec3 LIGHT_GRAY = vec3(0.78);
const vec3 RED = vec3(1, 0, 0);
const vec3 GREEN = vec3(0, .78, 0);
const vec3 BLUE = vec3(0, 0, 1);
const vec3 YELLOW = vec3(1, 1, 0);
const vec3 PINK = vec3(.78, 0, .78);
const vec3 LIGHT_BLUE = vec3(0, 1, 1);

const vec3 AMBIENT_FACTOR = vec3(0.1);

const float NO_HIT = 99999999.;

const int MTL_DIFFUSE = 0;
const int MTL_TRANSPARENT = 1;

const int OBJ_PLANE = 0;
const int OBJ_SPHERE = 1;

struct SL2C{
	vec2 a;
    vec2 b;
    vec2 c;
    vec2 d;
};
    
SL2C g_mobius;    

const vec2 COMPLEX_ONE = vec2(1, 0);
const vec2 COMPLEX_ZERO = vec2(0);
const SL2C MAT_UNIT = SL2C(COMPLEX_ONE, COMPLEX_ZERO,
                      	   COMPLEX_ZERO, COMPLEX_ONE);
const float PI_2 = PI / 2.;
const float TWO_PI = PI * 2.;
const float THREE_PI_2 = 3. * PI / 2.;

vec2 equirectangularCoord(vec3 coordOnSphere){
	vec3 dir = (coordOnSphere);
    float l = atan(dir.z, dir.x);
    if (l < 0.) l += TWO_PI;
    return vec2(l, acos(dir.y));
}

vec3 coordOnSphere(float theta, float phi){
	return vec3(sin(phi) * cos(theta), 
                cos(phi),
                sin(phi) * sin(theta));
} 

vec2 compProd(const vec2 a, const vec2 b){
	return vec2(a.x * b.x - a.y * b.y,
                a.x * b.y + a.y * b.x);
}

vec2 compQuot(const vec2 a, const vec2 b){
	float denom = dot(b, b);
    return vec2((a.x * b.x + a.y * b.y) / denom,
                (a.y * b.x - a.x * b.y) / denom);
}

vec2 conjugate(const vec2 a){
	const vec2 conj = vec2(1, -1);
    return a * conj;
}

SL2C matInverse(const SL2C m){
    vec2 invDet =  compQuot(COMPLEX_ONE, (compProd(m.a, m.d)-compProd(m.b, m.c)));
	return SL2C(compProd(m.d, invDet), compProd(m.b * -1., invDet),
                compProd(m.c * -1., invDet), compProd(m.a, invDet));
}

SL2C matProd(const SL2C m1, const SL2C m2){
    return SL2C(compProd(m1.a, m2.a) + compProd(m1.b, m2.c),
                compProd(m1.a, m2.b) + compProd(m1.b, m2.d),
                compProd(m1.c, m2.a) + compProd(m1.d, m2.c),
                compProd(m1.c, m2.b) + compProd(m1.d, m2.d));
}

vec4 applyMatVec(const SL2C m, const vec4 c){
	return vec4(compProd(m.a, c.xy) + compProd(m.b, c.zw),
                compProd(m.c, c.xy) + compProd(m.d, c.zw));
}

vec4 CP1FromSphere(vec3 pos){
	if(pos.y < 0.)
        return vec4(pos.x, pos.z, 1. - pos.y, 0);
    else
        return vec4(1. + pos.y, 0, pos.x, -pos.z);
}

vec3 sphereFromCP1(vec4 p){
	vec2 z1 = p.xy;
    vec2 z2 = p.zw;
    if(length(z2) > length(z1)){
    	vec2 z = compQuot(z1, z2);
        float denom = 1. + dot(z, z);
        return vec3(2. * z.x / denom, (denom - 2.) / denom, 2. * z.y / denom);
    }else{
    	vec2 z = conjugate(compQuot(z2, z1));
        float denom = 1. + dot(z, z);
        return vec3(2. * z.x / denom, (2. - denom) / denom, 2. * z.y / denom);
    }
}

vec2 reverseStereoProject(const vec3 pos){
	return vec2(pos.x, pos.z) / (1. - pos.z);
}

vec3 stereoProject(vec2 pos){
    pos *= .5;
    float x = pos.x;
    float y = pos.y;
    float x2y2 = x * x + y * y;
    return vec3((2. * x) / (1. + x2y2),
                (-1. + x2y2) / (1. + x2y2),
                (2. * y) / (1. + x2y2));
}

SL2C infZeroOneToTriple(const vec4 p, const vec4 q, const vec4 r){
	vec2 p1 = p.xy; vec2 p2 = p.zw;
    vec2 q1 = q.xy; vec2 q2 = q.zw;
    vec2 r1 = r.xy; vec2 r2 = r.zw;
    SL2C m = SL2C(p1, q1, p2, q2);
    SL2C mInv = matInverse(m);
    vec4 v = applyMatVec(mInv, r);
    return SL2C(compProd(v.xy, p1), compProd(v.zw, q1),
                compProd(v.xy, p2), compProd(v.zw, q2));
}

SL2C twoTriplesToSL(const vec4 a1, const vec4 b1, const vec4 c1,
                    const vec4 a2, const vec4 b2, const vec4 c2){
	return matProd(infZeroOneToTriple(a2, b2, c2), 
                   matInverse(infZeroOneToTriple(a1, b1, c1)));
}

vec3 vectorPerpToPQ(vec3 p, vec3 q){
    if(abs(dot(p, q) + 1.) < 0.0001){
        if(abs(dot(p, vec3(1, 0, 0))) > 0.999){
        	return vec3(0, 1, 0);
        }else{
        	return normalize(cross(p, vec3(1, 0, 0)));
        }
    }else{
    	return normalize(cross(p, q));
    }
}

SL2C rotateAroundAxisSpherePointsPQ(const vec3 p, const vec3 q, const float theta){
	vec4 CP1p = CP1FromSphere(p);
    vec4 CP1q = CP1FromSphere(q);
	vec3 r = vectorPerpToPQ(p, q);
    vec4 CP1r = CP1FromSphere(r);
    SL2C st = twoTriplesToSL(CP1p, CP1q, CP1r, 
            	           vec4(0, 0, 1, 0),
                           vec4(1, 0, 0, 0), 
                           vec4(1, 0, 1, 0));
    SL2C mTheta = SL2C(vec2(cos(theta), sin(theta)), COMPLEX_ZERO,
                       COMPLEX_ZERO, COMPLEX_ONE);
    return matProd( matProd(matInverse(st), mTheta), st);
}

SL2C rotateSpherePointsPQ(const vec3 p, const vec3 q){
	vec4 CP1p = CP1FromSphere(p);
    vec4 CP1q = CP1FromSphere(q);
    if(abs(dot(p, q) - 1.) < 0.0001){
    	return SL2C(COMPLEX_ONE, COMPLEX_ZERO, COMPLEX_ZERO, COMPLEX_ONE);
    }else{
    	vec3 r = vectorPerpToPQ(p, q);
        vec4 CP1r = CP1FromSphere(r);
        vec4 CP1mr = CP1FromSphere(-r);
        return twoTriplesToSL(CP1p, CP1r, CP1mr, CP1q, CP1r, CP1mr);
    }
}

SL2C rotateAroundAxis(const vec3 p, const float theta){
	return rotateAroundAxisSpherePointsPQ(p, -p, theta);
}

SL2C threePointsToThreePoints(const vec3 p1, const vec3 q1, const vec3 r1,
                              const vec3 p2, const vec3 q2, const vec3 r2){
	return twoTriplesToSL(CP1FromSphere(p1), CP1FromSphere(q1), CP1FromSphere(r1),
                          CP1FromSphere(p2), CP1FromSphere(q2), CP1FromSphere(r2));
}

SL2C translateAlongAxis(const vec3 p, const vec3 q,
                        const vec3 r1, const vec3 r2){
	return threePointsToThreePoints(p, q, r1, p, q, r2);
}

SL2C zoomIn(const vec3 p, const float zoomFactor){
	SL2C rot = rotateSpherePointsPQ(p, coordOnSphere(0., 0.));
    SL2C scl = SL2C(vec2(zoomFactor, 0), COMPLEX_ZERO,
                    COMPLEX_ZERO, COMPLEX_ONE);
    return matProd(matProd(matInverse(rot), scl), rot);
}

int g_objId = -1;
int g_mtl = -1;
vec4 intersectSphere(int objId, int mtl,
                     vec3 sphereCenter, float radius,
                     vec3 rayOrigin, vec3 rayDir,
                     vec4 isect){
    vec3 v = rayOrigin - sphereCenter;
    float b = dot(rayDir, v);
    float c = dot(v, v) - radius * radius;
    float d = b * b - c;
    if(d >= 0.){
        float s = sqrt(d);
        float t = -b - s;
        if(t <= EPSILON) t = -b + s;
        if(EPSILON < t && t < isect.x){
            vec3 intersection = (rayOrigin + t * rayDir);
            g_objId = objId;
            g_mtl = mtl;
            return vec4(t, normalize(intersection - sphereCenter));
        }
    }
    return isect;
}

vec4 intersectPlane(int objId, int mtl,
                    vec3 p, vec3 n,
                    vec3 rayOrigin, vec3 rayDir, vec4 isect){
    float d = -dot(p, n);
    float v = dot(n, rayDir);
    float t = -(dot(n, rayOrigin) + d) / v;
    if(EPSILON < t && t < isect.x){
        g_objId = objId;
        g_mtl = mtl;
        return vec4(t, n);
    }
    return isect;
}


vec2 circleInverse(vec2 pos, vec2 circlePos, float circleR){
    return ((pos - circlePos) * circleR * circleR)/(length(pos - circlePos) * length(pos - circlePos) ) + circlePos;
}

const float RECT_SIZE = PI/18.;
vec3 equirectangularMap(const vec2 angles){
    float theta = angles.x; float phi = angles.y;
    if(abs(phi - PI_2) < .05) return BLUE;
    if( phi <= .1 ) return PINK;
    if( phi >= PI - .1) return LIGHT_BLUE;
    if(abs(theta - PI) <= .05) return RED;
    if(abs(theta - PI_2) <= .05) return GREEN;
    if(abs(theta - THREE_PI_2) <= .05) return GREEN;
    if(theta <= .025 || theta >= TWO_PI - .025) return RED;

    int x = int(mod(floor(theta / RECT_SIZE)+floor(phi / RECT_SIZE), 2.));
    if(x == 0){
    	return WHITE;
    }else{
    	return BLACK;
    }
}

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec4 getIntersection(vec3 eye, vec3 ray){
    vec4 isect = vec4(NO_HIT);
    isect = intersectPlane(OBJ_PLANE, MTL_DIFFUSE,
                           vec3(0, -1, 0), vec3(0, 1, 0),
                           eye, ray, isect);
    isect = intersectSphere(OBJ_SPHERE, MTL_DIFFUSE,
                            vec3(0), 1.,
                            eye, ray, isect);
    return isect;
}

bool visible(vec3 eye, vec3 target){
    vec3 v = normalize(target - eye);
    return getIntersection(eye, v).x == NO_HIT;
}

vec3 diffuseLighting(const vec3 p, const vec3 n, const vec3 diffuseColor,
                     const vec3 lightPos, const vec3 lightPower){
    vec3 v = lightPos - p;
    float d = dot(n, normalize(v));
    float r = length(v);
    return (d > 0. )//&& visible(p + EPSILON * n, lightPos)) 
        ?
        (lightPower * (d / (fourPI * r * r))) * diffuseColor
        : BLACK;
}

vec3 LIGHT_DIR = normalize(vec3(0.0, 1., 0.5));
vec3 LIGHT_POS = vec3(3, 5, 0);
vec3 LIGHT_POWER = vec3(300.);

vec3 calcColor(vec3 eye, vec3 ray){
    vec3 l = BLACK;
    vec4 isect = getIntersection(eye, ray);
	if(isect.x != NO_HIT){
		vec3 matColor = WHITE;
        vec3 normal = isect.yzw;
		vec3 intersection = eye + isect.x * ray;
		if(g_objId == OBJ_PLANE){
			vec4 z = CP1FromSphere(stereoProject(intersection.xz));
            vec3 s = sphereFromCP1(applyMatVec(g_mobius, z));
			vec2 angles = equirectangularCoord(s);
			matColor = equirectangularMap(angles);
			//matColor = LIGHT_GRAY;
		}else if(g_objId == OBJ_SPHERE){
            vec4 z = CP1FromSphere(intersection);
			vec3 s = sphereFromCP1(applyMatVec(g_mobius, z));
            vec2 angles = equirectangularCoord(s); 
			matColor = equirectangularMap(angles);
		}
		// diffuse lighting by directionalLight
		//vec3 diffuse = clamp(dot(normal, LIGHT_DIR), 0., 1.) * matColor;
		vec3 diffuse = diffuseLighting(intersection, normal, matColor,
									   LIGHT_POS, LIGHT_POWER);
		vec3 ambient = matColor * AMBIENT_FACTOR;
		l += (diffuse + ambient);
	}  
    return l;
}

vec3 calcRay (const vec3 eye, const vec3 target,
              const vec3 up, const float fov,
              const float width, const float height, const vec2 coord){
    float imagePlane = (height * .5) / tan(radians(fov) * .5);
    vec3 v = normalize(target - eye);
    vec3 focalXAxis = normalize(cross(v, up));
    vec3 focalYAxis =  normalize(cross(v, focalXAxis ));
    vec3 center = v * imagePlane;
    vec3 origin = center - (focalXAxis * (width  * .5)) - (focalYAxis * (height * .5));
    return normalize(origin + (focalXAxis * coord.x) + (focalYAxis * (height - coord.y)));
}

vec3 sphericalView(vec3 dir){
    vec4 z = CP1FromSphere(dir);
    vec2 angles = equirectangularCoord(sphereFromCP1(applyMatVec(g_mobius, z)));
	return equirectangularMap(angles);

}

const float DISPLAY_GAMMA_COEFF = 1. / 2.2;
vec3 gammaCorrect(vec3 rgb) {
	return vec3((min(pow(rgb.r, DISPLAY_GAMMA_COEFF), 1.)),
				(min(pow(rgb.g, DISPLAY_GAMMA_COEFF), 1.)),
				(min(pow(rgb.b, DISPLAY_GAMMA_COEFF), 1.)));
}

float scene(float t, float w, float s){
    return clamp(t - w, 0.0, s) / s;  
}

const float SAMPLE_NUM = 5.;
void main() {



    float t = mod(TIME, 18.);
    float rotateT = scene(t, 0., 6.) * TWO_PI;
	float translateT = scene(t, 6., 6.) * TWO_PI;
	float zoomT = scene(t, 12., 6.) * TWO_PI;
    
    vec3 s1 = coordOnSphere(PI, 0.);
    vec3 s2 = coordOnSphere(PI, PI);
    vec3 r1 = coordOnSphere(PI, PI_2);
    vec3 r2 = coordOnSphere(PI + sin(translateT), PI_2 + sin(translateT));
    vec3 s = coordOnSphere(PI, PI_2);
    g_mobius = MAT_UNIT;
    g_mobius = matProd(matInverse(rotateAroundAxis(s, PI/4. * sin(rotateT))),
                       g_mobius);
    g_mobius = matProd(translateAlongAxis(s1, s2, r1, r2),
                       g_mobius);
	g_mobius = matProd(zoomIn(s, 1. + 2. * abs(sin(zoomT))),
                       g_mobius);
    float ratio = RENDERSIZE.x / RENDERSIZE.y / 2.0;
    const vec3 up = vec3(0, 1, 0);
    const float fov = 60.;
    vec3 sum = vec3(0);
    for(float i = 0. ; i < SAMPLE_NUM ; i++){
        vec2 coordOffset = rand2n(gl_FragCoord.xy, i);
        if(gl_FragCoord.x < RENDERSIZE.x/2.){
            float r = 3.;
            vec3 eye = vec3(r * sin(TIME), 1.5, r * cos(TIME));
    		vec3 target = vec3(0, 0, 0);
        	vec3 ray = calcRay(eye, target, up, fov,
                         	   RENDERSIZE.x/2., RENDERSIZE.y,
                         	   gl_FragCoord.xy + coordOffset);
        	sum += calcColor(eye, ray);
        }else{
            vec2 halfRes = RENDERSIZE.xy / 2.;
            vec2 size = RENDERSIZE.xx / vec2(2, 4); // 2:1
        	vec2 p = vec2(gl_FragCoord.x - size.x, gl_FragCoord.y) / size;//[0, 1]
            if(p.y <= 1.){
                float theta = p.x * 2. * PI;
                float phi = p.y * PI;
                vec4 z = CP1FromSphere(coordOnSphere(theta, phi));
				vec2 angles = equirectangularCoord(sphereFromCP1(applyMatVec(g_mobius, z)));
                sum += equirectangularMap(angles);
                //sum += equirectangularMap(theta, phi);
            }else{
                vec3 eye = vec3(0, 0, 0);
    			vec3 target = vec3(1, 0, 0);//vec3(sin(TIME), sin(TIME), cos(TIME));
            	vec3 ray = calcRay(eye, target, up, fov,
                         	  	   RENDERSIZE.x/2., RENDERSIZE.y - size.y,
                         	   	   gl_FragCoord.xy - vec2(RENDERSIZE.x/2., size.y) + coordOffset);
        		sum += sphericalView(ray);
            }
        }
    }
    gl_FragColor = vec4(gammaCorrect(sum/SAMPLE_NUM), 1.);
}

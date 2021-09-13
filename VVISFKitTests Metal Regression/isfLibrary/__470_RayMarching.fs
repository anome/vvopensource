/*{
	"CREDIT": "by PALUCK",
	"CATEGORIES" : [ "rayMarching de SOCIAMIX"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
	{
     	"NAME" :		"rCam",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.0,
     	"MIN" : 		-2,
      	"MAX" :			2
	},
	{
     	"NAME" :		"yCam",
     	"TYPE" : 		"float",
    	"DEFAULT" :		1.0,
     	"MIN" : 		0,
      	"MAX" :			10
	},
	{
     	"NAME" :		"zCam",
     	"TYPE" : 		"float",
    	"DEFAULT" :		-4.0,
     	"MIN" : 		-10,
      	"MAX" :			0
	},
  	{
     	"NAME" :		"rLigth",
     	"TYPE" : 		"float",
    	"DEFAULT" :		-1.0,
     	"MIN" : 		-2,
      	"MAX" :			2
	},
	{
     	"NAME" :		"hLigth",
     	"TYPE" : 		"float",
    	"DEFAULT" :		2.0,
     	"MIN" : 		0,
      	"MAX" :			10
	},
    {
      "NAME" : "ambiance",
      "TYPE" : "color",
      "DEFAULT" : [
        0.0,
        0.0,
        0.0,
        1.0
      ]
    },
    {
      "NAME" : "Fond",
      "TYPE" : "color",
      "DEFAULT" : [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
	{
			"NAME": "ImageFond",
			"TYPE": "image"
	},
  	{
     	"NAME" :		"Sphere",
     	"TYPE" : 		"bool",
    	"DEFAULT" :		0
	},
  	{
     	"NAME" :		"Torus",
     	"TYPE" : 		"bool",
    	"DEFAULT" :		1
	},
  	{
     	"NAME" :		"Box",
     	"TYPE" : 		"bool",
    	"DEFAULT" :		0
	},
  	{
     	"NAME" :		"HexaPrisme",
     	"TYPE" : 		"bool",
    	"DEFAULT" :		0
	},
  	{
     	"NAME" :		"Cylindre",
     	"TYPE" : 		"bool",
    	"DEFAULT" :		0
	},
  	{
     	"NAME" :		"Pyramide",
     	"TYPE" : 		"bool",
    	"DEFAULT" :		0
	},
	{
			"NAME": "objet",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2,
				3,
				4,
				5
			],
			"LABELS": [
				"Sphere",
				"Torus",
				"Box",
				"HexaPrisme",
				"Cylindre",
				"Pyramide"
			],
			"DEFAULT": 0
		},
	{
     	"NAME" :		"dimension1",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.5,
     	"MIN" : 		0,
      	"MAX" :			1
	},
	{
     	"NAME" :		"dimension2",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.5,
     	"MIN" : 		0,
      	"MAX" :			1
	},
	{
     	"NAME" :		"dimension3",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.5,
     	"MIN" : 		0,
      	"MAX" :			1
	},
	{
     	"NAME" :		"dimension4",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.5,
     	"MIN" : 		0,
      	"MAX" :			1
	},
	{
     	"NAME" :		"x1",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0,
     	"MIN" : 		-2,
      	"MAX" :			2
	},
	{
     	"NAME" :		"y1",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0,
     	"MIN" : 		-10,
      	"MAX" :			10
	},

	{
     	"NAME" :		"z1",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0,
     	"MIN" : 		-10,
      	"MAX" :			10
	},
		{
     	"NAME" :		"xRotation",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0,
     	"MIN" : 		-2,
      	"MAX" :			2
	},
	{
     	"NAME" :		"yRotation",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0,
     	"MIN" : 		-2,
      	"MAX" :			2
	},

	{
     	"NAME" :		"zRotation",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0,
     	"MIN" : 		-2,
      	"MAX" :			2
	},
	{
      "NAME" : "Objet",
      "TYPE" : "color",
      "DEFAULT" : [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
	{
			"NAME": "ImageObjet",
			"TYPE": "image"
	},
    {
      "NAME" : "Sol",
      "TYPE" : "color",
      "DEFAULT" : [
        0.5,
        1.0,
        0.5,
        1.0
      ]
    },
	{
			"NAME": "ImageSol",
			"TYPE": "image"
	},
	{
     	"NAME" :		"xSol",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0,
     	"MIN" : 		-1,
      	"MAX" :			1
	},
	{
     	"NAME" :		"ySol",
     	"TYPE" : 		"float",
    	"DEFAULT" :		1,
     	"MIN" : 		0,
      	"MAX" :			2
	},

	{
     	"NAME" :		"zSol",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0,
     	"MIN" : 		-1,
      	"MAX" :			1
	}
    ]
}
*/
#define STEPS 100
#define PI 3.14159265
#define dsi vec2(0,50.0);
uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

mat2 rot(float a) {
    float s=sin(a), c=cos(a);
    return mat2(c, -s, s, c);
}

vec2 dPlane(vec3 p, float h, float i) {
    return vec2(i, p.y - h);
}
vec2 dPlaneI(vec3 p, vec3 n, float h, float i) {
    return vec2(i, dot(p,n) - h);
}
vec2 dSphere (vec3 p, float r, float i) {
    return vec2(i,length(p) - r);
}
vec2 dTorus( vec3 p, vec2 t, float i )
{
    return vec2(i,length( vec2(length(p.xz)-t.x,p.y) )-t.y);
}
vec2 dBox( vec3 p, vec3 b, float i  )
{
    vec3 d = abs(p) - b;
    return vec2(i,min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0)));
}
vec2 dPyramide( vec3 p, vec3 e, float i  )
{
    float h2 =  1.0; //d.x +0.02;
    float m2 = h2*h2+0.9;
    // symmetry
  //  vec3 p = abs(c) - d;
    p.xz = abs(p.xz)*(e.x+0.4);
    p.xz = (p.z>p.x) ? p.zx : p.xz;
    p.xz -= 0.5*e.y*2.0;
	
    // project into face plane (2D)
    vec3 q = vec3( p.z, h2*p.y - 0.5*p.x, p.x + 0.5*p.y);
    float s = max(-q.x,0.0);
    float t = clamp( (q.y-0.5*p.z)/(m2+0.25), 0.0, 1.0 );
    
    float a = m2*(q.x+s)*(q.x+s) + q.y*q.y;
	float b = m2*(q.x+0.5*t)*(q.x+0.5*t) + (q.y-m2*t)*(q.y-m2*t);
    
    float d2 = min(q.y,-q.x*m2-q.y*0.5) > 0.0 ? 0.0 : min(a,h2);
    
    // recover 3D and scale, and add sign
    return vec2(i, sqrt( (d2+q.z*q.z)/m2 ) * sign(max(q.z,-p.y)));
}
vec2 dHexaPrisme( vec3 p, vec2 h, float i )
{
    vec3 q = abs(p);

    const vec3 k = vec3(-0.8660254, 0.5, 0.57735);
    p = abs(p);
    p.xy -= 2.0*min(dot(k.xy, p.xy), 0.0)*k.xy;
    vec2 d = vec2(
       length(p.xy - vec2(clamp(p.x, -k.z*h.x, k.z*h.x), h.x))*sign(p.y - h.x),
       p.z-h.y );
    return vec2(i,min(max(d.x,d.y),0.0) + length(max(d,0.0)));
}
vec2 dCylindre( vec3 p, float r, float h, float i)
{
    float dX = length(p.xz) - r;
    float dY = abs(p.y) - h;
    float dE = length(vec2(max(dX,0.0), max (dY, 0.0)));
    float dI = min(max(dX, dY),0.0);
    float d = dE + dI;
    return vec2(i,d);
}

vec2 minVec2(vec2 a, vec2 b) {
    return a.y < b.y ? a : b;
}
vec3 position(vec3 p) {
    vec3 pos = p- vec3(x1,y1,z1);
    pos.zy *= rot(xRotation);
    pos.xz *= rot(yRotation);
    pos.yx *= rot(zRotation);
    return pos;
}
vec2 scene (vec3 p) {
//    vec2 dp = dPlane(p, -0.5, 0.0);
vec2 dp = dPlaneI(p, normalize(vec3(xSol,ySol,zSol)), -0.5, 0.0);
    
    vec2 ds0 = dsi;
    vec2 ds1 = dsi;
    vec2 ds2 = dsi;
    vec2 ds3 = dsi;
    vec2 ds4 = dsi;
    vec2 ds5 = dsi;
    vec3 pos = position(p);
       if (Sphere) {
     ds0 = dSphere(pos , dimension1, 1.0) ;
       }
       if (Torus) {
     ds1 = dTorus( pos , vec2(dimension2, dimension1), 1.0);
       }
       if (Box) {
     ds2 = dBox( pos , vec3(dimension2, dimension1,dimension3), 1.0);
       }
       if (HexaPrisme) {
     ds3 = dHexaPrisme( pos , vec2(dimension2, dimension1), 1.0) ;
       }
       if (Cylindre) {
     ds4 = dCylindre( pos , dimension2, dimension1, 1.0) ;
       }
       if (Pyramide) {
     ds5 = dPyramide( pos ,vec3(dimension2, dimension1,dimension3), 1.0) ;
       }
//    ds1.y -= dimension4;
    return minVec2(dp, minVec2(ds0,minVec2(ds1,minVec2(ds2,minVec2(ds3,minVec2(ds4,ds5))))));
}

vec2 march(vec3 rO, vec3 rD) {
    vec3 cP = rO;
    float d = 0.0;
    vec2 s = vec2(0.0);
    for (int i = 0; i < STEPS; i++) {
        cP = rO + rD * d;
        s = scene (cP);
        d += s.y;
        if(s.y < 0.001) {
            break;
        }
         if (d > 20.0) {
             return vec2(100.0,100.0);
        }
    }
    s.y = d;
    return s;
}
vec3 normal(vec3 p) {
    float dP =scene(p).y;
    float eps = 0.01;
    float dX = scene(p + vec3(eps,0.0,0.0)).y - dP;
    float dY = scene(p + vec3(0.0,eps,0.0)).y - dP;
    float dZ = scene(p + vec3(0.0,0.0,eps)).y - dP;
    return normalize(vec3(dX,dY,dZ));
}
float lighting(vec3 p, vec3 n){
    vec3 lP = vec3(cos((rLigth -0.5) * PI),hLigth,sin((rLigth + 0.5) * PI));
    vec3 lD = lP - p;
    vec3 lN = normalize(lD);
    if(march(p + n * 0.01, lN).y < length(lD))
      return 0.0;
    return max(0.0,dot(n,lN));
}
vec4 material(float i){
    if (i < 0.5){
         return vec4(Sol.rgba) * IMG_THIS_PIXEL(ImageSol);
    } else if (i < 1.5){
         return vec4(Objet.rgba) * IMG_THIS_PIXEL(ImageObjet);
    } 
//    return vec4(Fond.rgba) * vec4(IMG_NORM_PIXEL(ImageFond,vec2(sin(isf_FragNormCoord.x),isf_FragNormCoord.y)).rgba);
        return vec4(Fond.rgba) * IMG_THIS_PIXEL(ImageFond);
}
vec4 material2(float i, vec3 p, vec3 n){
    vec4 col = vec4(0.0);
    float dif = (dot(n, normalize(vec3(1.0,2.0,3.0)))*0.5+0.5);
    col += dif*dif;
    n = abs(n);
    n *= pow(n, vec3(20.0));
    n /= n.x+n.y+n.z;
    vec4 colp = vec4(0.0);
    vec4 colXZ = vec4(0.0);
    vec4 colYZ = vec4(0.0);
    vec4 colXY = vec4(0.0);
    if (i < 0.5){
        colXZ = IMG_NORM_PIXEL(ImageSol, 0.4*p.xz*0.08+0.5).rgba;
        colYZ = IMG_NORM_PIXEL(ImageSol, p.yz*1.0+0.5).rgba;
        colXY = IMG_NORM_PIXEL(ImageSol, p.xy*1.0+0.5).rgba;
        colp = Sol;
    } else if (i < 1.5){
        colXZ = IMG_NORM_PIXEL(ImageObjet, p.xz*0.5+0.5).rgba;
        colYZ = IMG_NORM_PIXEL(ImageObjet, p.yz*0.5+0.5).rgba;
        colXY = IMG_NORM_PIXEL(ImageObjet, p.xy*0.5+0.5).rgba;
        colp = Objet;
    } else {
        colXZ = IMG_NORM_PIXEL(ImageFond, p.xz*0.5+0.5).rgba;
        colYZ = IMG_NORM_PIXEL(ImageFond, p.yz*0.5+0.5).rgba;
        colXY = IMG_NORM_PIXEL(ImageFond, p.xy*0.5+0.5).rgba;
        colp = Fond;
    };
    return colp *vec4(vec4(colYZ*n.x+ colXZ*n.y+colXY*n.z));
        
}
void main() 
{
    vec2 uv = (vec2(isf_FragNormCoord.x,isf_FragNormCoord.y) - 0.5);

    vec3 rO = vec3(cos((rCam - 0.5) * PI) * zCam,yCam,sin((rCam + 0.5) * PI) * zCam);
    vec3 target = vec3(0.0,0.0,0.0);

    vec3 fwd = normalize(target - rO);
    vec3 side = normalize(cross(vec3(0.0,1.0,0.0), fwd));
    vec3 up = cross(fwd, side);

    vec3 rD = normalize(fwd + side * uv.x + up * uv.y);
    
    vec2 s = march(rO, rD);
    float d = s.y;
    vec4 col = material(s.x);
    if(d < 100.0) {
        vec3 p = rO + rD * d;    
        vec3 nor = normal(p);
        float l = lighting(p, nor);
        vec4 a = vec4(ambiance.rgba);
        col = material2(s.x, p, nor);
        col = col * (a + l);
        col = pow(col,vec4(0.4545));
    }
    gl_FragColor = vec4(col.rgba);
	
}
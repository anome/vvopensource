/*{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/lsj3z3 by tux.  Illustrates an improved method for triplanar mapping specifically for height maps that reduces stretching by using different planes than xy, zx, and xz.\n\nSee comments for details.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "iMouse",
            "TYPE": "point2D"
        },
        {
            "NAME": "iChannel0",
            "TYPE": "image"
        },
        {
            "DEFAULT": 0,
            "MIN": 0,
            "NAME": "een",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1.5,
            "MAX": 10,
            "MIN": 0,
            "NAME": "zoom",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "MAX": 1,
            "MIN": 0,
            "NAME": "rotate",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "MAX": 10,
            "MIN": 0,
            "NAME": "Rotate2",
            "TYPE": "float"
        },
        {
            "DEFAULT": 7,
            "MAX": 7,
            "MIN": 0,
            "NAME": "Rotate3",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "MAX": 1,
            "MIN": 0,
            "NAME": "NOISE",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}
*/


// Created by tux
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// Much of the noise, fbm, and tracing code taken from the elevated shader toy by the awesome iq.
// The contribution of this toy is to show a different method of triplanar mapping that is more tailored
// to height maps given the observation that the xy and zy planes are likely going to introduce stretching.
// Instead if we pick planes that are informed by the terrain we can get much better results. (in this case hard
// coded to 45 degrees).

// =========  From other shader toys ==========
float hash( float n )
{
    return fract(sin(n)*43758.5453123);
}

vec3 noised( in vec2 x )
{
    vec2 p = floor(x);
    vec2 f = fract(x);

    vec2 u = f*f*(3.0-2.0*f);

    float n = p.x + p.y*57.0;

    float a = hash(n+  0.0);
    float b = hash(n+  1.0);
    float c = hash(n+ 57.0);
    float d = hash(n+ 58.0);
	return vec3(a+(b-a)*u.x+(c-a)*u.y+(a-b-c+d)*u.x*u.y,
				30.0*f*f*(f*(f-2.0)+1.0)*(vec2(b-a,c-a)+(a-b-c+d)*u.yx));

}

float noise( in vec2 x )
{
    vec2 p = floor(x);
    vec2 f = fract(x);

    f = f*f*(3.-2.0*f);

    float n = p.x + p.y*57.0;

    float res = mix(mix( hash(n+  0.0), hash(n+  1.0),f.x),
                    mix( hash(n+ 57.0), hash(n+ 58.0),f.x),f.y);

    return res;
}

 mat2 m2 = mat2(1.6,-1.2,1.2,1.6);
	
float fbm( vec2 p )
{
    float f = 0.0;

    f += 0.5000*noise( p ); p = m2*p*1.*(NOISE-0.4);
    f += 0.2500*noise( p ); p = m2*p*2.03;
    f += 0.1250*noise( p ); p = m2*p*2.01;

	return f/0.875;
}


float terrain( in vec2 p)
{
	return fbm(p * 0.2) * 3.0;	
}

float map( in vec3 p )
{
    return p.y - terrain(p.xz);
}

vec3 calcNormal( in vec3 pos, float t )
{
	float e = 0.001;
	e = 0.0001*t;
    vec3  eps = vec3(e,0.0,0.0);
    vec3 nor;
    nor.x = map(pos+eps.xyy) - map(pos-eps.xyy);
    nor.y = map(pos+eps.yxy) - map(pos-eps.yxy);
    nor.z = map(pos+eps.yyx) - map(pos-eps.yyx);
    return normalize(nor);
}

float intersect( in vec3 ro, in vec3 rd )
{
	float maxd = 40.0;
	float precis = 0.0001;
    float h=precis*2.0;
    float t = 0.0;
	float d = 0.0;
    float m = 1.0;
    for( int i=0; i<256; i++ )
    {
        if( h<precis||t>maxd ) continue;//break;
        t += h * 0.5;
	    h = map( ro+rd*t );
    }
	
	t = h < precis ? t : -1.0;

    return t;
}
// ========= End From other shader toys ==========

// draw a checkerboard based on the uv
float checkerboard(vec2 uv)
{
	uv = fract(uv);
	return ((uv.x < 0.5 && uv.y < 0.5) || (uv.x > 0.5 && uv.y > 0.5)) ? 0.0 : 1.0;
}

void main() {



	float time = TIME*.15;
		 float hit = 0.0;
		
	
	
	// Normal set up code for ray trace.
	vec2 xy = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 s = (-1.0 + 2.0* xy) * vec2(RENDERSIZE.x/RENDERSIZE.y, 1.0);
	
	
	vec3 ro = vec3(een * 10.0, zoom, cos(time * 0.1) * 10.0*rotate);
	vec3 cd = normalize(vec3(1., -Rotate3, Rotate2));
	vec3 cu = vec3(0.0, 1.0, 0.0);
	vec3 cr = normalize(cross(cd, cu));
	cu = normalize(cross(cr, cd));
	vec3 rd = normalize( s.x*cr + s.y*cu + 2.0*cd );
	vec3 col = vec3(0.0, 0.0, 0.0);
    float t = intersect(ro, rd);
   
    if(t > 0.0)
    {	
		// Get some information about our intersection
		vec3 pos = ro + t * rd;
		vec3 normal = calcNormal(pos, t);
		
		// Hang on to the sign of the normal because
		// we will be testing against the pairs of planes at the same time
		vec3 signs = sign(normal);
		
		
		// Calculate some weights for blending.5		// Note: I used the normal weights from tri planar assuming the
		// xy, zy, and zx planes. This could change based on the normal
		// dotted with each plane normal but I found the orthoginal nature
		// of this weighting allowed for some more flexibility.
		vec3 weights = max(abs(normal) - vec3(0.0, 0.4, 0.0), 0.0);
		weights /= max(max(weights.x, weights.y), weights.z);
		float sharpening = 10.0;
		weights = pow(weights, vec3(sharpening, sharpening, sharpening));
		weights /= dot(weights, vec3(1.0, 1.0, 1.0));
		
		// We are constructing a set of 5 planes:
		// 2 are rotated around x, 2 are rotated around y
		// and the last is the xz plane. these are the only
		// planes we care about for heightmap terrain
		// for this example I have chosen to use 45 degrees
		// as the angle to use, you can match your largest
		// slope and it will still be gauranteed better than
		// using normal triplanar mapping.
		float anglep = 3.14159265/4.0;
		
		// an angle of zero reverts back to standard triplanar mapping
		// draw a divider that you can scrub to see the difference
		if (xy.x < iMouse.x/RENDERSIZE.x)
		{
			anglep *= 0.0;
		}
		else hit = 1.0;
		// cache these as we are using a plane reflected around various
		// axes so these numbers are shared.
		float cosp = cos(anglep);
		float sinp = sin(anglep);
	
		// Set up the 3 planar projections that we will be using
		// first plane is rotated around z compensating for the sign of the normal
		vec3 p1t = vec3(0.0, 0.0, 1.0);
		vec3 p1b = vec3(-signs.x * cosp, sinp, 0.0);
		// second plane is just the xz plane
		vec3 p2t = vec3(0.0, 0.0, 1.0);
		vec3 p2b = vec3(1.0, 0.0, 0.0);
		
		/// third plane is rotated around x also compensating for the sign of the normal
		vec3 p3t = vec3(1.0, 0.0, 0.0);
		vec3 p3b = vec3(0.0, sinp, -signs.z * cosp);
		
		// Perform the uv projection on to each plane
		vec2 uvp1 = vec2(dot(pos, p1t), dot(pos, p1b));
		vec2 uvp2 = vec2(dot(pos, p2t), dot(pos, p2b));
		vec2 uvp3 = vec2(dot(pos, p3t), dot(pos, p3b));
		
		// draw some checkerboard debug pattern
		float p1c = checkerboard(uvp1 * 2.0) * weights.x;
		float p2c = checkerboard(uvp2 * 2.0) * weights.y;
		float p3c = checkerboard(uvp3 * 2.0) * weights.z;
		vec3 checkCol = vec3(p1c, p2c, p3c);
		
		// sample the texture, you can mess around with the assigned texture if you want
		vec3 texCol = IMG_NORM_PIXEL(iChannel0,mod(uvp1,1.0),0.0).xyz * weights.x +
			  IMG_NORM_PIXEL(iChannel0,mod(uvp2,1.0),0.0).xyz * weights.y +
			  IMG_NORM_PIXEL(iChannel0,mod(uvp3,1.0),0.0).xyz * weights.z;
		
		// alternate between the texure and the checkerboard
		col = mix(checkCol, texCol, smoothstep(-1.0, 1.0, 1.0)); 
		
		// At this point if you wanted to do normal mapping on top of the tri-planar projection
		// you just use the plane tangents and bi-normals as the frame, if you want you can re-
		// othogonize the matrix before using but it looks ok with out that.
		// vec3 p1bump = p1t * bump.x + p1b * bump.y
		// ...
		// normal = normalize(normal + p1bump * weights.x + p2bump * weights.y ... )
	}
	
	// draw the divider
	if (abs(xy.x - iMouse.x/RENDERSIZE.x) < 0.002)
		col = col * 0.5 + vec3(0.5, 0.5, 0.0);
		
		
	
	gl_FragColor=vec4(col,hit);
}


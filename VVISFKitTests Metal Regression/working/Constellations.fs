/*{
	"CREDIT": "Anomes",
	"DESCRIPTION": "https://www.shadertoy.com/view/MsjyW3",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		{
			"NAME": "DENSITY",
			"LABEL": "density",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2,
				3,
				4
			],
			"LABELS": [
				"8x8",
				"12x12",
				"16x16",
				"20x20",
				"24x24"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "SCALE",
			"LABEL": "scale",
			"TYPE": "point2D",
			"DEFAULT": [
				1.1,
				0.5
			],
			"MIN": [
				-10.0,
				-10.0
			],
			"MAX": [
				10.0,
				10.0
			]
		},
		{
			"NAME": "POSITION",
			"LABEL": "position",
			"TYPE": "point2D",
			"DEFAULT": [
				1.4,
				0.7
			],
			"MIN": [
				-10.0,
				-10.0
			],
			"MAX": [
				10.0,
				10.0
			]
		},
		{
			"NAME": "SPACING",
			"LABEL": "spacing",
			"TYPE": "float",
			"DEFAULT": 2.8,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "DEPTH_OF_FIELD",
			"LABEL": "depth of field",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "POINT_RADIUS",
			"LABEL": "point radius",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "CONNECTION",
			"LABEL": "connection",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.1,
			"MAX": 10.0
		},
		{
			"NAME": "GLOW_RADIUS",
			"LABEL": "glow radius",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "INTENSITY",
			"LABEL": "glow intensity",
			"TYPE": "float",
			"DEFAULT":  0.15,
			"MIN": 0.0,
			"MAX": 10.0
		}
	],
	"PASSES": [
		{
			"TARGET": "pass0Render",
			"FLOAT": true,
			"DESCRIPTION": 0
		},
		{
			"TARGET": "pass1Render",
			"FLOAT": true,
			"DESCRIPTION": 1
		},
		{
			"TARGET": "pass2Render",
			"DESCRIPTION": 2
		}
	]
}*/


/* Original shader at https://www.shadertoy.com/view/MsjyW3 */








#define POINTS_SIZE (4*DENSITY+8)
#define POINTS_NUMBER POINTS_SIZE*POINTS_SIZE

#define BLOCK_SIZE 10
#define BLOCK_NUMBER BLOCK_SIZE*BLOCK_SIZE

#define SELECTION_SIZE 20

#define mouse vec3(0.)
#define POINT_FINAL_RADIUS 0.025*POINT_RADIUS
#define CONNECTION_DISTANCE 0.2*CONNECTION

#define iGlobalTime TIME
#define iResolution RENDERSIZE








vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec2 mod289(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x*34.0)+1.0)*x); }

//
// Description : GLSL 2D simplex noise function
//      Author : Ian McEwan, Ashima Arts
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License : 
//  Copyright (C) 2011 Ashima Arts. All rights reserved.
//  Distributed under the MIT License. See LICENSE file.
//  https://github.com/ashima/webgl-noise
// 
float snoise(vec2 v) {

    // Precompute values for skewed triangular grid
    const vec4 C = vec4(0.211324865405187,
                        // (3.0-sqrt(3.0))/6.0
                        0.366025403784439,  
                        // 0.5*(sqrt(3.0)-1.0)
                        -0.577350269189626,  
                        // -1.0 + 2.0 * C.x
                        0.024390243902439); 
                        // 1.0 / 41.0

    // First corner (x0)
    vec2 i  = floor(v + dot(v, C.yy));
    vec2 x0 = v - i + dot(i, C.xx);

    // Other two corners (x1, x2)
    vec2 i1 = vec2(0.0);
    i1 = (x0.x > x0.y)? vec2(1.0, 0.0):vec2(0.0, 1.0);
    vec2 x1 = x0.xy + C.xx - i1;
    vec2 x2 = x0.xy + C.zz;

    // Do some permutations to avoid
    // truncation effects in permutation
    i = mod289(i);
    vec3 p = permute(
            permute( i.y + vec3(0.0, i1.y, 1.0))
                + i.x + vec3(0.0, i1.x, 1.0 ));

    vec3 m = max(0.5 - vec3(
                        dot(x0,x0), 
                        dot(x1,x1), 
                        dot(x2,x2)
                        ), 0.0);

    m = m*m ;
    m = m*m ;

    // Gradients: 
    //  41 pts uniformly over a line, mapped onto a diamond
    //  The ring size 17*17 = 289 is close to a multiple 
    //      of 41 (41*7 = 287)

    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;

    // Normalise gradients implicitly by scaling m
    // Approximation of: m *= inversesqrt(a0*a0 + h*h);
    m *= 1.79284291400159 - 0.85373472095314 * (a0*a0+h*h);

    // Compute final noise value at P
    vec3 g = vec3(0.0);
    g.x  = a0.x  * x0.x  + h.x  * x0.y;
    g.yz = a0.yz * vec2(x1.x,x2.x) + h.yz * vec2(x1.y,x2.y);
    return 130.0 * dot(m, g);
}









int blockIndexForPosition(vec2 pos)
{
    vec2 p2 = vec2(  (pos.x*iResolution.y/iResolution.x+1.)/2.  ,  (pos.y+1.)/2.  );
    float size = 1./float(BLOCK_SIZE);
    return int(p2.x/size) + int(p2.y/size)*BLOCK_SIZE;
}

vec4 rectForBlockIndex(int index)
{
    float size = 1./float(BLOCK_SIZE);
    int y = index/BLOCK_SIZE;
    int x = index - y*BLOCK_SIZE;
    return vec4( float(x)*size, float(y)*size, size, size );
}

vec4 rectInset(vec4 rect, float inset)
{
    return rect + vec4(-inset,-inset,2.*inset,2.*inset);
}

bool pointInRect(vec4 rect, vec2 point)
{
    return rect.x <= point.x && point.x < rect.x+rect.z &&
    rect.y <= point.y && point.y < rect.y+rect.w;
}

float sdSegment( in vec2 p, in vec2 a, in vec2 b )
{
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h );
}

vec3 colorForIndex(int index)
{
    index = int(  mod(float(index),12.)  );
    if( index == 0 )
    {
         return vec3(1., 0., 0.);
    }
    else if( index == 1 )
    {
         return vec3(0., 1., 0.);
    }
    else if( index == 2 )
    {
         return vec3(0., 0., 1.);
    }
    else if( index == 3 )
    {
         return vec3(1., 1., 0.);
    }
    else if( index == 4 )
    {
         return vec3(0., 1., 1.);
    }
    else if( index == 5 )
    {
         return vec3(1., 0., 1.);
    }
    else if( index == 6 )
    {
         return vec3(1., 1., 1.);
    }
    else if( index == 7 )
    {
         return vec3(1., 0.5, 0.5);
    }
    else if( index == 8 )
    {
         return vec3(0.5, 1., 0.5);
    }
    else if( index == 9 )
    {
         return vec3(1., 0.5, 0.);
    }
    else if( index == 10 )
    {
         return vec3(0., 0.5, 1.);
    }
    else
    {
         return vec3(0.5, 0.5, 1.);
    }
}
















// -------------------------------------------------------
// PASS 0 => PRECALCULATE/ANIMATE POINTS PER MACRO-BLOCK
// -------------------------------------------------------

vec4 pass0PointAtIndex(int index)
{
    float i = mod(float(index), float(POINTS_SIZE));
    float j = floor(  float(index)/float(POINTS_SIZE)  );
    float step = SPACING/float(POINTS_SIZE);
    vec4 point = vec4(SCALE.x*step*i-POSITION.x, SCALE.y*step*j-POSITION.y, 0., 0.);
    float factor = mod(  (j+1.)*(i+1.)  ,  22.  ) + 1.;
    point.x += sin((20.+TIME/2.)*0.03*factor+i*0.5)*0.3;
    point.y += cos((20.+TIME/3.)*0.01*factor)*0.3;
    point.z = DEPTH_OF_FIELD*pow(  cos((20.+TIME)*0.01*factor)  ,16.);
    point.w = float(  blockIndexForPosition(point.xy)  );
    return point;
    if( point.w == 0. )
    {
        point.xyz = vec3(1., 0., 0.);
    }
    else if( point.w == 1. )
    {
        point.xyz = vec3(0., 1., 0.);
    }
    else if( point.w == 2. )
    {
        point.xyz = vec3(0., 0., 1.);
    }
    else
    {
        point.xyz = vec3(1., 1., 0.);
    }
    return point;
}

vec4 pass0()
{
    vec4 color = vec4(-100.);
    if( gl_FragCoord.x <= float(POINTS_NUMBER) && gl_FragCoord.y < float(BLOCK_NUMBER) )
    {
        int index = int(gl_FragCoord.x);
        vec4 point = pass0PointAtIndex(index);
        int blockIndex = int(gl_FragCoord.y);
        vec4 rect = rectForBlockIndex(blockIndex);
        rect = rectInset(rect, 0.1);
        vec2 p2 = vec2(  (point.x*iResolution.y/iResolution.x+1.)/2.  ,  (point.y+1.)/2.  );
        bool inside = pointInRect(rect, p2);
        if(  inside  )
        {
            color = point;
        }
    }
    return color;
}




// -------------------------------------
// PASS 1 => SORT POINTS BY MACRO-BLOCK
// -------------------------------------

vec4 pass1PointAtIndex(int index, int blockIndex)
{
    vec2 pos = vec2(  float(index)+0.5  ,  float(blockIndex)+0.5  )  /  iResolution.xy;
    return IMG_NORM_PIXEL(pass0Render, pos);
}

vec4 pass1( )
{
    vec4 color = vec4(-100.);
    if( gl_FragCoord.x <= float(POINTS_NUMBER) && gl_FragCoord.y < float(BLOCK_NUMBER) )
    {
        int targetIndex = int(gl_FragCoord.x);
        int blockIndex = int(gl_FragCoord.y);
        int k = 0;
        for(int index=0; index<POINTS_NUMBER; index++)
        {
            vec4 point = pass1PointAtIndex(index, blockIndex);
            if( point.w < 0. )
            {
                continue;
            }
            if( k == targetIndex )
            {
                color = point;
                break;
            }
            k++;
        }
    }
    return color;
}




// -----------------------------------------------------
// PASS 2 => RENDER POINTS, CONNECTIONS AND BACKGROUND
// -----------------------------------------------------

vec4 pass2PointAtIndex(int index, int blockIndex)
{
    vec2 pos = vec2(  float(index)+0.5  ,  float(blockIndex)+0.5  )  /  iResolution.xy;
    return IMG_NORM_PIXEL(pass1Render, pos);
}

vec4 pass2()
{
    vec2 p = (2.0*gl_FragCoord.xy-iResolution.xy)/iResolution.y;
    vec2 p2 = vec2(  (p.x*iResolution.y/iResolution.x+1.)/2.  ,  (p.y+1.)/2.  );
    int blockIndex = blockIndexForPosition(p);
    
    // selection
    int k = 0;
    vec2 selectionPoints[SELECTION_SIZE];
    float selectionLengths[SELECTION_SIZE];
    float selectionBlurs[SELECTION_SIZE];
    vec3 selectionTints[SELECTION_SIZE];
    for(int i=0; i<POINTS_NUMBER && k<SELECTION_SIZE; i++)
    {
        vec4 a = pass2PointAtIndex(i, blockIndex);
        if( a.w < 0. )
        {
            break;
        }
        vec2 pa = p - a.xy;
        float d = length(pa);
        if( d < CONNECTION_DISTANCE )
        {
            selectionPoints[k] = a.xy;
            selectionLengths[k] = d;
            selectionBlurs[k] = abs(a.z);
            /*if( a.w < 0.34 )
            {
            	selectionTints[k] = vec3(0.8, 0.9, 1.); // blue
            }
            else if( a.w < 0.67 )
            {
            	selectionTints[k] = vec3(1., 1., 1.); // white
            }
            else
            {
            	selectionTints[k] = vec3(0.9, .9, .9); // gray
            }*/
            k++;
        }
    }
    
    
    // connections
    float h = 2.0/iResolution.y;
    float col = 0.0;
    float glow = 0.0;
    for(int i=0; i<k; i++)
    {
        vec2 a = selectionPoints[i];
        for(int j=0; j<k; j++)
        {
            vec2 b = selectionPoints[j];
            if( a == b )
            {
                continue;
            }
            vec2 ba = b - a;
            float d = length(ba);
            d = smoothstep(CONNECTION_DISTANCE, CONNECTION_DISTANCE-0.1, d);
    		float blur = (selectionBlurs[i]+selectionBlurs[j])/2.;
            float sd = sdSegment(p,a,b);
            col = max(  col  ,  d*(1.0-smoothstep(h/2.,max(blur*2.,1.)*h,sd)) 
                     	/ max(blur*1.5 , 1.)
                     );
            glow = max( glow,  (d*(1.0-smoothstep(0.,3.*GLOW_RADIUS*h,sd)) / max(blur*1.5 , 1.) )*INTENSITY );
        }
    }
    col = min(col+glow, 1.0);
    
    // points
    vec3 tint = vec3(0.8, 0.9, 1.);
    /*if( debug )
    {
    	tint = colorForIndex(blockIndex);
    }*/
    for(int i=0; i<k; i++)
    {
        float d = selectionLengths[i];
        float value = (1.0-smoothstep(0.,POINT_FINAL_RADIUS*max(selectionBlurs[i]/2.,1.)/3.,d))
                 	/ max(selectionBlurs[i]/1.5 , 1.);
        value += (  1.0-smoothstep(0.,GLOW_RADIUS*POINT_FINAL_RADIUS*max(selectionBlurs[i]/2.,1.)/3.,d)  )*INTENSITY;
        col = max(  col  ,  value  );
        //tint = mix(tint, selectionTints[i], col);
    }
 
    // background
    vec2 vel = vec2(iGlobalTime*.1);
    float background = snoise(2.*gl_FragCoord.xy/iResolution.y+vel)*.25+.25;
    float a = snoise(2.*gl_FragCoord.xy/iResolution.y*vec2(cos(iGlobalTime*.08),sin(iGlobalTime*0.1))*0.1)*3.1415;
    vel = vec2(cos(a),sin(a));
    background += snoise(2.*gl_FragCoord.xy/iResolution.y+vel)*.25+.25;
    background *= 0.33*gl_FragCoord.y/iResolution.y;
    col += background;
    /*if( debug )
    {
    	col += 0.25;
    }*/
    return vec4( col*tint.r, col*tint.g, col*tint.b, 1.0 );
}








// ----------------
// COMPOSITE PASSES
// ----------------

void main()
{
    if( PASSINDEX == 0 )
    {
        gl_FragColor = pass0();
    }
    else if( PASSINDEX == 1 )
    {
        gl_FragColor = pass1();
    }
    else if( PASSINDEX == 2 )
    {
        gl_FragColor = pass2();
    }
}





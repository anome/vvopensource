/*
	{
	"DESCRIPTION": "Spinning Disks",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "ISF Import by: Old Salt",
	"VSN": "1.0",
	"INPUTS":
		[
			{
			"NAME": "uC1",
			"TYPE": "color",
			"DEFAULT":[0.0,1.0,0.0,1.0]
			},
			{
			"NAME": "uC2",
			"TYPE": "color",
			"DEFAULT":[0.0,0.0,1.0,1.0]
			},
			{
			"NAME": "uC3",
			"TYPE": "color",
			"DEFAULT":[1.0,0.0,0.0,1.0]
			},
			{
			"NAME": "NoXforms",
			"TYPE": "color",
			"DEFAULT":[0.0,0.0,0.0,0.0]
			},
			{
			"LABEL": "Offset: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Zoom: ",
			"NAME": "uZoom",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": -1.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Cluster Spin Rate: ",
			"NAME": "uClustSpd",
			"TYPE": "float",
			"MAX": 5.0,
			"MIN": -5.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Number of Disks: ",
			"NAME": "uDiskNum",
			"TYPE": "float",
			"MAX": 100.0,
			"MIN": 1.0,
			"DEFAULT": 50
			},
			{
			"LABEL": "Disk Spin Rate: ",
			"NAME": "uDiskSpd",
			"TYPE": "float",
			"MAX": 5.0,
			"MIN": -5.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Disk Color Shift: ",
			"NAME": "uColShift",
			"TYPE": "float",
			"MAX": 10.0,
			"MIN": 0.0,
			"DEFAULT": 2.0
			},
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Defaults ",
				"Alternate Color Palette (3 used) "
				],
			"NAME": "uColMode",
			"TYPE": "long",
			"VALUES": [0,1],
			"DEFAULT": 0
			},
			{
			"LABEL": "Intensity: ",
			"NAME": "uIntensity",
			"TYPE": "float",
			"MAX": 4.0,
			"MIN": 0,
			"DEFAULT": 1.0
			}
		]
	}
*/
// Import from: https://www.shadertoy.com/view/lsfGDB

/* 
 This is a derived work and subject to the MIT License:
 Copyright © 2013 Inigo Quilez
 Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 and associated documentation files (the "Software"), to deal in the Software without restriction,
 including without limitation the rights to use, copy, modify, merge, publish, distribute, 
 sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
 furnished to do so, subject to the following conditions: The above copyright notice and this 
 permission notice shall be included in all copies or substantial portions of the Software. 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
 NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

#define SC 3.0

vec3 intersectCoordSys( in vec3 o, in vec3 d, vec3 c, vec3 u, vec3 v )
	{
	vec3  q = o - c;
	vec3  n = cross(u,v);
	float t = -dot(n,q)/dot(d,n);
	float r =  dot(u,q + d*t);
	float s =  dot(v,q + d*t);
	return vec3(t,s,r);
	}

vec3 hash3(float n)
	{
	return fract(sin(vec3(n,n+1.0,n+2.0))*vec3(43758.5453123,12578.1459123,19642.3490423));
	}

vec3 shade(vec4 res)
	{
	float ra = length(res.yz);
	float an = atan(res.y,res.z) + uDiskSpd*TIME;
	float pa = sin(3.0*an);
	vec3 cola = 0.5 + 0.5*sin( (res.w/64.0)*3.5 + vec3(0.0,1.0,2.0) );
	vec3 col = vec3(0.0);
	col += cola*0.4*(1.0-smoothstep( 0.90, 1.00, ra) );
	col += cola*1.0*(1.0-smoothstep( 0.00, 0.03, abs(ra-0.8)))*(0.5+0.5*pa);
	col += cola*1.0*(1.0-smoothstep( 0.00, 0.20, abs(ra-0.8)))*(0.5+0.5*pa);
	col += cola*0.5*(1.0-smoothstep( 0.05, 0.10, abs(ra-0.5)))*(0.5+0.5*pa);
	col += cola*0.7*(1.0-smoothstep( 0.00, 0.30, abs(ra-0.5)))*(0.5+0.5*pa);
	return col*0.3;
	}

vec3 render(vec3 ro, vec3 rd)
	{
	// raytrace
	vec3 col = vec3(0.0);
	for(float i=0.0; i<100.; i++)  // # of disks and color
		{
		if(i>=uDiskNum) break;
		// position disk
		vec3 r = 2.5*(-1.0 + 2.0*hash3(float(i)));
		r *= SC;		
		// orientate disk
		vec3 u = normalize(r.zxy);
		vec3 v = normalize(cross(u, vec3(0.0,1.0,0.0)));						   
		// intersect coord sys
		vec3 tmp = intersectCoordSys(ro, rd, r, u, v);
		tmp /= SC;		
		if(dot(tmp.yz,tmp.yz)<1.0 && tmp.x>0.0) 
			{
			// shade			
			col += shade(vec4(tmp,float(i)*uColShift));
			}
		}
	return col;
	}

void main()
	{
#ifdef XL_SHADER // These three lines must be removed for versions of xLights above
  discard;       // 2021.12.  There is no guarantee they will function correctly after
#endif           // that, as xLights started modifying the shader coordinate system.

	float zoom = (uZoom < 0.0) ? (1.0-abs(uZoom))*0.25 : (1.0+uZoom*9.0)*0.25;
	vec2 p =((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE.y- uOffset)/zoom;  // normalize coordinates (origin at center)
	// camera
	vec3 ro = 2.0*vec3(cos(uClustSpd*TIME),0.0,sin(uClustSpd*TIME));
	// camera matrix
	vec3 ww = normalize(-ro);
	vec3 uu = normalize(cross(ww,vec3(0.0,1.0,0.0)));
	vec3 vv = normalize(cross(uu,ww));
	// create view ray
	vec3 rd = normalize(p.x*uu + p.y*vv + 1.0*ww);
	vec3 col = render( ro*SC, rd);
	vec4 cShad = vec4(col, 1.0);  
	vec3 cOut = cShad.rgb;
	if (uColMode == 1)
		{
		cOut = uC1.rgb * cShad.r;
		cOut += uC2.rgb * cShad.g;
		cOut += uC3.rgb * cShad.b;
		}
	cOut = cOut * uIntensity;
	gl_FragColor = vec4(cOut.rgb,1.0);	
	}

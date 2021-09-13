/*
{
  "IMPORTED" : [
    {
      "NAME" : "iChannel0",
      "PATH" : "52d2a8f514c4fd2d9866587f4d7b2a5bfa1a11a0e772077d7682deb8b3b517e5.jpg"
    },
    {
      "NAME" : "iChannel1",
      "PATH" : "ad56fba948dfba9ae698198c109e71f118a54d209c0ea50d77ea546abad89c57.png"
    }
  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/XdtBWH by Gaktan.  Recently fell in love with kaleidoscopes. Decided to make a very basic one with nice colors",
  "INPUTS" : [
    {
      "NAME" : "iMouse",
      "TYPE" : "point2D"
    }
  ]
}
*/


//#define USE_TEXTURE

#define COLOR_TRANSITION_SIZE 0.02
//#define COLOR_TRANSITION_DEBUG

// Whether wrapping transition is linear or squared
//#define LINEAR

// Gives good results if LINEAR is ON
//#define FIX_X

#define KALEIDOSCOPE_SPEED_X    9.0
#define KALEIDOSCOPE_SPEED_Y  -20.0
#define KALEIDOSCOPE_SPLITS     6.0

//#define MOUSE

#define GAMMA_CORRECT


#define PI 3.14159265359

vec2 kaleidoscope(vec2 uv, vec2 offset, float splits)
{
    // XY coord to angle
    float angle = atan(uv.y, uv.x);
    // Normalize angle (0 - 1)
    angle = ((angle / PI) + 1.0) * 0.5;
    // Rotate by 90°
    angle = angle + 0.25;
    // Split angle 
    angle = mod(angle, 1.0 / splits) * splits;
    
    // Warp angle
#ifndef LINEAR
    float a = (2.0*angle - 1.0);
    angle = -a*a + 1.0;
    
    //angle = -pow(a, 0.4) + 1.0;
#else
    angle = -abs(2.0*angle - 1.0) + 1.0;
#endif
    
    angle = angle*0.1;
    
    // y is just dist from center
    float y = length(uv);
    //y = (y*30.0);
    
#ifdef FIX_X
    angle = angle * (y*3.0);
#endif
    
    return vec2(angle, y) + offset;
}

vec3 heatmapGradient(float t)
{
	return clamp((pow(t, 1.5) * 0.8 + 0.2) * vec3(smoothstep(0.0, 0.35, t) + t*0.5, smoothstep(0.5, 1.0, t), max(1.0 - t*1.7, t*7.0 - 6.0)), 0.0, 1.0);
}

vec3 customGradient(float t)
{
    t = mod(t*-0.9, 42.0);
	return 0.5 + 0.5*cos( 3.0 + t*0.075*t + vec3(0.0,0.6,1.0));
}

void main() {



    // Mobile friendly UVs
    vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    float time = TIME;
    
    // Start with a good color
    time += 1207.0;
#ifdef MOUSE
	time += 0.25 * iMouse.x;
#endif
    
    vec2 A = vec2(time * KALEIDOSCOPE_SPEED_X * 0.005, 
                  time * KALEIDOSCOPE_SPEED_Y * 0.005);
    
	uv = kaleidoscope(uv, A, KALEIDOSCOPE_SPLITS);
    
#ifdef USE_TEXTURE
    uv = uv * 0.7;
    vec4 tex = IMG_NORM_PIXEL(iChannel0,mod(uv,1.0));
    gl_FragColor = vec4(tex.rgb, 1.0);
#else
    float tex = IMG_NORM_PIXEL(iChannel1,mod(uv,1.0)).r;
    // frequency and shape of the transition
    float d = (cos(uv.y+1.2*cos(uv.x)*tex) * 0.5) + 0.5;
    d = smoothstep(0.5 - COLOR_TRANSITION_SIZE, 0.5 + COLOR_TRANSITION_SIZE, d);
#ifdef COLOR_TRANSITION_DEBUG
    gl_FragColor = vec4(d,d,d, 1.0);
    return;
#endif
    
    vec3 a = customGradient(tex);
    vec3 b = heatmapGradient(tex);
#ifdef GAMMA_CORRECT
    a = pow(a, vec3(2.2));
    b = pow(b, vec3(2.2));
#endif
    
    vec3 color = mix(a, b, d);
    
#ifdef GAMMA_CORRECT
    color = pow(color, vec3(1.0 / 2.2));
#endif
    
	gl_FragColor = vec4(color.rgb, 1.0);
#endif
}

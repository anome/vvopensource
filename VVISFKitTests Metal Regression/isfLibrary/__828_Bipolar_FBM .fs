/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/4dfBWX by duvengar.  The idea was to mix two fbm patterns together and see what happens. Use of mouse for variations.\n(Uncomment line 67 and comment line 65  for symmetry) ",
  "INPUTS" : [
    {
      "NAME" : "iMouse",
      "TYPE" : "point2D"
    },
    {
      "NAME" : "speed",
      "TYPE" : "float",
      "DEFAULT" : 1
    },
    {
      "NAME" : "rb",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.20000000000000001,
      "MIN" : 0
    },
    {
      "NAME" : "rorange",
      "TYPE" : "float",
      "DEFAULT" : 1
    },
    {
      "NAME" : "rg",
      "TYPE" : "float",
      "DEFAULT" : 0.6
    }
  ],
  "ISFVSN" : "2"
}
*/


//// COLORS ////
 vec3 ORANGE = vec3(rorange, 0.6, 0.2);
 
vec3 BLUE   = vec3(rb, rg, 0.6);
const vec3 BLACK  = vec3(0.1, 0.0, 0.1);

///// MATHS /////
const float PI = 3.14;

vec2 maplinear ( in vec2 x, in vec2 a1, in vec2 a2, in vec2 b1, in vec2 b2 ) {
    return b1 + (x - a1) * (b2 - b1) / (a2 - a1);
}

///// NOISE /////
float hash( float n ) {
    return fract(sin(n)*43758.5453123);   
}

float noise( in vec2 x ){
    vec2 p = floor(x);
    vec2 f = fract(x);
    f = f * f * (3.0 - 2.0 * f);
    float n = p.x + p.y * 57.0;
    return mix(mix( hash(n + 0.0), hash(n + 1.0), f.x), mix(hash(n + 57.0), hash(n + 58.0), f.x), f.y);
}

////// FBM ////// 
mat2 m = mat2( 0.6, 0.6, -0.6, 0.8);
float fbm(vec2 p){
 
    float f = 0.0;
    f += 0.5000 * noise(p); p *= m * 2.02;
    f += 0.2500 * noise(p); p *= m * 2.03;
    f += 0.1250 * noise(p); p *= m * 2.01;
    f += 0.0625 * noise(p); 
    f /= 0.9375;
    return f;
}

void main() {

    
    // pixel ratio  
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;  
    vec2 p = - 1. + 2. * uv;  
    p.x *= RENDERSIZE.x / RENDERSIZE.y ;
  	
    // 2 Fbm origins
    vec2  ctr  = vec2( 1.0, 0.0);
    vec2  ctr2 = vec2(-1.0, 0.0);  
    
    // MOUSE
    vec2 m = maplinear(iMouse.xy,
                       vec2(0.0),
                       vec2(RENDERSIZE.x, RENDERSIZE.y),
                       vec2(1.5, -PI),
                       vec2(7.0, 0.));
        
    // domains
    float rad  = sqrt(dot(p, p)); 
    float r    = sqrt(dot(p + ctr, p + ctr)) + m.y; 
    float r2   = sqrt(dot(p + ctr2, p + ctr2))+ m.y;  
    float a = r * r2;
                    
    // distortions      
    a *= fbm(m.x * p);
    // with symmetry
    //a *= fbm(m.x * sqrt(p*p));
    a -= DATE.w * 0.3 ;
    // colorize
    vec3 col = BLACK;
    float f = smoothstep(0.1, 0.9, fbm(vec2(a * 20.0, r * r2)));  
    col =  mix( col, BLUE, f); 
    f = smoothstep(0.4, 0.9, fbm(vec2(a * 2.0 , r * r2)));  
    col =  mix( col, ORANGE, f );
    f = smoothstep(0.3, 0.9, fbm(vec2(a * 50.0, r * r2)));  
    col *=  1.8 - f ; 
    gl_FragColor = vec4(col, 1.);
}

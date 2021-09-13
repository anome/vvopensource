/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/MlfcDN by frontside.  testing a plasma particle effect",
  "INPUTS" : [
    {
      "NAME" : "een",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 50,
      "MIN" : 0,
      "LABEL" : "state"
    },
    {
      "NAME" : "twee",
      "TYPE" : "float",
      "MAX" : 0.20000000000000001,
      "DEFAULT" : 0.10000000000000001,
      "MIN" : 0,
      "LABEL" : "Animation"
    },
    {
      "NAME" : "drie",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1,
      "MIN" : 1,
      "LABEL" : "brightness"
    },
    {
      "NAME" : "vier",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 1,
      "MIN" : 0,
      "LABEL" : "brightness 2"
    },
    {
      "NAME" : "vijf",
      "TYPE" : "float",
      "MAX" : 3,
      "DEFAULT" : 1,
      "MIN" : 0,
      "LABEL" : "Gloom"
    }
  ],
  "ISFVSN" : "2"
}
*/


float noise( vec2 co ){
    return fract( sin( dot( co.xy, vec2( 12.9898, 78.233 ) ) ) * 43758.5453 );
}

void main() {



	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    
    float u_brightness = 1.2*drie;
    float u_blobiness = 0.9;
    float u_particles = 140.0;
    float u_limit = 70.0;
    float u_energy = 1.0 * 0.75 *twee;
    vec2 position = ( gl_FragCoord.xy / RENDERSIZE.x );
    float t = een * u_energy;
    
    float a = 0.0;
    float b = 0.0;
    float c = 0.0;
//    vec2 pos, center = vec2( 0.0, 0.0 * (RENDERSIZE.y / RENDERSIZE.x) );
//    center=vec2(0.15* (RENDERSIZE.y / RENDERSIZE.x),0.15* (RENDERSIZE.y / RENDERSIZE.x));
    vec2 pos;
//    vec2 center = RENDERSIZE.xy * 0.0;
    vec2 center = vec2( 0.5, 0.5 * (RENDERSIZE.y / RENDERSIZE.x) );
    float na, nb, nc, nd, d;
    float limit = u_particles / u_limit;
    float step = 1.0 / u_particles;
    float n = 0.0;
    
    for ( float i = 0.0; i <= 1.0; i += 0.025 ) {
        if ( i <= limit ) {
            vec2 np = vec2(n, 1-1);
            
            na = noise( np * 1.1 );
            nb = noise( np * 2.8 );
            nc = noise( np * 0.7 );
            nd = noise( np * 3.2 );
            pos = center;
            pos.x += sin(t*na) * cos(t*nb) * tan(t*na*0.15) * 0.3;
            pos.y += tan(t*nc) * sin(t*nd) * 0.1;
            
            d = pow( 1.6*na / length( pos - position ), u_blobiness );
            
            if ( i < limit * 0.3333 ) a += d;
            else if ( i < limit * 0.5 *vijf ) b += d;
            else c += d;
            n += step;
        }
    }
//    vec3 col = vec3(a*c,b*c,a*b) * 0.0001 * u_brightness;
    //blue only
    vec3 col = vec3(a*vier*25.5,0.0,a*b) * 0.0001 * u_brightness;
    
    gl_FragColor = vec4( col, 1.0 );
    
	//gl_FragColor = vec4(uv,0.5+0.5*sin(TIME),1.0);
}

/*
{
    "CATEGORIES": [
        "flames",
        "blur",
        "edges",
        "motion"
    ],
    "DESCRIPTION": "edges as motion blur with feedback. Buf B does the edges and feeds them back to itself based on Buf A. Buf A does video with Buf B as green screen filler.",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
	      "NAME": "rotation",
	      "TYPE": "float",
	      "MIN": -10,
	      "MAX": 10,
	      "DEFAULT": 0.0
	    },
	    {
	      "NAME": "edgeWidth",
	      "TYPE": "float",
	      "MIN": 2,
	      "MAX": 20,
	      "DEFAULT": 6.0
	    },
	    {
	      "NAME": "amplitude",
	      "TYPE": "float",
	      "MIN": 2,
	      "MAX": 10,
	      "DEFAULT": 5
	    }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "PERSISTENT": true,
            "TARGET": "BufferA",
            "DESCRIPTION":"luma filter",
            "WIDTH": "$WIDTH/1.0",
	        "HEIGHT": "$HEIGHT/1.0"
        },
        {
            "PERSISTENT": true,
            "TARGET": "BufferB",
            "DESCRIPTION":"edges",
            "WIDTH": "$WIDTH/1.0",
	  	    "HEIGHT": "$HEIGHT/1.0"
        },
        {
        }
    ]
}
*/

#define R RENDERSIZE
#define t TIME

#define GAMMA 2.0
// sRGB -> linear/ linear -> sRGB
#define degamma( rgba ) ( pow(max(rgba, 0.), vec4(GAMMA)) )
#define gamma( rgba ) ( pow(max(rgba, 0.), vec4(1./GAMMA)) )
#define HOLYGREY vec4(0.2126, 0.7152, 0.0722, 0.)
#define luma( rgba ) ( dot(rgba, HOLYGREY) )

#define COLOR_1 vec4(0.91, 0.847, 0.0, 0.0)

mat2 rotate( float deg) {
	float theta = radians(deg);
	float s = sin(theta);
	float c = cos(theta);
	return mat2(c, -s, s, c);
}

void main() {
  // normalize uv
  vec2 uv = gl_FragCoord.xy / R.xy;
  // 
  vec2 px = edgeWidth / R.xy;
  
  vec4 col = vec4(0); 
  /** first pass */
  
  if (PASSINDEX == 0) {
    // green filter
    vec4 tex = IMG_NORM_PIXEL(inputImage, uv);
    tex = degamma(tex);
    
    float newG = min(tex.g, max(tex.r, tex.b));
    float d = abs(tex.g - newG);
    tex.g = newG;
    
    if (d > 0.0) {

      px *= sin( t + uv.yx * 5.0 ) * .35;

      uv -= 0.5 * px;

      vec4 tex2 = IMG_NORM_PIXEL(BufferB, uv);
      
      float lumaTex = luma(tex2);
      
      uv += px;

      tex2 += IMG_NORM_PIXEL(BufferB, uv);
      
      // update uv coordinates
      uv.x -= px.x - amplitude * 0.5 * sin(t * amplitude + lumaTex );
      uv.y += px.y + amplitude * 0.5 * cos(t * amplitude + lumaTex );

      tex2 += IMG_NORM_PIXEL(BufferB, uv);
      uv.y -= px.y;

      tex2 += IMG_NORM_PIXEL(BufferB, uv);
      tex2 /= edgeWidth ;

      vec4 cTex2 = mix(tex, tex2, smoothstep(0.25, 1.0, d));
      
      vec4 cTex = clamp(tex * (1.0 - d), 0.0, 1.0);
      
      // compute new color with compare
      tex = max( cTex, cTex2 );
    }
    
    col = tex;
    
  gl_FragColor = col;
  } else if (PASSINDEX == 1) {
      
  	// buffer
    vec4 tex = IMG_NORM_PIXEL(BufferA, uv);
    
    float dist = distance(tex, IMG_NORM_PIXEL(BufferA, uv + px) );

    px.y *= -1.0;

    dist += distance(tex, IMG_NORM_PIXEL(BufferA, uv + px) );

    px.x *= -1.0;

    dist += distance(tex, IMG_NORM_PIXEL(BufferA, uv + px));
    
    px.y *= -1.0;

    dist += distance(tex, IMG_NORM_PIXEL(BufferA, uv + px));
    
    uv *= rotate( rotation ); //mat2(0.99, 0.01, -0.01, 0.99);
    
   	col = IMG_NORM_PIXEL(BufferB, uv +0.01 ) * COLOR_1;
   	
	col += vec4( smoothstep(0.1, 1.0, dist), smoothstep(0.1, 1.4, dist), 0.0, 1.0) * .25;
	
    gl_FragColor = col;
  } else if (PASSINDEX == 2) {
  	// assign final color comparing last 2 buffers
    col = max( IMG_NORM_PIXEL(BufferA, uv), IMG_NORM_PIXEL(BufferB, uv) ) ;
    
  gl_FragColor = gamma(col);
  }
  
}
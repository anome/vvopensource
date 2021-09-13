/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    	"illusion",
    	"OpArt",
    	"equirectangular"
  ],
  "DESCRIPTION": "Spherical / Equirectangular version of TileIllusion",
  "INPUTS": [
     {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": -3.0,
      "MAX": 3.0
    },
     {
      "NAME": "divisions",
      "TYPE": "float",
      "DEFAULT": 18,
      "MIN": 5,
      "MAX": 35
    },
    {
      "NAME": "flip",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    },
    {
      "NAME": "flop",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    }
  ]
}*/


///////////////////////////////////////////////////////////
// EquiRecTileIllusion  by mojovideotech
//
// Spherical / Equirectangular version of 
// interactiveshaderformat.com/sketches/898
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	pi   	3.141592653589793  // pi

float f(float x) { return x + 0.2*sin(1.6*x); }

float solve(float x0,float x1,float y) {
    float y0=f(x0), y1=f(x1);
    if (y1<y0) { float x2=x1;x1=x0;x0=x2; float y2=y1;y1=y0;y0=y2; }
    float xn, yn;
    for (int i=0; i<20; i++) {
	    xn = x0 + (x1-x0)/(y1-y0)*(y-y0);
    	yn = f(xn);
        if (yn>y) { x1=xn; y1=yn; } 
        else { x0=xn; y0=yn; }
    }
    return xn;
}

void main() {
	vec3 uv;
	vec2 st = gl_FragCoord.xy / RENDERSIZE.xy;
	float t = TIME * rate;
	float th =  st.y * pi, ph = st.x * 2.0 * pi;
	vec3 pq = vec3(sin(th) * cos(ph), sin(th) * sin(ph), cos(th));
  	if (flip) {
  		uv = pq.zyx;
  		if (flop) uv = pq.zxy;
  	} 
  	else if (flop) uv = pq.yzx;
  	else uv = pq.xyz;
    uv *= floor(divisions);
    float y0 = solve(uv.y - 3.0, uv.y, floor(f(uv.y))),
          y1 = solve(uv.y, uv.y + 3.0, floor(f(uv.y)) + 1.0);
    uv.y = f(uv.y);
    uv.x /= (y1-y0); 
    float s = mod(floor(uv.y), 2.0), v = 0.55;
    if (fract(uv.y)>0.75*(divisions*0.01)) {
    	uv.x += (t*sign(s-0.5));
    	float c = mod(floor(uv.x)+floor(uv.y), 2.0);
    	v = c;
    }
	gl_FragColor = vec4(v);
}
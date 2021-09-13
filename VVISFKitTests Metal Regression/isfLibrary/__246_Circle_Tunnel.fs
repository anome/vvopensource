/*
{
  
  "CATEGORIES" : [
    "Generator",
    "INKA",
    "XXX"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/Xlt3R8 by Flyguy.   A 3d point based tunnel effect based off the scene from Second Reality.",
  "INPUTS" : [
    {
      "NAME" : "CIRCLE_SIZE",
      "TYPE" : "float",
      "MAX" : 200,
      "MIN" : 1,
      "DEFAULT": 200
    },
    {
      "NAME" : "SPEED",
      "TYPE" : "float",
      "MAX" : 1,
      "MIN" : 0,
      "DEFAULT": 0.2
    },
    {
      "NAME" : "MOVE_X",
      "TYPE" : "float",
      "MAX" : 1,
      "MIN" : -1
    },
    {
      "NAME" : "MOVE_Y",
      "TYPE" : "float",
      "MAX" : 1,
      "MIN" : -1
    },
	{
		"NAME": "POINT_COLOR_A",
		"TYPE": "color",
		"DEFAULT": [
			0.6,
			0.2,
			0.2,
			1.0
		]
	},
	{
		"NAME": "POINT_COLOR_B",
		"TYPE": "color",
		"DEFAULT": [
			0.8,
			0.4,
			0.4,
			1.0
		]
	}
  ],
  "ISFVSN" : "2"
}
*/


//Constants
#define TAU 6.2831853071795865

//Parameters
#define TUNNEL_LAYERS 30

//Square of x
float sq(float x)
{
	return x*x;   
}

//Tunnel/Camera path
vec2 TunnelPath(float x)
{
    vec2 offs = vec2(0, 0);
    
    offs.x = 0.3 * TAU * x * MOVE_X;//sin(TAU * x * 0.5) + 0.4 * sin(TAU * x * 0.2 + 0.3);
    offs.y = 0.15 * TAU * x *  MOVE_Y; //cos(TAU * x * 0.3) + 0.2 * cos(TAU * x * 0.1);
    
    return offs;
}

void main() {
    vec2 res = RENDERSIZE.xy / RENDERSIZE.y;
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.y;
    
    uv -= res/2.0;
    
    vec4 color = vec4(0);
    
    float pointSize = CIRCLE_SIZE/2.0/RENDERSIZE.y;
    
    float camZ = TIME * SPEED;
    vec2 camOffs = TunnelPath(camZ);
    
    for(int i = 1;i <= TUNNEL_LAYERS;i++)
    {
        float pz = 1.0 - (float(i) / float(TUNNEL_LAYERS));
        
        //Scroll the points towards the screen
        pz -= mod(camZ, 4.0 / float(TUNNEL_LAYERS));
        
        //Layer x/y offset
        vec2 offs = TunnelPath(camZ + pz) - camOffs;
        
        //Radius of the current ring
        float ringRad = 0.15 * (1.0 / sq(pz * 0.8 + 0.4));
        
        //Only draw points when uv is close to the ring.
        if(abs(length(uv + offs) - ringRad) < pointSize) 
        {
            //Stripes
            vec4 ptColor = (mod(float(i), 2.0) == 0.0) ? POINT_COLOR_A : POINT_COLOR_B;
            
            //Distance fade
            float shade = (1.0-pz);
            color = ptColor * shade;
        }
    }
    
	gl_FragColor = color;
}

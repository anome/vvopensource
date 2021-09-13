/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "raymarching",
    "primitives",
    "cross",
    "Automatically Converted"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/llG3WD by harrisscott. Simple ray marching example with help from reddit, adding boxes to create a primitive cross shape.\ncredit to this post for the fundamental raymarching principals: https://www.reddit.com/r/twotriangles/comments/1hy5qy/tutorial_1_writing_a_simple_distance_ Remixed by EboStudio",
  "INPUTS" : [
    {
      "NAME" : "EPSILON",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.001,
      "MIN" : 0.0001
    },
    {
      "NAME" : "side_1_h",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 2,
      "MIN" : 0.5
    },
    {
      "NAME" : "side_1_w",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.52,
      "MIN" : 0.10000000000000001
    },
    {
      "NAME" : "side_2_h",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1.2,
      "MIN" : 0.5
    },
    {
      "NAME" : "side_3_h",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1.9,
      "MIN" : 0.5
    },
    {
      "NAME" : "Slider1",
      "TYPE" : "float",
      "MAX" : 0.20000000000000001,
      "DEFAULT" : 0.12,
      "LABEL" : "Slider1",
      "MIN" : 0
    },
    {
      "NAME" : "Slider2",
      "TYPE" : "float",
      "MAX" : 12,
      "DEFAULT" : 5.8,
      "LABEL" : "Slider2",
      "MIN" : 1
    },
    {
      "NAME" : "Slider3",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 2.9,
      "LABEL" : "Slider3",
      "MIN" : 0
    },
    {
      "NAME" : "Slider4",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 2.69,
      "LABEL" : "Slider4",
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2"
}
*/


/* ***************************************************** */
float sphere(vec3 pos, float radius)
{
    return length(pos) - radius;
}

/* ***************************************************** */
float Box(vec3 pos, vec3 b)
{
    return length(max(abs(pos)-b,0.0));
}

/* ***************************************************** */
float Torus( vec3 p, vec2 t )
{
  vec2 q = vec2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}

/* ***************************************************** */
float AddTheseTwoShapes(float firstShape, float secondShape)
{
   return min(firstShape, secondShape);
}

/* ***************************************************** */
float BoxCross(vec3 pos)
{
    return AddTheseTwoShapes(AddTheseTwoShapes(Torus(pos, vec2(side_1_h,side_1_w)),Torus(pos, vec2(side_2_h,side_1_w))),Torus(pos, vec2(side_3_h,side_1_w))) * Slider1;
}

/* return AddTheseTwoShapes(AddTheseTwoShapes(Box(pos, vec3(side_1_h,side_1_w,0.5)),Box(pos, vec3(0.5,0.5,side_3_h))),Box(pos, vec3(0.5,side_2_h,0.5))) * Slider1; */

/* ***************************************************** */
float displacement(vec3 p)
{
   return sin(p.x)*sin(p.y)*sin(p.z);
}

/* ***************************************************** */
float opDisplace( vec3 p )
{
    float d1 = Torus(p, vec2(1.0,1.0));
    float d2 = displacement(p);
    return d1+d2;
}

float opRep(vec3 p, vec3 c)
{
    vec3 q = mod(p,c)-0.5*c;
    return opDisplace(q);
}
               

/* ***************************************************** */
float distfunc(vec3 pos)
{
    return BoxCross(pos);
   
}

/* ***************************************************** */
void main()
{
    vec3 cameraOrigin = vec3(Slider4, 1.7*cos(TIME), 1.8*sin(TIME));
    vec3 cameraTarget = vec3(0.0, 0.0, 0.0);
    vec3 upDirection = vec3(Slider4, 3.0, 0.0);
    
    vec3 cameraDir = normalize(cameraTarget - cameraOrigin);
    
    vec3 cameraRight = normalize(cross(upDirection, cameraOrigin));
    vec3 cameraUp = cross(cameraDir, cameraRight);
    
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    
    
    vec2 screenPos = -1.0 + 2.0 * gl_FragCoord.xy / RENDERSIZE.xy; // screenPos can range from -1 to 1
screenPos.x *= RENDERSIZE.x / RENDERSIZE.y; // Correct aspect ratio
    
    vec3 rayDir = normalize(cameraRight * screenPos.x + cameraUp * screenPos.y + cameraDir);
    
    const int MAX_ITER = 100; // 100 is a safe number to use, it won't produce too many artifacts and still be quite fast
const float MAX_DIST = 20.0; // Make sure you change this if you have objects farther than 20 units away from the camera
//const float EPSILON = 0.001; // At this distance we are close enough to the object that we have essentially hit it

    
    
    float totalDist = 0.0;
vec3 pos = cameraOrigin;
float dist = EPSILON;

for (int i = 0; i < MAX_ITER; i++)
{
    // Either we've hit the object or hit nothing at all, either way we should break out of the loop
    if (dist < EPSILON || totalDist > MAX_DIST)
        break; // If you use windows and the shader isn't working properly, change this to continue;

    dist = distfunc(pos); // Evalulate the distance at the current point
    totalDist += dist * Slider2;
    pos += dist * rayDir * Slider3; // Advance the point forwards in the ray direction by the distance
}
    if (dist < EPSILON)
{
    // Lighting code
}
else
{
    gl_FragColor = vec4(0.0);
}
    vec2 eps = vec2(0.0, EPSILON);
vec3 normal = normalize(vec3(
    distfunc(pos + eps.yxx) - distfunc(pos - eps.yxx),
    distfunc(pos + eps.xyx) - distfunc(pos - eps.xyx),
    distfunc(pos + eps.xxy) - distfunc(pos - eps.xxy)));
    
    float diffuse = max(0.0, dot(-rayDir, normal));
    float specular = pow(diffuse, 35.0);
    
    vec3 color = vec3(diffuse + specular);
gl_FragColor = vec4(color, 1.0);
    
	gl_FragColor = vec4(color,1.0);
}
/* ***************************************************** */

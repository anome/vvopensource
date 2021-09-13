/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "_count",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 20,
      "LABEL" : "Count",
      "MIN" : 1
    },
    {
      "NAME" : "light_1_color",
      "TYPE" : "color",
      "DEFAULT" : [
        0.843822181224823,
        0.30707782506942749,
        0.43918305635452271,
        1
      ],
      "LABEL" : "Light 1 Color"
    },
    {
      "NAME" : "light_2_color",
      "TYPE" : "color",
      "DEFAULT" : [
        0.070682711899280548,
        0.95574849843978882,
        0.89976900815963745,
        1
      ],
      "LABEL" : "Light 2 Color"
    },
    {
      "NAME" : "light_1_x",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 0,
      "LABEL" : "light 1 Pos x",
      "MIN" : -50
    },
    {
      "NAME" : "light_1_y",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 5,
      "LABEL" : "light 1 Pos y",
      "MIN" : -50
    },
    {
      "NAME" : "light_1_z",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 6,
      "LABEL" : "light 1 Pos z",
      "MIN" : -50
    },
    {
      "NAME" : "light_2_x",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 0,
      "LABEL" : "light 2 Pos x",
      "MIN" : -50
    },
    {
      "NAME" : "light_2_y",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 1,
      "LABEL" : "light 2 Pos y",
      "MIN" : -50
    },
    {
      "NAME" : "light_2_z",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 10,
      "LABEL" : "light 2 Pos z",
      "MIN" : -50
    }
  ],
  "CREDIT" : ""
}
*/

//globals for the ray march
#define MAX_STEPS 20
#define MAX_DIST 80.0
#define SURF_DIST .01
#define COUNT 8.0


//create some scene objects

float getDist(vec3 point){
	
	float d;
		
	for (int i = 0; i<int(COUNT); i++){
		
		float it = float(i);
		float index = it/COUNT;
		
		//define a sphere x,y,z,r
		vec4 sphere = vec4(-5.+it,sin(TIME+it)+1.0,sin(TIME+it)+5.,  .25+(cos(TIME) + 1.0) *.5 );
		float sphereDist = length(point-sphere.xyz)-sphere.w;
		if (i ==0) d = sphereDist;
		d = min(d,sphereDist) ;
		}
		
	//define a ground plane
	float plane = point.y;
	
	d = min(d,plane);
	
	return d;
	
}


//the ray march algorithm
float rayMarch(vec3 ro, vec3 rd){
	
	//track the distance of this ray (Distance Origin)
	float distOrigin = 0.0;
	//start the march
		for ( int i = 0; i < MAX_STEPS;i++){
			
			//add the origina and the direction by the iteration size
			vec3 point = ro + rd * distOrigin;
			
			// get the distance from the objects in the scene to point along the ray
			float ds = getDist(point);
			
			//increment the distance along the ray (march)
			distOrigin += ds;
			
			//break when the point along the ray exceeds the max distance (no hit), or is smaller than a small value distance to a surface (a hit)
			if (distOrigin>MAX_DIST || ds < SURF_DIST) break;
			
			}
		
		
		return distOrigin;
	
}


//how to get a normal
vec3 getNormal(vec3 point){
	
	//get the distance of the point
	float d = getDist(point);
	
	//subtract a vector from the point a tiny bit in x, y,z
	
	vec3 n = d - vec3(
		getDist(point- vec3(.01,0.0,0.0)),
		getDist(point- vec3(0.0,0.01,0.0)),
		getDist(point- vec3(0.0,0.0,0.01))
		);
	
	//normalize the result to get the direction that the point is facing
	
	return normalize(n);
}

vec3 getLight(vec3 point){
		//light position
		vec3 lightPos = vec3(light_1_x,light_1_y,light_1_z);
		vec3 lightColor = light_1_color.rgb;
		
		vec3 lightPos2 = vec3(light_2_x,light_2_y,light_2_z);
		vec3 lightColor2 = light_2_color.rgb;
		
		
		//diffuse lighting model
		
		//two normalized vectors, first is the from the point to the light, second is the normal
		vec3 l = normalize(lightPos - point);
		vec3 l2 = normalize(lightPos2 - point);
		vec3 n = getNormal(point);
		
		//compare them with the dot product, returns 1.0 if they are parallel, 0.0 if perpindicular. Clamp to make sure values are between 0 <-> 1
		
		vec3 diffuse = clamp(dot(n,l),0.0,1.0) * lightColor;
		diffuse += clamp(dot(n,l2),0.0,1.0) * lightColor2;
		
		//building Shadows
		
		// Ray March from the the point on the ground to the light.
		
		float d = rayMarch(point+n*.1,l);
		float d2 = rayMarch(point+n*.1,l2);
		
		// if it hits something, attenuate so it's darker
		if (d2 < length(lightPos2-point) )
			diffuse*=.5;
		if (d < length(lightPos-point) )
			diffuse*=.5;
			
	return vec3(diffuse);
}


void main()	{
	
		//set up the screen space
		vec2 uv = (gl_FragCoord.xy - .5 * RENDERSIZE.xy)/RENDERSIZE.y;
		
		//let's make a camera
		//camera position xyz
		vec3 ro = vec3(0.0,1.0,0.0);
		//camera rays
		vec3 rd = normalize(vec3(uv.x,uv.y,1.0));
		
		//get the distance from the camera to the objects in the scene
		float d = rayMarch(ro,rd);
		
		//get the points of the objects to pass into lighting
		vec3 objectPoints = ro + rd * d;
		
		//diffuse lighting
		float dist = clamp(getDist(objectPoints),0.0,1.0);
		vec3 diffuse = getLight(objectPoints);

		vec3 color = vec3(diffuse);//*vec3(dist);
		
		gl_FragColor = vec4(color,1.0);
}

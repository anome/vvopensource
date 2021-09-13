/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "_offset",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.5,
      "LABEL" : "Count",
      "MIN" : 0.0
    },
    
    {
    	
    	"NAME" : "_floor",
    	"TYPE" : "bool",
    	"LABEL" : "Floor",
    	"DEFAULT" : true
    	
    }
  ],
  "CREDIT" : ""
}
*/

//globals for the ray march
#define MAX_STEPS 100
#define MAX_DIST 100.0
#define SURF_DIST .01

#define COUNT 10.0


//create some scene objects

float getDist(vec3 point){
	
	
	float d;
	
	for (int i = 0; i<int(COUNT); i++){
		
		float it = float(i);
		float index = it/_offset;
		
		//define a sphere x,y,z,r
		vec4 sphere = vec4(-5.+it,sin(TIME+index)+1.0,5.0,.25);
		float sphereDist = length(point-sphere.xyz)-sphere.w;
		if (i ==0) d = sphereDist;
		d = min(d,sphereDist) ;
		}
		
	/*
	//define a sphere x,y,z,r
	vec4 sphere1 = vec4(0.0,1.0,6.0,1.0);
	float sphereDist = length(point-sphere1.xyz)-sphere1.w;
	
	//define a sphere x,y,z,r
	vec4 sphere2 = vec4(2.0,1.0,8.0,2.0);
	float sphereDist2 = length(point-sphere2.xyz)-sphere2.w;
	
	//let's mix the spheres and add the ground plane
	float d = mix(sphereDist, sphereDist2,sin(TIME));
	float d2 = min(d, planeDist);

	*/
	//define a ground plane
	if (_floor){
	float plane = point.y;
	d = min(d,plane);
	}
	return d;
	
}


//the ray march algorithm
float rayMarch(vec3 ro, vec3 rd){
	
	//track the distance of this ray (Distance Origin)
	float distOrigin = 0.0;
	//start the march
		for ( int i = 0; i < MAX_STEPS;i++){
			
			//add the original and the direction by the iteration size
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

float getLight(vec3 point){
		//light position
		vec3 lightPos = vec3(0.0,5.0,6.0);
		
		lightPos.xz +=vec2(sin(TIME), cos(TIME))*2.0;
		
		
		//diffuse lighting model
		
		//two normalized vectors, first is the from the point to the light, second is the normal
		vec3 l = normalize(lightPos - point);
		vec3 n = getNormal(point);
		
		//compare them with the dot product, returns 1.0 if they are parallel, 0.0 if perpindicular. Clamp to make sure values are between 0 <-> 1
		
		float diffuse = clamp(dot(n,l),0.0,.1);
		 diffuse+= .5;
		
		//building Shadows
		
		// Ray March from the the point on the ground to the light.
		
		float d = rayMarch(point+n*.1,l);
		
		// if it hits something, attenuate so it's darker
		if (d < length(lightPos-point))
			diffuse*=.5;
			
	return diffuse;
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
		float diffuse = getLight(objectPoints);

		vec3 color = vec3(diffuse);
		color *= vec3(1.4,1.1,.5);
		
		gl_FragColor = vec4(color,1.0);
}

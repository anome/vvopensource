/*{
	"CREDIT": "by mojovideotech",
    "CATEGORIES" : [
    "3d",
    "distancefield",
    "Automatically Converted"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/XdsXR4 by 2DArray.  A distance-field renderer with directional lights and ambient occlusion",
  "INPUTS" : [
  	{
		"NAME": "RX",
		"TYPE": "float",
		"DEFAULT": 0.0,
		"MIN": 0.0,
		"MAX": 6.2831853
	}
  ]
}
*/

// DistanceField by mojovideotech
// copied from:
// www.shadertoy.com/view/XdsXR4


vec3 frustum;
float drawDistance = 250.0;
float epsilon = 0.05;
float aoStep = 0.2;
float aoFactor = 0.04;
float tileSize = 5.0;

const float PI = 3.14159;

vec3 rotVec3(vec3 vector,vec3 eulerAngles) {
	float sinT;
	float cosT;
	mat3 matrix;
	eulerAngles = -eulerAngles;
	if (eulerAngles.x!=0.0) {
		sinT = sin(eulerAngles.x);
		cosT = cos(eulerAngles.x);
		matrix = mat3(1.0,0.0,0.0,0.0,cosT,-sinT,0.0,sinT,cosT);
		vector = matrix * vector;
	}
	if (eulerAngles.y!=0.0) {
		sinT = sin(eulerAngles.y);
		cosT = cos(eulerAngles.y);
		matrix = mat3(cosT,0.0,sinT,0.0,1.0,0.0,-sinT,0.0,cosT);
		vector = matrix * vector;
	}
	if (eulerAngles.z!=0.0) {
		sinT = sin(eulerAngles.z);
		cosT = cos(eulerAngles.z);
		matrix = mat3(cosT,-sinT,0.0,sinT,cosT,0.0,0.0,0.0,1.0);
		vector = matrix * vector;
	}
	return(vector);
}

float distanceToSphere(vec4 sphere,vec3 point) {
	return(length(point-sphere.xyz)-sphere.w);
}

float distanceToRect(vec3 rectExtents,vec3 rectCenter,vec3 point) {
	vec3 tPoint = point-rectCenter;
	return(length(max(abs(tPoint)-rectExtents,0.0)));
}
float distanceToRoundRect(vec4 rectExtents,vec3 rectCenter,vec3 point) {
	vec3 tPoint = point-rectCenter;
	return(length(max(abs(tPoint)-rectExtents.xyz,0.0)) - rectExtents.w);
}
float distanceToFloor(float floorHeight,vec3 point) {
	return(point.y-floorHeight);
}

float distanceField(vec3 point) {
	float dist = 10000.0;
	
	//point.x+=sin(point.y+TIME) * 0.5;
	
	point=vec3(mod(point.x+tileSize*1.5,tileSize*3.0)-tileSize*1.5,point.y,mod(point.z+tileSize,tileSize*2.0)-tileSize);
	
	vec4 theSphere = vec4(0.0,0.0,0.0,1.0);
	dist = min(dist, distanceToSphere(theSphere,point));
	
	vec3 rectCenter = vec3(3.0,0.0,0.0);
	vec3 rectExtents = vec3(1.0,2.0,1.0);
	dist = min(dist, distanceToRect(rectExtents,rectCenter,point));
	
	rectCenter = vec3(-3.0,0.0,0.0);
	vec4 rectExtents2 = vec4(1.0,1.0,1.0,0.7);
	dist = min(dist,distanceToRoundRect(rectExtents2,rectCenter,point));
	
	dist=min(dist,distanceToFloor(-2.0,point));
	
	return(dist);
}

vec3 findNormal(vec3 point) {
	vec3 normal;
	
	float ep = epsilon * .25;
	
	normal.x=distanceField(point+vec3(ep,0.0,0.0))-
			 distanceField(point-vec3(ep,0.0,0.0));
	normal.y=distanceField(point+vec3(0.0,ep,0.0))-
			 distanceField(point-vec3(0.0,ep,0.0));
	normal.z=distanceField(point+vec3(0.0,0.0,ep))-
			 distanceField(point-vec3(0.0,0.0,ep));
	
	return(normalize(normal));
}

void main()
{
	float camRot = RX;
	vec3 camPos = vec3(cos(camRot)+.1,1.0,sin(camRot))*8.0;
	
	frustum = vec3 (1.0,RENDERSIZE.y/RENDERSIZE.x,1.0);
	
	//tileSize = 7.0 + cos(TIME*0.2)*2.0;
	
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec3 uv2 = vec3(uv,1.0);
	uv2 = uv2 * 2.0 - 1.0;
	vec3 viewDir = normalize(frustum * uv2);
	
	viewDir = rotVec3(viewDir, vec3(.3,-camRot-PI*0.5,0.0));
	
	float dist;
	vec3 rayPos = camPos+viewDir*0.001;
	vec3 travelVector;
	
	float light=0.0;
	
	bool stop = false;
	
	vec3 normal;
	
	for (int i=0;i<64;i++) {
		if (stop == false) {
			travelVector=rayPos-camPos;
			
			dist=distanceField(rayPos);
			
			if (dist<epsilon) {
				light=1.0;
				normal=findNormal(rayPos);
				stop=true;
			} else if (dot(travelVector,travelVector)>drawDistance*drawDistance) {
				light=0.0;
				stop=true;
			} else {
				rayPos+=viewDir*dist;
			}
		}
	}
	
	
	vec3 finalColor;
	if (light<.5) {
		light=uv2.y*.5+.5;
		finalColor = vec3(light,light,light);
	} else {
		vec3 sun = normalize(vec3(1.0,1.0,1.0));
		light = clamp(dot(normal,sun),0.0,1.0);
		
		sun = normalize(vec3(-1.0,-1.0,-1.0));
		light+=clamp(dot(normal,sun)*.5,0.0,1.0);
		
		light=max(light,0.1);
		
		float oDist = dist;
		for (float j=0.0;j<8.0;j++) {
			rayPos+=normal*aoStep;
			dist=distanceField(rayPos);
			light-=(aoStep*(j+1.0)-(dist-oDist))*aoFactor;
		}
		light=clamp(light,0.0,1.0);
		
		finalColor=vec3(light,light,light);
	}
	//light=length(travelVector)/drawDistance;
	//finalColor = vec3(light,light,light);
	
	
	gl_FragColor = vec4(finalColor,1.0);
}

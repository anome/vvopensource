/*{
	"CREDIT": "by joshpbatty",
	"DESCRIPTION": "Cube Deformation",
	"CATEGORIES": [
		"Joshua Batty"
	],
	"INPUTS": [
		{
			"NAME": "mirrorX",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "mirrorY",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "size",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "cam_amp",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

//Simple raymarching sandbox with camera

//Raymarching Distance Fields
//About http://www.iquilezles.org/www/articles/raymarchingdf/raymarchingdf.htm
//Also known as Sphere Tracing


//Util Start
vec2 ObjUnion(in vec2 obj0,in vec2 obj1){
    if (obj0.x<obj1.x)
        return obj0;
    else
        return obj1;
}
//Util End

//Scene Start

//Floor
vec2 obj0(in vec3 p){
    //obj deformation
    p.y=p.y+sin(sqrt(p.x*p.x+p.z*p.z)-TIME*(speed*6.0))*0.5;
    //plane
    return vec2(p.y+3.0,0);
}
//Floor Color (checkerboard)
vec3 obj0_c(in vec3 p){
    if (fract(p.x*.5)>.5)
        if (fract(p.z*.5)>.5)
            return vec3(0,0,0);
        else
            return vec3(2,1,1);
        else
            if (fract(p.z*.5)>.5)
                return vec3(1,1,1);
            else
                return vec3(0,0,0);
}

float box_size = 0.0025 + size * 0.35;

//IQs RoundBox (try other objects http://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm)
vec2 obj1(in vec3 p){
    //obj deformation
    p.y=p.y+sin(sqrt(p.x*p.x+p.z*p.z)-TIME*(speed*6.0))*0.5;
    p.x=fract(p.x+0.5)-0.5;
    p.z=fract(p.z+0.5)-0.5;
    //p.y=p.y-1.0+sin(time*6.0);
    return vec2(length(max(abs(p)-vec3(box_size),0.0))-0.05,1);
}

//RoundBox with simple solid color
vec3 obj1_c(in vec3 p){
    return vec3(1.0,sin(p.x*0.2),sin(p.z*0.2));
}

//Objects union
vec2 inObj(in vec3 p){
    return ObjUnion(obj0(p),obj1(p));
}

//Scene End
void main() {

    vec2 vPos=-1.0+2.0*isf_FragNormCoord.xy;
    if(mirrorX) vPos.x = abs(vPos.x);
    if(mirrorY) vPos.y = abs(vPos.y);
 
    //Camera animation
    vec3 vuv=vec3(0,1,sin(TIME*0.1));//Change camere up vector here
    vec3 prp=vec3(-sin(TIME*0.16)*8.0,-1,cos(TIME*0.14)*(cam_amp*40.0)); //Change camera path position here
    vec3 vrp=vec3(0,0,0); //Change camere view here
   
    
    //Camera setup
    vec3 vpn=normalize(vrp-prp);
    vec3 u=normalize(cross(vuv,vpn));
    vec3 v=cross(vpn,u);
    vec3 vcv=(prp+vpn);
    vec3 scrCoord=vcv+vPos.x*u*RENDERSIZE.x/RENDERSIZE.y+vPos.y*v;
    vec3 scp=normalize(scrCoord-prp);
      
    //Raymarching
    const vec3 e=vec3(0.1,0,0);
    const float maxd=60.0; //Max depth
    
    vec2 s=vec2(0.1,0.0);
    vec3 c,p,n;
    
    float f=1.0;
    for(int i=0;i<256;i++){
        if (abs(s.x)<.01||f>maxd) break;
        f+=s.x;
        p=prp+scp*f;
        s=inObj(p);
    }
    
    if (f<maxd){
        if (s.y==0.0)
            c=obj0_c(p);
        else
            c=obj1_c(p);
        n=normalize(
                    vec3(s.x-inObj(p-e.xyy).x,
                         s.x-inObj(p-e.yxy).x,
                         s.x-inObj(p-e.yyx).x));
        float b=dot(n,normalize(prp-p));
        gl_FragColor = vec4((b*c+pow(b,8.0))*(1.0-f*.02),1.0);//simple phong LightPosition=CameraPosition
    }
    else gl_FragColor = vec4(0,0,0,1); //background color
}
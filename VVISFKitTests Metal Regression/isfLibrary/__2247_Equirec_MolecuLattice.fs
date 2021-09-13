/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "based on shadertoy/MlX3RB",
	"CATEGORIES": [
		"equirectangular"
	],
	"INPUTS": [
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		-1.0,
		"MAX" : 		1.0
	}
	]
}*/

////////////////////////////////////////////////////////////
// Equirec_MolecuLattice  by mojovideotech
//
// based on :
// shadertoy/MlX3RB
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	pi   	3.141592653589793 	// pi

mat3 xrot(float t) {
    return mat3(1.0, 0.0, 0.0, 0.0, cos(t), -sin(t), 0.0, sin(t), cos(t));
}

mat3 yrot(float t) {
    return mat3(cos(t), 0.0, -sin(t), 0.0, 1.0, 0.0, sin(t), 0.0, cos(t));
}

mat3 zrot(float t) {
    return mat3(cos(t), -sin(t), 0.0, sin(t), cos(t), 0.0, 0.0, 0.0, 1.0);
}

void main()
{
    float yy = pi*(gl_FragCoord.y/RENDERSIZE.y-.5);
    float xz = 2.0*pi*(gl_FragCoord.x/RENDERSIZE.x-.5);
    vec3 eye = vec3(sin(xz)*cos(yy), sin(yy), cos(xz)*cos(yy));
    float k, t, d, T = TIME * rate;
    vec3 coord, col;
    for(int i = 0; i < 16; ++i){
        vec3 pos = eye*t;
        pos = pos * xrot(-pi/4.0) * yrot(-pi/4.0);
        pos = pos * xrot(T) * yrot(T) * zrot(T);
    	pos += vec3 (0.5 + T, 0.25 + T, T);
        coord = floor(pos);
       	pos = (pos - coord) - 0.5;
        d = length(pos)-0.2;
        float idx = dot(coord,vec3(1.0));
        idx = floor(fract(idx/3.0)*3.0);
        if(idx==0.0){ col = vec3(1.0, 0.0, 0.0); }
        else if(idx==1.0){ col = vec3(0.0, 1.0, 0.0); }
        else if(idx==2.0){ col = vec3(0.0, 0.0, 1.0); }
		k = length(pos.xy)-0.05;
        if(k<d){
        	d=k;
            col=vec3(1.0,1.0,1.0);
        }
        k = length(pos.xz)-0.05;
        if(k<d){
        	d=k;
            col=vec3(1.0,1.0,1.0);
        }
        k = length(pos.yz)-0.05;
        if(k<d){
        	d=k;
            col=vec3(1.0,1.0,1.0);
        }
        t+=d;
    }
    
    float fog = 1.0 / (1.0 + t*t*0.5 + d*100.0);
    
	gl_FragColor = vec4(fog*col, 1.0);
}

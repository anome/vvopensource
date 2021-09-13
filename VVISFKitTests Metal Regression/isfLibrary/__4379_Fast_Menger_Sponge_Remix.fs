/*{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/lsjcWV by abje.  instead of using distance to closest point, it uses distance to hit(maybe), so i can lower the step count\nvoxel version(can't see as far) [url]https://www.shadertoy.com/view/MdlcWX[/url]",
    "IMPORTED": {
    },
    "INPUTS": [
        {
            "NAME": "iChannel0",
            "TYPE": "image"
        },
        {
            "DEFAULT": 5.1,
            "MAX": 20,
            "MIN": 0,
            "NAME": "Time",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.8,
            "MAX": 3,
            "MIN": 0,
            "NAME": "Hole",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}
*/


/**/
#define rot(spin) mat2(sin(spin),cos(spin),-cos(spin),sin(spin))

float mid(vec3 p) {
    p = min(p,p.yzx);
    return max(max(p.x,p.y),p.z);
}

float maxes(vec3 p) {
    return max(max(p.x,p.y),p.z);
}

#define time Time

void main() {



    vec3 var1 = vec3(3., 3.0,3.0); //the center of the hole
    vec3 var2 = vec3(Hole,Hole,Hole); //the size of the hole
    vec3 pos = vec3(3.0,1.666,time);
    
	vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    
    vec3 dir = normalize(vec3(uv,1.0));
    vec3 signdir = sign(dir);
    float dist = 0.0;
    vec3 normal;
    for (int i = 0; i < 8; i++) {
        vec3 pos2;
        float stepsize = 1.0;
        for (stepsize = 1.0; stepsize > 0.01; stepsize /= 3.0) {
            pos2 = mod(pos+dir*dist,6.0*stepsize)-var1*stepsize;
            if (mid(abs(pos2)) < stepsize*0.99)
                break;
        }
        
        vec3 num = (stepsize*var2-pos2*signdir)
            	   *step(abs(pos2),stepsize*var2)
            	   /dir*signdir;
        
        float len = mid(num);
        
        if (len < 0.001) {
            if (stepsize < 0.05) break;
            stepsize /= 3.0;
        } else {
            normal = vec3(equal(vec3(len),num));
        }
        
        dist += len;
    }
    pos += dir*dist;
    pos *= 4.0;
    //pos.xy*normal.z+pos.yz*normal.x+pos.xz*normal.y
    gl_FragColor = IMG_NORM_PIXEL(iChannel0,mod((vec2(dot(pos,normal.zxy),dot(pos.yzx,normal.zxy))),1.0))*dot(normal,vec3(0.5,0.75,1.0));
	//gl_FragColor = vec4((sin(pos*3.0+dist+TIME)*0.5+0.5)/(dist+1.0)*2.0+normal*0.2,1.0);
    //gl_FragColor.xyz = normal;
}
/**/

/**
//this one is for aiekick :P
#define rot(spin) mat2(sin(spin),cos(spin),-cos(spin),sin(spin))

#define fixbug

float mid(vec3 p) {
    p = min(p,p.yzx);
    return max(max(p.x,p.y),p.z);
}

#define time TIME

{
	vec2 uv = (fragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    
    float ft = floor(time/9.)+1., t = mod(time,9.);    
    //vec3 pos = (clamp(t-vec3(0,3,6),0.,3.)*2./3. + 1.) *pow(3.,ft);
    
    vec3 pos = vec3(3.0,3.0,time);
    
    vec3 dir = normalize(vec3(uv,1.0));
    vec3 signdir = sign(dir);
    float stepsize = 1.0;
    float dist = 0.0;
    vec3 normal;
    for (int i = 0; i < 8; i++) {
        
        vec3 pos2 = mod(pos-normal*signdir*stepsize,6.0*stepsize)-4.0*stepsize+normal*signdir*stepsize;
        
        vec3 num = (stepsize*2.0-pos2*signdir)
            	   *step(abs(pos2),vec3(stepsize*2.0))
            	   /dir*signdir;
        
        float len = mid(num);
        
        if (len < 0.01) {
            if (stepsize < 0.05) break;
            stepsize /= 3.0;
        } else normal = vec3(equal(vec3(len),num));
        
        pos += dir*len*1.001;
        dist += len*1.001;
    }
    fragColor = vec4((.5+.25*(sin(floor(pos)*10.0+time)*0.5+0.5)+.25*normal)*.9/dist, 1);
}
/**/

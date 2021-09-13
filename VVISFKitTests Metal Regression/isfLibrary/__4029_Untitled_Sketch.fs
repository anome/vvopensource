/*{
	"CREDIT": "by crackhouse",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
{
      "NAME": "R",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.1,
      "MAX":9
    },
    
{
      "NAME": "grid",
      "TYPE": "float",
      "DEFAULT": 5,
      "MIN": 0.1,
      "MAX": 1
    }

    
	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iTime = TIME;

// Anaglyph Structure
// Framed for https://fanzine.cookie.paris/
// Licensed under hippie love conspiracy
// Leon Denise (ponk) 2019.10.24
// Using code from Inigo Quilez


mat2 rot (float a) { float c=cos(a),s=sin(a); return mat2(c,-s,s,c); }
float random (in vec2 st) { return fract(sin(dot(st.xy,vec2(12.9898,78.233)))*43758.5453123); }

vec3 look (vec3 eye, vec3 target, vec2 anchor) {
    vec3 forward = normalize(target-eye);
    vec3 right = normalize(cross(forward, vec3(0,1,0.)));
    vec3 up = normalize(cross(right, forward));
    return normalize(forward * .5 + right * anchor.x + up * anchor.y);
}


float map (vec3 pos) {
    float scene = 10.0;
    float r = R;
    const float count = 8.0;
    for (float index = count; index > 0.0; --index)
    {
        pos.xz = abs(pos.xz)-1.5*r;
        pos.xz *= rot(0.4/r + iTime * 0.1)-sin(grid*0.03);
        pos.yz *= rot(1.5/r + iTime * 0.05)-sin(grid*0.03);
        pos.yx *= rot(.2/r + iTime * 0.05)+sin(grid*0.03);
        scene = min(scene, length(pos.xy)-0.001);
        scene = min(scene, length(pos)-.3*r)*cos(.2-grid*0.05);
        r /= 1.8;
    }
    return scene;
}

vec4 raymarch (vec3 eye, vec3 ray) {
    float dither = random(ray.xy+fract(iTime));
    vec4 result = vec4(eye, 0);
    float total = 0.0;
    float maxt = 20.0;
    const float count = 30.;
    for (float index = count; index > 0.0; --index) {
        result.xyz = eye + ray * total;
        float dist = map(result.xyz);
        if (dist < 0.001 + total * .002 || total > maxt) {
            result.w = index / count;
            break;
        }
        dist *= 0.9 + 0.1 * dither;
        total += dist;
    }
    result.w *= step(total, maxt);
    return result;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord ){
    vec2 uv = (fragCoord.xy-0.5*iResolution.xy)/iResolution.y;
    vec3 eye = vec3(1.,0.5,-4.5);
    vec3 at = vec3(0);
    vec3 ray = look(eye, at, uv);
    vec3 eyeoffset = 0.02*normalize(cross(normalize(at-eye), vec3(0,1.*grid,0)));

    vec4 resultLeft = raymarch(eye-eyeoffset, ray);
    vec4 resultRight = raymarch(eye+eyeoffset, ray);
    fragColor = vec4(resultLeft.w,vec2(resultRight.w),1);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
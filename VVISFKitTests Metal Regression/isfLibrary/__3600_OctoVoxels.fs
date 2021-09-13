/*{
	"CREDIT": "by mojovideotech",
	"CATEGORIES" : [
    "raytracing",
    "voxels"
  ],
 	"INPUTS" : [
    {
        "NAME" :        "rate",
        "TYPE" :        "float",
        "DEFAULT" :     0.25,
        "MIN" :         -1.5,
        "MAX" :         1.5
    }
  ],
	"IMPORTED" : [
    {
      "NAME" : "iChannel0",
      "PATH" : "VLtex3.png"
    }
  ],
    "ISFVSN" : 2.0
}
*/


/*  
	***********************************
	NOTE: code does not work in browser
	but runs fine in ISF Editor & VDMX
	must be something in GLSL ES spec?
	***********************************
*/


////////////////////////////////////////////////////////////////////
// OctoVoxels  by mojovideotech
//
// based on :
// shadertoy.com/view/4lcfDB by fizzer
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////

#ifdef GL_ES
precision highp float;
#endif


// Camera path
vec2 path(float z)
{
    vec2 p = vec2(0);
    p.x += cos(z / 4.) * 2. * sin(z / 6.) * .7 + cos(z / 2. + sin(z * .5) / 2.) * 3. * sin(z / 5.);
    p.y += sin(z / 3.) * 2. + cos(z / 5.) / 3. + sin(z / 5. + cos(z * 1.) / 3.) * 3.;
    return p;
}

// Voxel solid/empty function
float f(vec3 p)
{
    vec3 op = p;
    p.xy += path(p.z);
    float d = -(length(p.xy) - 4.);
    op.z = mod(op.z, 21.) - 10.5;
    return d + cos(p.x * 80.) + cos(p.y * 180.);
}

// Traces a ray
float trace(vec3 ro, vec3 rd, float maxt)
{
    vec3 p = ro, c, ofs;
    vec3 n;

    for(int i = 0; i < 64; ++i)
    {
        // Snap to nearest octahedron
        vec3 cp = fract(p) - .5; 
        vec3 acp = abs(cp); 
        ofs = step(acp.yzx, acp) * step(acp.zxy, acp) * sign(cp); 
        c = floor(p) + .5 + ofs * .5;

        // If this octahedron is solid then break out
        if(f(c) < 0.)
            break;

        // Get the 4 side plane normals that the ray is facing
        vec3 n0 = ofs + ofs.yzx;
        vec3 n1 = ofs - ofs.yzx;
        vec3 n2 = ofs + ofs.zxy;
        vec3 n3 = ofs - ofs.zxy;

        // Dot product of ray direction with side normals
        float d0 = dot(rd, n0);
        float d1 = dot(rd, n1);
        float d2 = dot(rd, n2);
        float d3 = dot(rd, n3);

        // Get intersection distances
        float t0 = (sign(d0) * .5 - dot(ro - c, n0)) / d0;
        float t1 = (sign(d1) * .5 - dot(ro - c, n1)) / d1;
        float t2 = (sign(d2) * .5 - dot(ro - c, n2)) / d2;
        float t3 = (sign(d3) * .5 - dot(ro - c, n3)) / d3;

        float mint = min(t0, min(t1, min(t2, t3)));
        
        // Update current point along ray
        p = ro + rd * (mint + 1e-3);
        
        if(mint > maxt)
            break;
    }

    return distance(p, ro);
}

void main() {

/*

    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * 2. - 1.;
    gl_FragColor.rgb = vec3(0);
    
    // Multisampling loop
    for(int y = 0; y < AA; ++y)
	    for(int x = 0; x < AA; ++x)
        {
            // Jittered time for motionblur
    		time = TIME-sin(0.1*cos(TIME)) ;//- IMG_PIXEL(iChannel1, ivec2(mod(gl_FragCoord.xy * float(AA) + vec2(x, y), 1024.)), 0).r * .02;
    		gl_FragColor.rgb += image(gl_FragCoord.xy + vec2(x, y) / float(AA));
        }
    
    gl_FragColor.rgb /= float(AA * AA);
    
*/
    vec4 fragColor = vec4 (0);
    gl_FragColor.rgb = vec3(0);
    // Set up primary ray direction
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * 2. - 1.;
    vec2 t = uv.xy;
    t.x *= RENDERSIZE.x / RENDERSIZE.y;
      
    vec3 ro = vec3(0., 0., -TIME * rate) + 1e-3, rd = normalize(vec3(t, 1.1));
    vec3 targ = ro;

    targ.z -= 4.;

    // Offset ray origin and camera target by path displacement
    ro.xy -= path(ro.z);
    targ.xy -= path(targ.z);

    // Camera coordinate system
    vec3 dir = normalize(targ - ro);
    vec3 left = normalize(cross(dir, vec3(0, 1, 0)));
    vec3 up = normalize(cross(left, dir));

    rd = rd.z * dir + rd.x * left + rd.y * up;

    // Trace primary ray
    float dist = trace(ro, rd, 100.);
    vec3 p = ro + rd * dist, n;

    // Snap to nearest octahedron
    vec3 cp = fract(p) - .5; 
    vec3 acp = abs(cp); 
    vec3 ofs = step(acp.yzx, acp) * step(acp.zxy, acp) * sign(cp); 
    vec3 c = floor(p) + .5 + ofs * .5;

    // Get surface normal
    vec2 u = vec2(dot(p - c, ofs.yzx), dot(p - c, ofs.zxy));
    u = step(abs(u).yx, abs(u)) * sign(u);
    n = normalize(u.x * ofs.yzx + u.y * ofs.zxy + sign(dot(p - c, ofs)) * ofs);

    // Directional shadow ray direction
    vec3 ld = normalize(vec3(1, 2, 3)) * 1.5;

    fragColor.a = 1.;
    
    // Distance darkening and directional light cosine term
    fragColor.rgb = vec3(exp(-dist / 5.) * pow(.5 + .5 * dot(n, normalize(ld)), 2.));

    // Colour selection
    float cs = (.5 + cos(c.z * 4. + 5. + c.x + c.y * 7.) * .5);
    
    // Apply colour
    fragColor.rgb *= mix(vec3(1,.1,.4),
                         mix(vec3(.1), vec3(.1, .4, .5), step(.66, cs)), step(.33,cs));
    
    // Darkening at octahedron edges
    float edges = 	smoothstep(0.01, .02, abs(dot(p - c, ofs))) *
        			smoothstep(0.01, .02, abs(dot(p - c, ofs.yzx + ofs.zxy))) *
        			smoothstep(0.01, .02, abs(dot(p - c, ofs.yzx - ofs.zxy)));
        
    fragColor.rgb *= mix(.5, 1., edges);

    // Trace directional shadow ray
    float st = trace(p + n * 2e-3, ld, length(ld) * 2.);

    // Apply (attenuated) directional shadow
    fragColor.rgb *= mix(.2, 1., clamp(st / length(ld), 0., 1.));
    
    // Fake AO
    fragColor.rgb *= 1. - smoothstep(2., 5.8, distance(p.xy, -path(p.z)));

    // Specular highlight
    fragColor.rgb *= 1. + pow(clamp(dot(normalize(ld), reflect(rd,n)), 0., 1.), 8.) * 4.;

    // Texture map
    vec2 tu;
    tu.x = dot(p, ofs.yzx) / 2.;
    tu.y = dot(p, ofs.zxy) / 2.;
    fragColor.rgb *= pow(IMG_NORM_PIXEL(iChannel0,mod(tu,1.0)).r, 1.5) * 1.1;
    
    // Fog
    fragColor.rgb = mix(vec3(.5), fragColor.rgb, exp(-dist / 1000.));
   
    // Vignette
    fragColor.rgb *= 1. - (pow(abs(uv.x), 5.) + pow(abs(uv.y), 5.)) * .3;
    
    // Tonemapping
    fragColor.rgb /= (fragColor.rgb + vec3(.4)) * .5;
    
    // Gamma
    fragColor.rgb = pow(fragColor.rgb, vec3(1. / 2.2));
    
    gl_FragColor = fragColor;
}



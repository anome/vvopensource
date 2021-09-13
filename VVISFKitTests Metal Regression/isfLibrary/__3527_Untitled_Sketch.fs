/*{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/ltccRl by irwatts.  Almost there.\nMouse drag rotates the camera.\nDroplets get pushed by the wind and stay alive longer when traveling along existing water trails.",
    "INPUTS": [],
    "ISFVSN": "2",
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        },
        {
        }
    ]
}
*/


// The MIT License
// Copyright © 2018 Ian Reichert-Watts
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// SHARED PARAMS (Must be same as Image :/)
const int NUM_PARTICLES = 150;
const float INTERACT_DATA_INDEX = float(NUM_PARTICLES)+1.0;
const float KINETIC_MOUSE_INDEX = INTERACT_DATA_INDEX+1.0;

// SHARED FUNCTIONS (Must be same as Image :/)
vec4 loadData( in float index ) 
{ 
    return IMG_NORM_PIXEL(BufferA,mod(vec2((index+0.5)/IMG_SIZE(BufferA).x,0.0),1.0),0.0); 
}

float floorHeight( in vec3 p )
{
    return (sin(p.z*0.00042)*0.2)+(sin(p.z*0.008)*0.64) + (sin(p.x*0.42+sin(p.z*0.000042)*420.0))*0.42-1.0;
}

// PARAMS
const float PARTICLE_LIFETIME_MIN = 0.02;
const float PARTICLE_LIFETIME_MAX = 4.2;
const float FALL_SPEED = 42.0;
const float JITTER_SPEED = 300.0;
const vec3 WIND_DIR = vec3(0.0,0.0,-1.0);
const float WIND_INTENSITY = 4.2;

// CONST
const float PI = 3.14159;
const float TAU = PI * 2.0;

float randFloat( in float n )
{
    return fract( sin( n*64.19 )*420.82 );
}
vec2 randVec2( in vec2 n )
{
    return vec2(randFloat( n.x*12.95+n.y*43.72 ),randFloat( n.x*16.21+n.y*90.23 )); 
}

// The MIT License
// Copyright © 2018 Ian Reichert-Watts
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.




// PARAMS
const vec3 COLOR_PRIMARY = vec3(0.79, 0.17, 0.32); // Red Magenta
const vec3 COLOR_SECONDARY = vec3(0.0022, 0.00, 0.0032); // Dark Purple
const vec3 COLOR_TERTIARY = vec3(0.0, 1.0, 0.75); // Teal

vec3 SUN_DIR ;

// CONST

const int STEPS = 128;
const float STEP_SIZE = 0.42;

const float T_MAX = float(STEPS)*STEP_SIZE;

float floorHeightRender( in vec3 p )
{
    float height = floorHeight(p);
    vec2 point = RENDERSIZE.xy * 0.5;
    vec2 coord = floor(p.xz/0.1)*0.1;
    height += sin(length(coord-point)*(10000.0+TIME*0.02))*0.1;
    return height;
}

vec4 render( in vec3 rayOrigin, in vec3 rayDir)
{ 
     SUN_DIR = normalize(vec3(0.0,-0.13,-1.0));
    vec4 col = vec4(0.0);
    // Sun
    float sunDot = dot(rayDir, -SUN_DIR);
    // Sun Bloom
    col.rgb = max(col.rgb, 0.1*abs(sin(TIME+sunDot*42.0))*COLOR_PRIMARY);
    col.rgb += vec3(pow(sunDot, 42.0)*0.42)*COLOR_TERTIARY;
    // Sun Body
    float sunAlpha = clamp(sunDot-0.99, 0.0, 1.0);
    vec3 rayDown = cross(vec3(1.0,0.0,0.0), rayDir);
    sunAlpha *= clamp(cos(PI*2.9*clamp(dot(rayDown, SUN_DIR)*20.0+0.2, 0.0, 42.0)), 0.0, 1.0);
    col.rgb = max(col.rgb, 200.0*sunAlpha*COLOR_PRIMARY);
    // Sun Burst
    col.rgb += 0.3*sin((1.0-sunDot)*PI)*pow(sunDot,8.0)*abs(sin(atan(rayDir.y-0.1, rayDir.x)*8.0))*COLOR_TERTIARY;
    
    float t = STEP_SIZE;
    for( int i=0; i<STEPS; i++ )
    {
        vec3 p = rayOrigin+(rayDir*t);
        
        float depth = (t/T_MAX);
        float distFade = pow(1.0-depth, 2.0);
        
        float delta = p.y - floorHeightRender(p);
        
        // Floor
        float alpha = pow(clamp(1.0 - abs(delta), 0.0, 1.0), 42.0);
        float gridX = pow(abs(sin(p.x+sin(p.z*0.033)*6.4)), 1.42);
        float gridZ = pow(abs(sin(p.z*0.042)+sin(p.x*0.013)*0.2), 20.0);
        col.rgb = max(col.rgb, alpha*gridX*COLOR_PRIMARY);
        col.rgb = max(col.rgb, alpha*gridZ*COLOR_PRIMARY);
        float lightX = pow(abs(sin(p.x*0.064)*10.2), 0.42);
        col.rgb += 0.015*(pow(alpha,0.2)*(lightX-gridZ)*COLOR_TERTIARY)*distFade;
        
        // Atmosphere
        float bandFreq = 0.42;
        float band;
        if (delta > 0.0)
        {
            band = sin(p.z-p.y*bandFreq)+cos(p.z*bandFreq);
        }
        else
        {
            band = sin(p.z+p.y*bandFreq)+cos(p.z*bandFreq);
        }
        band += 1.0-clamp(p.y*0.8, 0.0, 1.0);
        vec3 cloud = vec3(gridZ+band, (abs(gridZ+band)), (gridZ*band));
        col.rgb += 0.0042*(1.0-alpha)*cloud*COLOR_PRIMARY;
        col.rgb += 0.01*clamp(p.y*0.03, 0.0, 1.0)*COLOR_TERTIARY;
        
        // Fog
        col.rgb += COLOR_SECONDARY;
        
        t += STEP_SIZE;
    }
    
    return col;
}

void main() {
   
    SUN_DIR = normalize(vec3(0.0,-0.13,-1.0));

	if (PASSINDEX == 0)	{
    

	    if ( gl_FragCoord.y > RENDERSIZE.y-2.0 )
	    {
	        // Discard top pixels to avoid persistent data getting included in blur
	        discard;
	    }
	    else if ( gl_FragCoord.y < 2.0 )
	    {
	        if ( gl_FragCoord.y >= 1.0 || gl_FragCoord.x > float(NUM_PARTICLES+4) )
	        {
	            discard;
	        }
	        // Store persistent data in bottom pixel row
	        if ( gl_FragCoord.x < float(NUM_PARTICLES) )
	        {
	            vec4 particle;
	            float pidx = floor(gl_FragCoord.x);
	
	            if ( FRAMEINDEX == 0 )
	            {
	                float padding = 0.01;
	                float particleStep = (1.0-(padding*2.0))/float(NUM_PARTICLES);
	                particle = vec4(0.0);
	                float r1 = randFloat(pidx);
	                particle.xy = vec2(padding+(particleStep*pidx), 1.0+(1.0*r1));
	                particle.xy *= RENDERSIZE.xy;
	                particle.a = r1*(PARTICLE_LIFETIME_MAX-PARTICLE_LIFETIME_MIN);
	            }
	            else
	            {   
	               	vec4 interactData = loadData(INTERACT_DATA_INDEX);
	                
	                // Tick particles
	        		particle = loadData(pidx);
	                vec2 puv = particle.xy / RENDERSIZE.x;
	                vec4 pbuf = IMG_NORM_PIXEL(BufferA,mod(puv,1.0));
	                
	                // Camera must be the same as Image :/
	                float rotYaw = -(interactData.x/RENDERSIZE.x)*TAU;
	                float rotPitch = (interactData.y/RENDERSIZE.y)*PI;
	                vec3 rayOrigin = vec3(0.0, 0.1, TIME*80.0);
	                float floorY = floorHeight(rayOrigin);
	                rayOrigin.y = floorY*0.9 + 0.2;
	
	                vec3 forward = normalize( vec3(sin(rotYaw), rotPitch, cos(rotYaw)) );
	                vec3 wup = normalize(vec3((floorY-floorHeight(rayOrigin+vec3(2.0,0.0,0.0)))*-0.2,1.0,0.0));
	                vec3 right = normalize( cross( forward, wup ) );
	                vec3 up = normalize( cross( right, forward ) );
	                mat3 camMat = mat3(right, up, forward);
	
	                vec3 surfforward = normalize( vec3(sin(rayOrigin.z*0.01)*0.042, ((floorY-floorHeight(rayOrigin+vec3(0.0,0.0,-20.0)))*0.2)+0.12, 1.0) );
	                vec3 wright = vec3(1.0,0.0,0.0);
	                mat3 surfMat = mat3(wright, up, surfforward); 
	
	                vec2 centeredCoord = puv-vec2(0.5);
	                vec3 rayDir = normalize( surfMat*normalize( camMat*normalize( vec3(centeredCoord, 1.0) ) ) );
	                vec3 rayRight = normalize( cross( rayDir, up ) );
	                vec3 rayUp = normalize( cross( rayRight, rayDir ) );
	
	                // Wind
	                vec2 windShield = (puv-vec2(0.5, 0.0))*2.0;
	                float speedScale = 0.0015*(0.1+1.9*(sin(PI*0.5*pow( particle.z/particle.a, 2.0 ))))*RENDERSIZE.y;
	                particle.x += (windShield.x+WIND_INTENSITY*dot(rayRight, WIND_DIR))*FALL_SPEED*speedScale*TIMEDELTA;
	                particle.y += (windShield.y+WIND_INTENSITY*dot(rayUp, WIND_DIR))*FALL_SPEED*speedScale*TIMEDELTA;
	
	                // Jitter
	                particle.xy += 0.001*(randVec2( particle.xy+TIME )-vec2(0.5))*RENDERSIZE.y*JITTER_SPEED*TIMEDELTA;
	
	                // Age
	                // Don't age as much when traveling over existing particle trails
	                particle.z += (1.0-pbuf.b)*TIMEDELTA;
	
	                // Die of old age. Reset
	                if ( particle.z > particle.a )
	                {
	                    float seedX = particle.x*25.36+particle.y*42.92;
	                    float seedY = particle.x*16.78+particle.y*93.42;
	                    particle = vec4(0.0);
	                    particle.x = randFloat( seedX )*RENDERSIZE.x;
	                    particle.y = randFloat( seedY )*RENDERSIZE.y;
	                    particle.a = PARTICLE_LIFETIME_MIN+randFloat(pidx)*(PARTICLE_LIFETIME_MAX-PARTICLE_LIFETIME_MIN);
	                }
	            }
	            gl_FragColor = particle;
	        }
			else
	        {
	            float dataIndex = floor(gl_FragCoord.x);
	            vec4 interactData = loadData(INTERACT_DATA_INDEX);
	            vec4 kineticMouse = loadData(KINETIC_MOUSE_INDEX);
	            
	            
	                kineticMouse.zw *= 0.9;
	                interactData.xy += kineticMouse.zw;
	                interactData.y = clamp( interactData.y, -RENDERSIZE.y, RENDERSIZE.y );
	                kineticMouse.xy = vec2(1.0,1.0);
	            
	            gl_FragColor =  interactData;
	        }
	    }
	    else
	    {
	        // Draw Particles
	        vec2 blurUV = fract( (gl_FragCoord.xy + (fract( float(FRAMEINDEX)*0.5 )*2.0-0.5)) / RENDERSIZE.xy );
	        vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	        gl_FragColor = IMG_NORM_PIXEL(BufferA,mod(uv,1.0));
	        vec4 prevColor = gl_FragColor;
	
	        if ( gl_FragColor.a < 1.0 )
	        {
	            gl_FragColor = IMG_NORM_PIXEL(BufferA,mod(blurUV,1.0));
	        }
	        gl_FragColor.b *= 0.996;

	        for ( int i=0; i<NUM_PARTICLES; i++ )
	        {
	    		vec4 particle = loadData(float(i));
	            vec2 delta = gl_FragCoord.xy-particle.xy;
	            float dist = length(delta);
	            float radius = 0.002*(0.5+2.0*particle.a+abs(sin(1.0*TIME+float(i))))*RENDERSIZE.y;
	            radius += 4.0*randFloat( particle.x*35.26+particle.y*93.12 )*pow((particle.z/particle.a), 12.0);
	            if (true)// dist < radius )
	            {
	                // normal
	                vec2 dir = delta/dist;
	                gl_FragColor.r = dot(dir, vec2(1.0,0.0))*0.5+0.5;
	                gl_FragColor.g = dot(dir, vec2(0.0,1.0))*0.5+0.5;
	                // height
	                float height = sin( dist/radius*PI*0.5 );
	                height = pow( height, 8.0 );
	                height = 1.0-height;
	                gl_FragColor.b = max( height, prevColor.b );
	                // age
	                gl_FragColor.a = 1.0;
	            }
	        }
	        //gl_FragColor.a += 0.1*TIMEDELTA;
	         //gl_FragColor.w =  1.0;
	    }
	}
	else if (PASSINDEX == 1)	{


	    vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
	    vec4 interactData = loadData(INTERACT_DATA_INDEX);
	    
	    // Camera must be the same as Buf A :/
	    vec3 rayOrigin = vec3(0.0, 0.0, TIME*80.0);
	    float floorY = floorHeight(rayOrigin);
	    rayOrigin.y = floorY*0.9 + 0.2;
	    float rotYaw = -(interactData.x/RENDERSIZE.x)*TAU;
	    float rotPitch = (interactData.y/RENDERSIZE.y)*PI;
	    
	    vec3 forward = normalize( vec3(sin(rotYaw), rotPitch, cos(rotYaw)) );
	    vec3 wup = normalize(vec3((floorY-floorHeight(rayOrigin+vec3(2.0,0.0,0.0)))*0.2,1.0,0.0));
	    vec3 right = normalize( cross( forward, wup ) );
	    vec3 up = normalize( cross( right, forward ) );
	    mat3 camMat = mat3(right, up, forward); 
	    
	    vec3 surfforward = normalize( vec3(sin(rayOrigin.z*0.01)*0.042, ((floorY-floorHeight(rayOrigin+vec3(0.0,0.0,-20.0)))*0.2)+0.12, 1.0) );
	    vec3 wright = vec3(1.0,0.0,0.0);
	    mat3 surfMat = mat3(wright, up, surfforward); 
	    
	    vec2 centeredCoord = (gl_FragCoord.xy-(RENDERSIZE.xy*0.5))/RENDERSIZE.x;
	    
	    vec3 rayDir = normalize( surfMat*normalize( camMat*normalize( vec3(centeredCoord, 1.0) ) ) );
	    
	    float mask = 1.0-IMG_NORM_PIXEL(BufferA,mod(uv,1.0)).a;
	    if (mask > 0.0)
	    {
	        float height = IMG_NORM_PIXEL(BufferA,mod(uv,1.0)).b;
	        vec3 normal = -normalize(vec3(IMG_NORM_PIXEL(BufferA,mod(uv,1.0)).xy*2.0-vec2(1.0), -1.0));
	        float refraction = height*mask*0.3;
	        rayDir = normal*refraction + rayDir*(1.0-refraction);
	        rayDir = normalize(rayDir);
	    }
	    
	    //*/ Remove/Add initial '/' to toggle between Image and Buf A
	    // Image
	    gl_FragColor.w =  1;
	    gl_FragColor.xyz +=render(rayOrigin, rayDir).xyz;
	    //gl_FragColor = render(rayOrigin, rayDir);
	    /*/
		// Buf A
	    //gl_FragColor = IMG_NORM_PIXEL(BufferA,mod(uv,1.0));
		//*/
		//gl_FragColor = vec4(0.5,0.5,0.5,0.5);
	}
 
}

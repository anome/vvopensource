/*{
    "DESCRIPTION": "Your shader description",
    "CREDIT": "by you",
    "CATEGORIES": [
        "Your category"
    ],
      "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "vitesse",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 1.0,
      "DEFAULT": 0.25
    },
    {
      "NAME": "amplitude",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1000,
      "DEFAULT": 100
    }
          ]
          
          ,
    "IMPORTED": {
        "coucou": {
            "PATH": "perlin_noise_2.png"

        }
    }
}*/

//int main = 0;

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iTime = TIME;

    
void mainVideo( out vec4 fragColor, in vec2 fragCoord )
{
    // OK
    float loop = fract (TIME*vitesse);
    
    // OK
    vec2 uv = fragCoord.xy / iResolution.xy;
    
    // NOT OK
    //vec3 fire = IMG_NORM_PIXEL(noise,vec2(fract(uv.x),fract(uv.y+vitesse*TIME))).rgb/amplitude;
    //fragColor = vec4(fire.x*amplitude, fire.y, fire.z, 1.0);
    
    // ----- DEC 1
    // OK
    vec2 FIFI = vec2(fract(uv.x),fract(uv.y+vitesse*TIME));
    //fragColor = vec4(FIFI.x,FIFI.y,0.0,1.0);
    
    // Not OK
    vec4 fire = IMG_NORM_PIXEL(coucou,FIFI);
    
    // OK
    //fire = fire/amplitude;
    fragColor = vec4(fire.x*amplitude, fire.y, fire.z, 1.0);
    //vec3 fire = IMG_NORM_PIXEL(noise, vec2(fract(uv.x),fract(uv.y+vitesse*TIME)). ).rgb/amplitude;
    
    

    return;
    
    //fragColor = vec4(fire.x, fire.y, fire.z, 1.0);
    
    //vec2 where = (uv.xy-fire.xy);
    //vec3 texchur1 = IMG_NORM_PIXEL(inputImage,vec2(where.x,where.y)).rgb;
    //fragColor = vec4(texchur1,1.0);
}

void main(void) {
    mainVideo(gl_FragColor, gl_FragCoord.xy);
}

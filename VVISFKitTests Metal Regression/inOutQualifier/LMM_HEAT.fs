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
        "noise": {
            "PATH": "perlin_noise_2.png"

        },
        "_noise": {
        "PATH": "perlin_noise_2.png"

        }
    }
}*/



vec3 iResolution = vec3(RENDERSIZE, 1.);
float iTime = TIME;

    
void mainVideo( out vec4 fragColor, in vec2 fragCoord )
{


float noiseAnything = fract (TIME*vitesse);
    
    
    vec2 uv = fragCoord.xy / iResolution.xy;
    vec3 fire = IMG_NORM_PIXEL(noise,vec2(fract(uv.x),fract(uv.y+vitesse*TIME))).rgb/amplitude;
    vec3 fire2 = IMG_NORM_PIXEL(_noise,vec2(fract(uv.x),fract(uv.y+vitesse*TIME))).rgb/amplitude;
    vec2 where = (uv.xy-fire.xy);
    vec2 where2 = (uv.xy-fire2.xy);
    vec3 texchur1 = IMG_NORM_PIXEL(inputImage,vec2(where.x,where2.y)).rgb;
    
    fragColor = vec4(texchur1,noiseAnything);
    
    }

void main(void) {
    mainVideo(gl_FragColor, gl_FragCoord.xy);
}

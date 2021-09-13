
/*
{
  "CATEGORIES" : [
    "Generative art"
  ],
  "DESCRIPTION" : "Pixel sorting",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "imageTexture",
      "TYPE" : "image"
    },
    {
      "NAME" : "threshold",
      "TYPE" : "float",
      "MAX" : 2.5,
      "DEFAULT" : 1.15,
      "MIN" : 0.5,
      "IDENTITY" : 0
    }
  ],
"PASSES": [
	{
	  "TARGET": "selfTexture",
      "persistent": true,
      "WIDTH": "$WIDTH",
      "HEIGHT": "$HEIGHT"
	}
	],
  "CREDIT" : "Ivan Dianov"
}
*/


bool compare(vec4 a, vec4 b) {
    // return a.r + a.g + a.b > b.r + b.g + b.b;
    return a.r < b.r;
    // return a.r < b.r;
    // return a.r < a.r * b.g; 
}

void main() {
    vec2 uv = gl_FragCoord.xy;
    vec4 image = IMG_PIXEL(imageTexture, uv);
    vec4 self = IMG_PIXEL(selfTexture, uv / RENDERSIZE);
    if(mod(float(FRAMEINDEX), 100.) == 0.) {
        gl_FragColor = image;
    }
    else{
        float t;
        // t = float(FRAMEINDEX);
        t = floor(TIME * 100.);
        vec4 neighbour;
    
        const int direction = 1;
    
        // если индекс столбца + кадр = чётное,
        if (mod(gl_FragCoord[direction] + .5 + t, 2.) == 0.) {
            // берём соседа слева
            neighbour = IMG_PIXEL(selfTexture, (uv - vec2(0, 1)) / RENDERSIZE);
        } else {
            neighbour = IMG_PIXEL(selfTexture, (uv + vec2(0, 1)) / RENDERSIZE);
        }
    
        // gl_FragColor = neighbour;
        if (mod(gl_FragCoord[direction] + .5, 2.) == 0.) {
            // gl_FragColor = vec4(1,0,0,1);
            if (mod(t, 2.) == 0.) {
                gl_FragColor = compare(neighbour, self) ? neighbour : self;
            } else {
                gl_FragColor = !compare(neighbour, self) ? neighbour : self;
            }
        } else {
            if (mod(t, 2.) == 0.) {
                gl_FragColor = !compare(neighbour, self) ? neighbour : self;
            } else {
                gl_FragColor = compare(neighbour, self) ? neighbour : self;
            }
        }
        
        // // gl_FragColor = vec4(vec3(fract(gl_FragCoord.y)), 1);

    }
    
//     	vec4		inputPixelColor;

// 	inputPixelColor = IMG_THIS_PIXEL(imageTexture);
	
// 	if(length(inputPixelColor) < 0.1) {
// 		inputPixelColor.r += 1.0;
// 		inputPixelColor.b += 0.5;
// 	}
	
// 	gl_FragColor = inputPixelColor;
}
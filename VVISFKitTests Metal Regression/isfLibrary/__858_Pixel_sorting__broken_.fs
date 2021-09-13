
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
      "persistent": true
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
    vec2 uv = gl_FragCoord.xy / RENDERSIZE;
    vec4 image = IMG_NORM_PIXEL(imageTexture, uv);
    vec4 self = IMG_NORM_PIXEL(selfTexture, uv);
    if(self.r < 0.01) {
        self = image;
    }

    float t;
    t = float(FRAMEINDEX);
    // t = floor(TIME * 10.);
    vec4 neightbour;

    const int direction = 1;

    // если индекс столбца + кадр = чётное,
    if (mod(gl_FragCoord[direction] - .5 + t, 2.) == 0.) {
        // берём соседа слева
        neightbour = IMG_NORM_PIXEL(selfTexture, uv - vec2( 1. / float(RENDERSIZE.x) * float(1-direction), 1. / float(RENDERSIZE.y) * float(direction)));
    } else {
        neightbour = IMG_NORM_PIXEL(selfTexture, uv + vec2( 1. / RENDERSIZE.x * float(1-direction), 1. / RENDERSIZE.y * float(direction) ));
    }

    if (mod(gl_FragCoord[direction] - .5, 2.) == 0.) {
        // gl_FragColor = vec4(1,0,0,1);
        if (mod(t, 2.) == 0.) {
            gl_FragColor = compare(neightbour, self) ? neightbour : self;
        } else {
            gl_FragColor = !compare(neightbour, self) ? neightbour : self;
        }
    } else {
        if (mod(t, 2.) == 0.) {
            gl_FragColor = !compare(neightbour, self) ? neightbour : self;
        } else {
            gl_FragColor = compare(neightbour, self) ? neightbour : self;
        }
    }
    
    // gl_FragColor = self;
    
    
//     	vec4		inputPixelColor;

// 	inputPixelColor = IMG_THIS_PIXEL(imageTexture);
	
// 	if(length(inputPixelColor) < 0.1) {
// 		inputPixelColor.r += 1.0;
// 		inputPixelColor.b += 0.5;
// 	}
	
// 	gl_FragColor = inputPixelColor;
}
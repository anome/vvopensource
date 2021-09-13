/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/XljyDd by alecksia.  Tartan plaid. Based on the theme \"warm.\" First shader.",
  "INPUTS" : [
    {
      "NAME" : "color1",
      "TYPE" : "color",
      "DEFAULT" : [
        0.7764706015586853,
        0.090196080505847931,
        0.14117647707462311,
        0.5
      ]
    },
    {
      "NAME" : "color2",
      "TYPE" : "color",
      "DEFAULT" : [
        0.47058823704719543,
        0.047058824449777603,
        0.18039216101169586,
        1
      ]
    },
    {
      "NAME" : "color3",
      "TYPE" : "color",
      "DEFAULT" : [
        0.72549021244049072,
        0.65098041296005249,
        0.23137255012989044,
        1
      ]
    },
    {
      "NAME" : "color4",
      "TYPE" : "color",
      "DEFAULT" : [
        0.94901961088180542,
        0.94901961088180542,
        0.79607844352722168,
        0.10000000149011612
      ]
    },
    {
      "NAME" : "color5",
      "TYPE" : "color",
      "DEFAULT" : [
        0.019607843831181526,
        0.30196079611778259,
        0.68627452850341797,
        0.40435740351676941
      ]
    },
    {
      "NAME" : "color6",
      "TYPE" : "color",
      "DEFAULT" : [
        0.16470588743686676,
        0.0039215688593685627,
        0.21960784494876862,
        0.10000000149011612
      ]
    }
  ],
  "ISFVSN" : "2",
  "PASSES" : [
    {

    }
  ]
}
*/


#define FREQUENCY 75
#define TILT -60
#define PATTERN 0.7

//method used to define the grid line and its x & y offsets
float coordinateGrid(vec2 r, float lineWidth, float offset, bool doubleLine) {

	float pixel = 0.0;
	
	//draw grid lines
	for(float i = 0.0; i < 2.0; i += PATTERN) {
               
        float x = mod(i, PATTERN * 2.0); //even or naw? I want to offset negatively or positively based on this, so I get symmetry.
        
        if (doubleLine) { //some of these grid lines have pairs, but only a few don't need a pair, so I'll use my argument to draw pairs or not.
            
            if (x == 0.0) {
                pixel += 1.0 - step(lineWidth, abs(r.x - i - offset)); //first x line
                pixel += 1.0 - step(lineWidth, abs(r.y - i + offset)); //first y line
            } else {
                pixel += 1.0 - step(lineWidth, abs(r.x - i + offset)); //second x line
                pixel += 1.0 - step(lineWidth, abs(r.y - i - offset)); //second y line
            }
            
        } else { //make a single x and y line only
            pixel += 1.0 - step(lineWidth, abs(r.x - i*2.0 - offset)); //first x line
            pixel += 1.0 - step(lineWidth, abs(r.y - i*2.0 + offset)); //first y line
        }
	}

	return pixel;
}

void main() {

 
	vec2 r = vec2(gl_FragCoord.xy - 0.01*RENDERSIZE.xy)/RENDERSIZE.y;
    //define my color palette, initially I had a method to calculate the colors, but ultimately performed the calculations inline due to optimization - suggested by a friend.
    vec4 pixel = color1; //background color
    
    //adding my lines to the output
	pixel = mix(pixel, color2, coordinateGrid(r, 0.15, 0.0, true)); //paired line
    pixel = mix(pixel, color4, coordinateGrid(r, 0.01, 0.005, true)); //paired line
    pixel = mix(pixel, color4, coordinateGrid(r, 0.01, -0.35, false)); //paired line
    pixel = mix(pixel, color6, coordinateGrid(r, 0.01, -0.4, false)); //single line
    pixel = mix(pixel, color6, coordinateGrid(r, 0.01, -0.3, false)); //single line
    pixel = mix(pixel, color5, coordinateGrid(r, 0.02, 0.15, true)); //paired line
    pixel = mix(pixel, color3, coordinateGrid(r, 0.01, 0.05, true)); //paired line
    //add some stripes for texture
    float stripe = fract( dot(r, vec2(FREQUENCY,TILT))); //This adds a line based on the dot product between the current pixel and my defined frequency, and returns the decimal value with fract. This means is increments every time, creating a stripe.
    pixel = mix(pixel, color1, stripe);
	
	gl_FragColor = pixel; //return
   
}

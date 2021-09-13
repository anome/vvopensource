/*
{
  "CATEGORIES" : [
    "DistortionEffect",
    "Glitch"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "LABELS" : [
        "Vertical",
        "Horizontal"
      ],
      "NAME" : "smearDirection",
      "TYPE" : "long",
      "LABEL" : "Smear Direction",
      "VALUES" : [
        0,
        1
      ]
    },
    {
      "NAME" : "brightThreshold",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.5,
      "MIN" : 0,
      "LABEL" : "Brightness Threshold"
    },
    {
      "NAME" : "velocity",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 20,
      "MIN" : -50,
      "LABEL" : "Smear Velocity"
    },
    {
      "NAME" : "velocityDecay",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : 0.35,
      "LABEL" : "Velocity Sustain",
      "MIN" : 0
    },
    {
      "NAME" : "colourDecay",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.98,
      "MIN" : 0,
      "LABEL" : "Colour Sustain"
    }
  ],
  "PASSES" : [
    {
      "WIDTH" : "$WIDTH",
      "DESCRIPTION" : "This pass will store the velocity values as calculated by the brightness of our input image, with feedback based decay to allow the smearing motion to continue for a while after a bright patch has passed over the current pixel.",
      "HEIGHT" : "$HEIGHT",
      "TARGET" : "decayBuffer",
      "PERSISTENT" : true
    },
    {
      "TARGET" : "outputPass",
      "PERSISTENT" : true
    }
  ],
  "CREDIT" : ""
}
*/

//Simple greyscale brightness function, averages the RGB components of the colour:
float brightness(vec4 inputColour)
{
	return inputColour.r+inputColour.g+inputColour.b/3.0;
}

//Our main function:
void main()	
{
	
	//We don't want to calculate these more than once, so we'll store them.
	//The brightness of this pixel in the input image.
	float inputBrightness = brightness(IMG_THIS_PIXEL(inputImage));
	//The brightness of this pixel in the decay buffer.
	float decayBrightness = brightness(IMG_THIS_PIXEL(decayBuffer));
	
	//In our velocity decay pass:
	if(PASSINDEX == 0)
	{
		//We use max() to decide whether the input or decay pixel is brighter, and multiply the outcome by velocityDecay,
		float maxBrightness = max(inputBrightness,decayBrightness)*velocityDecay;
		//then we store the reslt in this buffer.
		gl_FragColor = vec4(vec3(maxBrightness),1.0);

	}
	
	//In our output pass:
	 else if (PASSINDEX == 1)
	{

		//If our input pixel is brighter than brightThreshold, we'll pass through and ignore it. 
		if(inputBrightness > brightThreshold)
		{
			gl_FragColor = IMG_THIS_NORM_PIXEL(inputImage);
			
		//Otherwise, we'll check if the user has selected Vertical or Horizontal movement:	
		} else {
			if(smearDirection == 0)
			{
				//If they've picked Vertical, we mix the current output pixel with another pixel that's decayBrightness*velocity pixels higher up. This can be a negative value, so we're not restriced to downwards motion.
				gl_FragColor = mix(IMG_PIXEL(outputPass,vec2(gl_FragCoord.x,gl_FragCoord.y+(decayBrightness*velocity))),IMG_THIS_PIXEL(outputPass),brightness(IMG_THIS_PIXEL(inputImage)))*colourDecay;
			} else {
				//If they've picked horizontal we do the same thing, but adding decayBrightness*velocity to the X coordinate instead.
				gl_FragColor = mix(IMG_PIXEL(outputPass,vec2(gl_FragCoord.x+(decayBrightness*velocity),gl_FragCoord.y)),IMG_THIS_PIXEL(outputPass),brightness(IMG_THIS_PIXEL(inputImage)))*colourDecay;
			}
		}
	}
	
}

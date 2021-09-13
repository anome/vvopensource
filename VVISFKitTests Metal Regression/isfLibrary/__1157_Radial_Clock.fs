/*
{
  "CATEGORIES" : [
    "Generator",
    "Clock"
  ],
  "DESCRIPTION" : "Shows the current month, day, hour, minute and second",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "monthColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0,
        1,
        1
      ]
    },
    {
      "NAME" : "dayColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.9623805,
        0.0009925489999999999,
        0.8101674,
        1
      ]
    },
    {
      "NAME" : "hourColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.9623805,
        0.940358,
        0,
        1
      ]
    },
    {
      "NAME" : "minuteColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.2775573,
        0.8837106,
        0.4519494,
        1
      ]
    },
    {
      "NAME" : "secondColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.9481362,
        0.2424277,
        0.1411925,
        1
      ]
    },
    {
      "LABELS" : [
        "Normal",
        "Flip",
        "Rotate",
        "RotateFlip"
      ],
      "NAME" : "orientation",
      "TYPE" : "long",
      "DEFAULT" : 0,
      "VALUES" : [
        0,
        1,
        2,
        3
      ]
    },
    {
      "NAME" : "radius",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
    {
      "NAME" : "radialSizeShift",
      "TYPE" : "float",
      "MAX" : 4,
      "DEFAULT" : 0.75,
      "MIN" : 0.25
    },
    {
      "NAME" : "twentyFourHours",
      "TYPE" : "bool",
      "DEFAULT" : false
    }
  ],
  "CREDIT" : "VIDVOX"
}
*/





const float pi = 3.14159265359;



void main()	{
	
	vec4		resultColor = vec4(0.0);
	vec4		rowColor = vec4(0.0);
	vec2		loc = isf_FragNormCoord.xy;
	
	if ((orientation == 2)||(orientation == 3))	{
		loc.x = isf_FragNormCoord.y;
		loc.y = isf_FragNormCoord.x;
	}
	
	if ((orientation == 1)||(orientation == 3))
		loc.y = 1.0 - loc.y;
	
	vec2		texSize = RENDERSIZE;
	vec2		tc = loc * texSize;
	vec2		modifiedCenter = vec2(0.5,0.5)*RENDERSIZE;
	float		r = distance(modifiedCenter, tc);
	float		a = 1.0 - (atan ((tc.y-modifiedCenter.y),(tc.x-modifiedCenter.x)) + pi) / (2.0 * pi);
	a -= 0.25;
	a = mod(a,1.0);
	float		radius_sized = radius * max(RENDERSIZE.x,RENDERSIZE.y) / 2.0;
	
	//	The first element of the vector is the year, the second element is the month,
	//	the third element is the day, and the fourth element is the time (in seconds) within the day.
	vec4		currentDate = DATE;
	
	//	Here we are going to show the month, day, hour, minute and second
	float		whichRow = r / radius_sized;
	whichRow = pow(whichRow,radialSizeShift);
	whichRow *= 5.0;
	float		normalizedTimeForRow = -1.0;
	if (whichRow < 0.0)	{
		//	fill with background color
	}
	else if (whichRow < 1.0)	{
		normalizedTimeForRow = (1.0 + currentDate.g) / 12.0;
		rowColor = monthColor;
	}
	else if (whichRow < 2.0)	{
		//	default to 30, override for other months
		float		daysInThisMonth = 30.0;
		
		//	jan
		if (currentDate.g == 1.0)	{
			daysInThisMonth = 31.0;
		}
		//	feb
		else if (currentDate.g == 2.0)	{
			//	check for leap year
			daysInThisMonth = (mod(currentDate.a, 4.0) == 0.0) ? 29.0 : 28.0;
		}
		//	march
		else if (currentDate.g == 3.0)	{
			daysInThisMonth = 31.0;
		}
		//	may
		else if (currentDate.g == 5.0)	{
			daysInThisMonth = 31.0;
		}
		//	july
		else if (currentDate.g == 7.0)	{
			daysInThisMonth = 31.0;
		}
		//	aug
		else if (currentDate.g == 8.0)	{
			daysInThisMonth = 31.0;
		}
		//	oct
		else if (currentDate.g == 10.0)	{
			daysInThisMonth = 31.0;
		}
		//	dec
		else if (currentDate.g == 12.0)	{
			daysInThisMonth = 31.0;
		}
		
		normalizedTimeForRow = (1.0 + currentDate.b) / daysInThisMonth;
		rowColor = dayColor;
	}
	else if (whichRow < 3.0)	{
		//	compute the hour
		float		tmpVal = currentDate.a;
		float		h = 0.0;
		float		m = 0.0;
		float		s = 0.0;

		s = mod(tmpVal,60.0);
		tmpVal = tmpVal / 60.0;
		m = mod(tmpVal,60.0);
		tmpVal = tmpVal / 60.0;
		h = mod(tmpVal,60.0);
		
		normalizedTimeForRow = (twentyFourHours) ? (h) / 24.0 : (mod(h,12.0)) / 12.0;
		rowColor = hourColor;
	}
	else if (whichRow < 4.0)	{
		//	compute the minute
		float		tmpVal = currentDate.a;
		float		m = 0.0;
		float		s = 0.0;

		s = mod(tmpVal,60.0);
		tmpVal = tmpVal / 60.0;
		m = mod(tmpVal,60.0);
		
		normalizedTimeForRow = (m) / 60.0;
		rowColor = minuteColor;
	}
	else if (whichRow < 5.0)	{
		//	compute the second
		float		tmpVal = currentDate.a;
		float		s = 0.0;

		s = mod(tmpVal,60.0);
		normalizedTimeForRow = s / 60.0;
		rowColor = secondColor;
	}
	
	resultColor = ((a > 0.0) && (a < normalizedTimeForRow)) ? rowColor : resultColor;
	
	gl_FragColor = resultColor;
}
	
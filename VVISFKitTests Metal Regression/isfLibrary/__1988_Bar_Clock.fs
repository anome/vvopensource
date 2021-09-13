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
        0.9623805284500122,
        0.0009925490517281553,
        0.8101673580870506,
        1
      ]
    },
    {
      "NAME" : "hourColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.9623805284500122,
        0.9403579715783096,
        0,
        1
      ]
    },
    {
      "NAME" : "minuteColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.2775573246905918,
        0.8837105631828308,
        0.4519493960655115,
        1
      ]
    },
    {
      "NAME" : "secondColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.9481362104415894,
        0.2424276713419971,
        0.1411924820379365,
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
      "VALUES" : [
        0,
        1,
        2,
        3
      ]
    }
  ],
  "CREDIT" : "VIDVOX"
}
*/







void main()	{
	vec4		resultColor = vec4(0.0);
	vec4		rowColor = vec4(0.0);
	vec2		loc = isf_FragNormCoord.xy;
	
	if ((orientation == 2)||(orientation == 3))	{
		loc.x = isf_FragNormCoord.y;
		loc.y = isf_FragNormCoord.x;
	}
	
	if ((orientation == 0)||(orientation == 2))
		loc.y = 1.0 - loc.y;
	
	//	The first element of the vector is the year, the second element is the month,
	//	the third element is the day, and the fourth element is the time (in seconds) within the day.
	vec4		currentDate = DATE;
	
	//	Here we are going to show the month, day, hour, minute and second
	float		whichRow = loc.y * 5.0;
	float		normalizedTimeForRow = -1.0;
	if (whichRow < 1.0)	{
		normalizedTimeForRow = floor(currentDate.g) / 12.0;
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
		
		normalizedTimeForRow = floor(currentDate.b) / daysInThisMonth;
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
		
		normalizedTimeForRow = floor(h) / 24.0;
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
		
		normalizedTimeForRow = floor(m) / 60.0;
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
	
	resultColor = (loc.x < normalizedTimeForRow) ? rowColor : resultColor;
	
	gl_FragColor = resultColor;
}

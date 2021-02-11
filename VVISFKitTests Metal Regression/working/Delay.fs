/*{
  "DESCRIPTION": "small 15-frames delay",
  "CREDIT": "by Anomes",
  "CATEGORIES": [
    "Delay"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "delay",
      "TYPE": "long",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 15
    }
  ],
  "PASSES": [
    {
      	"TARGET": "buffer00",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer01",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer02",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer03",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer04",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer05",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer06",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer07",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer08",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer09",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer10",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer11",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer12",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer13",
      	"PERSISTENT": true
    },
    {
      	"TARGET": "buffer14",
      	"PERSISTENT": true
    },
    {
    }
  ]
}*/


vec4 getPixelColor()
{
	if( PASSINDEX == 0 )
	{
		return IMG_THIS_PIXEL(buffer01);
	}
	else if( PASSINDEX == 1 )
	{
		return IMG_THIS_PIXEL(buffer02);
	}
	else if( PASSINDEX == 2 )
	{
		return IMG_THIS_PIXEL(buffer03);
	}
	else if( PASSINDEX == 3 )
	{
		return IMG_THIS_PIXEL(buffer04);
	}
	else if( PASSINDEX == 4 )
	{
		return IMG_THIS_PIXEL(buffer05);
	}
	else if( PASSINDEX == 5 )
	{
		return IMG_THIS_PIXEL(buffer06);
	}
	else if( PASSINDEX == 6 )
	{
		return IMG_THIS_PIXEL(buffer07);
	}
	else if( PASSINDEX == 7 )
	{
		return IMG_THIS_PIXEL(buffer08);
	}
	else if( PASSINDEX == 8 )
	{
		return IMG_THIS_PIXEL(buffer09);
	}
	else if( PASSINDEX == 9 )
	{
		return IMG_THIS_PIXEL(buffer10);
	}
	else if( PASSINDEX == 10 )
	{
		return IMG_THIS_PIXEL(buffer11);
	}
	else if( PASSINDEX == 11 )
	{
		return IMG_THIS_PIXEL(buffer12);
	}
	else if( PASSINDEX == 12 )
	{
		return IMG_THIS_PIXEL(buffer13);
	}
	else if( PASSINDEX == 13 )
	{
		return IMG_THIS_PIXEL(buffer14);
	}
	else if( PASSINDEX == 14 )
	{
		return IMG_THIS_PIXEL(inputImage);
	}
	else // PASSINDEX == 15
	{
		if( delay == 1 )
		{
			return IMG_THIS_PIXEL(buffer14);
		}
		else if( delay == 2 )
		{
			return IMG_THIS_PIXEL(buffer13);
		}
		else if( delay == 3 )
		{
			return IMG_THIS_PIXEL(buffer12);
		}
		else if( delay == 4 )
		{
			return IMG_THIS_PIXEL(buffer11);
		}
		else if( delay == 5 )
		{
			return IMG_THIS_PIXEL(buffer10);
		}
		else if( delay == 6 )
		{
			return IMG_THIS_PIXEL(buffer09);
		}
		else if( delay == 7 )
		{
			return IMG_THIS_PIXEL(buffer08);
		}
		else if( delay == 8 )
		{
			return IMG_THIS_PIXEL(buffer07);
		}
		else if( delay == 9 )
		{
			return IMG_THIS_PIXEL(buffer06);
		}
		else if( delay == 10 )
		{
			return IMG_THIS_PIXEL(buffer05);
		}
		else if( delay == 11 )
		{
			return IMG_THIS_PIXEL(buffer04);
		}
		else if( delay == 12 )
		{
			return IMG_THIS_PIXEL(buffer03);
		}
		else if( delay == 13 )
		{
			return IMG_THIS_PIXEL(buffer02);
		}
		else if( delay == 14 )
		{
			return IMG_THIS_PIXEL(buffer01);
		}
		else if( delay == 15 )
		{
			return IMG_THIS_PIXEL(buffer00);
		}
		else
		{
			return IMG_THIS_PIXEL(inputImage);
		}
	}
	return vec4(0);
}




void main()
{
	gl_FragColor = getPixelColor();
}

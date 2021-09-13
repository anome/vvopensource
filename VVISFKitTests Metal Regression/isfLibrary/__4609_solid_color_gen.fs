/*{
  "CATEGORIES": [
    "solid color gen"
  ],
  "INPUTS": [
  
  {
			"NAME": "red",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "green",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 1
		},
		 {
			"NAME": "blue",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 1
		},
		{
			"NAME": "alpha",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 1
		}
   
  ]
}*/


#ifdef GL_ES
precision mediump float;
#endif


void main( void ) {

	gl_FragColor = vec4( red,green,blue, alpha );
}
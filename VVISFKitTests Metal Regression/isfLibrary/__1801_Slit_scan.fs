/*{
	"CREDIT": "by msfeldstein",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "scanHeight",
			"TYPE": "float",
			"MIN": 1,
			"MAX": 50,
			"DEFAULT": 3
		}
	],
  "PERSISTENT_BUFFERS": [
    "persistence1",
    "persistence2",
    "persistence3",
    "persistence4",
    "persistence5"
  ],
  "PASSES": [
    {
      "TARGET": "persistence1"
    },
    {
      "TARGET": "persistence2"
    },
    {
      "TARGET": "persistence3"
    },
    {
      "TARGET": "persistence4"
    },
    {
      "TARGET": "persistence5"
    },
    {
    
    }
    ]
}*/

void main() {
	float y = mod(TIME * 100.0, RENDERSIZE.y) / RENDERSIZE.y;
  if (PASSINDEX == 0) {
    gl_FragColor = texture2D(persistence2, vv_FragNormCoord);
  } else if (PASSINDEX == 1) {
    gl_FragColor = texture2D(persistence3, vv_FragNormCoord);  
  } else if (PASSINDEX == 2) {
    gl_FragColor = texture2D(persistence4, vv_FragNormCoord);  
  } else if (PASSINDEX == 3) {
    gl_FragColor = texture2D(persistence5, vv_FragNormCoord);
  } else if (PASSINDEX == 4) {
    gl_FragColor = texture2D(inputImage, vv_FragNormCoord);
  } else {
  	int index = int(vv_FragNormCoord.y * 5.0 / RENDERSIZE.y);
  	if (index == 0) {
  		
  	}
  	gl_FragColor = texture2D(persistence1, vv_FragNormCoord);
  }

}
function TimeVar () {
  this.rate = 0.0;
  this.time = 0.0;
}

TimeVar.prototype.updateTime = function(val, dt) {
  this.time = this.time+val*this.rate*dt*60;
}

function Accumulator () {
  this.value = 0.0;
  this.decay = 0.9;
}

Accumulator.prototype.update = function(val, dt){
  this.value += (val - this.value) * (this.decay * dt * 60.0);
  if (this.value > 1.0){
    this.value = 1.0;
  } else if (this.value<0.0){
    this.value = 0.0;
  }
}

var timevar = new TimeVar();
var accumulator = new Accumulator();

var decimator = 0;
function update(dt) {
  timevar.rate = inputs.speed;
  accumulator.update(1.0, dt);
  timevar.updateTime(accumulator.value, dt);
  uniforms.script_time = timevar.time*0.04;

  if(decimator%50==0){

  }
  decimator ++;

}
function transition() {
  //log(5);
}

/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	gl_FragColor = inputPixelColor;
}

/*{
  "CREDIT": "original implementation as v002.blur in QC by anton marini and tom butterworth, ported by zoidberg",
  "CATEGORIES": [
    "Blur"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "intensity",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 20,
      "DEFAULT": 20
    },
    {
      "NAME": "blurAmount",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 10,
      "DEFAULT": 5
    },
    {
      "NAME": "invert_map",
      "TYPE": "bool",
      "DEFAULT": 0
    }
  ],
  "PASSES": [
    {
      "TARGET": "smallA",
      "WIDTH": "floor($WIDTH*min((0.2/($blurAmount/10.0)),1.0))",
      "HEIGHT": "floor($HEIGHT*min((0.2/($blurAmount/10.0)),1.0))"
    },
    {
      "TARGET": "smallB",
      "WIDTH": "floor($WIDTH*min((0.2/($blurAmount/10.0)),1.0))",
      "HEIGHT": "floor($HEIGHT*min((0.2/($blurAmount/10.0)),1.0))"
    },
    {
      "TARGET": "smallC",
      "WIDTH": "floor($WIDTH*min((0.3/($blurAmount/10.0)),1.0))",
      "HEIGHT": "floor($HEIGHT*min((0.3/($blurAmount/10.0)),1.0))"
    },
    {
      "TARGET": "smallD",
      "WIDTH": "floor($WIDTH*min((0.3/($blurAmount/10.0)),1.0))",
      "HEIGHT": "floor($HEIGHT*min((0.3/($blurAmount/10.0)),1.0))"
    },
    {
      "TARGET": "smallE",
      "WIDTH": "floor($WIDTH*min((0.5/($blurAmount/10.0)),1.0))",
      "HEIGHT": "floor($HEIGHT*min((0.5/($blurAmount/10.0)),1.0))"
    },
    {
      "TARGET": "smallF",
      "WIDTH": "floor($WIDTH*min((0.5/($blurAmount/10.0)),1.0))",
      "HEIGHT": "floor($HEIGHT*min((0.5/($blurAmount/10.0)),1.0))"
    },
    {
      "TARGET": "smallG",
      "WIDTH": "floor($WIDTH*min((0.8/($blurAmount/10.0)),1.0))",
      "HEIGHT": "floor($HEIGHT*min((0.8/($blurAmount/10.0)),1.0))"
    },
    {
      "TARGET": "smallH",
      "WIDTH": "floor($WIDTH*min((0.8/($blurAmount/10.0)),1.0))",
      "HEIGHT": "floor($HEIGHT*min((0.8/($blurAmount/10.0)),1.0))"
    },
    {
      "TARGET": "smallI"
    },
    {}
  ]
}*/


varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

varying vec2		texOffsets[3];

float gray(vec4 n)
{
	return (n.r + n.g + n.b)/3.0;
}


void main() {
	vec4		sample0;
	vec4		sample1;
	vec4		sample2;
	float		edge = 0.0;
	vec4 color = IMG_THIS_PIXEL(inputImage);
		
	if (PASSINDEX == 0)	{
		sample0 = IMG_NORM_PIXEL(inputImage,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(inputImage,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(inputImage,texOffsets[2]);
	}
	else if (PASSINDEX == 1)	{
		sample0 = IMG_NORM_PIXEL(smallA,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallA,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallA,texOffsets[2]);
	}
	else if (PASSINDEX == 2)	{
		sample0 = IMG_NORM_PIXEL(smallB,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallB,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallB,texOffsets[2]);
	}
	else if (PASSINDEX == 3)	{
		sample0 = IMG_NORM_PIXEL(smallC,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallC,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallC,texOffsets[2]);
	}
	else if (PASSINDEX == 4)	{
		sample0 = IMG_NORM_PIXEL(smallD,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallD,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallD,texOffsets[2]);
	}
	else if (PASSINDEX == 5)	{
		sample0 = IMG_NORM_PIXEL(smallE,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallE,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallE,texOffsets[2]);
	}
	else if (PASSINDEX == 6)	{
		sample0 = IMG_NORM_PIXEL(smallF,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallF,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallF,texOffsets[2]);
	}
	else if (PASSINDEX == 7)	{
		sample0 = IMG_NORM_PIXEL(smallG,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallG,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallG,texOffsets[2]);
	}
	else if (PASSINDEX == 8)	{
		sample0 = IMG_NORM_PIXEL(smallH,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallH,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallH,texOffsets[2]);
	}
	else if (PASSINDEX == 9)	{
		sample0 = IMG_NORM_PIXEL(smallI,texOffsets[0]);
		sample1 = IMG_NORM_PIXEL(smallI,texOffsets[1]);
		sample2 = IMG_NORM_PIXEL(smallI,texOffsets[2]);
		
		vec4 colorL = IMG_NORM_PIXEL(inputImage, left_coord);
		vec4 colorR = IMG_NORM_PIXEL(inputImage, right_coord);
		vec4 colorA = IMG_NORM_PIXEL(inputImage, above_coord);
		vec4 colorB = IMG_NORM_PIXEL(inputImage, below_coord);

		vec4 colorLA = IMG_NORM_PIXEL(inputImage, lefta_coord);
		vec4 colorRA = IMG_NORM_PIXEL(inputImage, righta_coord);
		vec4 colorLB = IMG_NORM_PIXEL(inputImage, leftb_coord);
		vec4 colorRB = IMG_NORM_PIXEL(inputImage, rightb_coord);


		float gx = (-1.0 * gray(colorLA)) + (-2.0 * gray(colorL)) + (-1.0 * gray(colorLB)) + (1.0 * gray(colorRA)) + (2.0 * gray(colorR)) + (1.0 * gray(colorRB));
		float gy = (1.0 * gray(colorLA)) + (2.0 * gray(colorA)) + (1.0 * gray(colorRA)) + (-1.0 * gray(colorRB)) + (-2.0 * gray(colorB)) + (-1.0 * gray(colorLB));

		edge = intensity * pow(gx*gx + gy*gy,0.5);
		if (invert_map)	{
			edge = 1.0 - edge;
		}
		
	}
	else	{
		sample0 = vec4(1,0,0,1);
		sample1 = vec4(1,0,0,1);
		sample2 = vec4(1,0,0,1);
	}
	gl_FragColor = mix(vec4((sample0 + sample1 + sample2).rgb / (3.0), 1.0), color, edge);
}

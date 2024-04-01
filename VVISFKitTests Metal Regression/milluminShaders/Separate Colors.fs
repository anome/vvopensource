/*
{
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME": "mode",
      "TYPE": "long",
      "VALUES": [
          0,
          1,
          2,
          3,
          4
      ],
      "LABELS": [
          "Color only",
          "Red only",
          "Green only",
          "Blue only",
          "Alpha only"
      ],
      "DEFAULT": 0,
    },
  ]
}
*/

void main()
{
    vec4 color = IMG_THIS_PIXEL(inputImage);
    if( mode == 0 )
    {
        color.rgb = mix(vec3(0.), color.rgb, color.a);
        color.a = 1.;
        gl_FragColor = color;
    }
    else if( mode == 1 )
    {
        color.rgb = vec3(color.r);
        color.a = 1.;
        gl_FragColor = color;
    }
    else if( mode == 2 )
    {
        color.rgb = vec3(color.g);
        color.a = 1.;
        gl_FragColor = color;
    }
    else if( mode == 3 )
    {
        color.rgb = vec3(color.b);
        color.a = 1.;
        gl_FragColor = color;
    }
    else if( mode == 4 )
    {
        color.rgb = vec3(color.a);
        color.a = 1.;
        gl_FragColor = color;
    }
}

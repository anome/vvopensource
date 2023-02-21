/*{
    "CREDIT": "",
    "DESCRIPTION": "FrameBuffer Concurrent ReadWrite",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
    ],
    "PASSES": [

         {
             "TARGET": "BufferA"
         }
    ]
}
*/

// This bug has been partially fixed on df8c5ae (21/02/2023)
// But not this particular case (which is an antipattern : a user would try to read a texture and write into it at the same time, but it's not a persistent texture)

void main() {
        vec2 blend_uv = gl_FragCoord.xy / RENDERSIZE.xy;
        vec2 uv = vec2(1.0-blend_uv.x, blend_uv.y);
        vec4 sourceUv = IMG_NORM_PIXEL(BufferA, uv);
        gl_FragColor = vec4(1.0-sourceUv.x, 0.0,0.0, 1.0);
}



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
             "TARGET": "BufferA",
             "PERSISTENT": "true"
         }
    ]
}
*/

// This bug has been fixed on df8c5ae (21/02/2023)


void main() {
        vec2 blend_uv = gl_FragCoord.xy / RENDERSIZE.xy;
        vec2 uv = vec2(1.0-blend_uv.x, blend_uv.y);
        vec4 sourceUv = IMG_NORM_PIXEL(BufferA, uv);
        gl_FragColor = vec4(1.0-sourceUv.x, 0.0,0.0, 1.0);
}



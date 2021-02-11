/*{
 "CREDIT": "ameisso (anomes)",
 "INPUTS": [
 {
 "LABEL": "Line Width",
 "NAME": "lineWidth",
 "TYPE": "float",
 "DEFAULT": 2.0,
 "MIN": 0.0,
 "MAX": 10.0
 },
 {
 "LABEL": "Divisions",
 "NAME": "lineCount",
 "TYPE": "float",
 "DEFAULT": 10.0,
 "MIN": 1.0,
 "MAX": 30.0
 },
 {
 "NAME" : "square",
 "TYPE" : "bool",
 "DEFAULT" : 1,
 "LABEL" : "square"
 },
 {
 "NAME" : "diagonal",
 "TYPE" : "bool",
 "DEFAULT" : 1,
 "LABEL" : "diagonal"
 },
 {
 "NAME" : "colors",
 "TYPE" : "bool",
 "DEFAULT" : 1,
 "LABEL" : "colors"
 },
 {
 "NAME": "color",
 "TYPE": "color",
 "DEFAULT": [
 1.0,
 1.0,
 1.0,
 1.0
 ]
 },
 
 ]
 }*/

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    float modulox = mod(fragCoord.x-RENDERSIZE.x/2.,(RENDERSIZE.x-lineWidth)/floor(lineCount));
    float moduloy = mod(fragCoord.y-RENDERSIZE.y/2.,(RENDERSIZE.y-lineWidth)/floor(lineCount));
    float ratio = RENDERSIZE.x/RENDERSIZE.y;
    
    
    fragColor = vec4(0.0,0.,0.,0.);
    //GRID
    if(square)
    {
        moduloy = mod(fragCoord.y-RENDERSIZE.y/2.,(RENDERSIZE.y*ratio-lineWidth)/floor(lineCount));
    }
    if( modulox < lineWidth || moduloy < lineWidth/ratio )
    {
        fragColor = color;
    }
    
    //COLORS
    if(colors && (color.r != color.g || color.g != color.b))
    {
        float modulox = mod(fragCoord.x-RENDERSIZE.x/2.,(RENDERSIZE.x-lineWidth)/floor(lineCount)*5.);
        float moduloy = mod(fragCoord.y-RENDERSIZE.y/2.,(RENDERSIZE.y-lineWidth)/floor(lineCount)*5.);
        float ratio = RENDERSIZE.x/RENDERSIZE.y;
        
        if(square)
        {
            moduloy = mod(fragCoord.y-RENDERSIZE.y/2.,(RENDERSIZE.y*ratio-lineWidth)/floor(lineCount)*5.);
        }
        if( modulox < lineWidth || moduloy < lineWidth/ratio )
        {
            fragColor = vec4(1.-color.b,1.-color.r,1.-color.g,1.);
        }
    }
    //DIAGONALS
    if(diagonal)
    {
        if(abs(fragCoord.y*ratio-fragCoord.x) < lineWidth || abs((RENDERSIZE.y-fragCoord.y)*ratio-fragCoord.x) < lineWidth)
        {
            if(colors && (color.r != color.g || color.g != color.b))
            {
                fragColor =  vec4(1.-color.r,1.-color.g,1.-color.b,1.);
            }
            else
            {
                fragColor = color;
            }
        }
    }
    //BORDERS
    float finalLineWidth = lineWidth + min(RENDERSIZE.x,RENDERSIZE.y)*0.01;
    if(fragCoord.y > RENDERSIZE.y-finalLineWidth || fragCoord.y < finalLineWidth || fragCoord.x > RENDERSIZE.x-finalLineWidth || fragCoord.x <finalLineWidth)
    {
        fragColor = color;
    }
}

void main(void)
{
    mainImage(gl_FragColor, gl_FragCoord.xy);
}

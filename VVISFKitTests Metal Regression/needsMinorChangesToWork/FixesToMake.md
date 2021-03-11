# Fixes to make

* Bump Distorsion : redefinition of distance, causing metal compilation error (ambigous use of distance)
* Time Glitch: two variables with different types are named "lastRow" - SpirV refuses variable redefinition
* VoronoiLines : variable and function sharing a common name (iGlobalTime) causes SpirV compilation error
* Zoom Blur : variable named 'sample' is forbidden by SpirV.
* Freeze Frame : variable named 'mix' is forbidden by SpirV
* variableNamedSampler : variable named 'sampler' used as sampler2D can cause Metal compilation error (must use 'struct' tag to refer to type 'sampler' in this scope)

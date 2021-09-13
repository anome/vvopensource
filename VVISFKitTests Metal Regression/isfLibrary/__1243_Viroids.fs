/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/ltscW8 by CaliCoastReplay.  Using my raymarcher on the viroid from :  http://www.michaelwalczyk.com/blog/2017/5/25/ray-marching .  Looks like plastic blood proteins.  My method seems to be useful for shiny materials.  EDIT:  Added colored lights!",
  "INPUTS" : [
    {
      "NAME" : "een",
      "TYPE" : "float",
      "MAX" : 10,
      "MIN" : 0
    },
    {
      "NAME" : "twee",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : -2
    },
    {
      "NAME" : "drie",
      "TYPE" : "float",
      "MAX" : 2,
      "MIN" : -2
    },
    {
      "NAME" : "vier",
      "TYPE" : "float",
      "MAX" : 20,
      "MIN" : 0
    },
    {
      "NAME" : "vijf",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "zes",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 1,
      "MIN" : 0
    },
    {
      "NAME" : "zeven",
      "TYPE" : "float",
      "MAX" : 100,
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2"
}
*/


//Learning from : http://www.michaelwalczyk.com/blog/2017/5/25/ray-marching

//Contains some experimental terms to simulate global illumination
//on backside of object via a variation
//on specular highlights, and the use of Fresnel terms
//for both light and shadow on the outside of objects.

//Comments encouraged - code will be commented for teaching purposes soon.

//--CaliCoastReplay

//Returns the distance from a world point to a sphere defined by a center and radius.
//Used in this example to "map the world" by returning distances to the viroid forms
//from points along the marched rays.
float distance_from_sphere(in vec3 world_point, in vec3 sphere_center, float radius)
{
    return length(world_point - sphere_center) - radius;
}

	
//Polynomial smooth minimum by Inigo Quilez.
//Used in this case to "join" the two viroids into a metaball - a normal minimum would return
//unjoined surface data.
float smin( float a, float b, float k )
{
    float h = clamp( 0.0+0.5*(b-a)/k, vijf, 1.0 )*zeven;
    return mix( b, a, h ) - k*h*(zes-h);
}

//Distance to the closest world object from a given point p along the raymarcher.  
//Used in this example to "map the world" - see above.
float distance_to_closest_object(in vec3 p)
{
    
    float sphere_0 = distance_from_sphere(p, vec3(twee), 2.0);    
    float sphere_1= distance_from_sphere(p, vec3(drie, 0.5,0.3), 2.0);
    
    float displacement0 = sin(-3.0  * p.x) * sin(1.5 *  p.y) * sin(2.0 * p.z) * 0.25;
    float displacement1 = cos(vier *   p.x) * cos(1.5 *  p.y) * cos(2.0 * p.z) * 0.5;

    return smin(sphere_0 + displacement0, sphere_1 + displacement1, .7);
   // return smin(sphere_0, sphere_1, .7);  //switch with this for simple spheres instead
}

//Estimates the normal vector (the vector perpendicular to the surface) at any given world point
//representing a surface.  Should only be used at world points representing collisions.  Samples
//the world at six more points bounding the world point along the three primary world axes (x, y, 
//and z) and then uses that data to estimate the normalize there.
vec3 calculate_normal(in vec3 world_point)
{
    const vec3 small_step = vec3(0.0025, 0.0, 0.0);

    float gradient_x = distance_to_closest_object(world_point + small_step.xyy)
        - distance_to_closest_object(world_point - small_step.xyy);
    float gradient_y = distance_to_closest_object(world_point + small_step.yxy) 
        - distance_to_closest_object(world_point - small_step.yxy);
    float gradient_z = distance_to_closest_object(world_point + small_step.yyx) 
        - distance_to_closest_object(world_point - small_step.yyx);

    vec3 normal = vec3(gradient_x, gradient_y, gradient_z);

    return normalize(normal);
}

//The actual raymarcher.  "Marches" a ray along a direction vector, starting at an eye/camera
//point, by adding that direction vector to the origin, and repeating that until it either
//hits something, travels a certain number of steps, or reaches a maximum distance.  If it hits
//a surface, it calculates the surface normal at that point, and uses that normal to calculate
//the lighting according to a modified Phong shading model.
vec4 ray_march(in vec3 ray_origin, in vec3 ray_direction)
{
    float total_distance_traveled = 0.0;
    const int NUMBER_OF_STEPS = 64;
    const float MINIMUM_HIT_DISTANCE = 0.001;
    const float MAXIMUM_TRACE_DISTANCE = 1000.0;

    for (int i = 0; i < NUMBER_OF_STEPS; ++i)
    {
        vec3 current_position = ray_origin + total_distance_traveled * ray_direction;
		float distance_to_closest = distance_to_closest_object(current_position);
        if (distance_to_closest < MINIMUM_HIT_DISTANCE) 
        {
            vec3 normal = calculate_normal(current_position);
            
            vec3 light_positions[3];
            light_positions[0] = vec3(1.0+sin(TIME)*5.0, -3.0+3.0*cos(TIME/3.0), 4.0 + 1.0 *sin(TIME/5.0));
            light_positions[1] = vec3(1.0-sin(TIME/2.0)*2.0, -1.0-cos(TIME/2.0), 7.0 + 1.0 -sin(TIME/4.0));
            light_positions[2] = vec3(2.0-sin(TIME/2.0)*2.0, -5.0-sin(TIME/4.0), 2.0 + 1.0 -sin(TIME/1.0));
            vec3 light_intensities[3];
            light_intensities[0] = vec3(0.8, 0.4, 0.4);
            light_intensities[1] = vec3(0.04, 0.9, 0.2);
            light_intensities[2] = vec3(0.1, 0.2, 0.8);
            vec3 direction_to_view = normalize(current_position - ray_origin);float fresnel_base = 1.0 + dot(direction_to_view, normal);
            float fresnel_intensity = 0.04*pow(fresnel_base, 2.0);
            float fresnel_shadowing = pow(fresnel_base, 8.0);            
            float fresnel_supershadowing = pow(fresnel_base, 40.0);      
            float fresnel_antialiasing = 4.0*pow(fresnel_base, 8.0);
            float attenuation =  pow(total_distance_traveled,2.0)/150.0;
            
            vec3 col = vec3(0.0);
            
            for (int j = 0; j < 3; j++)
            {
                vec3 direction_to_light = normalize(current_position - light_positions[j]);
                vec3 light_reflection_unit_vector =
                	 reflect(direction_to_light ,normal);                

                float diffuse_intensity = 0.6*pow(max(0.0, dot(normal, direction_to_light)),5.0);            
                float ambient_intensity = 0.2;            
                float specular_intensity = 
                    1.15* pow(clamp(dot(direction_to_view, light_reflection_unit_vector), 0.0,1.0), 90.0);
                float backlight_specular_intensity =             
                    0.01* pow(clamp(dot(direction_to_light, light_reflection_unit_vector),0.0,1.0), 3.0); 
                
                
            	vec3 colFromLight = vec3(0.0);
                colFromLight += vec3(0.89, 0.35, 0.15) * diffuse_intensity;
                colFromLight += vec3(0.3, 0.1, 0.1) * ambient_intensity;
                colFromLight += vec3(1.0) * specular_intensity;            
                colFromLight += vec3(1.0,0.5,0.5) * backlight_specular_intensity;            
                colFromLight += vec3(1.0, 0.1, 0.2) * fresnel_intensity;
                colFromLight -= vec3(0.0, 1.0, 1.0) * fresnel_shadowing ;
                colFromLight -= vec3(0.0, 1.0, 1.0) * fresnel_supershadowing ;
                colFromLight += vec3(.3, 0.1, 0.1) - attenuation ; 
               //	colFromLight *= 1.6;
               // colFromLight *= sqrt(light_intensities[j]);
                col += colFromLight;
            }
            return vec4(col, 1.0-fresnel_antialiasing);
        }

        if (total_distance_traveled > MAXIMUM_TRACE_DISTANCE)
        {
            break;
        }
        total_distance_traveled += distance_to_closest;
    }
    return vec4(0.0);
}

//The final image shader, taking in the screen coordinate and outputting color.  
//Uses an eye (camera) position as the ray origin, then uses a look-at point
//and a camera projection matrix to find the ray direction corresponding to the
//screen coordinate.  Passes that ro/rd to the raymarcher to get the color at that point.
void main() {



    vec2 uv = (-RENDERSIZE.xy + 2.0*gl_FragCoord.xy) / RENDERSIZE.y;
     // camera movement	
	float time_factor = (0.0*TIME)+een;
	vec3 camera_position = vec3( 7.0*cos(time_factor), 0.4, 7.0*sin(time_factor));
    vec3 ray_origin = camera_position;    
    vec3 look_at = vec3( 0.0, sin(time_factor), 0.0 );
    // camera matrix
    vec3 ww = normalize( look_at - ray_origin );
    vec3 uu = normalize( cross(ww,vec3(0.0,1.0,0.0) ) );
    vec3 vv = normalize( cross(uu,ww));
	// create view ray
	vec3 ray_direction = normalize( uv.x*uu + uv.y*vv + 1.5*ww );
    
    vec4 shaded_color = ray_march(ray_origin, ray_direction);
    gl_FragColor = vec4(shaded_color);
}

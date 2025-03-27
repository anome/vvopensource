# Metal ISF

Metal ISF uses a fork of MoltenVK for shader conversion. For a quick try out, a CLI binary is included in the project. However, better performance is achieved by re-building MoltenVK libs on your machine.

## Quick setup (low performance)

__NO LONGER SUPPORTED - Requires Update__
Open vvopensource project and remove the missing files in VVISFKit/METAL/Converter
Comment the line `#define USE_MOLTENVK_LIB` from the file `MISFShaderConverter.mm`



## Standard Setup - with latest dependencies [will cause regressions someday]

MoltenVK is added in the folder `external` as a git submodule

After cloning, make sure to fetch the submodule with :
`git submodule init `
`git submodule update`
    
Then :
`cd external/MoltenVK/`
`brew install cmake python3 ninja`
`./fetchDependencies --macos`


Then (this is to fix a bug discovered March 2024 -- hacky workaround for LM_HEAT.fs) : 
- Open up external/MoltenVK/ExternalDependencies.xcodeproj
- look for this line "replace_illegal_names(keywords);" and comment it



Lastly :
- open MoltenVKPackaging.xcodeproj
- build "MoltenVK Package (macOS only)"
- build "MoltenVKShaderConverter-macOS"
- (OLD _ BEFORE UPDATING LIBS - 2024) Run VVISFKitTest Metal Regression and _make sure you have_ these results : Tests 9302 / Passed 8168 / Failed 1134
- (LATEST, WITH LIBS UPDATE - MARCH 2025) Run VVISFKitTest Metal Regression and got these results (v1.1.0) : Tests 9302 / Passed 8476 / Failed 826
- The regression testing allow to detect changes as moltenVK libs update themselves. Keep an eye on the millumin-related tests bellow
- To avoid Millumin regressions : Make sure the tests are like this
![Tests Mars 2025](./tests_tests_v20250327.png)
- (Good practice) Test LM_HEAT with metalTestApp to make sure the replace_illegal_names() change works as attended
- build VVISFKit
- Enjoy



## Tackle visual regressions
Updating library can cause visual regressions that are not detectable with unit tests. Look for IsfMetalVisualRegressionTestPack2024 for visual testing (not in this repo).


### Other issues
if error on "MVKImage.mm" file, comment line `_swapchain->recordPresentTime(presentTimingInfo, drawable.presentedTime * 1.0e9);`


### Contact
MTO
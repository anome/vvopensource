# Metal ISF

Metal ISF uses a fork of MoltenVK for shader conversion. For a quick try out, a CLI binary is included in the project. However, better performance is achieved by re-building MoltenVK libs on your machine.

## Quick setup (low performance)

__NO LONGER SUPPORTED - Requires Update__
Open vvopensource project and remove the missing files in VVISFKit/METAL/Converter
Comment the line `#define USE_MOLTENVK_LIB` from the file `MISFShaderConverter.mm`


## Standard setup (high performance)

MoltenVK is added in the folder `external` as a git submodule

After cloning, make sure to fetch the submodule with :
`git submodule init `
`git submodule update`
    

Then :
`cd external/MoltenVK/`
`brew install cmake python3 ninja`
`./fetchDependencies --macos`

Note : the fetchDependencies script gets the LATEST version of glsc and other tools. __It may cause regressions over time !!__. To have a consistent build, run those commands (v1.1.0)
```
cd External/
cd SPIRV-Cross/
git checkout 762c3082
cd ../Volk/
git checkout 01986ac85fa2e5c70df09aeae9c907e27c5d50b2
cd ../Vulkan-Headers
git checkout 87aaa16d4c8e1ac70f8f04acdcd46eed4bd77209
cd ../Vulkan-Tools
git checkout 0387f633882df56347c368149759b825ec55fd71
cd ../VulkanSamples
git checkout 753fc69bac132e58da6d8864a9930cc587fde60a
cd ../cereal/
git checkout 51cbda5f30e56c801c07fe3d3aba5d7fb9e6cca4
cd ../glslang/
git checkout c594de23cdd790d64ad5f9c8b059baae0ee2941d
cd ../../
```

Then (this is to fix a bug discovered March 2024 -- hacky workaround for LM_HEAT.fs) : 
- Open up external/MoltenVK/ExternalDependencies.xcodeproj
- look for this line "replace_illegal_names(keywords);" and comment it

Then :
`./buildWithoutFetching --macos`

Lastly :
- open MoltenVKPackaging.xcodeproj
- build "MoltenVK Package (macOS only)"
- build "MoltenVKShaderConverter-macOS"
- Run VVISFKitTest Metal Regression and _make sure you have_ these results (v1.1.0) : Tests 9302 / Passed 8168 / Failed 1134
- To avoid Millumin regressions : Make sure the tests are like this
![Tests v1.1](./tests_v1.1.png)
- (Good practice) Test LM_HEAT with metalTestApp to make sure the replace_illegal_names() change works as attended
- build VVISFKit
- Enjoy


## Tackle visual regressions
Updating library can cause visual regressions that are not detectable with unit tests. Look for IsfMetalVisualRegressionTestPack2024 for visual testing (not in this repo).


### Other issues
if error on "MVKImage.mm" file, comment line `_swapchain->recordPresentTime(presentTimingInfo, drawable.presentedTime * 1.0e9);`

### Contact
MTO
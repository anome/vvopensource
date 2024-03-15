# Metal ISF

Metal ISF uses a fork of MoltenVK for shader conversion. For a quick try out, a CLI binary is included in the project. However, better performance is achieved by re-building MoltenVK libs on your machine.

## Quick setup (low performance)

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

Note : the fetchDependencies script gets the LATEST version of glsc and other tools. __It may cause regressions over time !!__

Then (this is to fix a bug discovered March 2023 -- hacky workaround) : 
- Open up external/MoltenVK/ExternalDependencies.xcodeproj
- look for this line "replace_illegal_names(keywords);" and comment it

Then :
`./buildWithoutFetching --macos`

Lastly :
- open MoltenVKPackaging.xcodeproj
- build "MoltenVK Package (macOS only)"
- build "MoltenVKShaderConverter-macOS"
- Run VVISFKitTest Metal Regression and expect these results (as of March 2023) : Passed 8165 / Failed 1134
- build VVISFKit

(if error on "MVKImage.mm" file, comment line with "_swapchain->recordPresentTime(presentTimingInfo, drawable.presentedTime * 1.0e9);")
```

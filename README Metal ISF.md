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

[Docs on submodules](https://git-scm.com/book/en/v2/Git-Tools-Submodules)

Then, go to external/MoltenVK, fetch dependencies for **macos**, and build the MoltenVK Package and ShaderConversionTool package for macos.


Snippet of MoltenVK readme for fetching dependencies :

1. Ensure you have `cmake` and `python3` installed:

		brew install cmake
		brew install python3

   For faster dependency builds, you can also optionally install `ninja`:

		brew install ninja

2. Clone the `MoltenVK` repository:

		git clone https://github.com/KhronosGroup/MoltenVK.git

3. Retrieve and build the external libraries:

		cd MoltenVK
		./fetchDependencies [platform...]

When running the `fetchDependencies` script, you must specify one or more platforms
for which to build the external libraries. The platform choices include:

	--all
	--macos
	--ios
	--iossim
	--maccat
	--tvos
	--tvossim
```
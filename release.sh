#!/bin/bash
rm -rf lib/

# Build for the wanted platforms and place it in releaseTemp
RELEASE_TARGETS=("win-x64" "osx-x64")
for target in "${RELEASE_TARGETS[@]}"
do
    dotnet publish CLI -c Release --runtime "$target" /p:PublishSingleFile=true -o releaseTemp
done

# Move from releaseTemp to lib, this way unneeded files (such as .pdb files) are omitted from the release
# Because dotnet Publish cannot give specific file, linux is seperate. Both output "CLI" executable.
dotnet publish CLI -c Release --runtime linux-musl-x64 /p:PublishSingleFile=true -o releaseLinux
mkdir lib
mv releaseTemp/CLI lib/ckc
mv releaseTemp/CLI.exe lib/ckc.exe
mv releaseLinux/CLI lib/ckcl
rm -rf releaseTemp/ releaseLinux
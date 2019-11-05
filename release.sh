#!/bin/bash
rm -rf lib/
# Build for the wanted platforms and place it in releaseTemp
RELEASE_TARGETS=("win-x64" "osx-x64")
for target in "${RELEASE_TARGETS[@]}"
do
    dotnet publish CLI -c Release --runtime "$target" /p:PublishSingleFile=true -o releaseTemp
done
# Move from releaseTemp to lib, this way unneeded files (such as .pdb files) are omitted from the release
mkdir lib
mv releaseTemp/CLI lib/ckc
mv releaseTemp/CLI.exe lib/ckc.exe
rm -rf releaseTemp/
# Get the previous tag from git history
PREVIOUSTAG=$(git describe --abbrev=0 --tags $(git rev-list --tags --skip=1 --max-count=1))
# Get the newest tag through parameters (see .travis.yml)
NEWTAG=$1
# Loop through files where the old version has to be replaced with the new version
# Technically, the paths to csproj files could be read out from the .sln, but this would make adding custom files (such as the README) more difficult
FILES=("ZDragon.NET.sln" "CLI/CLI.csproj" "Compiler/Compiler.csproj" "CompilerTests/CompilerTests.csproj" "Mapper.xsd/Mapper.xsd.csproj")
for FILE in "${FILES[@]}"
do
    sed -i '' -e "s/$PREVIOUSTAG/$NEWTAG/g" $FILE
done
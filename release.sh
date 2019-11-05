#!/bin/bash
rm -rf lib/
RELEASE_TARGETS=("win-x64" "osx-x64")
for target in "${RELEASE_TARGETS[@]}"
do
    dotnet publish CLI -c Release --runtime "$target" /p:PublishSingleFile=true -o releaseTemp
done
mkdir lib
mv releaseTemp/CLI lib/ckc
mv releaseTemp/CLI.exe lib/ckc.exe
rm -rf releaseTemp/
PREVIOUSTAG=$(git describe --abbrev=0 --tags)
NEWTAG=$1
# TODO change all project versions at the same time
FILES=("ZDragon.NET.sln" "CLI/CLI.csproj" "Compiler/Compiler.csproj" "CompilerTests/CompilerTests.csproj" "Mapper.xsd/Mapper.xsd.csproj")
for FILE in "${FILES[@]}"
do
    sed -i '' -e "s/$PREVIOUSTAG/$NEWTAG/g" $FILE
done
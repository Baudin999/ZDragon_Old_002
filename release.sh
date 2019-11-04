#!/bin/bash
rm -rf lib/*
RELEASE_TARGETS=("win-x64" "osx-x64")
for target in "${RELEASE_TARGETS[@]}"
do
    dotnet publish CLI -c Release --runtime "$target" /p:PublishSingleFile=true -o releaseTemp
done
mv temp/CLI lib/ckc
mv temp/CLI.exe lib/ckc.exe
rm -rf temp/
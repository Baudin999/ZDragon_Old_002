#!/bin/bash
rm -rf lib/*
RELEASE_TARGETS=("win-x64" "osx-x64")
for target in "${RELEASE_TARGETS[@]}"
do
    dotnet publish CLI -c Release --runtime "$target" /p:PublishSingleFile=true -o releaseTemp
done
mv releaseTemp/CLI lib/ckc
mv releaseTemp/CLI.exe lib/ckc.exe
rm -rf releaseTemp/
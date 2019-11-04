#!/bin/bash
RELEASE_TARGETS=("win-x64" "osx-x64")
for target in "${RELEASE_TARGETS[@]}"
do
    dotnet publish CLI -c Release --runtime "$target" /p:PublishSingleFile=true
done
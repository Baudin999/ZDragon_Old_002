#!/bin/bash -i

# Default bin location for mac
if [ -z ${ZDRAGON_PATH} ]; then
    read -p "Please enter your ZDragon.NET source path: `echo $'\n'`" -e
    if [ ! $(ls -a $REPLY | grep .sln) ]; then
        echo "This is not a valid dotnet project"
        exit 1;
    fi
    echo -e "export ZDRAGON_PATH=$REPLY\n" >> $HOME/.bash_profile
fi

OUTPUT_DIR="${1:-/usr/local/bin/}"
# Defaults to mac
OS="${2:-osx-x64}"
# If your zdragon source location differs from this, edit this variable.
CLI_PATH=${ZDRAGON_PATH}CLI
SOURCE_DIR="${3:-$CLI_PATH}"
APP_NAME="${4:-ckc}"
dotnet publish "$SOURCE_DIR" -c Release --runtime "$OS" /p:PublishSingleFile=true -o "$OUTPUT_DIR"
mv -v "$OUTPUT_DIR"CLI "$OUTPUT_DIR""$APP_NAME"
#!/bin/bash -i
if [ -z ${ZDRAGON_PATH} ]; then
    read -p "Please enter your ZDragon.NET source path: `echo $'\n'`" -e
    if [ ! $(ls -a $REPLY | grep .sln) ] || [ ! $(ls -a $REPLY | grep CLI) ] ; then
        echo "This is not a valid dotnet project, or it does not contain the ZDragon.NET project. Exiting."
        exit 1;
    fi
    PATH = $REPLY
    read -p "Would you like to add this path to your bash profile to be re-used in the next start? (y/n)" -n 1 -r
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo -e "export ZDRAGON_PATH=$PATH\n" >> $HOME/.bash_profile
        source ~/.bash_profile
        echo "Added $PATH to your bash profile."
    fi
fi

OUTPUT_DIR="${1:-/usr/local/bin/}"
# Defaults to mac
OS="${2:-osx-x64}"
# assumes that the path ends with /
CLI_PATH=${ZDRAGON_PATH}CLI
SOURCE_DIR="${3:-$CLI_PATH}"
APP_NAME="${4:-ckc}"
dotnet publish "$SOURCE_DIR" -c Release --runtime "$OS" /p:PublishSingleFile=true -o "$OUTPUT_DIR"
mv -v "$OUTPUT_DIR"CLI "$OUTPUT_DIR""$APP_NAME"

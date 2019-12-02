#!/bin/bash -i
RED='\033[0;31m'
NOCOLOR='\033[0m'
if [ -z ${ZDRAGON_PATH} ]; then
    read -p "Please enter your ZDragon.NET source path (folder with the dotnet solution): `echo $'\n'`" -e
    if [ ! $(ls -a $REPLY | grep .sln) ] || [ ! $(ls -a $REPLY | grep CLI) ] ; then
        echo "This is not a valid dotnet solution, or it does not contain the ZDragon.NET solution. Exiting."
        exit 1;
    fi
    ZDRAGON_PATH=$REPLY
    read -p "Would you like to add this path to your bash profile to be re-used in the next start? (y/n) `echo $'\n'`" -n 1 -r -e
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo -e "export ZDRAGON_PATH=$ZDRAGON_PATH\n" >> $HOME/.bash_profile
        echo -e "${RED}Please run 'source ~/.bash_profile'. This will prevent you from needing to re-do this the next run.${NOCOLOR}"
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
# The following variables are defaults, but can be overwritten as cli arguments, such as by running zdragon_release /usr/local/tempBinForDev
# This can be useful for one-time changes, but it is recommended to just edit the file if you want to do this often.

# Default bin location for mac
OUTPUT_DIR="${1:-/usr/local/bin/}"
# Defaults to mac
OS="${2:-osx-x64}"
# If your zdragon source location differs from this, edit this variable.
SOURCE_DIR="${3:-$HOME/projects/ZDragon.NET/CLI}"
APP_NAME="${4:-ckc}"
dotnet publish "$SOURCE_DIR" -c Release --runtime "$OS" /p:PublishSingleFile=true -o "$OUTPUT_DIR"
mv -v "$OUTPUT_DIR"CLI "$OUTPUT_DIR""$APP_NAME"
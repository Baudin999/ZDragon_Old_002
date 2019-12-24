
# Default bin location for mac
OUTPUT_DIR="${1:-/usr/local/bin/}"

# Defaults to mac
OS="${2:-osx-x64}"

# The name of the cli app
APP_NAME="${4:-ckc}"

# publish the app
dotnet publish ./CLI -c Release --runtime "$OS" /p:PublishSingleFile=true -o "$OUTPUT_DIR"

# rename
mv -v "$OUTPUT_DIR"CLI "$OUTPUT_DIR""$APP_NAME"
# Checkout first to get correct tag
 git checkout master 
 EXISTINGTAG="${2:-$(git describe --tag --abbrev=0)}"
 git fetch
 HEADHASH=$(git rev-parse HEAD)
 UPSTREAMHASH=$(git rev-parse master@{upstream})
 if [ -z "$(git status --porcelain)" ] && [ "$HEADHASH" == "$UPSTREAMHASH" ]; then 
     # clean working dir
     read -p "This will replace $EXISTINGTAG with "$1". Are you sure? (y/n)" -n 1 -r
     if [[ $REPLY =~ ^[Yy]$ ]]; then
         FILES=(
             "ZDragon.NET.sln" 
             "CLI/CLI.csproj" 
             "CLI/Program.cs"
             "Compiler/Compiler.csproj" 
             "Mapper.Application/Mapper.Application.csproj"
             "Mapper.XSD/Mapper.XSD.csproj" 
             "Mapper.HTML/Mapper.HTML.csproj" 
             "Mapper.JSON/Mapper.JSON.csproj"
             "README.md"
             "docs/index.md" 
             "docs/cli.md"
             "CompilerTests/CompilerTests.csproj" 
             "ApplicationTests/ApplicationTests.csproj" )

         for FILE in "${FILES[@]}"
         do
             sed -i '' "s/$EXISTINGTAG/$1/g" $FILE
         done
         git add .
         git commit -m "Release "$1""
         git tag "$1"
         git push origin master --tags 
     else
         echo "Exiting (no changes made)."
         exit 0
     fi
 else 
     # uncommited changes
     echo "You have uncommited or unpushed/pulled changes. Stash, commit or remove them. Exiting (no changes made)."
     exit 1
 fi 

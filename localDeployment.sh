NEWTAG=$1
#EXISTINGTAG=$(git describe --tag --abbrev=0)
EXISTINGTAG=$2
if [ -z "$(git status --porcelain)" ]; then 
	# clean working dir
	FILES=("ZDragon.NET.sln" "CLI/CLI.csproj" "Compiler/Compiler.csproj" "CompilerTests/CompilerTests.csproj" "Mapper.XSD/Mapper.XSD.csproj" "README.md"
	"Mapper.HTML/Mapper.HTML.csproj" "Mapper.JSON/Mapper.JSON.csproj"
	"CLI/Program.cs"
	)
	for FILE in "${FILES[@]}"
	do
		sed -i '' -e "s/$EXISTINGTAG/$NEWTAG/g" $FILE
	done
else 
	# uncommited changes
	echo "You have uncommited changes. Stash, commit or remove them."
	exit 1
fi
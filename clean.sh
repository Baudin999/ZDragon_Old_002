# Clean the solution
rm -rf */bin
rm -rf */obj
cd web
npm run build
cd ..
dotnet clean
dotnet build
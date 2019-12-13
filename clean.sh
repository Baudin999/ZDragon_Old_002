#!/bin/bash

# Clean the solution
rm -rf */bin
rm -rf */obj

# Recompile the web ui
cd web && npm i && npm run build

# Clean solution and rebuild.
cd .. && dotnet clean && dotnet build


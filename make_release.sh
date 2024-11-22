rm -rf obj
rm -rf bin

dotnet build FurnitureMod.csproj -f net6.0 -c Release

zip -j 'FurnitureMod.1.0.0.zip' './bin/Release/net6.0/BetterFurniturePlacementMod.dll' './bin/Release/net6.0/BetterFurniturePlacementMod.cfg'

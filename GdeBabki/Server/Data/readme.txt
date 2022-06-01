dotnet tool install --global dotnet-ef

https://www.entityframeworktutorial.net/efcore/cli-commands-for-ef-core-migration.aspx

dotnet ef dbcontext list -p .\gdebabki\server\GdeBabki.Server.csproj
dotnet ef migrations add InitialMigration -p .\gdebabki\server\GdeBabki.Server.csproj -o Data\Migrations
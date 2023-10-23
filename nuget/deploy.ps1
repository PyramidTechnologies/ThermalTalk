#
# deploy.ps1
#

Remove-Item *.nupkg

dotnet pack ThermalTalk/ThermalTalk.csproj --configuration Release
dotnet pack ThermalTalk.Imaging/ThermalTalk.Imaging.csproj --configuration Release

dotnet nuget push *.nupkg -source https://www.nuget.org/api/v2/package
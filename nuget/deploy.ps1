#
# deploy.ps1
# Execute in solution directory.
#

$PackageFiles = Get-ChildItem -Filter *.nupkg -Recurse
foreach ($File in $PackageFiles) {
    Remove-Item $PackageFile.FullName
}

dotnet pack ThermalTalk/ThermalTalk.csproj --configuration Release
dotnet pack ThermalTalk.Imaging/ThermalTalk.Imaging.csproj --configuration Release

$PackageFiles = Get-ChildItem -Filter *.nupkg -Recurse
foreach ($File in $PackageFiles) {
    dotnet nuget push $File.FullName -source https://www.nuget.org/api/v2/package
}
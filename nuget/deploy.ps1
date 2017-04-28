#
# deploy.ps1
#

del *.npkg
nuget pack -sym ../ThermalTalk/ThermalTalk.csproj -Prop Configuration=Release
nuget pack -sym ../ThermalTalk.Imaging/ThermalTalk.Imaging.csproj -Prop Configuration=Release

nuget push *.nupkg -Source https://www.nuget.org/api/v2/package
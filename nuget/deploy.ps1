#
# deploy.ps1
#

del *.nupkg
nuget pack ../ThermalTalk/ThermalTalk.csproj -Prop Configuration=Release
nuget pack ../ThermalTalk.Imaging/ThermalTalk.Imaging.csproj -Prop Configuration=Release

nuget push *.nupkg -Source https://www.nuget.org/api/v2/package
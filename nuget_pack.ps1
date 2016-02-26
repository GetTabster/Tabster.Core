$path = $env:APPVEYOR_BUILD_FOLDER + "\Tabster.Core.nuspec"
nuget pack $path -Version $env:APPVEYOR_BUILD_VERSION
param (
    [string]$sourceName = "Ally.RabbitMq",
    [string]$sourceUrl = "https://nexus.devops.allytransport.com.tw/repository/nuget-hosted/",
    [string]$apiKey = "a0c3a0c9-fb49-3975-819f-698b3fec4950",
    [string]$version = "0.0.3-NickTest"
)

$specPath = "..\$sourceName.sln"
$packagePath = "nupkg\$sourceName.$version.nupkg"

dotnet pack $specPath -o .\nupkg -c Release -p:PackageVersion=$version

dotnet nuget push -s $sourceUrl -k $apiKey $packagePath

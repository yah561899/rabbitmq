#!/bin/bash


sourceUrl="https://nexus.devops.allytransport.com.tw/repository/nuget-hosted/"
apiKey="a0c3a0c9-fb49-3975-819f-698b3fec4950"

specPath="src/$Ally.CommonUtils.sln"
packagePath="nupkg/$Ally.CommonUtils.99.99.nupkg"

dotnet pack $specPath -o ./nupkg -c Release -p:PackageVersion=99.99

dotnet nuget push -s $sourceUrl -k $apiKey $packagePath

if [[ -z "${NUGET_API_KEY}" ]]; then
  echo NUGET_API_KEY variable is required
  exit 1
fi

dotnet pack -c Release
dotnet nuget push src/Webpack.AspNetCore/bin/Release/Webpack.AspNetCore.2.0.0-beta4.nupkg -k $NUGET_API_KEY -s https://api.nuget.org/v3/index.json

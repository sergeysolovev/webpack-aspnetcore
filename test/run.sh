dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $dir/Webpack.AspNetCore.Tests.Integration
npm install
npm run start &
dotnet restore && dotnet xunit
kill %1

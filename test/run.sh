dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# run the dev server
cd $dir/WebSites/BasicWebSite
npm install
npm run start &

# run the tests
cd $dir/Webpack.AspNetCore.Tests.Integration
dotnet restore && dotnet test

kill %1

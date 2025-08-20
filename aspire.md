https://learn.microsoft.com/en-us/dotnet/aspire/get-started/add-aspire-existing-app?tabs=unix&pivots=vscode

dotnet new aspire-apphost -o DemoApi.AppHost
dotnet new aspire-servicedefaults -o DemoApi.ServiceDefaults

dotnet sln add DemoApi.AppHost/DemoApi.AppHost.csproj
dotnet sln add DemoApi.ServiceDefaults/DemoApi.ServiceDefaults.csproj

dotnet add DemoApi.AppHost/DemoApi.AppHost.csproj reference DemoApi/DemoApi.csproj
dotnet add DemoApi/DemoApi.csproj reference DemoApi.ServiceDefaults/DemoApi.ServiceDefaults.csproj
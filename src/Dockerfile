FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY *.sln ./

COPY TaaS.Api.WebApi/*.csproj TaaS.Api.WebApi/
COPY TaaS.Common/*.csproj TaaS.Common/
COPY TaaS.Core.Domain/*.csproj TaaS.Core.Domain/
COPY TaaS.Core.Entity/*.csproj TaaS.Core.Entity/
COPY TaaS.Infrastructure.Contract/*.csproj TaaS.Infrastructure.Contract/
COPY TaaS.Infrastructure.Implementation/*.csproj TaaS.Infrastructure.Implementation/
COPY TaaS.Persistence/*.csproj TaaS.Persistence/
COPY TaaS.Persistence.Migration/*.csproj TaaS.Persistence.Migration/

RUN dotnet restore -r linux-musl-x64

COPY . .
WORKDIR /src/TaaS
RUN dotnet publish -c Release -o /app -r linux-musl-x64 --self-contained false --no-restore ../TaaS.sln 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "TaaS.Api.WebApi.dll"]

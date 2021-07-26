FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY *.sln ./

COPY Thankify.Api.WebApi/*.csproj Thankify.Api.WebApi/
COPY Thankify.Common/*.csproj Thankify.Common/
COPY Thankify.Core.Domain/*.csproj Thankify.Core.Domain/
COPY Thankify.Core.Entity/*.csproj Thankify.Core.Entity/
COPY Thankify.Infrastructure.Contract/*.csproj Thankify.Infrastructure.Contract/
COPY Thankify.Infrastructure.Implementation/*.csproj Thankify.Infrastructure.Implementation/
COPY Thankify.Persistence/*.csproj Thankify.Persistence/
COPY Thankify.Persistence.Migration/*.csproj Thankify.Persistence.Migration/

RUN dotnet restore -r linux-musl-x64

COPY . .
WORKDIR /src/Thankify
RUN dotnet publish -c Release -o /app -r linux-musl-x64 --self-contained false --no-restore ../Thankify.sln 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://*:5100
EXPOSE 5100

ENTRYPOINT ["dotnet", "Thankify.Api.WebApi.dll"]

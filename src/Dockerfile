FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src

COPY *.sln ./

COPY Thankifi.Api/*.csproj Thankifi.Api/
COPY Thankifi.Core.Application/*.csproj Thankifi.Core.Application/
COPY Thankifi.Core.Application.Entity/*.csproj Thankifi.Core.Application.Entity/
COPY Thankifi.Core.Domain/*.csproj Thankifi.Core.Domain/
COPY Thankifi.Core.Domain.Contract/*.csproj Thankifi.Core.Domain.Contract/
COPY Thankifi.Core.Entity/*.csproj Thankifi.Core.Entity/
COPY Thankifi.Persistence/*.csproj Thankifi.Persistence/
COPY Thankifi.Persistence.Migrations/*.csproj Thankifi.Persistence.Migrations/

COPY nuget.config nuget.config

ARG GITHUB_USER=GITHUB_USER
ARG GITHUB_TOKEN=GITHUB_TOKEN

RUN sed -i "s|</configuration>|<packageSourceCredentials><github><add key=\"Username\" value=\"${GITHUB_USER}\" /><add key=\"ClearTextPassword\" value=\"${GITHUB_TOKEN}\" /></github></packageSourceCredentials></configuration>|" nuget.config
RUN dotnet restore -r linux-musl-x64

COPY . .
WORKDIR /src/Thankifi
RUN dotnet publish -c Release -o /app -r linux-musl-x64 --self-contained false --no-restore ../Thankifi.sln 

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://*:5100
EXPOSE 5100

ENTRYPOINT ["dotnet", "Thankifi.Api.dll"]

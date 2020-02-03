FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS http://*:5100
EXPOSE 5100

FROM microsoft/dotnet:sdk as builder
ARG Configuration=Release
WORKDIR /src
COPY *.sln ./

COPY TaaS.Api.WebApi/*.csproj TaaS.Api.WebApi/
COPY TaaS.Common/*.csproj TaaS.Common/
COPY TaaS.Core.Domain/*.csproj TaaS.Domain/
COPY TaaS.Core.Entity/*.csproj TaaS.Domain.Entity/
COPY TaaS.Infrastructure.Contract/*.csproj TaaS.Infrastructure.Contract/
COPY TaaS.Infrastructure.Implementation/*.csproj TaaS.Infrastructure.Implementation/
COPY TaaS.Persistence/*.csproj TaaS.Persistence/
COPY TaaS.Persistence.Migration/*.csproj TaaS.Persistence.Migration/

RUN dotnet restore
COPY . .
WORKDIR /src/TaaS
RUN dotnet build -c $Configuration -o /app ../TaaS.sln 

FROM builder as publish
ARG Configuration=Release
RUN dotnet publish -c $Configuration -o /app ../TaaS.sln

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TaaS.Api.WebApi.dll"]

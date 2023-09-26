FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["ApiGateways/OcelotApiGateway/OcelotApiGateway.csproj", "ApiGateways/OcelotApiGateway/"]
RUN dotnet restore "ApiGateways/OcelotApiGateway/OcelotApiGateway.csproj"
COPY . .
WORKDIR "/src/ApiGateways/OcelotApiGateway"
RUN dotnet build "OcelotApiGateway.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "OcelotApiGateway.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OcelotApiGateway.dll"]

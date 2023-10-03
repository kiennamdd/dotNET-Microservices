FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 9000

ENV ASPNETCORE_URLS=http://+:80,http://+:9000

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Webs/ASPNET-MVC/ASPNET-MVC.csproj", "Webs/ASPNET-MVC/"]
RUN dotnet restore "Webs/ASPNET-MVC/ASPNET-MVC.csproj"
COPY . .
WORKDIR "/src/Webs/ASPNET-MVC"
RUN dotnet build "ASPNET-MVC.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ASPNET-MVC.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ASPNET-MVC.dll"]

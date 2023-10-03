FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8005

ENV ASPNETCORE_URLS=http://+:80,http://+:8005

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Services/Email/Email.API/Email.API.csproj", "Services/Email/Email.API/"]
RUN dotnet restore "Services/Email/Email.API/Email.API.csproj"
COPY . .
WORKDIR "/src/Services/Email/Email.API"
RUN dotnet build "Email.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Email.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Email.API.dll"]

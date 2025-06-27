# Imagen base para .NET
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia el csproj y restaura dependencias
COPY EncriptadoApi/EncriptadoApi.csproj EncriptadoApi/
RUN dotnet restore EncriptadoApi/EncriptadoApi.csproj

# Copia el resto del código
COPY . .

WORKDIR /src/EncriptadoApi
RUN dotnet publish EncriptadoApi.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EncriptadoApi.dll"]
 
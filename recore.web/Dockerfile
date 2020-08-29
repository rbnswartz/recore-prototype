#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["recore.web/recore.web.csproj", "recore.web/"]
COPY ["recore.db/recore.db.csproj", "recore.db/"]
RUN dotnet restore "recore.web/recore.web.csproj"
COPY . .
WORKDIR "/src/recore.web"
RUN dotnet build "recore.web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "recore.web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "recore.web.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["abs-insurance/abs-insurance.csproj", "abs-insurance/"]
COPY ["Repository/Repository.csproj", "Repository/Repository.csproj"]
COPY ["Contracts/Contracts.csproj", "Contracts/Contracts.csproj"]
COPY ["Entities/Entities.csproj", "Entities/Entities.csproj"]
COPY ["Shared/Shared.csproj", "Shared/Shared.csproj"]
COPY ["Services/Services.csproj", "Services/Services.csproj"]


RUN dotnet restore "abs-insurance/abs-insurance.csproj"
COPY . .
WORKDIR "/src/abs-insurance"
RUN dotnet build "abs-insurance.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "abs-insurance.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "abs-insurance.dll"]

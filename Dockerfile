FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CatDogBearMicroservice.csproj", "src/"]
RUN dotnet restore "src/CatDogBearMicroservice.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "src/CatDogBearMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/CatDogBearMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatDogBearMicroservice.dll"]
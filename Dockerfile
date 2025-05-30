FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MojiSpravy/MojiSpravy.csproj", "MojiSpravy/"]
RUN dotnet restore "MojiSpravy/MojiSpravy.csproj"
COPY . .
WORKDIR "/src/MojiSpravy"
RUN dotnet build "MojiSpravy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MojiSpravy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MojiSpravy.dll"] 
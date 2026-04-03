FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# csproj kopyala (cache için)
COPY CodeBeam.Website/CodeBeam.Website/CodeBeam.Website.csproj CodeBeam.Website/CodeBeam.Website/
COPY CodeBeam.Website/CodeBeam.Website.Client/CodeBeam.Website.Client.csproj CodeBeam.Website/CodeBeam.Website.Client/

# restore
RUN dotnet restore CodeBeam.Website/CodeBeam.Website/CodeBeam.Website.csproj

# tüm dosyaları kopyala
COPY . .

# publish
RUN dotnet publish CodeBeam.Website/CodeBeam.Website/CodeBeam.Website.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CodeBeam.Website.dll"]
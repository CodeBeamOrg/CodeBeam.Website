FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CodeBeam.Website/*.csproj CodeBeam.Website/
RUN dotnet restore CodeBeam.Website/CodeBeam.Website.csproj

COPY . .
RUN dotnet publish CodeBeam.Website/CodeBeam.Website.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CodeBeam.Website.dll"]
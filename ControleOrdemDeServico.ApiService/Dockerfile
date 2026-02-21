FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/Apis/OsService.ApiService/OsService.ApiService.csproj \
  -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
RUN mkdir -p /data/uploads

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "OsService.ApiService.dll"]
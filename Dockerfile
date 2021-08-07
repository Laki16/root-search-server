# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy everything and build app with restore
COPY ./* ./
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
# COPY --from=build /source/rootsearch.pfx ./
COPY --from=build /app ./
# ENTRYPOINT ["dotnet", "ApiServer.dll"]

# for heroku build
CMD ASPNETCORE_URLS=http://*:$PORT dotnet ApiServer.dll

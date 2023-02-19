FROM microsoft/mssql-server-windows-express
COPY ./create-database.sql .
ENV ACCEPT_EULA Y
ENV MSSQL_SA_PASSWORD Password123!
RUN sqlcmd -S mssql -i create-database.sql 

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["VotingSystem.csproj", "."]
RUN dotnet restore "VotingSystem.csproj"
COPY . .
RUN dotnet build "VotingSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VotingSystem.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT Docker
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VotingSystem.dll"]
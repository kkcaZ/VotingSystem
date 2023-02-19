# Inspired by:
# https://medium.com/bright-days/basic-docker-image-dockerfile-sql-server-with-custom-prefill-db-script-8f12f197867a
FROM mcr.microsoft.com/mssql/server:2017-latest
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=Password123!
ENV MSSQL_TCP_PORT=1433 
WORKDIR /src 
COPY ./create-database.sql ./create-database.sql
RUN (/opt/mssql/bin/sqlservr --accept-eula & ) &&  /opt/mssql-tools/bin/sqlcmd -S 127.0.0.1 -U SA -P Password123! -i create-database.sql
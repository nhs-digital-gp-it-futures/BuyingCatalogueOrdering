FROM mcr.microsoft.com/mssql/server:2019-latest

WORKDIR /var/opt/mssql/config

COPY /ShellScripts .
COPY /Deployment sql/Deployment
COPY /Indexes sql/Indexes
COPY /Tables sql/Tables
COPY /Security sql/Security

ENTRYPOINT ["/bin/bash", "entrypoint.sh"]

CMD ["tail -f /dev/null"]

HEALTHCHECK --interval=15s CMD /opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -Q "SELECT 1;" && grep -q "MSSQL CONFIG COMPLETE" /var/opt/mssql/log/config.log

FROM mcr.microsoft.com/mssql/server:2019-latest

WORKDIR ~/config

COPY /ShellScripts .
COPY /Deployment sql/Deployment
COPY /Indexes sql/Indexes
COPY /Tables sql/Tables
COPY /Security sql/Security

ENTRYPOINT ["/bin/bash", "entrypoint.sh"]

CMD ["tail -f /dev/null"]

HEALTHCHECK --interval=15s CMD /opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -Q "select 1"

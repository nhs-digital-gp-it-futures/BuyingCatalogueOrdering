FROM mcr.microsoft.com/mssql/server:2019-latest

WORKDIR /usr/config

COPY /ShellScripts .
COPY /Deployment sql/Deployment
COPY /Indexes sql/Indexes
COPY /Tables sql/Tables
COPY /Security sql/Security

ENTRYPOINT ["./entrypoint.sh"]

CMD ["tail -f /dev/null"]

HEALTHCHECK --interval=15s CMD /opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -Q "select 1"

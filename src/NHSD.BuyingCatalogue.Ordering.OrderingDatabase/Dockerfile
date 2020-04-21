FROM microsoft/mssql-server-linux:latest

RUN mkdir -p /usr/config
WORKDIR /usr/config

COPY /ShellScripts .
COPY /Deployment sql/Deployment
COPY /Indexes sql/Indexes
COPY /Tables sql/Tables
COPY /Security sql/Security

RUN chmod +x /usr/config/entrypoint.sh
RUN chmod +x /usr/config/configure-db.sh

ENTRYPOINT ["./entrypoint.sh"]

CMD ["tail -f /dev/null"]

HEALTHCHECK --interval=15s CMD /opt/mssql-tools/bin/sqlcmd -U sa -P $SA_PASSWORD -Q "select 1" && grep -q "MSSQL CONFIG COMPLETE" ./config.log

#!/bin/bash

# wait for MSSQL server to start
export STATUS=1
i=0

while [[ $STATUS -ne 0 ]] && [[ $i -lt 30 ]]; do
	i=$i+1
	/opt/mssql-tools/bin/sqlcmd -t 1 -U sa -P $SA_PASSWORD -Q "select 1" >> /dev/null
	STATUS=$?
done

if [ $STATUS -ne 0 ]; then 
	echo "Error: MSSQL SERVER took more than thirty seconds to start up."
	exit 1
fi

echo "======= MSSQL SERVER STARTED ========" | tee -a ./config.log

# Run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -Q "CREATE DATABASE $DB_NAME;"
for file in ./sql/Security/Logins/*.sql; do /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i "$file"; done
for file in ./sql/Security/Users/*.sql; do /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d $DB_NAME -i "$file"; done

cd sql/Deployment
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d $DB_NAME -i "CreateTables.sql"
for file in ../Indexes/*.sql; do /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d $DB_NAME -I -i "$file"; done
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d $DB_NAME -I -i "PostDeployment.sql"

echo "======= MSSQL CONFIG COMPLETE =======" | tee -a ./config.log

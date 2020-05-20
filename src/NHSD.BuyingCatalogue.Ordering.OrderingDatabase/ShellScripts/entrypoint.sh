#!/bin/bash

echo "===== STARTED entrypoint script ======"

# Start SQL Server
/opt/mssql/bin/sqlservr &

# Start the script to create the DB and user
bash configure-db.sh

# Call extra command
eval $1

echo "===== FINISHED entrypoint script ======"

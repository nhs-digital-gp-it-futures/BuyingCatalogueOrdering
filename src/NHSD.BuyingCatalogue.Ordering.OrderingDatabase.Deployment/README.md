# BuyingCatalogueOrdering – `NHSD.BuyingCatalogueOrdering.OrderingDatabase.Deployment` Project

## Background

The SQL Server Database Project format (`*.sqlproj`) has not been ported to .NET Core yet, so it is only possible to build the `NHSD.BuyingCatalogueOrdering.OrderingDatabase` project on a Windows machine.

The `NHSD.BuyingCatalogueOrdering.OrderingDatabase.Deployment` project exists to generate the `.dacpac` file used to deploy the database to the containerized SQL Server instance and as a container for database deployment artefacts, currently only the Dockerfile and the entrypoint script for the deployment container.

## Limitations

The SDK used to generate the `.dacpac` does not currently support post-deployment scripts. Post-deployment actions are currently initiated by the [entrypoint.sh](#entrypoint.sh) script.

## Making Changes

Changes to the contents of this project should only be required when making changes to the database deployment process for the development and test environments. All database schema changes need to be made in the `NHSD.BuyingCatalogueOrdering.OrderingDatabase` project.

## Deployment Artefacts

### Dockerfile

The Dockerfile defines two images:

- `dacpacbuild` used to build the `.dacpac` file
- `dacfx` used to deploy the `.dacpac` to SQL Server & run post-deployment scripts

### Entrypoint.sh

The entrypoint script performs the following functions:

1. Checks that the SQL Server instance is available.
2. Initiates the DACPAC deployment using `sqlpackage`.
3. Executes the post-deployment script using `sqlcmd`.

## Notes

When running the database container or viewing its logs you may notice the following output that can be ignored.

### Login Failure

One or more occurences of:

```powershell
2020-05-07 11:26:47.55 Logon       Error: 18456, Severity: 14, State: 7.
2020-05-07 11:26:47.55 Logon       Login failed for user 'sa'. Reason: An error occurred while evaluating the password. [CLIENT: 172.24.0.4]
```

This is caused by a deployment attempt occurring before SQL Server has finished initalizing.

### 'Unnamed' Database Artefacts

```powershell
ordering_api_ordering_db_deploy                  | Creating <unnamed>...
ordering_api_ordering_db_deploy                  | Creating <unnamed>...
ordering_api_ordering_db_deploy                  | Creating <unnamed>...
```

This is the assignation of database roles/permissions.

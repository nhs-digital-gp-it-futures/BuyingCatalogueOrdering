# Buying Catalogue Ordering - Ordering service for the NHS Digital Buying Catalogue

## IMPORTANT NOTES

**You can use either the latest version of Visual Studio or .NET CLI for Windows, Mac and Linux**.

### Requirements

- .NET Core Version 3.1
- Docker

> Before you begin please install **.NET Core 3.1** & **Docker** on your machine.

## Overview

This application uses **.NET core** to provide an ordering API.

### Project structure

This repository uses **.NET Core**,  **Docker**.

It contains the following endpoints:

- health/live
  - Returns the health status of the service.

- health/ready
  - Returns the health status of the service dependacies  

The application is broken down into the following project libraries:

- API
  - Defines and exposes the available Buying Catalogue Ordering endpoints.
- API.UnitTests
  - Contains all of the unit tests for the API project.
- API.IntegrationTests
  - Contains all of the integration tests for the API project.
- Ordering.OrderingDatabase
  - Defines the artefacts for the Ordering database.

#### Database project

The database project is a SQL Server project, which is only fully supported by Visual Studio on Windows. However, some limited functionality should still be available in other editors.

When making changes to the database make sure to remove the Docker volume as described [below](#to-stop-the-application) before [running the application](#running-the-application).


##### How to connect

| From                       | Host                       | Port  | TLS |
|            :-:             |            :-:             |  :-:  | :-: |
| outside the docker network | localhost                  | 5104  |  X  |


Navigate yourself to [localhost:5104/health/live](http://localhost:5104health/live) to view the health status of the service 

## Running the Application

To start up the web application, run the following command from the root directory of the repository.

```shell
docker-compose up -d --build
```

This will start the application in a docker container. You can verify that the service has launched correctly by navigating to the following urls via any web browser.

```http
http://localhost:5104//health/live
http://localhost:5104//health/ready
```
If both URLs return 'Healthy', the environment is configured correctly, and can be accessed via the public endpoints.

If the ready URL returns 'Unhealthy', the associated dependencies of the application may have failed to launch, or cannot be accessed.

### To stop the application

To stop the application, run the following command from the root directory of the repository, which will stop the service within the docker container.

```shell
docker-compose down -v
```

### Running the Integration Tests

Start the application as described in [running the application](#running-the-application) and run the integration tests using your preferred test runner.

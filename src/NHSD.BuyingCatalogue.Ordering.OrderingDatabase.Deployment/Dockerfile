FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS dacpacbuild
WORKDIR /dacpac
COPY src/NHSD.BuyingCatalogue.Ordering.OrderingDatabase.Deployment/*.csproj .
COPY src/NHSD.BuyingCatalogue.Ordering.OrderingDatabase .
RUN rm -rf Deployment \
    && dotnet build "NHSD.BuyingCatalogue.Ordering.OrderingDatabase.Deployment.csproj" -c Release -o build

FROM mcr.microsoft.com/mssql-tools:latest AS dacfx
RUN apt-get update && apt-get install libunwind8 libicu-dev wget unzip -y
WORKDIR /db-deploy
RUN wget https://go.microsoft.com/fwlink/?linkid=2128144 -O sqlpackage.zip \
    && mkdir sqlpackage \
    && unzip sqlpackage.zip -d /sqlpackage \
    && chmod a+x /sqlpackage/sqlpackage
COPY --from=dacpacbuild /dacpac/build/*.dacpac .
COPY src/NHSD.BuyingCatalogue.Ordering.OrderingDatabase.Deployment/entrypoint.sh .
COPY src/NHSD.BuyingCatalogue.Ordering.OrderingDatabase/Deployment .
ENTRYPOINT ["/bin/bash", "entrypoint.sh"]

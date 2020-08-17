FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/NHSD.BuyingCatalogue.Ordering.Api/NHSD.BuyingCatalogue.Ordering.Api.csproj", "src/NHSD.BuyingCatalogue.Ordering.Api/"]
COPY ["src/NHSD.BuyingCatalogue.Ordering.Common/NHSD.BuyingCatalogue.Ordering.Common.csproj", "src/NHSD.BuyingCatalogue.Ordering.Common/"]
COPY ["src/NHSD.BuyingCatalogue.Ordering.Application/NHSD.BuyingCatalogue.Ordering.Application.csproj", "src/NHSD.BuyingCatalogue.Ordering.Application/"]
COPY ["src/NHSD.BuyingCatalogue.Ordering.Domain/NHSD.BuyingCatalogue.Ordering.Domain.csproj", "src/NHSD.BuyingCatalogue.Ordering.Domain/"]
COPY ["src/NHSD.BuyingCatalogue.Ordering.Persistence/NHSD.BuyingCatalogue.Ordering.Persistence.csproj", "src/NHSD.BuyingCatalogue.Ordering.Persistence/"]
COPY .nuget/NuGet.config ./
RUN dotnet restore "src/NHSD.BuyingCatalogue.Ordering.Api/NHSD.BuyingCatalogue.Ordering.Api.csproj"
COPY . .
WORKDIR "/src/src/NHSD.BuyingCatalogue.Ordering.Api"
RUN dotnet build "NHSD.BuyingCatalogue.Ordering.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSD.BuyingCatalogue.Ordering.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSD.BuyingCatalogue.Ordering.Api.dll"]
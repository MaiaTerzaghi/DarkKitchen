# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0.100-bookworm-slim AS build
WORKDIR /src

COPY DarkKitchen.sln ./
COPY DarkKitchen.Domain/DarkKitchen.Domain.csproj                                     DarkKitchen.Domain/
COPY DarkKitchen.DTOs/DarkKitchen.DTOs.csproj                                         DarkKitchen.DTOs/
COPY DarkKitchen.IBusinessLogic/DarkKitchen.IBusinessLogic.csproj                     DarkKitchen.IBusinessLogic/
COPY DarkKitchen.IDataAccess/DarkKitchen.IDataAccess.csproj                           DarkKitchen.IDataAccess/
COPY DarkKitchen.BusinessLogic/DarkKitchen.BusinessLogic.csproj                       DarkKitchen.BusinessLogic/
COPY DarkKitchen.DataAccess/DarkKitchen.DataAccess.csproj                             DarkKitchen.DataAccess/
COPY DarkKitchen.ServiceFactory/DarkKitchen.ServiceFactory.csproj                     DarkKitchen.ServiceFactory/
COPY DarkKitchen.Importers.Contracts/DarkKitchen.Importers.Contracts.csproj           DarkKitchen.Importers.Contracts/
COPY DarkKitchen.Importers.Json/DarkKitchen.Importers.Json.csproj                     DarkKitchen.Importers.Json/
COPY DarkKitchen.Importers.Xml/DarkKitchen.Importers.Xml.csproj                       DarkKitchen.Importers.Xml/
COPY DarkKitchen.Importers.Loader/DarkKitchen.Importers.Loader.csproj                 DarkKitchen.Importers.Loader/
COPY DarkKitchen.WebApi/DarkKitchen.WebApi.csproj                                     DarkKitchen.WebApi/
COPY Tests/DarkKitchen.BusinessLogicTest/DarkKitchen.BusinessLogicTest.csproj          Tests/DarkKitchen.BusinessLogicTest/
COPY Tests/DarkKitchen.DataAccess.Test/DarkKitchen.DataAccess.Test.csproj              Tests/DarkKitchen.DataAccess.Test/
COPY Tests/DarkKitchen.WebApi.Test/DarkKitchen.WebApi.Test.csproj                      Tests/DarkKitchen.WebApi.Test/
COPY Tests/DarkKitchen.Importers.Json.Test/DarkKitchen.Importers.Json.Test.csproj      Tests/DarkKitchen.Importers.Json.Test/
COPY Tests/DarkKitchen.Importers.Xml.Test/DarkKitchen.Importers.Xml.Test.csproj        Tests/DarkKitchen.Importers.Xml.Test/

RUN dotnet restore DarkKitchen.WebApi/DarkKitchen.WebApi.csproj

COPY . .
RUN dotnet publish DarkKitchen.WebApi/DarkKitchen.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:5222
EXPOSE 5222

ENTRYPOINT ["dotnet", "DarkKitchen.WebApi.dll"]

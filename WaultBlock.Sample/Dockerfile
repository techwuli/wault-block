FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY WaultBlock.sln ./
COPY WaultBlock.Sample/WaultBlock.Sample.csproj WaultBlock.Sample/
COPY WaultBlock.Identities/WaultBlock.Identities.csproj WaultBlock.Identities/
COPY indy-sdk-dotnet/indy-sdk-dotnet.csproj indy-sdk-dotnet/
COPY WaultBlock.Utils/WaultBlock.Utils.csproj WaultBlock.Utils/
COPY WaultBlock.Models/WaultBlock.Models.csproj WaultBlock.Models/
COPY WaultBlock.Data/WaultBlock.Data.csproj WaultBlock.Data/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/WaultBlock.Sample
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WaultBlock.Sample.dll"]

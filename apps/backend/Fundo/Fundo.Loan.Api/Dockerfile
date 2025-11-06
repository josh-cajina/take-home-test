FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY Directory.Packages.props .
COPY Directory.Build.props .

COPY Fundo.Loan.Api/Fundo.Loan.Api.csproj Fundo.Loan.Api/
COPY Fundo.Loan.Domain/Fundo.Loan.Domain.csproj Fundo.Loan.Domain/
COPY Fundo.Loan.Application/Fundo.Loan.Application.csproj Fundo.Loan.Application/
COPY Fundo.Loan.Infrastructure/Fundo.Loan.Infrastructure.csproj Fundo.Loan.Infrastructure/

RUN dotnet restore Fundo.Loan.Api/Fundo.Loan.Api.csproj

COPY . .
WORKDIR /src/Fundo.Loan.Api
RUN dotnet build Fundo.Loan.Api.csproj -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Fundo.Loan.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
USER appuser
ENTRYPOINT ["dotnet", "Fundo.Loan.Api.dll"]
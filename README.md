# Logistics App by Alexander Cubillos Jauregui (git username: al3xjaur)

## Clean Architecture / Onion

Es un enfoque que organiza el código en capas concéntricas, separando la lógica de negocio del acceso a datos y la infraestructura. Las dependencias siempre apuntan hacia el núcleo, lo que facilita el mantenimiento, las pruebas y la escalabilidad del sistema.

## Diagrama de dependencias

```text
+-------------------+
|   Logistics.Web   |   <-- ASP.NET Core MVC + API
|-------------------|
| Controllers       |
| Views (Razor)     |
| DI wiring         |
+---------+---------+
          |
          v
+-----------------------+
| Logistics.Application |   <-- Interfaces, DTOs, lógica de negocio
|-----------------------|
| ITransporterService   |
| IShipmentStatusClient |
| Validators / DTOs     |
+---------+-------------+
          ^
          |
+--------------------------+
| Logistics.Infrastructure |   <-- Implementaciones técnicas
|--------------------------|
| TransporterService (EF)  |
| FakeStoreClient (Http)   |
| LogisticsDbContext       |
+---------+----------------+
          ^
          |
+-------------------+
|  Logistics.Domain |   <-- Entidades puras
|-------------------|
| Transporter       |
| ShipmentStatus    |
+-------------------+
```

### Lectura del diagrama

- **Domain:** el núcleo, sin dependencias.
- **Application:** define las abstracciones (interfaces) y los contratos de datos que necesita el dominio de negocio. Usa Domain.
- **Infrastructure:** implementa esas abstracciones definidas en 'Application', pero no define contratos nuevos. Usa Application y Domain. Cosas como EF Core, HttpClient, etc.
- **Web:** consume Application (a través de interfaces) y conecta con Infrastructure vía DI.

## Diagrama de despliegue en Azure

```text
                   +---------------------+
                   |   GitHub Repo       |
                   |  (source code)      |
                   +----------+----------+
                              |
                              | Push / PR
                              v
                   +---------------------+
                   | GitHub Actions CI/CD|
                   |  Build + Test +     |
                   |  Deploy             |
                   +----------+----------+
                              |
                              | Publish Profile / OIDC
                              v
+-------------------------------------------------------------+
|                     Azure Cloud                             |
|                                                             |
|   +-------------------+       +--------------------------+  |
|   | Azure Web App     |<----->| Azure AD B2C             |  |
|   | (Logistics.Web)   |       | (Authentication service) |  |
|   |-------------------|       +--------------------------+  |
|   | Runs MVC + API    |                                     |
|   | Protected by AAD  |                                     |
|   | Exposes:          |                                     |
|   |  - /Transporters  |                                     |
|   |  - /api/...       |                                     |
|   +---------+---------+                                     |
|             |                                               |
|             | EF Core (ADO.NET)                             |
|             v                                               |
|   +-------------------+                                     |
|   | Azure SQL Database|                                     |
|   | (Transporters)    |                                     |
|   +-------------------+                                     |
|                                                             |
|   +-------------------+                                     |
|   | External API      |                                     |
|   | (FakeStore API)   |                                     |
|   +-------------------+                                     |
|                                                             |
+-------------------------------------------------------------+

```

### Flujo de despliegue y ejecución

1. Desarrollo local → Push a GitHub.

2. GitHub Actions:

    - Restaura dependencias.
    - Compila y ejecuta tests.
    - Publica artefacto.
    - Despliega al Azure Web App usando Publish Profile o federated credentials.

3. Azure Web App:

    - Hospeda la aplicación ASP.NET Core MVC + API.
    - Usa Azure AD B2C para autenticar usuarios (MVC con OpenID Connect, API con JWT Bearer).
    - Se conecta a Azure SQL Database para persistencia de transportistas.
    - Consume FakeStore API (u otro servicio externo) para estados de envíos.

4. Usuarios finales:

    - Acceden vía navegador (MVC) o cliente HTTP (API).
    - Se autentican contra Azure AD B2C.
    - Interactúan con datos en Azure SQL y estados de envíos externos.

## GitHub Actions workflow (CI/CD)

Secrets requeridos:

- AZURE_WEBAPP_NAME: nombre del Azure Web App.

- AZURE_PUBLISH_PROFILE: contenido del Publish Profile XML del Web App.

- Azure App Settings: conexión DB y AzureAdB2C config (set en Azure portal).

## Migrations

### EF Core migrations

```bash
dotnet ef migrations add InitialCreate -p Logistics.Infrastructure -s Logistics.Web
dotnet ef database update -p Logistics.Infrastructure -s Logistics.Web
```

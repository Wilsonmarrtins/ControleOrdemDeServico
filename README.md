# OsService

Sistema simples para um prestador de serviços registrar **Clientes**, abrir **Ordens de Serviço**, acompanhar **Status**, registrar **Valor** e (opcionalmente) anexar **fotos antes/depois**.

> Esta solução foi construída com ASP.NET Core + MediatR (CQS) + Dapper + SQL Server e inclui testes **unitários** e **de integração** com xUnit.

---

## Sumário

- [Tecnologias](#tecnologias)
- [Como executar](#como-executar)
  - [Via Docker Compose (recomendado)](#via-docker-compose-recomendado)
  - [Local (Visual Studio / CLI)](#local-visual-studio--cli)
- [Banco de dados](#banco-de-dados)
  - [Tabelas](#tabelas)
  - [Criação automática de tabelas](#criação-automática-de-tabelas)
- [Endpoints](#endpoints)
- [Testes](#testes)

---

## Tecnologias

- .NET (API)
- MediatR (CQS)
- Dapper
- SQL Server 2022
- xUnit (unit + integration)

---

## Como executar

### Via Docker Compose (recomendado)

Pré-requisitos:
- Docker + Docker Compose

Suba **banco + API** com um único comando (na raiz do repositório):

```bash
docker compose up --build
```

Quando estiver online:

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

> Observação: o `docker-compose.yml` já cria a rede, sobe o SQL Server e só inicia a API depois que o banco estiver **healthy**.  
> A API recebe a connection string por variável de ambiente (`ConnectionStrings__DefaultConnection`). fileciteturn11file0L22-L35

#### Connection string (Docker)

Dentro do container da API, **não use `localhost`** para o banco.
O host correto é o nome do serviço do compose: `sqlserver`.

Exemplo recomendado (mesma senha do compose):

```text
Server=sqlserver,1433;Database=OsServiceDb;User Id=sa;Password=SqlServer2024!Strong#;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;
```

> No compose atual, o DB configurado está como `Database=OsService` (não `OsServiceDb`). Ajuste para manter consistente com seu `appsettings`. fileciteturn11file0L31-L33

#### Sobre o `docker-compose.override.yml`

Existe um `docker-compose.override.yml` que altera a porta para `5021`. fileciteturn11file2L1-L7  
Para avaliação, recomenda-se **não depender do override** e manter a porta padrão `8080`.

---

### Local (Visual Studio / CLI)

Pré-requisitos:
- SDK do .NET compatível com o TargetFramework do projeto
- SQL Server (local ou via Docker)

1) Suba o SQL Server localmente (ou via Docker) na porta 1433.

2) Configure `appsettings.Development.json` (exemplo):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OsServiceDb;User Id=sa;Password=SqlServer2024!Strong#;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;",
    "CreateTable": "Server=localhost,1433;Database=master;User Id=sa;Password=SqlServer2024!Strong#;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

3) Rode a API:
- Visual Studio: defina `OsService.ApiService` como Startup Project e pressione **F5**
- CLI:
```bash
dotnet run --project src/Apis/OsService.ApiService
```

---

## Banco de dados

### Tabelas

A aplicação utiliza (no mínimo) as tabelas abaixo:

#### `dbo.Customers`

- `Id` (PK, uniqueidentifier, not null)
- `Name` (nvarchar(150), not null)
- `Phone` (nvarchar(30), null)
- `Email` (nvarchar(120), null)
- `Document` (nvarchar(30), null)
- `CreatedAt` (datetime2(7), not null)

Índices:
- `IX_Customers_Phone` (Phone)
- `IX_Customers_Document` (Document)

#### `dbo.ServiceOrders`

- `Id` (PK, uniqueidentifier, not null)
- `Number` (identity, unique, inicia em 1000)
- `CustomerId` (FK → Customers.Id)
- `Description` (nvarchar(500), not null)
- `Status` (int, not null)
- `OpenedAt` (datetime2, not null)
- `Price` (decimal(18,2), null)
- `Coin` (varchar(4), null)
- `UpdatedPriceAt` (datetime, null)

Índices:
- `UX_ServiceOrders_Number` (unique)
- `IX_ServiceOrders_CustomerId`

#### `dbo.ServiceOrderAttachments` (opcional)

- `Id` (PK, uniqueidentifier, not null)
- `ServiceOrderId` (FK → ServiceOrders.Id)
- `Type` (int, not null) — ex.: Before/After
- `FileName` (nvarchar(255), not null)
- `ContentType` (nvarchar(100), not null)
- `SizeBytes` (bigint, not null)
- `StoragePath` (nvarchar(500), not null)
- `UploadedAt` (datetime2(7), not null)

> Se você habilitou attachments, o upload é salvo localmente em `/data/uploads` (ideal usar volume no Docker). fileciteturn11file7L18-L37

---

### Criação automática de tabelas

Existe um componente `DatabaseGenerantor` (Dapper) com SQL idempotente para criar tabelas se não existirem:

- Cria `dbo.Customers`
- Cria `dbo.ServiceOrders`

Exemplo (trecho):

```csharp
public class DatabaseGenerantor(IDefaultSqlConnectionFactory factory)
{
    public async Task CreateIfNotExistsAsync()
    {
        using var conn = factory.Create();
        await conn.ExecuteAsync(CreateTablesSql);
    }
}
```

#### Como garantir que as tabelas sejam criadas ao subir a API

Se a sua API **já** chama o `DatabaseGenerantor` no startup, não é necessário fazer nada.

Se ainda **não** estiver chamando, adicione no `Program.cs` (após `var app = builder.Build();`):

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseGenerantor>();
    await db.CreateIfNotExistsAsync();
}
```

Isso facilita o uso em avaliação (principalmente via Docker), pois o schema é criado automaticamente.

---

## Endpoints

### Clientes
- `POST /v1/customers`
- `GET /v1/customers/{id}`
- `GET /v1/customers/search?phone=...` (se implementado)
- `GET /v1/customers/search?document=...` (se implementado)

### Ordens de Serviço
- `POST /v1/service-orders`
- `GET /v1/service-orders/{id}`
- `PATCH /v1/service-orders/{id}/status`
- `PUT /v1/service-orders/{id}/price`

### Attachments (opcional)
- `POST /v1/service-orders/{id}/attachments/before`
- `POST /v1/service-orders/{id}/attachments/after`
- `GET /v1/service-orders/{id}/attachments`

> A lista completa e exemplos de payloads podem ser consultados no Swagger.

---

## Testes

Rodar todos os testes:

```bash
dotnet test
```

- Unit tests: `OsService.Tests`
- Integration tests: utilizam `WebApplicationFactory` e acessam a API via `HttpClient`

---

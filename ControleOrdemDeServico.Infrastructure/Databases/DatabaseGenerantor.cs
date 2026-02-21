using Dapper;
using System.Data;

namespace OsService.Infrastructure.Databases;

public sealed class DatabaseGenerantor(IDefaultSqlConnectionFactory factory)
{
    private const string CreateTablesSql = """
/* =========================
   Customers
   ========================= */
IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Phone NVARCHAR(30) NULL,
        Email NVARCHAR(120) NULL,
        Document NVARCHAR(30) NULL,
        CreatedAt DATETIME2(7) NOT NULL
    );

    CREATE INDEX IX_Customers_Phone ON dbo.Customers(Phone);
    CREATE INDEX IX_Customers_Document ON dbo.Customers(Document);
END;

/* =========================
   ServiceOrders
   ========================= */
IF OBJECT_ID(N'dbo.ServiceOrders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceOrders (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Number INT IDENTITY(1000, 1) NOT NULL,
        CustomerId UNIQUEIDENTIFIER NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        Status INT NOT NULL,
        OpenedAt DATETIME2(7) NOT NULL,
        Price DECIMAL(18, 2) NULL,
        Coin VARCHAR(4) NULL,
        UpdatedPriceAt DATETIME NULL,
        StartedAt DATETIME2(7) NULL,
        FinishedAt DATETIME2(7) NULL,
        CONSTRAINT FK_ServiceOrders_Customers
            FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id)
    );

    CREATE UNIQUE INDEX UX_ServiceOrders_Number ON dbo.ServiceOrders(Number);
    CREATE INDEX IX_ServiceOrders_CustomerId ON dbo.ServiceOrders(CustomerId);
END;

/* =========================
   ServiceOrderAttachments
   ========================= */
IF OBJECT_ID(N'dbo.ServiceOrderAttachments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ServiceOrderAttachments (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ServiceOrderId UNIQUEIDENTIFIER NOT NULL,
        Type INT NOT NULL,
        FileName NVARCHAR(255) NOT NULL,
        ContentType NVARCHAR(100) NOT NULL,
        SizeBytes BIGINT NOT NULL,
        StoragePath NVARCHAR(500) NOT NULL,
        UploadedAt DATETIME2(7) NOT NULL,
        CONSTRAINT FK_ServiceOrderAttachments_ServiceOrders
            FOREIGN KEY (ServiceOrderId) REFERENCES dbo.ServiceOrders(Id)
    );

    CREATE INDEX IX_ServiceOrderAttachments_ServiceOrderId
        ON dbo.ServiceOrderAttachments(ServiceOrderId);
END;

/* =========================
   Migrações leves (ALTER ADD)
   Mantém idempotente se tabelas já existirem
   ========================= */

/* Customers.CreatedAt precisão */
-- (sem ALTER aqui, porque alterar tipo pode ser perigoso em ambientes com dados)

/* ServiceOrders.StartedAt */
IF COL_LENGTH('dbo.ServiceOrders', 'StartedAt') IS NULL
BEGIN
    ALTER TABLE dbo.ServiceOrders ADD StartedAt DATETIME2(7) NULL;
END;

/* ServiceOrders.FinishedAt */
IF COL_LENGTH('dbo.ServiceOrders', 'FinishedAt') IS NULL
BEGIN
    ALTER TABLE dbo.ServiceOrders ADD FinishedAt DATETIME2(7) NULL;
END;

/* ServiceOrders.UpdatedPriceAt */
IF COL_LENGTH('dbo.ServiceOrders', 'UpdatedPriceAt') IS NULL
BEGIN
    ALTER TABLE dbo.ServiceOrders ADD UpdatedPriceAt DATETIME NULL;
END;
""";

    public async Task CreateIfNotExistsAsync()
    {
        using var conn = factory.Create();
        await conn.ExecuteAsync(CreateTablesSql);
    }
}
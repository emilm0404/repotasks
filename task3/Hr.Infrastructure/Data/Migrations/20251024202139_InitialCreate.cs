using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hr.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DECLARE @EmployeesId INT = OBJECT_ID(N'[dbo].[Employees]', 'U');

IF @EmployeesId IS NULL
BEGIN
    CREATE TABLE [dbo].[Employees]
    (
        [Id] INT NOT NULL IDENTITY(1, 1),
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [EmployeeNumber] NVARCHAR(32) NOT NULL,
        [CreatedAtUtc] DATETIME2 NOT NULL CONSTRAINT [DF_Employees_CreatedAtUtc] DEFAULT (SYSUTCDATETIME()),
        [UpdatedAtUtc] DATETIME2 NOT NULL CONSTRAINT [DF_Employees_UpdatedAtUtc] DEFAULT (SYSUTCDATETIME()),
        [RowVersion] ROWVERSION NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
    );

    SET @EmployeesId = OBJECT_ID(N'[dbo].[Employees]', 'U');
END;

IF COL_LENGTH('dbo.Employees', 'FirstName') IS NULL
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [FirstName] NVARCHAR(100) NULL;
END;

IF COL_LENGTH('dbo.Employees', 'LastName') IS NULL
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [LastName] NVARCHAR(100) NULL;
END;

IF COL_LENGTH('dbo.Employees', 'EmployeeNumber') IS NULL
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [EmployeeNumber] NVARCHAR(32) NULL;
END;

IF COL_LENGTH('dbo.Employees', 'CreatedAtUtc') IS NULL
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [CreatedAtUtc] DATETIME2 NULL;
END;

IF COL_LENGTH('dbo.Employees', 'UpdatedAtUtc') IS NULL
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [UpdatedAtUtc] DATETIME2 NULL;
END;

IF COL_LENGTH('dbo.Employees', 'RowVersion') IS NULL
BEGIN
    ALTER TABLE [dbo].[Employees] ADD [RowVersion] ROWVERSION NOT NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints kc
    WHERE kc.[type] = 'PK'
      AND kc.[parent_object_id] = OBJECT_ID(N'[dbo].[Employees]')
)
BEGIN
    ALTER TABLE [dbo].[Employees]
        ADD CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]);
END;

IF COLUMNPROPERTY(OBJECT_ID(N'[dbo].[Employees]'), 'Id', 'IsIdentity') <> 1
BEGIN
    THROW 50000, 'Employees.Id must be an identity column.', 1;
END;

IF COL_LENGTH('dbo.Employees', 'CreatedAtUtc') IS NOT NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM sys.default_constraints dc
        JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'[dbo].[Employees]')
          AND c.name = N'CreatedAtUtc'
    )
    BEGIN
        ALTER TABLE [dbo].[Employees]
            ADD CONSTRAINT [DF_Employees_CreatedAtUtc] DEFAULT (SYSUTCDATETIME()) FOR [CreatedAtUtc];
    END;

    UPDATE [dbo].[Employees]
    SET [CreatedAtUtc] = COALESCE([CreatedAtUtc], SYSUTCDATETIME());

    ALTER TABLE [dbo].[Employees]
        ALTER COLUMN [CreatedAtUtc] DATETIME2 NOT NULL;
END;

IF COL_LENGTH('dbo.Employees', 'UpdatedAtUtc') IS NOT NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM sys.default_constraints dc
        JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'[dbo].[Employees]')
          AND c.name = N'UpdatedAtUtc'
    )
    BEGIN
        ALTER TABLE [dbo].[Employees]
            ADD CONSTRAINT [DF_Employees_UpdatedAtUtc] DEFAULT (SYSUTCDATETIME()) FOR [UpdatedAtUtc];
    END;

    UPDATE e
    SET [UpdatedAtUtc] = COALESCE(e.[UpdatedAtUtc], e.[CreatedAtUtc], SYSUTCDATETIME())
    FROM [dbo].[Employees] e;

    ALTER TABLE [dbo].[Employees]
        ALTER COLUMN [UpdatedAtUtc] DATETIME2 NOT NULL;
END;

IF COL_LENGTH('dbo.Employees', 'FirstName') IS NOT NULL
BEGIN
    UPDATE [dbo].[Employees]
    SET [FirstName] = COALESCE(NULLIF(LTRIM(RTRIM([FirstName])), N''), N'Unknown');

    ALTER TABLE [dbo].[Employees]
        ALTER COLUMN [FirstName] NVARCHAR(100) NOT NULL;
END;

IF COL_LENGTH('dbo.Employees', 'LastName') IS NOT NULL
BEGIN
    UPDATE [dbo].[Employees]
    SET [LastName] = COALESCE(NULLIF(LTRIM(RTRIM([LastName])), N''), N'Unknown');

    ALTER TABLE [dbo].[Employees]
        ALTER COLUMN [LastName] NVARCHAR(100) NOT NULL;
END;

IF COL_LENGTH('dbo.Employees', 'EmployeeNumber') IS NOT NULL
BEGIN
    UPDATE e
    SET e.EmployeeNumber = CONCAT('E', RIGHT(REPLICATE('0', 6) + CAST(e.Id AS NVARCHAR(32)), 6))
    FROM [dbo].[Employees] e
    WHERE e.EmployeeNumber IS NULL OR LTRIM(RTRIM(e.EmployeeNumber)) = N'';

    UPDATE [dbo].[Employees]
    SET [EmployeeNumber] = UPPER(LTRIM(RTRIM([EmployeeNumber])));

    IF EXISTS (
        SELECT 1
        FROM [dbo].[Employees]
        WHERE LEN([EmployeeNumber]) > 32
    )
    BEGIN
        THROW 50002, 'EmployeeNumber values must be 32 characters or less.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM [dbo].[Employees]
        GROUP BY [EmployeeNumber]
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 50001, 'Duplicate EmployeeNumber values detected. Resolve duplicates before applying migrations.', 1;
    END;

    ALTER TABLE [dbo].[Employees]
        ALTER COLUMN [EmployeeNumber] NVARCHAR(32) NOT NULL;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.indexes i
        WHERE i.name = N'IX_Employees_EmployeeNumber'
          AND i.object_id = OBJECT_ID(N'[dbo].[Employees]')
    )
    BEGIN
        EXEC(N'CREATE UNIQUE INDEX [IX_Employees_EmployeeNumber] ON [dbo].[Employees] ([EmployeeNumber])');
    END;
END;
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
IF OBJECT_ID(N'[dbo].[Employees]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Employees];
END;
""");
        }
    }
}

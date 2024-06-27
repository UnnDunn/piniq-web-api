IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [CatalogSnapshots] (
        [Id] int NOT NULL IDENTITY,
        [Imported] datetime2 NOT NULL,
        [Published] datetime2 NULL,
        [MachineJsonResponse] nvarchar(max) NOT NULL,
        [MachineGroupJsonResponse] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Updated] datetime2 NOT NULL,
        CONSTRAINT [PK_CatalogSnapshots] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [OpdbChangelogs] (
        [Id] int NOT NULL IDENTITY,
        [OpdbId] nvarchar(max) NOT NULL,
        [NewOpdbId] nvarchar(max) NULL,
        [Action] int NOT NULL,
        [Date] datetime2 NOT NULL,
        CONSTRAINT [PK_OpdbChangelogs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [PinballFeatures] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_PinballFeatures] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [PinballKeywords] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_PinballKeywords] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [PinballMachineGroups] (
        [Id] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [ShortName] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_PinballMachineGroups] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [PinballManufacturers] (
        [Id] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NULL,
        CONSTRAINT [PK_PinballManufacturers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [PinballTypes] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_PinballTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [PinballMachines] (
        [Id] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
        [CommonName] nvarchar(max) NOT NULL,
        [IpdbId] int NULL,
        [ManufactureDate] datetime2 NULL,
        [ManufacturerId] int NULL,
        [PlayerCount] smallint NOT NULL,
        [TypeId] nvarchar(450) NULL,
        [MachineGroupId] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CS_AS NULL,
        CONSTRAINT [PK_PinballMachines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PinballMachines_PinballMachineGroups_MachineGroupId] FOREIGN KEY ([MachineGroupId]) REFERENCES [PinballMachineGroups] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_PinballMachines_PinballManufacturers_ManufacturerId] FOREIGN KEY ([ManufacturerId]) REFERENCES [PinballManufacturers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_PinballMachines_PinballTypes_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [PinballTypes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [PinballMachineFeatureMapping] (
        [FeatureId] uniqueidentifier NOT NULL,
        [MachineId] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
        CONSTRAINT [PK_PinballMachineFeatureMapping] PRIMARY KEY ([FeatureId], [MachineId]),
        CONSTRAINT [FK_PinballMachineFeatureMapping_PinballFeatures_FeatureId] FOREIGN KEY ([FeatureId]) REFERENCES [PinballFeatures] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PinballMachineFeatureMapping_PinballMachines_MachineId] FOREIGN KEY ([MachineId]) REFERENCES [PinballMachines] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE TABLE [PinballMachineKeywordMapping] (
        [KeywordId] uniqueidentifier NOT NULL,
        [MachineId] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
        CONSTRAINT [PK_PinballMachineKeywordMapping] PRIMARY KEY ([KeywordId], [MachineId]),
        CONSTRAINT [FK_PinballMachineKeywordMapping_PinballKeywords_KeywordId] FOREIGN KEY ([KeywordId]) REFERENCES [PinballKeywords] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PinballMachineKeywordMapping_PinballMachines_MachineId] FOREIGN KEY ([MachineId]) REFERENCES [PinballMachines] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[PinballTypes]'))
        SET IDENTITY_INSERT [PinballTypes] ON;
    EXEC(N'INSERT INTO [PinballTypes] ([Id], [Name])
    VALUES (N''ss'', N''Solid-State''),
    (N''em'', N''Electro-Mechanical''),
    (N''me'', N''Mechanical''),
    (N''dmd'', N''Dot-Matrix Display'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[PinballTypes]'))
        SET IDENTITY_INSERT [PinballTypes] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_PinballMachineFeatureMapping_MachineId] ON [PinballMachineFeatureMapping] ([MachineId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_PinballMachineKeywordMapping_KeywordId] ON [PinballMachineKeywordMapping] ([KeywordId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_PinballMachineKeywordMapping_MachineId] ON [PinballMachineKeywordMapping] ([MachineId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_PinballMachines_MachineGroupId] ON [PinballMachines] ([MachineGroupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_PinballMachines_ManufacturerId] ON [PinballMachines] ([ManufacturerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    CREATE INDEX [IX_PinballMachines_TypeId] ON [PinballMachines] ([TypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206220625_InitialMigration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210206220625_InitialMigration', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206235522_AddPinballMachineNameField'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PinballMachines]') AND [c].[name] = N'CommonName');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [PinballMachines] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [PinballMachines] ALTER COLUMN [CommonName] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206235522_AddPinballMachineNameField'
)
BEGIN
    ALTER TABLE [PinballMachines] ADD [Name] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20210206235522_AddPinballMachineNameField'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210206235522_AddPinballMachineNameField', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240112173916_TimedEntities'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OpdbChangelogs]') AND [c].[name] = N'Date');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [OpdbChangelogs] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [OpdbChangelogs] ALTER COLUMN [Date] datetimeoffset NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240112173916_TimedEntities'
)
BEGIN
    ALTER TABLE [OpdbChangelogs] ADD [Created] datetimeoffset NOT NULL DEFAULT (sysdatetimeoffset());
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240112173916_TimedEntities'
)
BEGIN
    ALTER TABLE [OpdbChangelogs] ADD [Updated] datetimeoffset NOT NULL DEFAULT (sysdatetimeoffset());
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240112173916_TimedEntities'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CatalogSnapshots]') AND [c].[name] = N'Updated');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [CatalogSnapshots] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [CatalogSnapshots] ALTER COLUMN [Updated] datetimeoffset NOT NULL;
    ALTER TABLE [CatalogSnapshots] ADD DEFAULT (sysdatetimeoffset()) FOR [Updated];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240112173916_TimedEntities'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CatalogSnapshots]') AND [c].[name] = N'Published');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [CatalogSnapshots] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [CatalogSnapshots] ALTER COLUMN [Published] datetimeoffset NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240112173916_TimedEntities'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CatalogSnapshots]') AND [c].[name] = N'Imported');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [CatalogSnapshots] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [CatalogSnapshots] ALTER COLUMN [Imported] datetimeoffset NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240112173916_TimedEntities'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CatalogSnapshots]') AND [c].[name] = N'Created');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [CatalogSnapshots] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [CatalogSnapshots] ALTER COLUMN [Created] datetimeoffset NOT NULL;
    ALTER TABLE [CatalogSnapshots] ADD DEFAULT (sysdatetimeoffset()) FOR [Created];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240112173916_TimedEntities'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240112173916_TimedEntities', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240420225943_OpdbCatalogSnapshotRefactor'
)
BEGIN
    ALTER TABLE [CatalogSnapshots] ADD [KeywordCount] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240420225943_OpdbCatalogSnapshotRefactor'
)
BEGIN
    ALTER TABLE [CatalogSnapshots] ADD [MachineCount] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240420225943_OpdbCatalogSnapshotRefactor'
)
BEGIN
    ALTER TABLE [CatalogSnapshots] ADD [MachineGroupCount] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240420225943_OpdbCatalogSnapshotRefactor'
)
BEGIN
    ALTER TABLE [CatalogSnapshots] ADD [MachineGroups] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240420225943_OpdbCatalogSnapshotRefactor'
)
BEGIN
    ALTER TABLE [CatalogSnapshots] ADD [Machines] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240420225943_OpdbCatalogSnapshotRefactor'
)
BEGIN
    ALTER TABLE [CatalogSnapshots] ADD [ManufacturerCount] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240420225943_OpdbCatalogSnapshotRefactor'
)
BEGIN
    ALTER TABLE [CatalogSnapshots] ADD [NewestMachine] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240420225943_OpdbCatalogSnapshotRefactor'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240420225943_OpdbCatalogSnapshotRefactor', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DROP TABLE [PinballMachineFeatureMapping];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DROP TABLE [PinballMachineKeywordMapping];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DROP TABLE [PinballFeatures];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DROP TABLE [PinballKeywords];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DROP TABLE [PinballMachines];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DROP TABLE [PinballMachineGroups];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DROP TABLE [PinballManufacturers];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DROP TABLE [PinballTypes];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OpdbChangelogs]') AND [c].[name] = N'OpdbId');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [OpdbChangelogs] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [OpdbChangelogs] ALTER COLUMN [OpdbId] nvarchar(18) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OpdbChangelogs]') AND [c].[name] = N'NewOpdbId');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [OpdbChangelogs] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [OpdbChangelogs] ALTER COLUMN [NewOpdbId] nvarchar(18) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    ALTER TABLE [OpdbChangelogs] ADD [CatalogId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223448_RemovePinballData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240624223448_RemovePinballData', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    IF SCHEMA_ID(N'pinball') IS NULL EXEC(N'CREATE SCHEMA [pinball];');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE TABLE [pinball].[PinballFeatures] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(256) NULL,
        CONSTRAINT [PK_PinballFeatures] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE TABLE [pinball].[PinballKeywords] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(36) NOT NULL,
        CONSTRAINT [PK_PinballKeywords] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE TABLE [pinball].[PinballMachineGroups] (
        [Id] int NOT NULL IDENTITY,
        [OpdbId] nvarchar(18) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [ShortName] nvarchar(50) NULL,
        [Description] nvarchar(1024) NULL,
        CONSTRAINT [PK_PinballMachineGroups] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE TABLE [pinball].[PinballManufacturers] (
        [Id] int NOT NULL,
        [Name] nvarchar(30) NOT NULL,
        [FullName] nvarchar(100) NULL,
        CONSTRAINT [PK_PinballManufacturers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE TABLE [pinball].[PinballTypes] (
        [Id] nvarchar(5) NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_PinballTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE TABLE [pinball].[PinballMachines] (
        [Id] int NOT NULL IDENTITY,
        [OpdbId] nvarchar(18) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [CommonName] nvarchar(100) NULL,
        [IpdbId] int NULL,
        [ManufactureDate] datetime2 NULL,
        [ManufacturerId] int NULL,
        [PlayerCount] smallint NOT NULL,
        [TypeId] nvarchar(5) NULL,
        [MachineGroupId] int NULL,
        CONSTRAINT [PK_PinballMachines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PinballMachines_PinballMachineGroups_MachineGroupId] FOREIGN KEY ([MachineGroupId]) REFERENCES [pinball].[PinballMachineGroups] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_PinballMachines_PinballManufacturers_ManufacturerId] FOREIGN KEY ([ManufacturerId]) REFERENCES [pinball].[PinballManufacturers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_PinballMachines_PinballTypes_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [pinball].[PinballTypes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE TABLE [pinball].[PinballMachineFeatureMapping] (
        [FeatureId] uniqueidentifier NOT NULL,
        [MachineId] int NOT NULL,
        CONSTRAINT [PK_PinballMachineFeatureMapping] PRIMARY KEY ([FeatureId], [MachineId]),
        CONSTRAINT [FK_PinballMachineFeatureMapping_PinballFeatures_FeatureId] FOREIGN KEY ([FeatureId]) REFERENCES [pinball].[PinballFeatures] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PinballMachineFeatureMapping_PinballMachines_MachineId] FOREIGN KEY ([MachineId]) REFERENCES [pinball].[PinballMachines] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE TABLE [pinball].[PinballMachineKeywordMapping] (
        [KeywordId] uniqueidentifier NOT NULL,
        [MachineId] int NOT NULL,
        CONSTRAINT [PK_PinballMachineKeywordMapping] PRIMARY KEY ([KeywordId], [MachineId]),
        CONSTRAINT [FK_PinballMachineKeywordMapping_PinballKeywords_KeywordId] FOREIGN KEY ([KeywordId]) REFERENCES [pinball].[PinballKeywords] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PinballMachineKeywordMapping_PinballMachines_MachineId] FOREIGN KEY ([MachineId]) REFERENCES [pinball].[PinballMachines] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[pinball].[PinballTypes]'))
        SET IDENTITY_INSERT [pinball].[PinballTypes] ON;
    EXEC(N'INSERT INTO [pinball].[PinballTypes] ([Id], [Name])
    VALUES (N''dmd'', N''Dot-Matrix Display''),
    (N''em'', N''Electro-Mechanical''),
    (N''me'', N''Mechanical''),
    (N''ss'', N''Solid-State'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[pinball].[PinballTypes]'))
        SET IDENTITY_INSERT [pinball].[PinballTypes] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE INDEX [IX_PinballMachineFeatureMapping_MachineId] ON [pinball].[PinballMachineFeatureMapping] ([MachineId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE INDEX [IX_PinballMachineKeywordMapping_KeywordId] ON [pinball].[PinballMachineKeywordMapping] ([KeywordId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE INDEX [IX_PinballMachineKeywordMapping_MachineId] ON [pinball].[PinballMachineKeywordMapping] ([MachineId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE INDEX [IX_PinballMachines_MachineGroupId] ON [pinball].[PinballMachines] ([MachineGroupId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE INDEX [IX_PinballMachines_ManufacturerId] ON [pinball].[PinballMachines] ([ManufacturerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    CREATE INDEX [IX_PinballMachines_TypeId] ON [pinball].[PinballMachines] ([TypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624223659_RestoreUpdatedPinballSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240624223659_RestoreUpdatedPinballSchema', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240626180607_AddCatalogChangelogs'
)
BEGIN
    CREATE TABLE [CatalogChangelogs] (
        [Id] int NOT NULL,
        [PinballMachines] nvarchar(max) NULL,
        [PinballMachineGroups] nvarchar(max) NULL,
        [PinballManufacturers] nvarchar(max) NULL,
        [Created] datetimeoffset NOT NULL DEFAULT (sysdatetimeoffset()),
        [Updated] datetimeoffset NOT NULL DEFAULT (sysdatetimeoffset()),
        CONSTRAINT [PK_CatalogChangelogs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240626180607_AddCatalogChangelogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240626180607_AddCatalogChangelogs', N'8.0.6');
END;
GO

COMMIT;
GO


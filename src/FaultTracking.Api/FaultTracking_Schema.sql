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

CREATE TABLE [Users] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FaultReports] (
    [Id] nvarchar(450) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Location] nvarchar(max) NOT NULL,
    [Priority] int NOT NULL,
    [Status] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_FaultReports] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FaultReports_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_FaultReports_UserId] ON [FaultReports] ([UserId]);
GO

CREATE INDEX [IX_FaultStatusLogs_ChangedById] ON [FaultStatusLogs] ([ChangedById]);
GO

CREATE INDEX [IX_FaultStatusLogs_FaultReportId] ON [FaultStatusLogs] ([FaultReportId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260330114453_InitialCreate', N'8.0.5');
GO

COMMIT;
GO


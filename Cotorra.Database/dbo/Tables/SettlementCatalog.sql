
CREATE TABLE [dbo].[SettlementCatalog] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (100)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [company]          UNIQUEIDENTIFIER NOT NULL,    
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,      

    [ValidityDate]     DATETIME NOT NULL,
    [Code]             INT NOT NULL,
    [Number]           INT NOT NULL DEFAULT(1),
    [CASUSMO]          DECIMAL(18,6) NOT NULL,
    [CASISR86]          DECIMAL(18,6) NOT NULL,
    [CalDirecPerc]      DECIMAL(18,6) NOT NULL,
    Indem90             DECIMAL(18,6) NOT NULL,
    Indem20          DECIMAL(18,6) NOT NULL,
    PrimaAntig          DECIMAL(18,6) NOT NULL,

    CONSTRAINT [PK_SettlementCatalog] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_SettlementCatalog_InstanceID]
    ON [dbo].[SettlementCatalog]([InstanceID], [Active]);
GO


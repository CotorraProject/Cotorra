CREATE TABLE [dbo].[AnualEmploymentSubsidy] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (100)   NOT NULL,
    [Description]      NVARCHAR (250)   NOT NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [company]          UNIQUEIDENTIFIER NOT NULL,
    [LowerLimit]	   DECIMAL(18, 6) NOT NULL, 
    [AnualSubsidy]   DECIMAL(18, 6) NOT NULL, 
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
    ValidityDate       DATETIME NOT NULL DEFAULT('2020-01-01'),

    CONSTRAINT [PK_AnualEmploymentSubsidy] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_AnualEmploymentSubsidy_InstanceID]
    ON [dbo].[AnualEmploymentSubsidy]([InstanceID], [Active]);
GO
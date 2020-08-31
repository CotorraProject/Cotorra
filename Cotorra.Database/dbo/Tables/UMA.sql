CREATE TABLE [dbo].[UMA] (
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
    [Value]			   DECIMAL(18, 6) NOT NULL, 
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
	[ValidityDate]	   DATETIME2 (7)    NOT NULL,
    CONSTRAINT [PK_UMAIncomeTax] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_UMA_InstanceID]
    ON [dbo].[UMA]([InstanceID], [Active]);
GO
--Calculation 
CREATE NONCLUSTERED INDEX [IX_UMA_Calculation_InstanceID]
    ON [dbo].[UMA]([InstanceID],[company], [Active]);
GO

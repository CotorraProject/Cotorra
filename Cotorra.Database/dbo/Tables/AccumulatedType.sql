
CREATE TABLE [dbo].[AccumulatedType] (
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

	TypeOfAccumulated int NOT NULL, 

    [Code] INT NULL, 
    CONSTRAINT [PK_AccumulatedType] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_AccumulatedType_InstanceID]
    ON [dbo].[AccumulatedType]([InstanceID], [Active]);
GO

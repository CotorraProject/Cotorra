CREATE TABLE [dbo].[JobPosition] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Active]       BIT              NOT NULL,
    [DeleteDate]   DATETIME2 (7)    NULL,
    [Timestamp]    DATETIME2 (7)    NOT NULL,
    [Name]         NVARCHAR (100)   NOT NULL,
    [Description]  NVARCHAR (250)   NOT NULL,
    [StatusID]     INT              NOT NULL,
    [CreationDate] DATETIME2 (7)    NOT NULL,
    [user]         UNIQUEIDENTIFIER NOT NULL,
    [company]      UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]   UNIQUEIDENTIFIER NOT NULL,
    [JobPositionRiskType] INT NOT NULL , 
    CONSTRAINT [PK_JobPosition] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_JobPosition_InstanceID]
    ON [dbo].[JobPosition]([InstanceID], [Active]);
GO


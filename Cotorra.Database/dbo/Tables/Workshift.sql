CREATE TABLE [dbo].[Workshift] (
    [ID]                  UNIQUEIDENTIFIER NOT NULL,
    [Active]              BIT              NOT NULL,
    [DeleteDate]          DATETIME2 (7)    NULL,
    [Timestamp]           DATETIME2 (7)    NOT NULL,
    [Name]                NVARCHAR (100)   NOT NULL,
    [Description]         NVARCHAR (250)   NOT NULL,
    [StatusID]            INT              NOT NULL,
    [CreationDate]        DATETIME2 (7)    NOT NULL,
    [user]                UNIQUEIDENTIFIER NOT NULL,
    [company]             UNIQUEIDENTIFIER NOT NULL,
    [Hours]               FLOAT (53)       NOT NULL,
    [InstanceID]          UNIQUEIDENTIFIER NOT NULL,
    [ShiftWorkingDayType] INT              NOT NULL,
    CONSTRAINT [PK_Workshift] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_Workshift_InstanceID]
    ON [dbo].[Workshift]([InstanceID], [Active]);
GO

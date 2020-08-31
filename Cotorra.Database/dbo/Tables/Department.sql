CREATE TABLE [dbo].[Department] (
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
    [Number]           INT              NOT NULL,
    [BanksBeneficiary] NVARCHAR (MAX)   NULL,
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
    [AreaID] UNIQUEIDENTIFIER  NULL,	 
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Department_Area_AreaID] FOREIGN KEY ([AreaID]) REFERENCES [dbo].[Area] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_Department_InstanceID]
    ON [dbo].[Department]([InstanceID], [Active]);
GO


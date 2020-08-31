CREATE TABLE [dbo].[IMSSWorkRisk]
(
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
    [Value]	    DECIMAL(18, 6)   NOT NULL, 

    CONSTRAINT [PK_IMSSWorkRisk] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_IMSSWorkRisk_InstanceID]
    ON [dbo].[IMSSWorkRisk]([InstanceID], [Active]);
GO


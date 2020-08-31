CREATE TABLE [dbo].[CancelationFiscalDocument] (
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
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
    [CancelationRequestXMLID] UNIQUEIDENTIFIER NULL,
    [CancelationResponseXMLID] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_CancelationFiscalDocument] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_CancelationFiscalDocument_InstanceID]
    ON [dbo].[CancelationFiscalDocument]([InstanceID], [Active]);
GO


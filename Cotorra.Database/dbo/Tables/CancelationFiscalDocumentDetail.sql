CREATE TABLE [dbo].[CancelationFiscalDocumentDetail] (
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
    [CancelationFiscalDocumentID]   UNIQUEIDENTIFIER NOT NULL,
    [OverdraftID]      UNIQUEIDENTIFIER NOT NULL,
    [CancelationFiscalDocumentStatus]   INT NOT NULL,
    CONSTRAINT [PK_CancelationFiscalDocumentDetail] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CancelationFiscalDocumentDetail_CancelationFiscalDocument_CancelationFiscalDocumentID] FOREIGN KEY ([CancelationFiscalDocumentID]) REFERENCES [dbo].[CancelationFiscalDocument] ([ID]),
    CONSTRAINT [FK_CancelationFiscalDocumentDetail_Overdraft_OverdraftID] FOREIGN KEY ([OverdraftID]) REFERENCES [dbo].[Overdraft] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_CancelationFiscalDocumentDetail_InstanceID]
    ON [dbo].[CancelationFiscalDocumentDetail]([InstanceID], [Active]);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_CancelationFiscalDocumentDetail_OverdraftID]
    ON [dbo].[CancelationFiscalDocumentDetail]([OverdraftID]);
GO


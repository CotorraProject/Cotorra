CREATE TYPE [dbo].[cancelstampoverdrafttabletype] AS TABLE (
    [OverdraftID]                     UNIQUEIDENTIFIER NOT NULL,
    [CancelationFiscalDocumentStatus] INT              NOT NULL);


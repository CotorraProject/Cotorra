CREATE TABLE [dbo].[InfonavitInsurance] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [Value]			   DECIMAL(18, 6) NOT NULL, 
	[ValidityDate]	   DATETIME2 (7)    NOT NULL,
    CONSTRAINT [PK_InfonavitInsurance] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

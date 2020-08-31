CREATE TABLE [dbo].[NOMEvaluationGuide] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [Number]			INT              NOT NULL,
    CONSTRAINT [PK_NOMEvaluationGuide] PRIMARY KEY CLUSTERED ([ID] ASC)
	 
);
GO


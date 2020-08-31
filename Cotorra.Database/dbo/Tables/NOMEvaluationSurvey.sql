CREATE TABLE [dbo].[NOMEvaluationSurvey] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [NOMEvaluationGuideID] UNIQUEIDENTIFIER NOT NULL,	 
    [Number]			INT              NOT NULL,
    CONSTRAINT [PK_NOMEvaluationSurvey] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_NOMEvaluationSurvey_NOMEvaluationGuide_NOMEvaluationGuideID] FOREIGN KEY ([NOMEvaluationGuideID]) REFERENCES [dbo].[NOMEvaluationGuide] ([ID])
);
GO


CREATE TABLE [dbo].[NOMEvaluationQuestion] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [NOMEvaluationPhaseID] UNIQUEIDENTIFIER NOT NULL,	
    [Number]			INT              NOT NULL,
    [NOMEvaluationCategoryID] UNIQUEIDENTIFIER NULL, 
    [NOMEvaluationDomainID] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_NOMEvaluationQuestion] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_NOMEvaluationQuestion_NOMEvaluationPhase_EvaluationPhaseID] FOREIGN KEY ([NOMEvaluationPhaseID]) REFERENCES [dbo].[NOMEvaluationPhase] ([ID]),
    CONSTRAINT [FK_NOMEvaluationQuestion_NOMEvaluationCategory_NOMEvaluationCategoryID] FOREIGN KEY ([NOMEvaluationCategoryID]) REFERENCES [dbo].[NOMEvaluationCategory] ([ID]),
    CONSTRAINT [FK_NOMEvaluationQuestion_NOMEvaluationDomain_NOMEvaluationDomainID] FOREIGN KEY ([NOMEvaluationDomainID]) REFERENCES [dbo].[NOMEvaluationDomain] ([ID])
);
GO

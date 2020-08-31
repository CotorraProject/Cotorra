CREATE TABLE [dbo].[NOMAnswer] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL, 
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [company]          UNIQUEIDENTIFIER NOT NULL,    
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL, 
    [NOMSurveyReplyID] UNIQUEIDENTIFIER NOT NULL,	 
    [NOMEvaluationQuestionID] UNIQUEIDENTIFIER NOT NULL,	 
    [Value]			INT              NOT NULL,
    CONSTRAINT [PK_NOMAnswer] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_NOMAnswer_NOMSurveyReply_NOMSurveyReplyID] FOREIGN KEY ([NOMSurveyReplyID]) REFERENCES [dbo].[NOMSurveyReply] ([ID]),
	CONSTRAINT [FK_NOMAnswer_NOMEvaluationQuestion_NOMEvaluationQuestionID] FOREIGN KEY ([NOMEvaluationQuestionID]) REFERENCES [dbo].[NOMEvaluationQuestion] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_NOMAnswer_InstanceID]
    ON [dbo].[NOMAnswer]([InstanceID], [Active] ASC);
GO
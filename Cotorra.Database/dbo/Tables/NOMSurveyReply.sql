CREATE TABLE [dbo].[NOMSurveyReply] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [user]         UNIQUEIDENTIFIER NOT NULL,
    [company]      UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]   UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL, 
    [NOMEvaluationSurveyID] UNIQUEIDENTIFIER NOT NULL,	
    [NOMEvaluationPeriodID] UNIQUEIDENTIFIER NOT NULL,	
    [EmployeeID] UNIQUEIDENTIFIER NOT NULL,	
    [ResultType] int NOT NULL,	
    [Result]  int NOT NULL,		
    [EvaluationState]  int NOT NULL,	

    CONSTRAINT [PK_NOMSurveyReply] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_NOMSurveyReply_NOMSurveyReply_NOMEvaluationSurveyID] FOREIGN KEY ([NOMEvaluationSurveyID]) REFERENCES [dbo].[NOMEvaluationSurvey] ([ID]),
	CONSTRAINT [FK_NOMSurveyReply_NOMEvaluationPeriod_NOMEvaluationPeriodID] FOREIGN KEY ([NOMEvaluationPeriodID]) REFERENCES [dbo].[NOMEvaluationPeriod] ([ID]),
	CONSTRAINT [FK_NOMSurveyReply_Employee_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID])

);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_NOMSurveyReply_InstanceID]
    ON [dbo].[NOMSurveyReply]([InstanceID], [Active] ASC);
GO
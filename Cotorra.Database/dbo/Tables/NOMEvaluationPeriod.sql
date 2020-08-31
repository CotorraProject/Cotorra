CREATE TABLE [dbo].[NOMEvaluationPeriod] (
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
    [Period] nvarchar(200) NOT NULL, 
	[State] BIT NOT NULL, 
    [InitialDate]       DATETIME2 (7)    NULL,
    [FinalDate]       DATETIME2 (7)    NULL,

    CONSTRAINT [PK_NOMEvaluationPeriod] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO


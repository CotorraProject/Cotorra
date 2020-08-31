CREATE TABLE [dbo].[ConceptPayment] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (100)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [company]          UNIQUEIDENTIFIER NOT NULL,    
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,  
    Code INT NULL, 
    ConceptType INT NOT NULL,
	GlobalAutomatic BIT NULL, 
	AutomaticDismissal BIT NULL, 
	Kind BIT NULL, 
	[Print] BIT NULL, 
	SATGroupCode NVARCHAR(10) NULL, 
	[Label] nvarchar(200) NULL, 
    [Label1] NVARCHAR(500) NULL, 
    [Label2] NVARCHAR(500) NULL, 
    [Label3] NVARCHAR(500) NULL, 
    [Label4] NVARCHAR(500) NULL, 
    [Formula] NVARCHAR(500) NULL, 
    [FormulaValue] NVARCHAR(500) NULL, 
    [Formula1] NVARCHAR(500) NULL, 
    [Formula2] NVARCHAR(500) NULL, 
    [Formula3] NVARCHAR(500) NULL, 
    [Formula4] NVARCHAR(500) NULL, 
    CONSTRAINT [PK_ConceptPayment] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_ConceptPayment_InstanceID]
    ON [dbo].[ConceptPayment]([InstanceID], [Active]);
GO

--/*Mejora la consulta 81.428%*/
CREATE NONCLUSTERED INDEX [IX_ConceptPayment_Active_Company_Instance]
ON [dbo].[ConceptPayment] ([Active],[company],[InstanceID])
INCLUDE ([DeleteDate],[Timestamp],[Name],[Description],[StatusID],[CreationDate],[user],[Code],[ConceptType],[GlobalAutomatic],[AutomaticDismissal],[Kind],[Print],[SATGroupCode],[Label],[Label1],[Label2],[Label3],[Label4],[Formula],[Formula1],[Formula2],[Formula3],[Formula4])
GO
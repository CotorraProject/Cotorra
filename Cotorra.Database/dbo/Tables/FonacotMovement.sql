CREATE TABLE [dbo].[FonacotMovement]
(
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
    [EmployeeID]   UNIQUEIDENTIFIER NOT NULL,
    [ConceptPaymentID]  UNIQUEIDENTIFIER NOT NULL,
    [CreditNumber] NVARCHAR(100) NOT NULL,
    [Month]        INT NOT NULL,
    [Year]        INT NOT NULL,
    [RetentionType] INT NOT NULL,   
    [FonacotMovementStatus] INT NOT NULL,
    [Observations] NVARCHAR(4000) NULL,
    [EmployeeConceptsRelationID] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_FonacotMovement] PRIMARY KEY CLUSTERED ([ID] ASC), 
	CONSTRAINT [FK_FonacotMovement_Employee] FOREIGN KEY (EmployeeID) REFERENCES Employee([ID]),
    CONSTRAINT [FK_FonacotMovement_ConceptPayment] FOREIGN KEY (ConceptPaymentID) REFERENCES ConceptPayment([ID]),
    CONSTRAINT [FK_FonacotMovement_EmployeeConceptsRelation] FOREIGN KEY (EmployeeConceptsRelationID) REFERENCES EmployeeConceptsRelation([ID])
    );

GO


CREATE NONCLUSTERED INDEX [IX_FonacotMovement_InstanceID]
    ON [dbo].[FonacotMovement]([InstanceID], [Active]);
GO

--Calculation
CREATE NONCLUSTERED INDEX [IX_FonacotMovement_Calculation_InstanceID]
    ON [dbo].[FonacotMovement]([InstanceID], company, EmployeeID, FonacotMovementStatus);
GO
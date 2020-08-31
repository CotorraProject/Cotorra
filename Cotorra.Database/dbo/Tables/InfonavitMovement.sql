CREATE TABLE [dbo].[InfonavitMovement] (
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

    [EmployeeID]       UNIQUEIDENTIFIER NOT NULL,
    [CreditNumber]     NVARCHAR(40) NOT NULL,
    [InfonavitCreditType]  INT NOT NULL,
    [InitialApplicationDate]  DATETIME NOT NULL,
    [RegisterDate]  DATETIME NOT NULL,
    [MonthlyFactor]  DECIMAL(18,6) NOT NULL,
    [IncludeInsurancePayment_D14]  BIT NOT NULL,
    [AccumulatedAmount] DECIMAL(18,6) NOT NULL,
    [AppliedTimes]  INT NOT NULL,
    [InfonavitStatus]  BIT NOT NULL,
    [ConceptPaymentID]  UNIQUEIDENTIFIER NOT NULL,
    [EmployeeConceptsRelationID] UNIQUEIDENTIFIER NOT NULL,
    [EmployeeConceptsRelationInsuranceID] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_InfonavitMovement] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_InfonavitMovement_Employee_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID]),
    CONSTRAINT [FK_InfonavitMovement_ConceptPayment] FOREIGN KEY (ConceptPaymentID) REFERENCES ConceptPayment([ID]),
    CONSTRAINT [FK_InfonavitMovement_EmployeeConceptsRelation] FOREIGN KEY (EmployeeConceptsRelationID) REFERENCES EmployeeConceptsRelation([ID]),
    CONSTRAINT [FK_InfonavitMovement_EmployeeConceptsRelation_Insurance] FOREIGN KEY (EmployeeConceptsRelationInsuranceID) REFERENCES EmployeeConceptsRelation([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_InfonavitMovement_InstanceID]
    ON [dbo].[InfonavitMovement]([InstanceID], [Active]);
GO
--Infonavit for calculation 
CREATE NONCLUSTERED INDEX [IX_InfonavitMovement_Calculation_InstanceID]
    ON [dbo].[InfonavitMovement]([InstanceID], company,EmployeeID, InfonavitStatus, InitialApplicationDate, [Active]);
GO

CREATE TABLE [dbo].[EmployeeConceptsRelationDetail]
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
    [EmployeeConceptsRelationID] UNIQUEIDENTIFIER NOT NULL,    
    [PaymentDate] DATETIME NOT NULL,
    [OverdraftID] UNIQUEIDENTIFIER NOT NULL,    
    [ValueApplied] DECIMAL(18,6) NOT NULL,
    [AmountApplied] DECIMAL(18,6) NOT NULL,
    [ConceptsRelationPaymentStatus] INT NOT NULL,
    [IsAmountAppliedCapturedByUser] BIT NOT NULL DEFAULT(0),
    CONSTRAINT [PK_EmployeeConceptsRelationDetail] PRIMARY KEY CLUSTERED ([ID] ASC), 
	CONSTRAINT [FK_EmployeeConceptsRelationDetail_EmployeeConceptsRelation] FOREIGN KEY (EmployeeConceptsRelationID) REFERENCES EmployeeConceptsRelation([ID]),
    CONSTRAINT [FK_EmployeeConceptsRelationDetail_Overdraft] FOREIGN KEY (OverdraftID) REFERENCES Overdraft([ID])
);
GO

--For Calculation
CREATE NONCLUSTERED INDEX [IX_EmployeeConceptsRelationDetail_EmployeeConceptsRelationID_InstanceID]
    ON [dbo].[EmployeeConceptsRelationDetail]([InstanceID], [EmployeeConceptsRelationID], [Active]);
GO

--For Calculation
CREATE NONCLUSTERED INDEX [IX_EmployeeConceptsRelationDetail_OverdraftID_InstanceID]
    ON [dbo].[EmployeeConceptsRelationDetail]([InstanceID], [OverdraftID], [Active]);
GO

--Get All
CREATE NONCLUSTERED INDEX [IX_EmployeeConceptsRelationDetail_InstanceID]
    ON [dbo].[EmployeeConceptsRelationDetail]([InstanceID], company, [Active]);
GO

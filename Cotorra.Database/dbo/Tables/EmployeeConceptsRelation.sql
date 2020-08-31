CREATE TABLE [dbo].[EmployeeConceptsRelation] (
    [ID]                                  UNIQUEIDENTIFIER NOT NULL,
    [Active]                              BIT              NOT NULL,
    [DeleteDate]                          DATETIME2 (7)    NULL,
    [Timestamp]                           DATETIME2 (7)    NOT NULL,
    [Name]                                NVARCHAR (100)   NULL,
    [Description]                         NVARCHAR (250)   NULL,
    [StatusID]                            INT              NOT NULL,
    [CreationDate]                        DATETIME2 (7)    NOT NULL,
    [user]                                UNIQUEIDENTIFIER NOT NULL,
    [company]                             UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]                          UNIQUEIDENTIFIER NOT NULL,
    [EmployeeID]                          UNIQUEIDENTIFIER NOT NULL,
    [ConceptPaymentID]                    UNIQUEIDENTIFIER NOT NULL,
    [CreditAmount]                        DECIMAL (18, 6)  NOT NULL,
    [OverdraftDetailValue]                DECIMAL (18, 6)  NOT NULL,
    [OverdraftDetailAmount]               DECIMAL (18, 6)  NOT NULL,
    [PaymentsMadeByOtherMethod]           DECIMAL (18, 6)  NOT NULL,
    [ConceptPaymentStatus]                INT              NOT NULL,
    [BalanceCalculated]                   AS               (([CreditAmount]-[PaymentsMadeByOtherMethod])-[dbo].[getAccumulatedAmountWithHeld]([ID])) ,
    [AccumulatedAmountWithHeldCalculated] AS               ([dbo].[getAccumulatedAmountWithHeld]([ID])),
    [InitialCreditDate] DATETIME NOT NULL DEFAULT getdate(), 
    CONSTRAINT [PK_EmployeeConceptsRelation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_EmployeeConceptsRelation_ConceptPayment] FOREIGN KEY ([ConceptPaymentID]) REFERENCES [dbo].[ConceptPayment] ([ID]),
    CONSTRAINT [FK_EmployeeConceptsRelation_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID])
);


GO

CREATE NONCLUSTERED INDEX [IX_EmployeeConceptsRelation_InstanceID]
    ON [dbo].[EmployeeConceptsRelation]([InstanceID], [Active]);
GO

--For Calculation
CREATE NONCLUSTERED INDEX [IX_EmployeeConceptsRelation_InstanceID_EmployeeID_ConceptPaymentID]
    ON [dbo].[EmployeeConceptsRelation]([InstanceID], company, EmployeeID, ConceptPaymentID);
GO

--For Auth and apply overdraft to credit
CREATE NONCLUSTERED INDEX [IX_EmployeeConceptsRelation_ConceptPaymentID]
    ON [dbo].[EmployeeConceptsRelation](ConceptPaymentID);
GO
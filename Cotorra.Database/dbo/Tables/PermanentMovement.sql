CREATE TABLE [dbo].[PermanentMovement]
(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (100)   NOT NULL,
    [Description]      NVARCHAR (250)   NOT NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [company]          UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
    
    [ConceptPaymentID] UNIQUEIDENTIFIER NOT NULL, 
    [EmployeeID]       UNIQUEIDENTIFIER NOT NULL, 
    [InitialApplicationDate] datetime not null,
    [PermanentMovementType] int not null,
    [PermanentMovementStatus] int not null,
    [Amount] decimal(18,6) not null,
    [TimesToApply] int not null,
    [TimesApplied] INT NOT NULL, 
    [LimitAmount] DECIMAL(18, 6) NULL, 
    [AccumulatedAmount] DECIMAL(18, 6) NULL, 
    [RegistryDate] DATETIME NOT NULL,
    [ControlNumber] INT NOT NULL

    CONSTRAINT [PK_PermanentMovement] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PermanentMovement_ConceptPaymentID] FOREIGN KEY ([ConceptPaymentID]) REFERENCES [dbo].[ConceptPayment] ([ID]),
    CONSTRAINT [FK_PermanentMovement_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_PermanentMovement_InstanceID]
    ON [dbo].[PermanentMovement]([InstanceID], [Active]);
GO


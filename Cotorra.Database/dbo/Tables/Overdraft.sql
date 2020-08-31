CREATE TABLE [dbo].[Overdraft]
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
    PeriodDetailID     UNIQUEIDENTIFIER NOT NULL,
    EmployeeID         UNIQUEIDENTIFIER NOT NULL,
    [OverdraftType] INT NOT NULL DEFAULT(1), 
    [WorkingDays] DECIMAL(18,6) NULL DEFAULT(0),
    [UUID] UNIQUEIDENTIFIER NOT NULL, 
    [OverdraftStatus] INT NOT NULL DEFAULT(0),  
    [HistoricEmployeeID] UNIQUEIDENTIFIER NULL,
    [OverdraftPreviousCancelRelationshipID] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Overdraft] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Overdraft_PeriodDetail_PeriodDetailID] FOREIGN KEY ([PeriodDetailID]) REFERENCES [dbo].[PeriodDetail] ([ID]),
    CONSTRAINT [FK_Overdraft_Employee_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID]),
    CONSTRAINT [FK_Overdraft_HistoricEmployee_HistoricEmployeeID] FOREIGN KEY ([HistoricEmployeeID]) REFERENCES [dbo].[HistoricEmployee] ([ID]),
    CONSTRAINT [FK_Overdraft_Overdraft_OverdraftPreviousCancelRelationshipID] FOREIGN KEY ([OverdraftPreviousCancelRelationshipID]) REFERENCES [dbo].[Overdraft] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_Overdraft_InstanceID]
    ON [dbo].[Overdraft]([InstanceID], [Active]);
GO

--Get for Authorization 
CREATE NONCLUSTERED INDEX [IX_Overdraft_Auth_CompanyID_InstanceID]
    ON [dbo].[Overdraft]([company], [InstanceID], [PeriodDetailID], [Active]);
GO

--Get for Authorization With Status
CREATE NONCLUSTERED INDEX [IX_Overdraft_Auth_CompanyID_InstanceID_Status]
    ON [dbo].[Overdraft]([company], [InstanceID], [PeriodDetailID],[OverdraftStatus], [Active]);
GO

CREATE TABLE [dbo].[Settlement]
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
    EmployeeID         UNIQUEIDENTIFIER NOT NULL,
    [SettlementBaseSalary]              DECIMAL(18, 6) NOT NULL, 
    [SettlementEmployeeSeparationDate]  DATETIME2 (7)    NOT NULL,
    [SettlementCause]                   INT NOT NULL,
    [CompleteISRYears]                  DECIMAL(18, 6) NOT NULL, 
    [ISRoSUBSDirectCalculus]            BIT NOT NULL,
    [ApplyEmployeeSubsidyInISRUSMOCalculus]  BIT NOT NULL,

    CONSTRAINT [PK_Settlement] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Settlement_Employee_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_Settlement_InstanceID]
    ON [dbo].[Settlement]([InstanceID], [Active]);
GO


CREATE TABLE [dbo].[EmployeeSalaryIncrease] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Active]       BIT              NOT NULL,
    [DeleteDate]   DATETIME2 (7)    NULL,
    [Timestamp]    DATETIME2 (7)    NOT NULL,
    [user]         UNIQUEIDENTIFIER NOT NULL,
    [company]      UNIQUEIDENTIFIER NOT NULL,
    [StatusID]         INT              NOT NULL, 
    [Name] NVARCHAR(150) NOT NULL, 
    [Description]      NVARCHAR (250)   NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [InstanceID]   UNIQUEIDENTIFIER NOT NULL,
    [ModificationDate] DATETIME2 (7) NOT NULL,
    [EmployeeID] UNIQUEIDENTIFIER NOT NULL,   
    [DailySalary] DECIMAL(18, 6)   NULL, 	
    [EmployeeSBCAdjustmentID] UNIQUEIDENTIFIER NULL, 

    CONSTRAINT [PK_EmployeeSalaryIncrease] PRIMARY KEY CLUSTERED ([ID] ASC), 
    CONSTRAINT [FK_EmployeeSalaryIncrease_Employee] FOREIGN KEY (EmployeeID) REFERENCES Employee([ID])  ,
    CONSTRAINT [FK_EmployeeSalaryIncrease_EmployeeSBCAdjustment] FOREIGN KEY ([EmployeeSBCAdjustmentID]) REFERENCES [dbo].[EmployeeSBCAdjustment] ([ID]) 
);

GO
--GetAll 
CREATE NONCLUSTERED INDEX [IX_EmployeeSalaryIncrease_InstanceID]
    ON [dbo].[EmployeeSalaryIncrease]([InstanceID], [Active]);
GO
   

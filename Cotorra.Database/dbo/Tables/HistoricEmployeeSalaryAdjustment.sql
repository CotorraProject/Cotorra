CREATE TABLE [dbo].[HistoricEmployeeSalaryAdjustment] (
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
    [HistoricEmployeeSBCAdjustmentID] UNIQUEIDENTIFIER NULL, 

    CONSTRAINT [PK_HistoricEmployeeSalaryAdjustment] PRIMARY KEY CLUSTERED ([ID] ASC), 
    CONSTRAINT [FK_HistoricEmployeeSalaryAdjustment_Employee] FOREIGN KEY (EmployeeID) REFERENCES Employee([ID]),
    CONSTRAINT [FK_HistoricEmployeeSalaryAdjustment_HistoricEmployeeSBCAdjustment] FOREIGN KEY ([HistoricEmployeeSBCAdjustmentID]) REFERENCES [dbo].[HistoricEmployeeSBCAdjustment] ([ID]) 

);

GO
--GetAll 
CREATE NONCLUSTERED INDEX [IX_HistoricEmployeeSalaryAdjustment_InstanceID]
    ON [dbo].[HistoricEmployeeSalaryAdjustment]([InstanceID], [Active]);
GO
   

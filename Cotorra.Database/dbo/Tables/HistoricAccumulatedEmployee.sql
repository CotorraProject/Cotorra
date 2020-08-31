CREATE TABLE [dbo].[HistoricAccumulatedEmployee] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Active]       BIT              NOT NULL,
    [DeleteDate]   DATETIME2 (7)    NULL,
    [Timestamp]    DATETIME2 (7)    NOT NULL,
    [user]         UNIQUEIDENTIFIER NOT NULL,
    [company]      UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]   UNIQUEIDENTIFIER NOT NULL,
    [StatusID]     INT              NOT NULL, 
    [Name]         NVARCHAR (250)   NOT NULL,
    [Description]  NVARCHAR (250)   NOT NULL,
    [CreationDate] DATETIME2 (7)    NOT NULL,
    [EmployeeID]   UNIQUEIDENTIFIER NOT NULL,
    [AccumulatedTypeID] UNIQUEIDENTIFIER NOT NULL,
    [ExerciseFiscalYear] INT NOT NULL,
    [InitialExerciseAmount] decimal(18,6) NOT NULL,
    [PreviousExerciseAccumulated] decimal(18,6) NOT NULL,
    [January] decimal(18,6) NOT NULL, 
    [February] decimal(18,6) NOT NULL,
    [March] decimal(18,6) NOT NULL,
    [April] decimal(18,6) NOT NULL,
    [May] decimal(18,6) NOT NULL,
    [June] decimal(18,6) NOT NULL,
    [July] decimal(18,6) NOT NULL,
    [August] decimal(18,6) NOT NULL,
    [September] decimal(18,6) NOT NULL,
    [October] decimal(18,6) NOT NULL,
    [November] decimal(18,6) NOT NULL,
    [December] decimal(18,6) NOT NULL,

    CONSTRAINT [PK_HistoricAccumulatedEmployee] PRIMARY KEY CLUSTERED ([ID] ASC), 
    CONSTRAINT [FK_HistoricAccumulatedEmployee_AccumulatedType] FOREIGN KEY (AccumulatedTypeID) REFERENCES AccumulatedType([ID]),
    CONSTRAINT [FK_HistoricAccumulatedEmployee_Employee] FOREIGN KEY (EmployeeID) REFERENCES Employee([ID])  
);
GO

CREATE UNIQUE INDEX AK_HistoricAccumulatedEmployee_Unique   
   ON HistoricAccumulatedEmployee (InstanceID, EmployeeID, AccumulatedTypeID, ExerciseFiscalYear);   
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_HistoricAccumulatedEmployee_InstanceID]
    ON [dbo].[HistoricAccumulatedEmployee]([InstanceID], [Active]);
GO


--GetAll by employeeid
CREATE NONCLUSTERED INDEX [IX_HistoricAccumulatedEmployee_EmployeeID_AccumulatedTypeID_ExerciseFiscalYear_InstanceID]
    ON [dbo].[HistoricAccumulatedEmployee]([InstanceID], [EmployeeID], [AccumulatedTypeID], [ExerciseFiscalYear], [Active]);
GO

--GetAll by employeeid
CREATE NONCLUSTERED INDEX [IX_HistoricAccumulatedEmployee_EmployeeID_ExerciseFiscalYear_InstanceID]
    ON [dbo].[HistoricAccumulatedEmployee]([InstanceID], [EmployeeID], [ExerciseFiscalYear], [Active]);
GO

--GetAll by instance and fiscal year
CREATE NONCLUSTERED INDEX [IX_HistoricAccumulatedEmployee_ExerciseFiscalYear_InstanceID]
    ON [dbo].[HistoricAccumulatedEmployee]([InstanceID], [ExerciseFiscalYear], [Active]);
GO

--GetAll by instance and fiscal year - vista para el cálculo
CREATE NONCLUSTERED INDEX [IX_HistoricAccumulatedEmployee_ExerciseFiscalYear_InstanceID_company_active]
    ON [dbo].[HistoricAccumulatedEmployee]([InstanceID], [company], [EmployeeID], [ExerciseFiscalYear], [Active]);
GO

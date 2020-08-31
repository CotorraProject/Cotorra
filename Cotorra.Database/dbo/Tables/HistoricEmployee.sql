CREATE TABLE [dbo].[HistoricEmployee] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Active]       BIT              NOT NULL,
    [DeleteDate]   DATETIME2 (7)    NULL,
    [Timestamp]    DATETIME2 (7)    NOT NULL,
    [user]         UNIQUEIDENTIFIER NOT NULL,
    [company]      UNIQUEIDENTIFIER NOT NULL,
    [StatusID]         INT              NOT NULL, 
    [Description]      NVARCHAR (250)   NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [InstanceID]   UNIQUEIDENTIFIER NOT NULL,
    [PeriodDetailID] UNIQUEIDENTIFIER NOT NULL,
    [EmployeeID] UNIQUEIDENTIFIER NOT NULL,
    [RFC] NVARCHAR(13) NOT NULL, 
    [BirthDate] DATETIME2 NULL, 
    [NSS] NVARCHAR(11) NOT NULL, 
    [BornPlace] NVARCHAR(100) NULL, 
    [CivilStatus] INT NULL, 
    [Code] NVARCHAR(100) NOT NULL, 
    [CURP] NVARCHAR(18) NOT NULL, 
    [Name] NVARCHAR(150) NOT NULL, 
    [FirstLastName] NVARCHAR(150) NOT NULL, 
    [SecondLastName] NVARCHAR(150) NULL, 
    [Gender] INT NOT NULL, 
    [JobPositionDescription] nvarchar(250) NOT NULL, 
    [JobPositionID] uniqueidentifier NOT NULL, 
    [DepartmentDescription] nvarchar(250) NOT NULL, 
    [DepartmentID] uniqueidentifier NOT NULL, 
    [PeriodTypeDescription] nvarchar(250) NOT NULL, 
    [PeriodTypeID] uniqueidentifier NOT NULL, 
    [WorkshiftDescription] nvarchar(250) NOT NULL, 
    [WorkshiftID] uniqueidentifier NOT NULL, 
    [EmployerRegistrationDescription] nvarchar(250) NULL, 
    [EmployerRegistrationZipCode] nvarchar(250) NULL, 
    [EmployerRegistrationCode] nvarchar(250) NULL, 
    [EmployerRegistrationFederalEntity] nvarchar(250) NULL, 
    [EmployerRegistrationID] uniqueidentifier NULL, 
	[EntryDate] DATETIME2   NULL, 
	[ContractType] INT   NULL, 
    [DailySalary] DECIMAL(18, 6)   NULL, 
	[ContributionBase] INT   NULL,  
	[SBCFixedPart] DECIMAL(18, 6)  NULL, 
	[SBCVariablePart] DECIMAL(18, 6)   NULL, 
	[SBCMax25UMA] DECIMAL(18, 6)   NULL,  

    [PaymentBase] INT   NULL, 
    [PaymentMethod] INT   NULL, 
    [SalaryZone] INT   NULL, 
    [RegimeType] INT   NULL, 
    [Email] NVARCHAR(320)   NULL, 
    [UMF] NVARCHAR(100)   NULL,
    [Phone] NVARCHAR(50)   NULL,
    [BankAccount] NVARCHAR(50)  NULL,    
	[ZipCode] NVARCHAR(100) NULL, 
	[FederalEntity] NVARCHAR(100) NULL,
	[Municipality] NVARCHAR(100) NULL,
	[Street] NVARCHAR(100) NULL,
	[ExteriorNumber] NVARCHAR(100) NULL,
	[InteriorNumber] NVARCHAR(100) NULL,
	[Suburb] NVARCHAR(100) NULL,   
    [IdentityUserID]   UNIQUEIDENTIFIER  NULL,
    [EmployeeTrustLevel] INT   NOT NULL default(1), 
    [JobPositionRiskType] INT   NOT NULL default(1), 
    [PaymentPeriodicity]  INT   NOT NULL default(1), 
    [ShiftWorkingDayType] INT   NOT NULL default(1), 
    [BankCode] INT NULL,
    [SettlementSalaryBase] DECIMAL(18, 6)  NOT NULL DEFAULT(0), 
    [BenefitType] INT  NOT NULL default(1), 
    [BankBranchNumber] NVARCHAR(50)  NULL,
    [CLABE] NVARCHAR(20)  NULL,
    [LocalStatus]         INT              NOT NULL  DEFAULT(0),  
    [LastStatusChange]     DATETIME2 (7)    NOT NULL  DEFAULT(getdate()),  
    [ImmediateLeaderEmployeeID] uniqueidentifier NULL,
	[BankID] UNIQUEIDENTIFIER NULL,
    [UnregisteredDate] DATETIME2  NULL, 
    CONSTRAINT [PK_HistoricEmployee] PRIMARY KEY CLUSTERED ([ID] ASC), 
	CONSTRAINT [FK_HistoricEmployee_PeriodDetail] FOREIGN KEY (PeriodDetailID) REFERENCES PeriodDetail([ID]),
    CONSTRAINT [FK_HistoricEmployee_Employee] FOREIGN KEY (EmployeeID) REFERENCES Employee([ID])  
);

GO
--GetAll 
CREATE NONCLUSTERED INDEX [IX_HistoricEmployee_InstanceID]
    ON [dbo].[HistoricEmployee]([InstanceID], [Active]);
GO
   

--GetAll by periodDetail
CREATE NONCLUSTERED INDEX [IX_HistoricEmployee_PeriodDetailID_InstanceID]
    ON [dbo].[HistoricEmployee]([InstanceID], PeriodDetailID, [Active]);
GO
   

--GetAll by periodDetail employeeid
CREATE NONCLUSTERED INDEX [IX_HistoricEmployee_PeriodDetailID_EmployeeID_InstanceID]
    ON [dbo].[HistoricEmployee]([InstanceID], [EmployeeID], PeriodDetailID, [Active]);
GO
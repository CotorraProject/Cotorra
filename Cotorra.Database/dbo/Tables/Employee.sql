CREATE TABLE [dbo].[Employee] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Active]       BIT              NOT NULL,
    [DeleteDate]   DATETIME2 (7)    NULL,
    [Timestamp]    DATETIME2 (7)    NOT NULL,
    [user]         UNIQUEIDENTIFIER NOT NULL,
    [company]      UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]   UNIQUEIDENTIFIER NOT NULL,
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
    [JobPositionID] UNIQUEIDENTIFIER NOT NULL, 
    [DepartmentID] UNIQUEIDENTIFIER NOT NULL, 
    [PeriodTypeID] UNIQUEIDENTIFIER NOT NULL, 
    [WorkshiftID] UNIQUEIDENTIFIER NOT NULL, 
    [BankID] UNIQUEIDENTIFIER NULL,
	[EntryDate] DATETIME2  NULL, 
    [ReEntryDate] DATETIME2  NULL, 
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
    [EmployerRegistrationID] UNIQUEIDENTIFIER NULL, 
	[ZipCode] NVARCHAR(100) NULL, 
	[FederalEntity] NVARCHAR(100) NULL,
	[Municipality] NVARCHAR(100) NULL,
	[Street] NVARCHAR(100) NULL,
	[ExteriorNumber] NVARCHAR(100) NULL,
	[InteriorNumber] NVARCHAR(100) NULL,
	[Suburb] NVARCHAR(100) NULL,
    [Reference] NVARCHAR(100) NULL,
    [StatusID]         INT              NOT NULL, 
    [Description]      NVARCHAR (250)   NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [IdentityUserID]   UNIQUEIDENTIFIER  NULL,
    [EmployeeTrustLevel] INT   NOT NULL default(1), 
    [SettlementSalaryBase] DECIMAL(18, 6)  NOT NULL DEFAULT(0), 
    [BankBranchNumber] NVARCHAR(50)  NULL,
    [CLABE] NVARCHAR(20)  NULL,
    [LocalStatus]         INT              NOT NULL  DEFAULT(0),  
    [LastStatusChange]     DATETIME2 (7)    NOT NULL , 
    [BenefitType] INT   NOT NULL default(1), 
    [ImmediateLeaderEmployeeID] uniqueidentifier NULL,
    [UnregisteredDate] DATETIME2  NULL, 
    [Cellphone] NVARCHAR(15) NULL,
    [IsKioskEnabled] bit NOT NULL default(0),
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ID] ASC), 
	CONSTRAINT [FK_Employee_JobPosition] FOREIGN KEY (JobPositionID) REFERENCES JobPosition([ID]),
    CONSTRAINT [FK_Employee_Department] FOREIGN KEY (DepartmentID) REFERENCES Department([ID]),
	CONSTRAINT [FK_Employee_PeriodType] FOREIGN KEY (PeriodTypeID) REFERENCES PeriodType([ID]),
	CONSTRAINT [FK_Employee_Workshift] FOREIGN KEY (WorkshiftID) REFERENCES Workshift([ID]),
    CONSTRAINT [FK_Employee_EmployerRegistration] FOREIGN KEY (EmployerRegistrationID) REFERENCES EmployerRegistration([ID]),
    CONSTRAINT [FK_Employee_Bank] FOREIGN KEY (BankID) REFERENCES Bank([ID]),
    CONSTRAINT [FK_Employee_LeaderEmployee] FOREIGN KEY (ImmediateLeaderEmployeeID) REFERENCES Employee([ID]),
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_Employee_InstanceID]
    ON [dbo].[Employee]([InstanceID], [Active]);
GO

--GetBy InstanceId and Company
CREATE NONCLUSTERED INDEX [IX_Employee_InstanceID_company]
    ON [dbo].[Employee]([InstanceID], [company]);
GO

--GetBy RegistrationId
CREATE NONCLUSTERED INDEX [IX_Employee_RegistrationID]
    ON [dbo].[Employee]([InstanceID], [company], [EmployerRegistrationID]);
GO

--GetUserIdentityID
CREATE NONCLUSTERED INDEX [IX_Employee_IdentityUserID]
    ON [dbo].[Employee]([IdentityUserID], [Active]);
GO


CREATE PROCEDURE [dbo].[AuthorizeByOverdrafts]
@OverdraftIDs dbo.guidlisttabletype READONLY,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS

BEGIN TRAN T1;  

Declare @PeriodID uniqueidentifier
Declare @FiscalYear as integer
Declare @Month as integer
Declare @PeriodTypeID uniqueidentifier

SELECT top 1 
		@FiscalYear = p.FiscalYear, 
		@Month = month(pd.InitialDate), 
		@PeriodTypeID = p.PeriodTypeID,
	    @PeriodID = pd.PeriodID
from [Period] p
INNER JOIN PeriodDetail pd 
ON p.ID = pd.PeriodID 
INNER JOIN Overdraft o
ON o.PeriodDetailID = pd.ID
INNER JOIN @OverdraftIDs odi
ON odi.ID = o.ID
where pd.InstanceID = @InstanceId and pd.company = @company

DECLARE @EmployeeIDTable TABLE(
	ID uniqueidentifier NOT NULL
)

INSERT INTO @EmployeeIDTable (ID)
SELECT e.ID from Employee e
INNER JOIN Overdraft o
ON o.EmployeeID = e.ID
INNER JOIN @OverdraftIDs oids
ON oids.ID = o.ID

--1. CREATE HISTORIC EMPLOYEE ACCUMULATED FOR FISCAL YEAR IF NOT EXISTS
PRINT N'1. Create Historic Accumulated for each Employee.';  

EXEC CreateHistoricAccumulatedEmployeebyFiscalYear @FiscalYear, @InstanceId, @company, @user

PRINT N'4. Create Historic Employee Info.'; 

DECLARE @HistoricEmployee_Temp AS TABLE 
(	[ID] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[DeleteDate] [datetime2](7) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[company] [uniqueidentifier] NOT NULL,
	[StatusID] [int] NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[PeriodDetailID] [uniqueidentifier] NOT NULL,
	[EmployeeID] [uniqueidentifier] NOT NULL,
	[RFC] [nvarchar](13) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[NSS] [nvarchar](11) NOT NULL,
	[BornPlace] [nvarchar](100) NULL,
	[CivilStatus] [int] NULL,
	[Code] [nvarchar](100) NOT NULL,
	[CURP] [nvarchar](18) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[FirstLastName] [nvarchar](150) NOT NULL,
	[SecondLastName] [nvarchar](150) NOT NULL,
	[Gender] [int] NOT NULL,
	[JobPositionDescription] [nvarchar](250) NOT NULL,
	[JobPositionID] [uniqueidentifier] NOT NULL,
	[DepartmentDescription] [nvarchar](250) NOT NULL,
	[DepartmentID] [uniqueidentifier] NOT NULL,
	[PeriodTypeDescription] [nvarchar](250) NOT NULL,
	[PeriodTypeID] [uniqueidentifier] NOT NULL,
	[WorkshiftDescription] [nvarchar](250) NOT NULL,
	[WorkshiftID] [uniqueidentifier] NOT NULL,
	[EmployerRegistrationDescription] [nvarchar](250) NULL,
	[EmployerRegistrationZipCode] [nvarchar](250) NULL,
	[EmployerRegistrationCode] [nvarchar](250) NULL,
	[EmployerRegistrationFederalEntity] [nvarchar](250) NULL,
	[EmployerRegistrationID] [uniqueidentifier] NULL,
	[EntryDate] [datetime2](7) NULL,
	[ContractType] [int] NULL,
	[DailySalary] [decimal](18, 6) NULL,
	[ContributionBase] [int] NULL,
	[SBCFixedPart] [decimal](18, 6) NULL,
	[SBCVariablePart] [decimal](18, 6) NULL,
	[SBCMax25UMA] [decimal](18, 6) NULL,
	[PaymentBase] [int] NULL,
	[PaymentMethod] [int] NULL,
	[SalaryZone] [int] NULL,
	[RegimeType] [int] NULL,
	[Email] [nvarchar](320) NULL,
	[UMF] [nvarchar](100) NULL,
	[Phone] [nvarchar](50) NULL,
	[BankAccount] [nvarchar](50) NULL,
	[ZipCode] [nvarchar](100) NULL,
	[FederalEntity] [nvarchar](100) NULL,
	[Municipality] [nvarchar](100) NULL,
	[Street] [nvarchar](100) NULL,
	[ExteriorNumber] [nvarchar](100) NULL,
	[InteriorNumber] [nvarchar](100) NULL,
	[Suburb] [nvarchar](100) NULL,
	[IdentityUserID] [uniqueidentifier] NULL,
	[EmployeeTrustLevel] [int] NOT NULL,
	[JobPositionRiskType] [int] NOT NULL,
	[PaymentPeriodicity] [int] NOT NULL,
	[ShiftWorkingDayType] [int] NOT NULL,
	[BankCode] [int] NULL,
	[BenefitType] INT   NOT NULL default(1), 
	[SettlementSalaryBase] [decimal](18, 6) NOT NULL,
	[OverdraftID] uniqueidentifier NOT NULL,
	[BankBranchNumber] NVARCHAR(50)  NULL,
    [CLABE] NVARCHAR(20)  NULL,
    [LocalStatus]         INT              NOT NULL  DEFAULT(0),  
    [LastStatusChange]     DATETIME2 (7)    NOT NULL ,  
    [ImmediateLeaderEmployeeID] uniqueidentifier NULL,
	[BankID] UNIQUEIDENTIFIER NULL,
	[UnregisteredDate] DATETIME2  NULL
 )	

--4. Insert Historic Employee
insert into @HistoricEmployee_Temp(
	ID,
	Active,
	DeleteDate,
	[Timestamp],
	[user],
	company,
	StatusID,
	[Description],
	CreationDate,
	InstanceID,
	PeriodDetailID,
	EmployeeID,
	RFC,
	BirthDate,
	NSS,
	BornPlace,
	CivilStatus,
	Code,
	CURP,
	[Name],
	FirstLastName,
	SecondLastName,
	Gender,
	JobPositionDescription,
	JobPositionID,
	DepartmentDescription,
	DepartmentID,
	PeriodTypeDescription,
	PeriodTypeID,
	WorkshiftDescription,
	WorkshiftID,
	EmployerRegistrationDescription,
	EmployerRegistrationZipCode,
	EmployerRegistrationCode,
	EmployerRegistrationFederalEntity,
	EmployerRegistrationID,
	EntryDate,
	ContractType,
	DailySalary,
	ContributionBase,
	SBCFixedPart,
	SBCVariablePart,
	SBCMax25UMA,
	PaymentBase,
	PaymentMethod,
	SalaryZone,
	RegimeType,
	Email ,
	UMF,
	Phone,
	BankAccount,
	ZipCode,
	FederalEntity,
	Municipality,
	Street,
	ExteriorNumber,
	InteriorNumber,
	Suburb,
	IdentityUserID,
	EmployeeTrustLevel,
	JobPositionRiskType,
	PaymentPeriodicity,
	ShiftWorkingDayType,
	BankCode,
	[BenefitType],
	SettlementSalaryBase,
	[OverdraftID],
	[BankBranchNumber],
    [CLABE] ,
    [LocalStatus],
    [LastStatusChange],
    [ImmediateLeaderEmployeeID],
	[BankID],
	[UnregisteredDate]
	)
	select 
		ID=newid(), 
		Active=  1,
		DeleteDate=  null,
		[Timestamp]= getdate() ,
		[user]=  @user,
		company=  @company,
		StatusID= 1,
		[Description]= '',
		CreationDate=  getdate(),
		InstanceID=  @InstanceId,
		PeriodDetailID=  o.PeriodDetailID,
		EmployeeID= e.ID,
		RFC=  e.RFC,
		BirthDate=  e.BirthDate,
		NSS=  e.NSS,
		BornPlace= e.BornPlace,
		CivilStatus= e.CivilStatus,
		Code= e.Code ,
		CURP= e.CURP ,
		[Name]= e.Name ,
		FirstLastName= e.FirstLastName ,
		SecondLastName= e.SecondLastName ,
		Gender= e.Gender ,
		JobPositionDescription=  jp.Name,
		JobPositionID= e.JobPositionID ,
		DepartmentDescription=  d.Name,
		DepartmentID= e.DepartmentID ,
		PeriodTypeDescription= pt.Name ,
		PeriodTypeID= e.PeriodTypeID ,
		WorkshiftDescription= w.Name ,
		WorkshiftID= e.WorkshiftID ,
		EmployerRegistrationDescription= er.Code ,
		EmployerRegistrationZipCode= ad.ZipCode ,
		EmployerRegistrationCode= er.Code ,
		EmployerRegistrationFederalEntity= ad.FederalEntity ,
		EmployerRegistrationID= e.EmployerRegistrationID ,
		EntryDate= e.EntryDate ,
		ContractType= e.ContractType ,
		DailySalary= e.DailySalary ,
		ContributionBase= e.ContributionBase ,
		SBCFixedPart= e.SBCFixedPart ,
		SBCVariablePart=  e.SBCVariablePart,
		SBCMax25UMA= e.SBCMax25UMA ,
		PaymentBase= e.PaymentBase ,
		PaymentMethod= e.PaymentMethod ,
		SalaryZone= e.SalaryZone ,
		RegimeType= e.RegimeType ,
		Email = e.Email,
		UMF= e.UMF ,
		Phone= e.Phone ,
		BankAccount= e.BankAccount ,
		ZipCode= e.ZipCode ,
		FederalEntity= e.FederalEntity ,
		Municipality= e.Municipality ,
		Street= e.Street ,
		ExteriorNumber= e.ExteriorNumber ,
		InteriorNumber= e.InteriorNumber ,
		Suburb= e.Suburb ,
		IdentityUserID= e.IdentityUserID ,
		EmployeeTrustLevel= e.EmployeeTrustLevel ,
		JobPositionRiskType= jp.JobPositionRiskType ,
		PaymentPeriodicity= pt.PaymentPeriodicity ,
		ShiftWorkingDayType= w.ShiftWorkingDayType ,
		BankCode=  bk.Code,
		[BenefitType] = e.BenefitType,
		SettlementSalaryBase =e.SettlementSalaryBase,
		[OverdraftID] = o.ID,
		[BankBranchNumber] = e.BankBranchNumber,
		[CLABE]  = e.CLABE,
		[LocalStatus] = e.LocalStatus,
		[LastStatusChange] = e.LastStatusChange,
		[ImmediateLeaderEmployeeID] = e.ImmediateLeaderEmployeeID,
		[BankID] = e.BankID,
		[UnregisteredDate] = e.UnregisteredDate
		from Employee e	
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = e.ID	
		LEFT JOIN Overdraft o
			on o.EmployeeID = e.ID  and o.company = @company and o.InstanceID = @InstanceId
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		LEFT JOIN JobPosition jp
			on jp.ID = e.JobPositionID and jp.company = @company and jp.InstanceID = @InstanceId
		LEFT JOIN Department d
			on d.ID = e.DepartmentID and d.company = @company and d.InstanceID = @InstanceId
		LEFT JOIN PeriodType pt
			on pt.ID = e.PeriodTypeID and pt.company = @company and pt.InstanceID = @InstanceId
		LEFT JOIN Workshift w
			on w.ID = e.WorkshiftID and w.company = @company and w.InstanceID = @InstanceId
		LEFT JOIN EmployerRegistration er
			on er.ID = e.EmployerRegistrationID and er.company = @company and er.InstanceID = @InstanceId
		LEFT JOIN [Address] ad
			on ad.ID = er.AddressID and ad.company = @company and ad.InstanceID = @InstanceId
		LEFT JOIN [Bank] bk
			on bk.ID = e.BankID
		where 
			e.company = @company and 
			e.InstanceID = @InstanceId 

-- Insert into table
insert into HistoricEmployee (
	ID,
	Active,
	DeleteDate,
	[Timestamp],
	[user],
	company,
	StatusID,
	[Description],
	CreationDate,
	InstanceID,
	PeriodDetailID,
	EmployeeID,
	RFC,
	BirthDate,
	NSS,
	BornPlace,
	CivilStatus,
	Code,
	CURP,
	[Name],
	FirstLastName,
	SecondLastName,
	Gender,
	JobPositionDescription,
	JobPositionID,
	DepartmentDescription,
	DepartmentID,
	PeriodTypeDescription,
	PeriodTypeID,
	WorkshiftDescription,
	WorkshiftID,
	EmployerRegistrationDescription,
	EmployerRegistrationZipCode,
	EmployerRegistrationCode,
	EmployerRegistrationFederalEntity,
	EmployerRegistrationID,
	EntryDate,
	ContractType,
	DailySalary,
	ContributionBase,
	SBCFixedPart,
	SBCVariablePart,
	SBCMax25UMA,
	PaymentBase,
	PaymentMethod,
	SalaryZone,
	RegimeType,
	Email ,
	UMF,
	Phone,
	BankAccount,
	ZipCode,
	FederalEntity,
	Municipality,
	Street,
	ExteriorNumber,
	InteriorNumber,
	Suburb,
	IdentityUserID,
	EmployeeTrustLevel,
	JobPositionRiskType,
	PaymentPeriodicity,
	ShiftWorkingDayType,
	BankCode,
	[BenefitType],
	SettlementSalaryBase,
    [BankBranchNumber],
    [CLABE] ,
    [LocalStatus],
    [LastStatusChange],
    [ImmediateLeaderEmployeeID],
	[BankID],
	[UnregisteredDate]
	)
select ID,
	Active,
	DeleteDate,
	[Timestamp],
	[user],
	company,
	StatusID,
	[Description],
	CreationDate,
	InstanceID,
	PeriodDetailID,
	EmployeeID,
	RFC,
	BirthDate,
	NSS,
	BornPlace,
	CivilStatus,
	Code,
	CURP,
	[Name],
	FirstLastName,
	SecondLastName,
	Gender,
	JobPositionDescription,
	JobPositionID,
	DepartmentDescription,
	DepartmentID,
	PeriodTypeDescription,
	PeriodTypeID,
	WorkshiftDescription,
	WorkshiftID,
	EmployerRegistrationDescription,
	EmployerRegistrationZipCode,
	EmployerRegistrationCode,
	EmployerRegistrationFederalEntity,
	EmployerRegistrationID,
	EntryDate,
	ContractType,
	DailySalary,
	ContributionBase,
	SBCFixedPart,
	SBCVariablePart,
	SBCMax25UMA,
	PaymentBase,
	PaymentMethod,
	SalaryZone,
	RegimeType,
	Email ,
	UMF,
	Phone,
	BankAccount,
	ZipCode,
	FederalEntity,
	Municipality,
	Street,
	ExteriorNumber,
	InteriorNumber,
	Suburb,
	IdentityUserID,
	EmployeeTrustLevel,
	JobPositionRiskType,
	PaymentPeriodicity,
	ShiftWorkingDayType,
	BankCode,
	[BenefitType],
	SettlementSalaryBase,
	[BankBranchNumber],
    [CLABE] ,
    [LocalStatus],
    [LastStatusChange],
    [ImmediateLeaderEmployeeID],
	[BankID],
	[UnregisteredDate]
from @HistoricEmployee_Temp

	
PRINT N'5. Create Accumulates for each AccumulateType per Employee, Fiscal Year, Month'; 
--5. Accumulates per Employee, FiscalYear, Accumulate, Year
DECLARE @AccumulateTable AS TABLE 
(EmployeeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeName NVARCHAR(100) NOT NULL,
 ConceptPaymentType int not null,
 TotalAmount DECIMAL(18,6) NOT NULL
 )	

 INSERT INTO @AccumulateTable
select o.EmployeeID, at.ID as AccumulatedTypeID, at.Name as AccumulatedTypeName,
cpr.ConceptPaymentType,
	case 
		when cpr.ConceptPaymentType = 1 then sum(od.Amount) 
		when cpr.ConceptPaymentType = 2 then sum(od.Taxed) 
		when cpr.ConceptPaymentType = 3 then sum(od.Exempt)
		when cpr.ConceptPaymentType = 4 then sum(od.IMSSTaxed)
		when cpr.ConceptPaymentType = 5 then sum(od.IMSSExempt)
	end as TotalAmount
from [AccumulatedType] at
INNER JOIN ConceptPaymentRelationship cpr
	ON cpr.AccumulatedTypeID = at.ID and cpr.InstanceID =  @InstanceId and cpr.company = @company
INNER JOIN ConceptPayment cp
	ON cp.ID = cpr.ConceptPaymentID and cp.InstanceID = @InstanceId and cp.company = @company
INNER JOIN OverdraftDetail od
	ON od.ConceptPaymentID = cp.ID and od.InstanceID = @InstanceId and od.company = @company
INNER JOIN Overdraft o
	ON o.ID = od.OverdraftID and o.InstanceID =  @InstanceId and o.company = @company 
INNER JOIN @OverdraftIDs oids
	ON oids.ID = o.ID
INNER JOIN @EmployeeIDTable eis
	ON eis.ID = o.EmployeeID
WHERE 
at.InstanceID = @InstanceId and at.company = @company 
group by o.EmployeeID, at.ID, at.Name, cpr.ConceptPaymentType

DECLARE @AccumulateTable_Grouped AS TABLE 
(EmployeeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeName NVARCHAR(100) NOT NULL,
 TotalAmount DECIMAL(18,6) NOT NULL
 )	

 INSERT INTO @AccumulateTable_Grouped
select EmployeeID, AccumulatedTypeID, AccumulatedTypeName, sum(TotalAmount) 
from @AccumulateTable
group by EmployeeID, AccumulatedTypeID, AccumulatedTypeName


IF (@Month = 1)
	UPDATE
		ae
	SET
		ae.January = ae.January + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 2)
	UPDATE
		ae
	SET
		ae.February = ae.February + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 3)
	UPDATE
		ae
	SET
		ae.March = ae.March + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 4)
	UPDATE
		ae
	SET
		ae.April = ae.April + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 5)
	UPDATE
		ae
	SET
		ae.May = ae.May + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 6)
	UPDATE
		ae
	SET
		ae.June = ae.June + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 7)
	UPDATE
		ae
	SET
		ae.July = ae.July + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear  

IF (@Month = 8)
	UPDATE
		ae
	SET
		ae.August = ae.August + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 9)
	UPDATE
		ae
	SET
		ae.September = ae.September + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 10)
	UPDATE
		ae
	SET
		ae.October = ae.October + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 11)
	UPDATE
		ae
	SET
		ae.November = ae.November + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

IF (@Month = 12)
	UPDATE
		ae
	SET
		ae.December = ae.December + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable_Grouped as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.EmployeeID = ae.EmployeeID and o.InstanceID = @InstanceId and o.company = @company
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = o.ID
		INNER JOIN @EmployeeIDTable eis
			ON eis.ID = o.EmployeeID
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 



PRINT N'9. Update the overdrafts to Authorized Status'; 
--9. Actualiza los overdraft en status de autorizados = 1

UPDATE
		overdraft
	SET
		overdraft.OverdraftStatus = 1,
		overdraft.HistoricEmployeeID = het.ID,
		overdraft.[Timestamp] = getdate()
	FROM
		Overdraft AS overdraft
		INNER JOIN @HistoricEmployee_Temp het
			ON het.OverdraftID = overdraft.ID
		INNER JOIN @OverdraftIDs oids
			ON oids.ID = overdraft.ID
		where 
			overdraft.InstanceID = @InstanceId and overdraft.company = @company

COMMIT TRAN T1;  
  --ROLLBACK TRAN T1;
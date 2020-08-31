CREATE PROCEDURE [dbo].[AuthorizeOverdraft]
@PeriodDetailId uniqueidentifier,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS

BEGIN TRAN T1;  

--1. CREATE HISTORIC EMPLOYEE ACCUMULATED FOR FISCAL YEAR IF NOT EXISTS
PRINT N'1. Create Historic Accumulated for each Employee.';  
EXEC CreateHistoricAccumulatedEmployee @PeriodDetailId, @InstanceId, @company, @user


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
		PeriodDetailID=  @PeriodDetailId,
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
		LEFT JOIN Overdraft o
			on o.EmployeeID = e.ID  and o.company = @company and o.InstanceID = @InstanceId
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
			e.InstanceID = @InstanceId and
			o.PeriodDetailID = @PeriodDetailId and o.OverdraftStatus = 0 and o.OverdraftType in (1, 2)

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
WHERE o.PeriodDetailID = @PeriodDetailId and o.OverdraftStatus = 0 and o.OverdraftType in (1, 2) and
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

Declare @PeriodID uniqueidentifier
Declare @FiscalYear as integer
Declare @Month as integer
Declare @PeriodTypeID uniqueidentifier

SELECT @FiscalYear = p.FiscalYear, @Month = month(pd.InitialDate), @PeriodTypeID = p.PeriodTypeID,
	   @PeriodID = pd.PeriodID
from [Period] p
INNER JOIN PeriodDetail pd 
ON p.ID = pd.PeriodID 
where pd.ID = @PeriodDetailId 

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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
		where 
			ae.InstanceID = @InstanceId and 
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
			ON o.EmployeeID = ae.EmployeeID and 
			o.PeriodDetailID = @PeriodDetailId and 
			o.InstanceID = @InstanceId and 
			o.company = @company
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 

PRINT N'7. Update the period to authorized status'; 
--7. Actualiza el periodo en Status Authorizado
UPDATE PeriodDetail set PeriodStatus = 2, [Timestamp] = getdate() where ID = @PeriodDetailId

PRINT N'8. Enable next period in calculation status'; 
--8. Enable next period in calculating status = 1

Declare @NextPeriodId uniqueidentifier
Declare @NextPeriodFinalDate datetime
Declare @NextPeriodStatus int

SELECT top 1 @NextPeriodId = pd2.ID, @NextPeriodFinalDate = pd2.FinalDate, @NextPeriodStatus = pd2.PeriodStatus FROM PeriodDetail pd1
INNER JOIN PeriodDetail pd2
	ON pd2.InstanceID = @InstanceId and 
	   pd2.company = @company and   
	   pd2.ID <> pd1.ID and pd1.PeriodID = pd2.PeriodID
WHERE pd1.InstanceID = @InstanceId and 
	  pd1.company = @company and pd1.ID = @PeriodDetailId and 
	  pd2.InitialDate > pd1.FinalDate
order by pd2.InitialDate

--Verificamos si fue el último PeriodDetail del Periodo, si es así tenemos que irnos al siguiente
IF (@NextPeriodId is null)
BEGIN 
	--Cerramos el periodo actual
	Update [Period] set IsActualFiscalYear = 0, IsFiscalYearClosed = 1 where ID = @PeriodID

	--Quiere decir que hay que seleccionar un periodo nuevo (siguiente año)
	SELECT top 1 @NextPeriodId = pd.ID, @NextPeriodFinalDate = pd.FinalDate FROM [Period] p
	INNER JOIN [PeriodDetail] pd
		ON pd.PeriodID = p.ID and 
		   pd.InstanceID = @InstanceID
	where 
		p.InstanceID = @InstanceId and 
		p.PeriodTypeID = @PeriodTypeID and 
		p.FiscalYear = @FiscalYear + 1
	order by pd.InitialDate
END

--Solo si el siguiente periodo es abierto
IF (@NextPeriodStatus = 0)
BEGIN
	UPDATE PeriodDetail Set PeriodStatus = 1, [Timestamp] = getdate() where ID = @NextPeriodId
END

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
		where overdraft.PeriodDetailID = @PeriodDetailId and overdraft.OverdraftStatus = 0 and 
		overdraft.OverdraftType in (1, 2) and overdraft.InstanceID = @InstanceId and overdraft.company = @company

PRINT N'10. Creates the new overdraft for each employee in periodType'; 
--Verifica que no exista ya un sobrerecibo para ese periodo
IF NOT EXISTS(SELECT ID FROM Overdraft where PeriodDetailID = @NextPeriodId)
BEGIN
		--10. Crea los siguientes sobrerecibos de todos los empleados del periodo
		DECLARE @OverdraftTable AS TABLE(
			[ID] [uniqueidentifier] NOT NULL,
			[Active] [bit] NOT NULL,
			[DeleteDate] [datetime2](7) NULL,
			[Timestamp] [datetime2](7) NOT NULL,
			[Name] [nvarchar](100) NOT NULL,
			[Description] [nvarchar](250) NOT NULL,
			[StatusID] [int] NOT NULL,
			[CreationDate] [datetime2](7) NOT NULL,
			[user] [uniqueidentifier] NOT NULL,
			[company] [uniqueidentifier] NOT NULL,
			[InstanceID] [uniqueidentifier] NOT NULL,
			[PeriodDetailID] [uniqueidentifier] NOT NULL,
			[EmployeeID] [uniqueidentifier] NOT NULL,
			[OverdraftType] [int] NOT NULL,
			[WorkingDays] [decimal](18, 6) NULL,
			[UUID] [uniqueidentifier] NOT NULL,
			[OverdraftStatus] [int] NOT NULL,
			[HistoricEmployeeID] [uniqueidentifier] NULL,
			[OverdraftPreviousCancelRelationshipID] [uniqueidentifier] NULL)

		INSERT INTO @OverdraftTable
		SELECT ID = newid(),
			   Active = 1,
			   DeleteDate = null,
			   [Timestamp] = getdate(),
			   [Name] = 'S',
			   [Description] = 'S',
			   StatusID = 1,
			   CreationDate = getdate(),
			   [user] = @user,
			   company = @company,
			   InstanceID = @InstanceId,
			   PeriodDetailId = @NextPeriodId,
			   EmployeeID = e.ID,
			   OverdraftType = 1,
			   WorkingDays = 0,
			   UUID = '00000000-0000-0000-0000-000000000000',
			   OverdraftStatus = 0,
			   HistoricEmployeeID = null,
			   OverdraftPreviousCancelRelationshipID = null
		FROM Employee e
		where e.InstanceID = @InstanceId and 
			  e.company = @company and 
			  e.PeriodTypeID = @PeriodTypeID and
			  e.LocalStatus = 0 and
			  e.EntryDate <= @NextPeriodFinalDate

		--Insert to Overdarft table
		INSERT INTO Overdraft
		SELECT * FROM @OverdraftTable

		DECLARE @ConceptPayments AS TABLE 
		(ConceptPaymentID UNIQUEIDENTIFIER NOT NULL,
		company UNIQUEIDENTIFIER NOT NULL,
		InstanceID UNIQUEIDENTIFIER NOT NULL 
		 )	 

		 INSERT INTO @ConceptPayments
		 SELECT cp.ID, cp.company, cp.InstanceID FROM ConceptPayment cp 
		 WHERE 
		  cp.InstanceID = @InstanceId and 
		  cp.company = @company and 
		  cp.GlobalAutomatic = 1

		INSERT INTO [dbo].[OverdraftDetail]
				   ([ID]
				   ,[Active]
				   ,[DeleteDate]
				   ,[Timestamp]
				   ,[Name]
				   ,[Description]
				   ,[StatusID]
				   ,[CreationDate]
				   ,[user]
				   ,[company]
				   ,[InstanceID]
				   ,[OverdraftID]
				   ,[ConceptPaymentID]
				   ,[Value]
				   ,[Amount]
				   ,[Label1]
				   ,[Label2]
				   ,[Label3]
				   ,[Label4]
				   ,[Taxed]
				   ,[Exempt]
				   ,[IMSSTaxed]
				   ,[IMSSExempt]
				   ,[IsGeneratedByPermanentMovement]
				   ,[IsValueCapturedByUser]
				   ,[IsTotalAmountCapturedByUser]
				   ,[IsAmount1CapturedByUser]
				   ,[IsAmount2CapturedByUser]
				   ,[IsAmount3CapturedByUser]
				   ,[IsAmount4CapturedByUser])
		SELECT 
			ID = newid(),
			Active = 1,
			DeleteDate = null,
			[Timestamp] = getdate(),
			[Name] = 'SD',
			[Description] = 'SD',
			StatusID = 1,
			CreationDate = getdate(),
			[user]= @user,
			company = @company,
			InstanceID = @InstanceId,
			OverdraftID = o.ID, 
			ConceptPaymentId = cp.ConceptPaymentID,
			[Value] = 0,
			[Amount] = 0,
			Label1 = '',
			Label2 = '',
			Label3 = '',
			Label4 = '',
			Taxed = 0,
			Exempt = 0,
			IMSSTaxed = 0,
			IMSSExempt = 0,
			IsGneratedByPermanent = 0,
			IsValueCapturedByUser = 0,
			IsTotalCBU = 0,
			IsAmount1CBU = 0,
			IsAmount2CBU = 0,
			IsAmount3CBU = 0,
			IsAmount4CBU = 0
			FROM @OverdraftTable o
			LEFT JOIN @ConceptPayments cp
				ON cp.InstanceID = @InstanceId and cp.company = @company
			WHERE o.InstanceID = @InstanceId and o.company = @company 
			ORDER BY o.Id, cp.ConceptPaymentID
END

--Apply next salary adjustments
EXEC ApplySalaryAdjustments @NextPeriodId, @InstanceId, @company, @user

--Vacations
EXEC CreateVacationsConceptsForAuhtorization @company, @InstanceId, @user, @NextPeriodId

--Aplicar montos pagados de créditos activos
EXEC ApplyConceptsRelation @InstanceId, @company, @user, @PeriodDetailId

--Agregar los conceptos de retenciones FONACOT, INFONAVIT, MOV_PERMANENTES, ETC
EXEC AddConceptPayment_FromEmployeeConceptsRelation @InstanceId, @company, @user

COMMIT TRAN T1;  
  --ROLLBACK TRAN T1;
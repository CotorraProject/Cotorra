CREATE PROCEDURE [dbo].[Report_Raya]
@EmployerRegistrationID uniqueidentifier = NULL,
@PeriodDetailID uniqueidentifier,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS

--Fecha inicial y final del periodo proporcionado
Declare @InitialDate Datetime
Declare @FinalDate Datetime
Declare @PeriodNumber int
Declare @PeriodName nvarchar(100)

SELECT @InitialDate = pd.InitialDate,
	   @FinalDate = pd.FinalDate,
	   @PeriodName = pt.Name,
	   @PeriodNumber = pd.Number
FROM PeriodDetail pd
INNER JOIN Period p
ON p.ID = pd.PeriodID
INNER JOIN PeriodType pt
ON pt.ID = p.PeriodTypeID
WHERE pd.ID = @PeriodDetailID and pd.InstanceID = @InstanceId and pd.company = @company

--Crea la estructura de la tabla temporal
DECLARE @EmployeeTable TABLE(
	[ID] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[DeleteDate] [datetime2](7) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[company] [uniqueidentifier] NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
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
	[JobPositionID] [uniqueidentifier] NOT NULL,
	[DepartmentID] [uniqueidentifier] NOT NULL,
	[PeriodTypeID] [uniqueidentifier] NOT NULL,
	[WorkshiftID] [uniqueidentifier] NOT NULL,
	[BankID] [uniqueidentifier] NULL,
	[EntryDate] [datetime2](7) NULL,
	[ReEntryDate] [datetime2](7) NULL,
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
	[EmployerRegistrationID] [uniqueidentifier] NULL,
	[ZipCode] [nvarchar](100) NULL,
	[FederalEntity] [nvarchar](100) NULL,
	[Municipality] [nvarchar](100) NULL,
	[Street] [nvarchar](100) NULL,
	[ExteriorNumber] [nvarchar](100) NULL,
	[InteriorNumber] [nvarchar](100) NULL,
	[Suburb] [nvarchar](100) NULL,
	[StatusID] [int] NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[IdentityUserID] [uniqueidentifier] NULL,
	[EmployeeTrustLevel] [int] NOT NULL,
	[SettlementSalaryBase] [decimal](18, 6) NOT NULL,
	[BankBranchNumber] [nvarchar](50) NULL,
	[CLABE] [nvarchar](20) NULL,
	[LocalStatus] [int] NOT NULL,
	[LastStatusChange] [datetime2](7) NOT NULL,
	[BenefitType] [int] NOT NULL,
	[ImmediateLeaderEmployeeID] [uniqueidentifier] NULL,
	[UnregisteredDate] [datetime2](7) NULL)

--Validamos si se proporcionó el registro patronal
IF (@EmployerRegistrationID is not null)
BEGIN
	INSERT INTO @EmployeeTable
	SELECT e.[ID],
	e.[Active] ,
	e.[DeleteDate],
	e.[Timestamp] ,
	e.[user] ,
	e.[company] ,
	e.[InstanceID] ,
	e.[RFC] ,
	e.[BirthDate] ,
	e.[NSS] ,
	e.[BornPlace] ,
	e.[CivilStatus] ,
	e.[Code] ,
	e.[CURP] ,
	e.[Name] ,
	e.[FirstLastName] ,
	e.[SecondLastName] ,
	e.[Gender] ,
	e.[JobPositionID] ,
	e.[DepartmentID] ,
	e.[PeriodTypeID] ,
	e.[WorkshiftID] ,
	e.[BankID] ,
	e.[EntryDate] ,
	e.[ReEntryDate] ,
	e.[ContractType] ,
	e.[DailySalary] ,
	e.[ContributionBase] ,
	e.[SBCFixedPart] ,
	e.[SBCVariablePart] ,
	e.[SBCMax25UMA] ,
	e.[PaymentBase] ,
	e.[PaymentMethod] ,
	e.[SalaryZone] ,
	e.[RegimeType] ,
	e.[Email] ,
	e.[UMF] ,
	e.[Phone],
	e.[BankAccount] ,
	e.[EmployerRegistrationID] ,
	e.[ZipCode] ,
	e.[FederalEntity] ,
	e.[Municipality] ,
	e.[Street],
	e.[ExteriorNumber],
	e.[InteriorNumber] ,
	e.[Suburb] ,
	e.[StatusID] ,
	e.[Description] ,
	e.[CreationDate] ,
	e.[IdentityUserID] ,
	e.[EmployeeTrustLevel] ,
	e.[SettlementSalaryBase] ,
	e.[BankBranchNumber],
	e.[CLABE] ,
	e.[LocalStatus] ,
	e.[LastStatusChange] ,
	e.[BenefitType] ,
	e.[ImmediateLeaderEmployeeID] ,
	e.[UnregisteredDate] FROM Employee e
	WHERE e.InstanceID = @InstanceId and e.company = @company
	and e.EmployerRegistrationID = @EmployerRegistrationID

	SELECT 
		pcc.SocialReason,  
		er.Code as EmployerRegistrationCode, 
		pcc.RFC,
		'Lista de Raya del ' + convert(varchar, @InitialDate, 5) + ' al ' + convert(varchar, @FinalDate, 5) as ReportTitle,
		'Periodo ' + @PeriodName + ' No. ' +  CAST(@PeriodNumber as VARCHAR(5)) as PeriodTitle
	FROM PayrollCompanyConfiguration pcc
	INNER JOIN EmployerRegistration er
	ON er.InstanceID = pcc.InstanceID 
	WHERE pcc.InstanceID = @InstanceId 
	
END
ELSE
BEGIN
	INSERT INTO @EmployeeTable
	SELECT e.[ID],
	e.[Active] ,
	e.[DeleteDate],
	e.[Timestamp] ,
	e.[user] ,
	e.[company] ,
	e.[InstanceID] ,
	e.[RFC] ,
	e.[BirthDate] ,
	e.[NSS] ,
	e.[BornPlace] ,
	e.[CivilStatus] ,
	e.[Code] ,
	e.[CURP] ,
	e.[Name] ,
	e.[FirstLastName] ,
	e.[SecondLastName] ,
	e.[Gender] ,
	e.[JobPositionID] ,
	e.[DepartmentID] ,
	e.[PeriodTypeID] ,
	e.[WorkshiftID] ,
	e.[BankID] ,
	e.[EntryDate] ,
	e.[ReEntryDate] ,
	e.[ContractType] ,
	e.[DailySalary] ,
	e.[ContributionBase] ,
	e.[SBCFixedPart] ,
	e.[SBCVariablePart] ,
	e.[SBCMax25UMA] ,
	e.[PaymentBase] ,
	e.[PaymentMethod] ,
	e.[SalaryZone] ,
	e.[RegimeType] ,
	e.[Email] ,
	e.[UMF] ,
	e.[Phone],
	e.[BankAccount] ,
	e.[EmployerRegistrationID] ,
	e.[ZipCode] ,
	e.[FederalEntity] ,
	e.[Municipality] ,
	e.[Street],
	e.[ExteriorNumber],
	e.[InteriorNumber] ,
	e.[Suburb] ,
	e.[StatusID] ,
	e.[Description] ,
	e.[CreationDate] ,
	e.[IdentityUserID] ,
	e.[EmployeeTrustLevel] ,
	e.[SettlementSalaryBase] ,
	e.[BankBranchNumber],
	e.[CLABE] ,
	e.[LocalStatus] ,
	e.[LastStatusChange] ,
	e.[BenefitType] ,
	e.[ImmediateLeaderEmployeeID] ,
	e.[UnregisteredDate] FROM Employee e
	WHERE e.InstanceID = @InstanceId and e.company = @company	

	SELECT 
		pcc.SocialReason,  
		'No seleccionado' as EmployerRegistrationCode,
		pcc.RFC,
		'Lista de Raya del ' + convert(varchar, @InitialDate, 5) + ' al ' + convert(varchar, @FinalDate, 5) as ReportTitle,
		'Periodo ' + @PeriodName + ' No. ' +  CAST(@PeriodNumber as VARCHAR(5)) as PeriodTitle
	FROM PayrollCompanyConfiguration pcc
	WHERE pcc.InstanceID = @InstanceId 
	
END

DECLARE @SalaryTable TABLE(
EmployeeID uniqueidentifier,
WorkingDays decimal(18,6),
Salary decimal(18,6),
ExtraHours decimal(18,6),
TotalPerceptions decimal(18,6),
TotalDeductions decimal(18,6),
TotalLiabilities decimal(18,6)
)

INSERT INTO @SalaryTable
SELECT 
	e.ID as EmployeeID,
	SUM(CASE WHEN cp.SATGroupCode = 'P-001' or cp.SATGroupCode = 'P-046' THEN od.[Value] ELSE 0 END) as [WorkingDays],
	SUM(CASE WHEN cp.Code = 1 and cp.ConceptType = 1 THEN od.[Amount] ELSE 0 END) as [Salary],
	SUM(CASE WHEN cp.Code = 4 and cp.ConceptType = 1 THEN od.[Value] ELSE 0 END) as [ExtraHours],
	CAST(SUM(CASE WHEN cp.ConceptType = 1 THEN od.[Amount] ELSE 0 END) AS NUMERIC(18,2)) as [TotalPerceptions],
	CAST(SUM(CASE WHEN cp.ConceptType = 3 THEN od.[Amount] ELSE 0 END) AS NUMERIC(18,2)) as [TotalDeductions],
	CAST(SUM(CASE WHEN cp.ConceptType = 2 THEN od.[Amount] ELSE 0 END) AS NUMERIC(18,2)) as [TotalLiabilities]
FROM @EmployeeTable e
INNER JOIN Overdraft o
ON o.EmployeeID = e.ID and o.PeriodDetailID = @PeriodDetailID and o.OverdraftType = 1 and o.InstanceID = @InstanceId and o.company = @company
INNER JOIN OverdraftDetail od
ON od.OverdraftID = o.ID and od.InstanceID = @InstanceId and od.company = @company
INNER JOIN ConceptPayment cp
ON cp.ID = od.ConceptPaymentID and cp.InstanceID = @InstanceId and cp.company = @company
WHERE e.InstanceID = @InstanceId and e.company = @company and
((cp.[Print] = 1 and cp.Kind = 0 and cp.ConceptType in (3,2)) or (cp.ConceptType = 1)) 
GROUP BY e.ID

SELECT 
	e.ID as EmployeeID,
	er.Code as EmployerRegistrationCode,
	e.Code as Code, 
	e.[Name], 
	e.FirstLastName, 
	e.SecondLastName, 
	jp.[Name] as JobPositionName,
	e.RFC,
	e.NSS,
	e.CURP,
	convert(varchar, e.EntryDate, 5) as EntryDate,	
	e.DailySalary,
	e.SBCFixedPart,
	CASE e.ContributionBase WHEN 1 THEN 'Fijo' WHEN 2 THEN 'Variable' ELSE 'MIXTO' END as ContributionBase,
	st.[WorkingDays],
	st.[WorkingDays] * w.Hours as WorkingHours,	
	w.Hours as WorkingHoursPerDay,
	st.ExtraHours,
	st.Salary,
	st.TotalPerceptions,
	st.TotalDeductions,
	st.TotalLiabilities,
	st.TotalPerceptions - st.TotalDeductions as Neto
FROM @EmployeeTable e
INNER JOIN JobPosition jp
ON jp.Id = e.JobPositionID
INNER JOIN @SalaryTable st
ON st.EmployeeID = e.ID
INNER JOIN Workshift w
ON w.ID = e.WorkshiftID
LEFT JOIN EmployerRegistration er
ON er.ID = e.EmployerRegistrationID

--Datos detalle de los sobrerecibos
SELECT e.ID as EmployeeID, cp.Code, cp.Name, 
CASE WHEN cp.ConceptType = 1 THEN 'Percepciones' 
     WHEN cp.ConceptType = 2 THEN 'Obligaciones'
     WHEN cp.ConceptType = 3 THEN 'Deducciones' END as ConceptType, 
CAST(od.Value AS NUMERIC(18,2)) as [Value], 
CAST(od.Amount AS NUMERIC(18,2)) as Amount,
cp.Kind as Kind
FROM ConceptPayment cp
INNER JOIN OverdraftDetail od
ON od.ConceptPaymentID = cp.ID
INNER JOIN Overdraft o
ON o.ID = od.OverdraftID and o.PeriodDetailID = @PeriodDetailID and o.OverdraftType = 1
INNER JOIN @EmployeeTable e
ON e.ID = o.EmployeeID
WHERE cp.InstanceID = @InstanceId and cp.company = @company and od.Amount != 0 and
((cp.ConceptType in (3,2)) or (cp.ConceptType = 1)) 

--Globales
SELECT 
	cp.ID,
	cp.Code,
	cp.Name,	 
   CASE WHEN cp.ConceptType = 1 THEN 'Percepciones' 
        WHEN cp.ConceptType = 2 THEN 'Obligaciones'
	    WHEN cp.ConceptType = 3 THEN 'Deducciones' END as ConceptType, 
	CAST(SUM(od.Amount) AS NUMERIC(18,2)) as Total,
	CASE WHEN cp.Kind = 1 THEN 1 ELSE 0 END as Kind
FROM Overdraft o
INNER JOIN OverdraftDetail od
ON od.OverdraftID = o.ID
INNER JOIN ConceptPayment cp
ON cp.ID = od.ConceptPaymentID
WHERE o.InstanceID = @InstanceId and o.company = @company  and 
o.PeriodDetailID = @PeriodDetailID and o.OverdraftType = 1 and od.Amount != 0 and
((cp.ConceptType in (3,2)) or (cp.ConceptType = 1)) 
GROUP BY cp.ID, cp.code, cp.ConceptType, cp.Name, cp.Kind

--Rubros IMSS
SELECT 	
    newid() as ID,
	cp.Code,	
	CASE WHEN cp.Code = 5 THEN 'Invalidez y Vida' 
	     WHEN cp.Code = 6 THEN 'Cesantia y Vejez' 
		 WHEN  cp.Code = 11 THEN 'Ret Enf. Gral'
		 END [Name],
	'' as ConceptType,
	CAST(Sum(CASE WHEN cp.ConceptType = 3 THEN od.Amount ELSE 0 END) AS NUMERIC(18,2)) as Empleado,
	CAST(Sum(CASE WHEN cp.ConceptType = 2 THEN od.Amount ELSE 0 END) AS NUMERIC(18,2)) as Empresa 
FROM Overdraft o
INNER JOIN OverdraftDetail od
ON od.OverdraftID = o.ID
INNER JOIN ConceptPayment cp
ON cp.ID = od.ConceptPaymentID
WHERE o.InstanceID = @InstanceId and 
o.PeriodDetailID = @PeriodDetailID and o.OverdraftType = 1 and od.Amount != 0 and
cp.Code in (5,6,11) and ConceptType in (2,3)
GROUP BY cp.code
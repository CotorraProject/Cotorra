CREATE PROCEDURE [dbo].[Report_Fonacot]
(
@InitialYear int,
@FinalYear int,
@InitialMonth int ,
@FinalMonth int ,
@CreditStatus int ,
@EmployeeStatus int,
@EmployeeFilter uniqueidentifier = NULL,
@InstanceId uniqueidentifier,
@company uniqueidentifier ,
@user uniqueidentifier
)
AS

DECLARE @InitialMonthWord nvarchar(100), @FinalMonthWord nvarchar(100)

--Get Month Description
SELECT @InitialMonthWord = dbo.getDescriptionMonth(@InitialMonth, 1)
SELECT @FinalMonthWord = dbo.getDescriptionMonth(@FinalMonth, 1)
 
DECLARE @EmployeeTable TABLE(
    [ID] [uniqueidentifier] NOT NULL,   
	[InstanceID]  [uniqueidentifier] NOT NULL,   
    [RFC] [nvarchar](13) NOT NULL,   
    [NSS] [nvarchar](11) NOT NULL,   
    [Code] [nvarchar](100) NOT NULL,
    [CURP] [nvarchar](18) NOT NULL,
    [Name] [nvarchar](150) NOT NULL,
    [FirstLastName] [nvarchar](150) NOT NULL,
    [SecondLastName] [nvarchar](150) NOT NULL,   
    [LocalStatus] [int] NOT NULL) 

DECLARE @FonacotTable TABLE(
[ID] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[DeleteDate] [datetime2](7) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Description] [nvarchar](250) NULL,
	[StatusID] [int] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[company] [uniqueidentifier] NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[EmployeeID] [uniqueidentifier] NOT NULL,
	[ConceptPaymentID] [uniqueidentifier] NOT NULL,
	[CreditNumber] [nvarchar](100) NOT NULL,
	[Month] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[RetentionType] [int] NOT NULL,
	[FonacotMovementStatus] [int] NOT NULL,
	[Observations] [nvarchar](4000) NULL,
	[EmployeeConceptsRelationID] [uniqueidentifier] NOT NULL
)

--Filtro de status del crédito
IF (@CreditStatus = -1)
BEGIN 
    INSERT INTO @FonacotTable
    SELECT * FROM FonacotMovement
    WHERE InstanceID = @InstanceId 
END
ELSE
BEGIN
    INSERT INTO @FonacotTable
    SELECT * FROM FonacotMovement
    WHERE InstanceID = @InstanceId and FonacotMovementStatus = @CreditStatus
END

--Filtro de empleados
IF (@EmployeeFilter is not null)
BEGIN
--Filtro de status de empleado (Todos)
IF (@EmployeeStatus <> -1)
    BEGIN     
        INSERT INTO @EmployeeTable
        SELECT e.[ID],   
        e.[InstanceID] ,
        e.[RFC] ,   
        e.[NSS] ,  
        e.[Code] ,
        e.[CURP] ,
        e.[Name] ,
        e.[FirstLastName] ,
        e.[SecondLastName] ,  
        e.[LocalStatus]
       FROM Employee e
        WHERE e.InstanceID = @InstanceId 
        and e.company = @company
        and e.ID = @EmployeeFilter 
        and e.LocalStatus = @EmployeeStatus
    END
ELSE
    BEGIN
        INSERT INTO @EmployeeTable
        SELECT e.[ID],   
        e.[InstanceID] ,
        e.[RFC] ,   
        e.[NSS] ,  
        e.[Code] ,
        e.[CURP] ,
        e.[Name] ,
        e.[FirstLastName] ,
        e.[SecondLastName] ,  
        e.[LocalStatus]
       FROM Employee e
        WHERE e.InstanceID = @InstanceId 
        and e.company = @company
        and e.ID = @EmployeeFilter
    END
END
ELSE
BEGIN
IF (@EmployeeStatus <> -1)
    BEGIN     
        INSERT INTO @EmployeeTable
         SELECT e.[ID],   
        e.[InstanceID] ,
        e.[RFC] ,   
        e.[NSS] ,  
        e.[Code] ,
        e.[CURP] ,
        e.[Name] ,
        e.[FirstLastName] ,
        e.[SecondLastName] ,  
        e.[LocalStatus]
       FROM Employee e
        WHERE e.InstanceID = @InstanceId 
        and e.company = @company
	    and e.LocalStatus = @EmployeeStatus
    END
ELSE 
    BEGIN
         INSERT INTO @EmployeeTable
         SELECT e.[ID],   
        e.[InstanceID] ,
        e.[RFC] ,   
        e.[NSS] ,  
        e.[Code] ,
        e.[CURP] ,
        e.[Name] ,
        e.[FirstLastName] ,
        e.[SecondLastName] ,  
        e.[LocalStatus]
       FROM Employee e
        WHERE e.InstanceID = @InstanceId 
        and e.company = @company
    END
END

--Encabezado del reporte
   SELECT 
        pcc.SocialReason,
        pcc.RFC,
        'Estado de cuenta créditos FONACOT ' as ReportTitle,
        ('De ' + @InitialMonthWord + ' ' + cast(@InitialYear as nvarchar(100)) + ' a ' + 
		@FinalMonthWord + ' ' + cast(@FinalYear as nvarchar(100)) ) as PeriodTitle
    FROM PayrollCompanyConfiguration pcc
    WHERE pcc.InstanceID = @InstanceId  

--Empleados / Resumen de crédito
select e.ID as EmployeeID, 
    e.Code, 
    (e.Name + ' ' + e.FirstLastName + ' ' + e.SecondLastName) as Fullname, 
	CASE WHEN e.LocalStatus = 0 THEN 'Activo' 
		 WHEN e.LocalStatus = 1 THEN 'Baja'
		 WHEN e.LocalStatus = 2 THEN 'Inactivo'
	END as EmployeeStatus, 
	CASE WHEN ecr.ConceptPaymentStatus = 0 THEN 'Inactivo'
	WHEN ecr.ConceptPaymentStatus = 1 THEN 'Activo'
	END as CreditStatus, 
    fm.CreditNumber, 
    fm.Description, 
    ecr.CreditAmount, 
    ecr.BalanceCalculated as ActualBalance,
	ecr.PaymentsMadeByOtherMethod,
	isnull((SELECT ecr.CreditAmount - ecr.PaymentsMadeByOtherMethod - SUM(ecrd2.AmountApplied) 
		FROM EmployeeConceptsRelationDetail ecrd2 
		INNER JOIN EmployeeConceptsRelation ecr2
		ON ecr.ID = ecrd2.EmployeeConceptsRelationID and ecr2.EmployeeID = e.ID
		INNER JOIN @FonacotTable fm2
		ON fm2.EmployeeConceptsRelationID = ecr2.ID and fm2.ID = fm.ID
		WHERE month(ecrd2.[PaymentDate]) < @InitialMonth and 		
		year(ecrd2.[PaymentDate]) <= @InitialYear),0) as BalancePreviousPeriods,
	isnull((SELECT SUM(ecrd2.AmountApplied) 
		FROM EmployeeConceptsRelationDetail ecrd2 
		INNER JOIN EmployeeConceptsRelation ecr2
		ON ecr2.ID = ecrd2.EmployeeConceptsRelationID and ecr2.EmployeeID = e.ID
		INNER JOIN @FonacotTable fm2
		ON fm2.EmployeeConceptsRelationID = ecr2.ID and fm2.ID = fm.ID
		WHERE month(ecrd2.[PaymentDate]) < @InitialMonth and 
		year(ecrd2.[PaymentDate]) <= @InitialYear),0) as TotalPaymentsPreviousPeriods
from @EmployeeTable e
INNER JOIN @FonacotTable fm
  ON fm.EmployeeID = e.ID
INNER JOIN EmployeeConceptsRelation as ecr
    on fm.EmployeeConceptsRelationID = ecr.ID 

--Creditos
SELECT year(pd.FinalDate) as [year], dbo.getDescriptionMonth(month(pd.FinalDate), 1) as [month],
month(pd.FinalDate) as monthNumber,
pert.Name as periodname, pd.Number as periodNumber,
fm.CreditNumber,
ecr.CreditAmount,
ecr.PaymentsMadeByOtherMethod,
e.ID as EmployeeID,
convert(varchar, pd.InitialDate, 5) as InitialDate, 
convert(varchar, pd.FinalDate, 5) as FinalDate, 
ecrd.AmountApplied as credit,
    (SELECT ecr.CreditAmount - ecr.PaymentsMadeByOtherMethod - SUM(ecrd2.AmountApplied) 
        FROM EmployeeConceptsRelationDetail ecrd2 
		inner join PeriodDetail pd2 on pd2.ID = pd.id 
		INNER JOIN EmployeeConceptsRelation ecr2
		ON ecr.ID = ecrd2.EmployeeConceptsRelationID and ecr2.EmployeeID = e.ID
		INNER JOIN @FonacotTable fm2 ON fm2.EmployeeConceptsRelationID = ecr2.ID and fm2.ID = fm.ID
		WHERE ecrd2.[PaymentDate] <= pd2.InitialDate and ecr2.EmployeeID = e.ID) as balance 
  FROM @FonacotTable as fm 
  inner join EmployeeConceptsRelation as ecr
    on fm.EmployeeConceptsRelationID = ecr.ID 
  inner join EmployeeConceptsRelationDetail as ecrd
    on ecrd.EmployeeConceptsRelationID = ecr.ID 
  inner join Overdraft as od
    on od.ID = ecrd.OverdraftID 
  inner join @EmployeeTable e
	on e.ID = od.EmployeeID
  inner join PeriodDetail as pd
    on pd.ID = od.PeriodDetailID 
  inner join Period as pInitial
    on pInitial.ID = pd.PeriodID and pInitial.FiscalYear = @InitialYear 
  inner join Period pFinal
    on pFinal.ID = pd.PeriodID and pInitial.FiscalYear = @FinalYear 
  inner join PeriodType as pert
    on pert.ID = pInitial.PeriodTypeID
WHERE year(pd.InitialDate) >= pInitial.FiscalYear and 
month(pd.InitialDate) >= @InitialMonth and 
month(pd.InitialDate) <= @FinalMonth and 
year(pd.InitialDate) <= pFinal.FiscalYear
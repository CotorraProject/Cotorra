CREATE PROCEDURE [dbo].[CancelAuthorizeOverdraft]
@OverdarftId uniqueidentifier,
@PeriodDetailId uniqueidentifier,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS


BEGIN TRAN T1_UnapplyAuth;  

--Period Initial and Final Date
DECLARE @PeriodInitialDate DATETIME
DECLARE @PeriodFinalDate DATETIME

SELECT @PeriodInitialDate = pd.InitialDate, @PeriodFinalDate = pd.FinalDate from PeriodDetail pd
where pd.ID = @PeriodDetailId

--PRINT N'1. Upapply salary adjustments'; 
----Unapply Salary adjustments
--DECLARE @EmployeeSalariesToUnApply guidlisttabletype

--INSERT INTO @EmployeeSalariesToUnApply
--SELECT esi.ID FROM EmployeeSalaryIncrease esi
--where esi.ModificationDate >= @PeriodInitialDate and esi.ModificationDate <= @PeriodFinalDate

--EXEC dbo.UnApplySalaryAdjustments @EmployeeSalariesToUnApply

--Obtiene todos los Overdraft -> HistoricEmployees a borrar
DECLARE @OverdraftTable TABLE
	(OverdraftID uniqueidentifier NOT NULL,
	HistoricEmployeeID uniqueidentifier NULL)

INSERT INTO @OverdraftTable
SELECT o.ID, o.HistoricEmployeeID FROM Overdraft o 
where o.PeriodDetailID = @PeriodDetailId and 
o.OverdraftStatus = 1 and --solo los autorizados
o.InstanceID = @InstanceId and o.company = @company

--Actualiza los sobrerecibos de ese periodo en status authorizado = 1 a none = 0
PRINT N'2. Update the overdrafts to None Status'; 
--2. Actualiza los overdraft en status none = 0 de sobrerecibos que esten en status autorizados = 1

UPDATE
		overdraft
	SET
		overdraft.OverdraftStatus = 0, --none
		overdraft.HistoricEmployeeID = null,
		overdraft.[Timestamp] = getdate(),
		overdraft.[user] = @user
	FROM
		Overdraft AS overdraft		
		INNER JOIN @OverdraftTable ot
		ON ot.OverdraftID = overdraft.ID

--3. Elimina los historicos de los empleados de ese periodo
PRINT N'3. Delete Historic Employees'; 
DELETE HistoricEmployee WHERE ID in (SELECT ot.HistoricEmployeeID from @OverdraftTable ot)

--4. Actualiza el periodo a Calculando
PRINT N'4. Update period to calculating'; 
UPDATE PeriodDetail Set PeriodStatus = 1, [Timestamp] = getdate() where ID = @PeriodDetailId

--5. Undo the application of accumulates per Employee, FiscalYear, Accumulate, Year
PRINT N'5. Undo the application of accumulates for each AccumulateType per Employee, Fiscal Year, Month'; 
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
WHERE o.PeriodDetailID = @PeriodDetailId and 
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
		ae.January = ae.January - att.TotalAmount,
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
		ae.February = ae.February - att.TotalAmount,
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
		ae.March = ae.March - att.TotalAmount,
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
		ae.April = ae.April - att.TotalAmount,
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
		ae.May = ae.May - att.TotalAmount,
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
		ae.June = ae.June - att.TotalAmount,
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
		ae.July = ae.July - att.TotalAmount,
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
		ae.August = ae.August - att.TotalAmount,
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
		ae.September = ae.September - att.TotalAmount,
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
		ae.October = ae.October - att.TotalAmount,
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
		ae.November = ae.November - att.TotalAmount,
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
		ae.December = ae.December - att.TotalAmount,
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


--6. Si es el ultimo periodo actualizar el period
--Update [Period] set IsActualFiscalYear = 0, IsFiscalYearClosed = 1 where ID = @PeriodID

COMMIT TRAN T1_UnapplyAuth;
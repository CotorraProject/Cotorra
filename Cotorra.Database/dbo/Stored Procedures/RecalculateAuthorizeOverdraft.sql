CREATE PROCEDURE [dbo].[RecalculateAuthorizeOverdraft]
@OverdarftId uniqueidentifier,
@PeriodDetailId uniqueidentifier,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS


BEGIN TRAN T1;  

PRINT N'5. Create Accumulates for each AccumulateType per Employee, Fiscal Year, Month'; 
--5. Accumulates per Employee, FiscalYear, Accumulate, Year
DECLARE @AccumulateTable AS TABLE 
(EmployeeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeName NVARCHAR(100) NOT NULL,
 TotalAmount DECIMAL(18,6) NOT NULL
 )	

 INSERT INTO @AccumulateTable
select o.EmployeeID, at.ID as AccumulatedTypeID, at.Name as AccumulatedTypeName, 
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
WHERE o.ID = @OverdarftId and o.PeriodDetailID = @PeriodDetailId and 
at.InstanceID = @InstanceId and at.company = @company 
group by o.EmployeeID, at.ID, at.Name, cpr.ConceptPaymentType
order by o.EmployeeID, at.Name

Declare @FiscalYear as integer
Declare @Month as integer
Declare @PeriodTypeID uniqueidentifier

SELECT @FiscalYear = p.FiscalYear, @Month = month(pd.InitialDate), @PeriodTypeID = p.PeriodTypeID from [Period] p
INNER JOIN PeriodDetail pd 
ON p.ID = pd.PeriodID and pd.InstanceID = @InstanceId and pd.company = @company 
where pd.ID = @PeriodDetailId  and p.InstanceID = @InstanceId and p.company = @company 

IF (@Month = 1)
	UPDATE
		ae
	SET
		ae.January = ae.January + att.TotalAmount,
		ae.[Timestamp] = getdate()
	FROM
		HistoricAccumulatedEmployee AS ae
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and o.PeriodDetailID = @PeriodDetailId and o.InstanceID = @InstanceId and o.company = @company
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
		INNER JOIN @AccumulateTable as att
			ON att.AccumulatedTypeID = ae.AccumulatedTypeID and att.EmployeeID = ae.EmployeeID
		INNER JOIN Overdraft o
			ON o.ID = @OverdarftId and o.EmployeeID = ae.EmployeeID and 
			o.PeriodDetailID = @PeriodDetailId and 
			o.InstanceID = @InstanceId and 
			o.company = @company
		where ae.InstanceID = @InstanceId and 
			ae.company = @company and 
			ae.ExerciseFiscalYear = @FiscalYear 


PRINT N'9. Update the overdrafts to None Status'; 
--9. Actualiza los overdraft en status de Autorizado = 1

UPDATE
		overdraft
	SET
		overdraft.OverdraftStatus = 1,
		overdraft.[Timestamp] = getdate()
	FROM
		Overdraft AS overdraft
		where overdraft.ID = @OverdarftId and overdraft.PeriodDetailID = @PeriodDetailId and 
		overdraft.InstanceID = @InstanceId and overdraft.company = @company


COMMIT TRAN T1;  
  --ROLLBACK TRAN T1;
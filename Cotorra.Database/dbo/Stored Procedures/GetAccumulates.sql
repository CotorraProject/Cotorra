CREATE PROCEDURE [dbo].[GetAccumulates]
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier,
@FiscalYear as integer
AS

DECLARE @HistoricAccumulatedEmployeeTable AS TABLE 
(
	[ID] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[DeleteDate] [datetime2](7) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[company] [uniqueidentifier] NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[StatusID] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[EmployeeID] [uniqueidentifier] NOT NULL,
	[AccumulatedTypeID] [uniqueidentifier] NOT NULL,
	[ExerciseFiscalYear] [int] NOT NULL,
	[InitialExerciseAmount] [decimal](18, 6) NOT NULL,
	[PreviousExerciseAccumulated] [decimal](18, 6) NOT NULL,
	[January] [decimal](18, 6) NOT NULL,
	[February] [decimal](18, 6) NOT NULL,
	[March] [decimal](18, 6) NOT NULL,
	[April] [decimal](18, 6) NOT NULL,
	[May] [decimal](18, 6) NOT NULL,
	[June] [decimal](18, 6) NOT NULL,
	[July] [decimal](18, 6) NOT NULL,
	[August] [decimal](18, 6) NOT NULL,
	[September] [decimal](18, 6) NOT NULL,
	[October] [decimal](18, 6) NOT NULL,
	[November] [decimal](18, 6) NOT NULL,
	[December] [decimal](18, 6) NOT NULL)	 

--Insertar en la tabla temporal los historic acumulados que debería tener el empleado
INSERT INTO @HistoricAccumulatedEmployeeTable([ID]
           ,[Active]
           ,[DeleteDate]
           ,[Timestamp]
           ,[user]
           ,[company]
           ,[InstanceID]
           ,[StatusID]
           ,[Name]
           ,[Description]
           ,[CreationDate]
           ,[EmployeeID]
           ,[AccumulatedTypeID]
           ,[ExerciseFiscalYear]
           ,[InitialExerciseAmount]
           ,[PreviousExerciseAccumulated]
           ,[January]
           ,[February]
           ,[March]
           ,[April]
           ,[May]
           ,[June]
           ,[July]
           ,[August]
           ,[September]
           ,[October]
           ,[November]
           ,[December])
SELECT 
	ID = newid(),
	Active = 1,
	DeleteDate = null,
	[Timestamp] = getdate(),
	[user] = @user,
	company = @company,
	InstanceID = @InstanceId,
	StatusID = 1,
	[Name] = 'ae',
	[Description] = 'ae',
	CreationDate = getdate(),
	EmployeeID = e.ID, 
	AccumulatedTypeID = at.ID,
	ExerciseFiscalYear = @FiscalYear,
	InitialExerciseAmount = 0,
	PreviousExerciseAccumulated = 0,
	January = 0,
	Febrary = 0,
	March = 0,
	April = 0,
	May = 0,
	June = 0,
	July = 0,
	August = 0,
	September = 0,
	October = 0,
	November = 0,
	December = 0
FROM Employee e
LEFT JOIN AccumulatedType at
ON at.company = @company and at.InstanceID = @InstanceId
WHERE e.company = @company and e.InstanceID = @InstanceId


DECLARE @AccumulateTable AS TABLE 
(EmployeeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeName NVARCHAR(100) NOT NULL,
 ConceptPaymentType int not null,
 TotalAmount DECIMAL(18,6) NOT NULL,
 MonthDate INT NOT NULL
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
	end as TotalAmount, month(pd.FinalDate) MonthDate
from [AccumulatedType] at
INNER JOIN ConceptPaymentRelationship cpr
	ON cpr.AccumulatedTypeID = at.ID and cpr.InstanceID =  @InstanceId and cpr.company = @company
INNER JOIN ConceptPayment cp
	ON cp.ID = cpr.ConceptPaymentID and cp.InstanceID = @InstanceId and cp.company = @company
INNER JOIN OverdraftDetail od
	ON od.ConceptPaymentID = cp.ID and od.InstanceID = @InstanceId and od.company = @company
INNER JOIN Overdraft o
	ON o.ID = od.OverdraftID and o.InstanceID =  @InstanceId and o.company = @company 
INNER JOIN PeriodDetail pd
	ON pd.ID = o.PeriodDetailID
INNER JOIN [Period] p
	ON p.ID = pd.PeriodID
WHERE p.FiscalYear = @FiscalYear and
at.InstanceID = @InstanceId and at.company = @company 
group by o.EmployeeID, at.ID, at.Name, cpr.ConceptPaymentType, month(pd.FinalDate)

DECLARE @AccumulateTable_Grouped AS TABLE 
(EmployeeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeID UNIQUEIDENTIFIER NOT NULL,
 AccumulatedTypeName NVARCHAR(100) NOT NULL,
 TotalAmount DECIMAL(18,6) NOT NULL,
 MonthDate INT NOT NULL
 )	

 INSERT INTO @AccumulateTable_Grouped
select EmployeeID, AccumulatedTypeID, AccumulatedTypeName, sum(TotalAmount) as TotalAmount, MonthDate
from @AccumulateTable
group by EmployeeID, AccumulatedTypeID, AccumulatedTypeName, MonthDate

Declare @PeriodID uniqueidentifier
Declare @Month as integer
Declare @PeriodTypeID uniqueidentifier


select  
	EmployeeID, AccumulatedTypeID, AccumulatedTypeName, [1] as January, [2] as February, [3] as March, [4] as April,
	[5] as May, [6] as June, [7] as July, [8] as August, [9] as September, [10] as October, [11] as November, [12] as December
from @AccumulateTable_Grouped atg 
PIVOT(Sum(atg.TotalAmount) FOR [MonthDate] IN([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])) AS PivotTable;
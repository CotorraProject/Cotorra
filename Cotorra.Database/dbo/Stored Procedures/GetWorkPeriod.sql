CREATE PROCEDURE [dbo].[GetWorkPeriod]
@InstanceId uniqueidentifier,
@company uniqueidentifier, 
@PeriodDetailID uniqueidentifier,
@OverdraftID uniqueidentifier = NULL
AS

SELECT
o.ID as 'OverdraftID',
o.OverdraftStatus as 'OverdraftStatus',
o.OverdraftType as 'OverdraftType',
o.EmployeeID as 'EmployeeID',
o.UUID 'UUID',
SUM(case when cp.ConceptType = 1 then od.Amount end) as 'TotalPerceptions',
SUM(case when cp.ConceptType = 3 then od.Amount end) as 'TotalDeductions',
SUM(case when cp.ConceptType = 2 then od.Amount end) as 'TotalLiabilities'
FROM OverdraftDetail od 
inner join Overdraft o on od.OverdraftID = o.ID
inner join ConceptPayment cp on od.ConceptPaymentID = cp.ID
WHERE 
od.company = @company and
od.InstanceID = @InstanceId and
o.PeriodDetailID = @PeriodDetailID and
cp.Kind = 0 and
(@OverdraftID IS NULL OR o.ID = @OverdraftID)
   
group by o.ID, o.OverdraftStatus, o.OverdraftType, o.EmployeeID, o.UUID
 
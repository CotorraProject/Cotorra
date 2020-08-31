CREATE PROCEDURE [dbo].[ApplyConceptsRelation]
@instanceID uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier,
@PeriodDetailId uniqueidentifier
AS

BEGIN TRAN T_ApplyConceptRetention;  

UPDATE ecrd 
SET [ConceptsRelationPaymentStatus] = 1
FROM EmployeeConceptsRelationDetail ecrd
INNER JOIN Overdraft o
  ON o.ID = ecrd.OverdraftID
INNER JOIN OverdraftDetail od
  ON od.OverdraftID = o.ID
INNER JOIN PeriodDetail pd
  ON pd.ID = o.PeriodDetailID
INNER JOIN EmployeeConceptsRelation ecr
  ON ecr.EmployeeID = o.EmployeeID
WHERE ecr.ConceptPaymentID = od.ConceptPaymentID and o.PeriodDetailID = @PeriodDetailId


COMMIT TRAN T_ApplyConceptRetention;
CREATE PROCEDURE [dbo].[RemoveConceptPayment_FromEmployeeConceptsRelation]
@instanceID uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS

BEGIN TRAN T_RemoveConceptRetention;  

DECLARE @ConceptPaymentsTable AS TABLE 
(ConceptPaymentID UNIQUEIDENTIFIER NOT NULL,
ValueOverdraft decimal(18,6) NULL,
AmountOverdraft decimal(18,6) NOT NULL,
company UNIQUEIDENTIFIER NOT NULL,
InstanceID UNIQUEIDENTIFIER NOT NULL,
EmployeeID UNIQUEIDENTIFIER NOT NULL)	 

--Update the credit status FONACOT if balance is 0
UPDATE fm SET
fm.FonacotMovementStatus = 0
FROM FonacotMovement fm
INNER JOIN EmployeeConceptsRelation ecr
ON ecr.ID = fm.EmployeeConceptsRelationID
WHERE ecr.BalanceCalculated <= 0

--Update the credit status EmployeeConceptsRelation if balance is 0
UPDATE EmployeeConceptsRelation SET
ConceptPaymentStatus = 0
WHERE BalanceCalculated <= 0

 --Todos los conceptos para retención inactivos a eliminar
 INSERT INTO @ConceptPaymentsTable
 SELECT ecr.ConceptPaymentID, ecr.OverdraftDetailValue, ecr.OverdraftDetailAmount, 
	    ecr.company, ecr.InstanceID, ecr.EmployeeID FROM EmployeeConceptsRelation ecr 
  WHERE   
  ecr.InstanceID = @InstanceId and 
  ecr.company = @company and 
  (ecr.ConceptPaymentStatus = 0 or ecr.BalanceCalculated <= 0) --inactivos o saldados

--Todos los sobrerecibos pendientes (sin autorización, sin timbrado, sin cancelación)
--que no tenga en sus detalles (conceptos de retenciones)

Declare @overdraftsIdsTable as Table(
ID uniqueidentifier NOT NULL)

INSERT INTO @overdraftsIdsTable
select distinct od.OverdraftID from OverdraftDetail od
INNER JOIN Overdraft o
ON o.ID = od.OverdraftID and o.OverdraftStatus = 0
INNER JOIN @ConceptPaymentsTable cpt
ON cpt.ConceptPaymentID = od.ConceptPaymentID 
where 
od.InstanceID = @instanceID and 
od.company = @company and
o.EmployeeID = cpt.EmployeeID 


IF NOT EXISTS (SELECT TOP 1 od.ID FROM OverdraftDetail od
	INNER JOIN @overdraftsIdsTable oids
	  ON oids.ID = od.OverdraftID
	INNER JOIN Overdraft o
	  ON o.ID = oids.ID and o.OverdraftStatus = 0
	INNER JOIN @ConceptPaymentsTable cpt
	  ON cpt.ConceptPaymentID = od.ConceptPaymentID and  cpt.EmployeeID = o.EmployeeID
	INNER JOIN EmployeeConceptsRelationDetail ecrd
	  ON ecrd.OverdraftID = o.ID
	INNER JOIN EmployeeConceptsRelation ecr
	  ON ecr.ID = ecrd.EmployeeConceptsRelationID 
	INNER JOIN FonacotMovement fm
	  ON fm.EmployeeConceptsRelationID = ecr.ID and fm.FonacotMovementStatus = 1)
BEGIN
	DELETE od
	FROM OverdraftDetail od
	INNER JOIN @overdraftsIdsTable oids
	  ON oids.ID = od.OverdraftID
	INNER JOIN Overdraft o
	  ON o.ID = oids.ID and o.OverdraftStatus = 0
	INNER JOIN @ConceptPaymentsTable cpt
	  ON cpt.ConceptPaymentID = od.ConceptPaymentID and  cpt.EmployeeID = o.EmployeeID
	INNER JOIN EmployeeConceptsRelationDetail ecrd
	  ON ecrd.OverdraftID = o.ID
	INNER JOIN EmployeeConceptsRelation ecr
	  ON ecr.ID = ecrd.EmployeeConceptsRelationID 
	INNER JOIN FonacotMovement fm
	  ON fm.EmployeeConceptsRelationID = ecr.ID 
END

 DELETE ecrd 
 FROM EmployeeConceptsRelationDetail ecrd
 INNER JOIN @overdraftsIdsTable oids
   ON oids.ID = ecrd.OverdraftID
INNER JOIN EmployeeConceptsRelation ecr
  ON ecr.ID = ecrd.EmployeeConceptsRelationID 
INNER JOIN FonacotMovement fm
  ON fm.EmployeeConceptsRelationID = ecr.ID and fm.FonacotMovementStatus = 0

COMMIT TRAN T_RemoveConceptRetention;
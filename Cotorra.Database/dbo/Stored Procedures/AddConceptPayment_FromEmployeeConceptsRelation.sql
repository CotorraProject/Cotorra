CREATE PROCEDURE [dbo].[AddConceptPayment_FromEmployeeConceptsRelation]
@instanceID uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS

BEGIN TRAN T_AddConceptRetention;  

DECLARE @ConceptPaymentsTable AS TABLE 
(
ConceptPaymentID UNIQUEIDENTIFIER NOT NULL,
EmployeeID UNIQUEIDENTIFIER NOT NULL)	 

 --Todos los conceptos para retención activos
 INSERT INTO @ConceptPaymentsTable
 SELECT distinct ecr.ConceptPaymentID, ecr.EmployeeID FROM EmployeeConceptsRelation ecr  
  WHERE   
  ecr.InstanceID = @InstanceId and 
  ecr.company = @company and 
  ecr.ConceptPaymentStatus = 1 --activos
  GROUP BY ecr.ID, ecr.ConceptPaymentID, ecr.company, ecr.InstanceID, ecr.EmployeeID

Declare @OverdraftTable as Table(
	[OverdraftID] [uniqueidentifier] NOT NULL,	
	[InstanceID] [uniqueidentifier] NOT NULL,	
	[EmployeeID] [uniqueidentifier] NOT NULL,
	[ConceptPaymentID] [uniqueidentifier] NOT NULL)

--Todos los sobrerecibos pendientes (sin autorización, sin timbrado, sin cancelación)
--que no tenga en sus detalles (conceptos de retenciones)

INSERT INTO @OverdraftTable
select DISTINCT o.ID, o.InstanceID, o.EmployeeID, ecr.[ConceptPaymentID] from overdraft o
INNER JOIN PeriodDetail pd
 ON o.PeriodDetailID = pd.ID
INNER JOIN @ConceptPaymentsTable cpt
ON cpt.EmployeeID = o.EmployeeID
INNER JOIN  EmployeeConceptsRelation ecr
 ON ecr.EmployeeID = o.EmployeeID and ecr.ConceptPaymentID = cpt.ConceptPaymentID
where  pd.PeriodStatus = 1 and 
       o.OverdraftStatus = 0 and
       ecr.InitialCreditDate <= pd.FinalDate and
       o.InstanceID = @instanceID and 
	   o.company = @company and o.ID not in 
		(select distinct od2.OverdraftID from OverdraftDetail od2 
		 INNER JOIN Overdraft o2
		 ON cpt.EmployeeID = o2.EmployeeID and o2.PeriodDetailID = pd.ID and ecr.EmployeeID = o2.EmployeeID
		 WHERE ecr.ConceptPaymentID = od2.ConceptPaymentID)

--Inserta el detalle del sobre-recibo con los conceptos
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
	[Name] = 'Auto_ByConcept_Retention',
	[Description] = 'Auto_ByConcept_Retention',
	StatusID = 1,
	CreationDate = getdate(),
	[user]= @user,
	company = @company,
	InstanceID = @InstanceId,
	OverdraftID = o.OverdraftID, 
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
	LEFT JOIN @ConceptPaymentsTable cp
		ON cp.EmployeeID = o.EmployeeID and o.ConceptPaymentID = cp.ConceptPaymentID
	WHERE o.InstanceID = @InstanceId  

--Eliminar los conceptos de retenciones FONACOT, INFONAVIT, MOV_PERMANENTES, ETC Inactivos
EXEC RemoveConceptPayment_FromEmployeeConceptsRelation @InstanceId, @company, @user

COMMIT TRAN T_AddConceptRetention;
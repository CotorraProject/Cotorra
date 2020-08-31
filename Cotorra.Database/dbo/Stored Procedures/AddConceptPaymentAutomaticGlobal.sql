CREATE PROCEDURE [dbo].[AddConceptPaymentAutomaticGlobal]
@instanceID uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier,
@guidConceptPayments dbo.guidlisttabletype READONLY
AS

BEGIN TRAN T_AddConcept;  

DECLARE @ConceptPaymentsTable AS TABLE 
(ConceptPaymentID UNIQUEIDENTIFIER NOT NULL,
company UNIQUEIDENTIFIER NOT NULL,
InstanceID UNIQUEIDENTIFIER NOT NULL)	 

 --Todos los conceptos automático global (inyectados los ids)
 INSERT INTO @ConceptPaymentsTable
 SELECT cp.ID, cp.company, cp.InstanceID FROM ConceptPayment cp 
 INNER JOIN @guidConceptPayments gcp
 ON gcp.ID = cp.ID
 WHERE   
  cp.InstanceID = @InstanceId and 
  cp.company = @company and 
  cp.GlobalAutomatic = 1

Declare @OverdraftTable as Table(
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

--Todos los sobrerecibos pendientes (sin autorización, sin timbrado, sin cancelación)
--que no tenga en sus detalles (conceptos inyectados automáticos global)

Declare @overdraftsIdsTable as Table(ID uniqueidentifier NOT NULL)

INSERT INTO @overdraftsIdsTable
select distinct od.OverdraftID from OverdraftDetail od
where 
od.InstanceID = @instanceID and 
od.company = @company and
od.ConceptPaymentID in (SELECT cpt.ConceptPaymentID FROM @ConceptPaymentsTable cpt 
	where cpt.ConceptPaymentID = od.ConceptPaymentID)

INSERT INTO @OverdraftTable
select o.* from overdraft o
INNER JOIN PeriodDetail pd
 ON o.PeriodDetailID = pd.ID
where  pd.PeriodStatus = 1 and 
       o.InstanceID = @instanceID and 
	   o.company = @company and 
	   o.id not in (select ot.ID from @overdraftsIdsTable ot)

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
	[Name] = 'Auto_ByConcept',
	[Description] = 'Auto_ByConcept',
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
	LEFT JOIN @ConceptPaymentsTable cp
		ON cp.InstanceID = @InstanceId and cp.company = @company
	WHERE o.InstanceID = @InstanceId and o.company = @company 
	ORDER BY o.Id, cp.ConceptPaymentID

select ot.ID FROM @OverdraftTable ot

COMMIT TRAN T_AddConcept;
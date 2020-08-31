CREATE PROCEDURE [dbo].[CreateVacationsConcepts]
@company uniqueidentifier, 
@InstanceID uniqueidentifier,
@user uniqueidentifier,
@EmployeeID uniqueidentifier, 
@InitialDate datetime,
@FinalDate datetime,
@Days decimal(18,6) = NULL, 
@PrimaDays decimal(18,6) = NULL
AS
BEGIN TRAN T_ConceptVacations


DECLARE @ConceptPaymentTable TABLE([ID] [uniqueidentifier] NOT NULL, 
InstanceID [uniqueidentifier] NOT NULL,
Code int not null)

--Vacaciones reportadas a tiempo y Prima
IF (@Days is not null)
BEGIN
	IF NOT EXISTS (
		SELECT o.ID
		from Overdraft o
		INNER JOIN PeriodDetail pd
		ON pd.ID = o.PeriodDetailID
		INNER JOIN OverdraftDetail od
		ON od.OverdraftID = o.ID
		INNER JOIN ConceptPayment cp
		ON cp.ID = od.ConceptPaymentID
		WHERE o.EmployeeID = @EmployeeID and 
		@InitialDate >= pd.InitialDate and 
		@InitialDate <= pd.FinalDate and 
		cp.Code = 19 and cp.ConceptType = 1 and
		pd.PeriodStatus = 1
		)
	BEGIN
		INSERT INTO @ConceptPaymentTable
		SELECT cp.ID, cp.InstanceID, cp.Code FROM ConceptPayment cp where cp.ConceptType = 1 and (Code = 19)
		and InstanceID = @InstanceID and company = @company
	END
END

IF (@PrimaDays is not null)
BEGIN
IF NOT EXISTS (
		SELECT o.ID
		from Overdraft o
		INNER JOIN PeriodDetail pd
		ON pd.ID = o.PeriodDetailID
		INNER JOIN OverdraftDetail od
		ON od.OverdraftID = o.ID
		INNER JOIN ConceptPayment cp
		ON cp.ID = od.ConceptPaymentID
		WHERE o.EmployeeID = @EmployeeID and 
		@InitialDate >= pd.InitialDate and 
		@InitialDate <= pd.FinalDate and 
		cp.Code = 20 and cp.ConceptType = 1 and
		pd.PeriodStatus = 1
		)
	BEGIN
		INSERT INTO @ConceptPaymentTable
		SELECT cp.ID, cp.InstanceID, cp.Code FROM ConceptPayment cp where cp.ConceptType = 1 and (Code = 20)
		and InstanceID = @InstanceID and company = @company
	END

IF NOT EXISTS (
		SELECT o.ID
		from Overdraft o
		INNER JOIN PeriodDetail pd
		ON pd.ID = o.PeriodDetailID
		INNER JOIN OverdraftDetail od
		ON od.OverdraftID = o.ID
		INNER JOIN ConceptPayment cp
		ON cp.ID = od.ConceptPaymentID
		WHERE o.EmployeeID = @EmployeeID and 
		@InitialDate >= pd.InitialDate and 
		@InitialDate <= pd.FinalDate and 
		cp.Code = 43 and cp.ConceptType = 3 and
		pd.PeriodStatus = 1
		)
	BEGIN
		INSERT INTO @ConceptPaymentTable
		SELECT cp.ID, cp.InstanceID, cp.Code FROM ConceptPayment cp where cp.ConceptType = 3 and (Code = 43)
		and InstanceID = @InstanceID and company = @company
	END
END

INSERT INTO OverdraftDetail ([ID]
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
SELECT [ID] = newid()
      ,[Active] = 1
      ,[DeleteDate] = null
      ,[Timestamp] = getdate()
      ,[Name] = ''
      ,[Description] = ''
      ,[StatusID] = 1
      ,[CreationDate] = getdate()
      ,[user] = @user
      ,[company] = @company
      ,[InstanceID] = @InstanceID
      ,[OverdraftID] = o.ID
      ,[ConceptPaymentID] = cpt.ID
      ,[Value] = CASE WHEN cpt.Code = 19 THEN @Days WHEN cpt.Code = 20 THEN @PrimaDays ELSE 0 END
      ,[Amount] = 0
      ,[Label1] = ''
      ,[Label2] = ''
      ,[Label3] = ''
      ,[Label4] = ''
      ,[Taxed] = 0
      ,[Exempt] = 0
      ,[IMSSTaxed] = 0
      ,[IMSSExempt] = 0
      ,[IsGeneratedByPermanentMovement] = 0
      ,[IsValueCapturedByUser] = 0
      ,[IsTotalAmountCapturedByUser] = 0
      ,[IsAmount1CapturedByUser] = 0
      ,[IsAmount2CapturedByUser] = 0
      ,[IsAmount3CapturedByUser] = 0
      ,[IsAmount4CapturedByUser] = 0
from Overdraft o
INNER JOIN PeriodDetail pd
ON pd.ID = o.PeriodDetailID
INNER JOIN @ConceptPaymentTable cpt
ON cpt.InstanceID = pd.InstanceID
WHERE o.EmployeeID = @EmployeeID and @InitialDate >= pd.InitialDate and @InitialDate <= pd.FinalDate

COMMIT TRAN T_ConceptVacations
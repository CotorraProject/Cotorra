CREATE PROCEDURE [dbo].[CreateVacationsConceptsForAuhtorization]
@company uniqueidentifier, 
@InstanceID uniqueidentifier,
@user uniqueidentifier,
@PeriodDetailID uniqueidentifier
AS
BEGIN TRAN T_ConceptVacationsForAuth

DECLARE @InitialDate datetime
DECLARE @FinalDate datetime

SELECT @InitialDate = pd.InitialDate, 
	   @FinalDate = pd.FinalDate from PeriodDetail pd
	   WHERE pd.ID = @PeriodDetailID

DECLARE @VacationTable TABLE(
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
	[VacationsCaptureType] [int] NOT NULL,
	[InitialDate] [datetime2](7) NOT NULL,
	[FinalDate] [datetime2](7) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[Break_Seventh_Days] [decimal](18, 6) NOT NULL,
	[VacationsBonusDays] [decimal](18, 6) NOT NULL,
	[VacationsDays] [decimal](18, 6) NOT NULL,
	[VacationsBonusPercentage] [decimal](18, 6) NOT NULL)

DECLARE @EmployeeIdsWithVacationsWithPrima TABLE(EmployeeID [uniqueidentifier] NOT NULL)
DECLARE @EmployeeIdsWithVacationsWithoutPrima TABLE(EmployeeID [uniqueidentifier] NOT NULL)

INSERT INTO @EmployeeIdsWithVacationsWithPrima
select distinct EmployeeID from Vacation v
WHERE v.InitialDate >= @InitialDate and v.InitialDate <= @FinalDate and v.VacationsCaptureType = 0

INSERT INTO @EmployeeIdsWithVacationsWithoutPrima
select distinct EmployeeID from Vacation v
WHERE v.InitialDate >= @InitialDate and v.InitialDate <= @FinalDate and v.VacationsCaptureType <> 0

INSERT INTO @VacationTable
select v.* from Vacation v
WHERE v.InitialDate >= @InitialDate and v.InitialDate <= @FinalDate

IF EXISTS (SELECT Top 1 ID from @VacationTable)
BEGIN

	DECLARE @ConceptPaymentTable TABLE([ID] [uniqueidentifier] NOT NULL, 
	InstanceID [uniqueidentifier] NOT NULL,
	Code int not null)

	--Vacaciones reportadas a tiempo y Prima
		
			INSERT INTO @ConceptPaymentTable
			SELECT cp.ID, cp.InstanceID, cp.Code FROM ConceptPayment cp where cp.ConceptType = 1 and (Code = 19)
			and InstanceID = @InstanceID and company = @company
	
	--Prima de vacaciones
			INSERT INTO @ConceptPaymentTable
			SELECT cp.ID, cp.InstanceID, cp.Code FROM ConceptPayment cp where cp.ConceptType = 1 and (Code = 20)
			and InstanceID = @InstanceID and company = @company
		
		--insert with prima and vacations
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
		  ,[Value] = 0
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
	INNER JOIN @EmployeeIdsWithVacationsWithPrima ei
		ON ei.EmployeeID = o.EmployeeID
	WHERE pd.ID = @PeriodDetailID and (cpt.Code = 19 or cpt.Code = 20)

	--insert only vacations
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
		  ,[Value] = 0
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
	INNER JOIN @EmployeeIdsWithVacationsWithoutPrima ei
		ON ei.EmployeeID = o.EmployeeID
	WHERE pd.ID = @PeriodDetailID and (cpt.Code = 19)

END
COMMIT TRAN T_ConceptVacationsForAuth
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE PROCEDURE [dbo].[ApplySalaryAdjustments]
@PeriodDetailId uniqueidentifier,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS

BEGIN TRAN T_ApplySalaryAdjustments

--PeriodStatus
DECLARE @PeriodStatus int
DECLARE @PeriodDetailInitialDate DateTime
DECLARE @PeriodDetailFinalDate DateTime

--Consulta los periodos
SELECT @PeriodStatus = pd.PeriodStatus, 
	   @PeriodDetailInitialDate = pd.InitialDate, 
	   @PeriodDetailFinalDate = pd.FinalDate
FROM PeriodDetail pd
where pd.ID = @PeriodDetailID and pd.InstanceID = @InstanceId

--Si el periodo está en calculando se modifican los sueldos
IF (@PeriodStatus = 1)
BEGIN
--Get Into variable all of modifications between the specifed period
	DECLARE @EmployeeSalaryIncrease AS TABLE (
	[ID] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[DeleteDate] [datetime2](7) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[company] [uniqueidentifier] NOT NULL,
	[StatusID] [int] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[ModificationDate] [datetime2](7) NOT NULL,
	[EmployeeID] [uniqueidentifier] NOT NULL,
	[DailySalary] [decimal](18, 6) NULL,
	[EmployeeSBCAdjustmentID]  [uniqueidentifier] NULL)

	--Todas las modificaciones, siempre y cuando no exista otra en la misma fecha
	INSERT INTO @EmployeeSalaryIncrease
	SELECT esi.* FROM [dbo].[EmployeeSalaryIncrease] as esi 
	where esi.InstanceID = @InstanceId and esi.ModificationDate >= @PeriodDetailInitialDate and esi.ModificationDate <= @PeriodDetailFinalDate 
	and esi.ModificationDate not in (select hsa.ModificationDate FROM HistoricEmployeeSalaryAdjustment hsa 
		where hsa.ModificationDate = esi.ModificationDate and hsa.InstanceID = @InstanceId and hsa.EmployeeID = esi.EmployeeID)

	IF EXISTS (SELECT ID FROM @EmployeeSalaryIncrease)
	BEGIN
	--Inserta el historico del ajuste al sueldo (anterior)
	INSERT INTO HistoricEmployeeSalaryAdjustment (
		   [ID]
		  ,[Active]
		  ,[DeleteDate]
		  ,[Timestamp]
		  ,[user]
		  ,[company]
		  ,[StatusID]
		  ,[Name]
		  ,[Description]
		  ,[CreationDate]
		  ,[InstanceID]
		  ,[ModificationDate]
		  ,[EmployeeID]
		  ,[DailySalary])
	SELECT 
		   ID = newid()
		  ,Active = 1
		  ,DeleteDate = null
		  ,[Timestamp] = getdate()
		  ,[user] = @user
		  ,company = @company
		  ,StatusID = 1
		  ,[Name] = ''
		  ,[Description] = ''
		  ,CreationDate = getdate()
		  ,InstanceID = @InstanceId
		  ,ModificationDate = esi.ModificationDate
		  ,EmployeeID = esi.EmployeeID
		  ,DailySalary = e.DailySalary
	FROM Employee e
	INNER JOIN @EmployeeSalaryIncrease esi
		ON e.ID = esi.EmployeeID

	--Actualiza los datos del empleado
	UPDATE e
	SET e.DailySalary = esi.DailySalary		
	FROM Employee as e
	INNER JOIN @EmployeeSalaryIncrease as esi 
	ON  esi.EmployeeID = e.ID 
	END

	--Si el periodo está en calculando se modifican los sueldos
IF (@PeriodStatus = 1)
BEGIN
--Get Into variable all of modifications between the specifed period
	DECLARE @EmployeeSBCAdjustment AS TABLE (
	[ID] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[DeleteDate] [datetime2](7) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[company] [uniqueidentifier] NOT NULL,
	[StatusID] [int] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[ModificationDate] [datetime2](7) NOT NULL,
	[EmployeeID] [uniqueidentifier] NOT NULL,
	[SBCFixedPart] DECIMAL(18, 6)  NULL, 
	[SBCVariablePart] DECIMAL(18, 6)   NULL, 
	[SBCMax25UMA] DECIMAL(18, 6)   NULL)

	--Todas las modificaciones, siempre y cuando no exista otra en la misma fecha
	INSERT INTO @EmployeeSBCAdjustment
	SELECT esi.* FROM [dbo].[EmployeeSBCAdjustment] as esi 
	where esi.InstanceID = @InstanceId and esi.ModificationDate >= @PeriodDetailInitialDate and esi.ModificationDate <= @PeriodDetailFinalDate 
	and esi.ModificationDate not in (select hsa.ModificationDate FROM HistoricEmployeeSBCAdjustment hsa 
		where hsa.ModificationDate = esi.ModificationDate and hsa.InstanceID = @InstanceId  and hsa.EmployeeID = esi.EmployeeID)

	IF EXISTS (SELECT ID FROM @EmployeeSBCAdjustment)
	BEGIN
	--Inserta el historico del ajuste al sueldo (anterior)
	INSERT INTO HistoricEmployeeSBCAdjustment (
		   [ID]
		  ,[Active]
		  ,[DeleteDate]
		  ,[Timestamp]
		  ,[user]
		  ,[company]
		  ,[StatusID]
		  ,[Name]
		  ,[Description]
		  ,[CreationDate]
		  ,[InstanceID]
		  ,[ModificationDate]
		  ,[EmployeeID]
		  ,[SBCFixedPart]  
		  ,[SBCVariablePart]  
	      ,[SBCMax25UMA] )
	SELECT 
		   ID = newid()
		  ,Active = 1
		  ,DeleteDate = null
		  ,[Timestamp] = getdate()
		  ,[user] = @user
		  ,company = @company
		  ,StatusID = 1
		  ,[Name] = ''
		  ,[Description] = ''
		  ,CreationDate = getdate()
		  ,InstanceID = @InstanceId
		  ,ModificationDate = esi.ModificationDate
		  ,EmployeeID = esi.EmployeeID
		  ,SBCFixedPart = e.SBCFixedPart
		  ,SBCVariablePart = e.SBCVariablePart
		  ,SBCMax25UMA = e.SBCMax25UMA
	FROM Employee e
	INNER JOIN @EmployeeSBCAdjustment esi
		ON e.ID = esi.EmployeeID

	--Actualiza los datos del empleado
	UPDATE e
	SET e.SBCFixedPart = esi.SBCFixedPart		
	, e.SBCVariablePart = esi.SBCVariablePart		
	, e.SBCMax25UMA = esi.SBCMax25UMA		
	FROM Employee as e
	INNER JOIN @EmployeeSBCAdjustment as esi 
	ON  esi.EmployeeID = e.ID 
	END
	END
END

COMMIT TRAN T_ApplySalaryAdjustments
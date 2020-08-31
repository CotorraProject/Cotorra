
CREATE PROCEDURE [dbo].[UnApplySalaryAdjustments]
@EmployeeSalaryIncreaseIds  dbo.guidlisttabletype READONLY
AS

BEGIN TRAN TUnApplySalaryAdjustment;  

--Un apply salary adjustment of the calculating period
IF EXISTS (SELECT ID FROM @EmployeeSalaryIncreaseIds)
BEGIN
	DECLARE @EmployeeSalaryIncreaseTable AS TABLE(
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
	[EmployeeSBCAdjustmentID] [uniqueidentifier] NULL,
	[SBCFixedPart] [decimal](18, 6) NULL,
	[SBCVariablePart] [decimal](18, 6) NULL,
	[SBCMax25UMA] [decimal](18, 6) NULL)

	INSERT INTO @EmployeeSalaryIncreaseTable
	SELECT esi.*, esbc.SBCFixedPart, esbc.[SBCVariablePart], esbc.[SBCMax25UMA] FROM EmployeeSalaryIncrease esi
	inner join @EmployeeSalaryIncreaseIds esit
	on esi.ID = esit.ID
	inner join EmployeeSBCAdjustment esbc
	on esbc.ID = esi.EmployeeSBCAdjustmentID


	DECLARE @HistoricEmployeeSalaryAdjustmentTable as TABLE(
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
	[EmployeeSBCAdjustmentID] [uniqueidentifier] NULL,
	[SBCFixedPart] [decimal](18, 6) NULL,
	[SBCVariablePart] [decimal](18, 6) NULL,
	[SBCMax25UMA] [decimal](18, 6) NULL,
	[HistoricEmployeeSBCAdjustmentID] [uniqueidentifier] NULL)


	INSERT INTO @HistoricEmployeeSalaryAdjustmentTable
	SELECT hesa.*, hesbc.SBCFixedPart, hesbc.[SBCVariablePart], hesbc.[SBCMax25UMA], hesbc.[ID] from HistoricEmployeeSalaryAdjustment hesa
	INNER JOIN @EmployeeSalaryIncreaseTable esit
	ON hesa.EmployeeID = esit.EmployeeID and hesa.ModificationDate = esit.ModificationDate
	inner join HistoricEmployeeSBCAdjustment hesbc
	ON hesa.EmployeeID = hesbc.EmployeeID and hesa.ModificationDate = hesbc.ModificationDate

	IF EXISTS (SELECT ID FROM @HistoricEmployeeSalaryAdjustmentTable)
	BEGIN
		--update employee with the previous salary 
		UPDATE e 
		SET e.DailySalary = hesa.DailySalary,
			e.SBCFixedPart = hesa.SBCFixedPart,
			e.SBCMax25UMA = hesa.SBCMax25UMA,
			e.SBCVariablePart = hesa.SBCVariablePart
		FROM Employee e
		INNER JOIN @HistoricEmployeeSalaryAdjustmentTable hesa
		ON hesa.EmployeeID = e.ID

				 --delete historic sbc adjustment
		delete hsbc from HistoricEmployeeSBCAdjustment hsbc
		where hsbc.ID in (SELECT HistoricEmployeeSBCAdjustmentID FROM @HistoricEmployeeSalaryAdjustmentTable)
		
		--delete historic salary adjustment
		delete hesa from HistoricEmployeeSalaryAdjustment hesa
		where hesa.ID in (SELECT ID FROM @HistoricEmployeeSalaryAdjustmentTable)



	END
	  
END


COMMIT TRAN TUnApplySalaryAdjustment;  
 --ROLLBACK TRAN TUnApplySalaryAdjustment;
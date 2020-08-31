
CREATE PROCEDURE [dbo].[UnApplySBCAdjustments]
@EmployeeSBCAdjustmentsIds  dbo.guidlisttabletype READONLY
AS

BEGIN TRAN TUnApplySBCAdjustments;  

--Un apply salary adjustment of the calculating period
IF EXISTS (SELECT ID FROM @EmployeeSBCAdjustmentsIds)
BEGIN
	DECLARE @EmployeeSBCAdjustmentTable AS TABLE(
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
	[SBCFixedPart] [decimal](18, 6) NULL,
	[SBCVariablePart] [decimal](18, 6) NULL,
	[SBCMax25UMA] [decimal](18, 6) NULL)

	INSERT INTO @EmployeeSBCAdjustmentTable
	SELECT esb.* FROM EmployeeSBCAdjustment esb
	inner join @EmployeeSBCAdjustmentsIds esit
	on esb.ID = esit.ID 


	DECLARE @HistoricEmployeeSBCAdjustmentTable as TABLE(
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
	[SBCFixedPart] [decimal](18, 6) NULL,
	[SBCVariablePart] [decimal](18, 6) NULL,
	[SBCMax25UMA] [decimal](18, 6) NULL)


	INSERT INTO @HistoricEmployeeSBCAdjustmentTable
	SELECT hesa.* from HistoricEmployeeSBCAdjustment hesa
	INNER JOIN @EmployeeSBCAdjustmentTable esit
	ON hesa.EmployeeID = esit.EmployeeID and hesa.ModificationDate = esit.ModificationDate
	

	IF EXISTS (SELECT ID FROM @HistoricEmployeeSBCAdjustmentTable)
	BEGIN
		--update employee with the previous salary 
		UPDATE e 
		SET 
			e.SBCFixedPart = hesa.SBCFixedPart,
			e.SBCMax25UMA = hesa.SBCMax25UMA,
			e.SBCVariablePart = hesa.SBCVariablePart
		FROM Employee e
		INNER JOIN @HistoricEmployeeSBCAdjustmentTable hesa
		ON hesa.EmployeeID = e.ID

		 --delete historic sbc adjustment
		delete hsbc from HistoricEmployeeSBCAdjustment hsbc
		where hsbc.ID in (SELECT ID FROM @HistoricEmployeeSBCAdjustmentTable)
		

	END
	  
END


COMMIT TRAN TUnApplySBCAdjustments;  
 --ROLLBACK TRAN TUnApplySBCAdjustments;
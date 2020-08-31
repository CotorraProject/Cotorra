CREATE PROCEDURE [dbo].[CreateOverdraftByEmployee]
@EmployeeID uniqueidentifier,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS
BEGIN
	 
	BEGIN TRY 
		BEGIN TRANSACTION; 
		--Get PeriodType of Employee
		Declare @PeriodTypeID uniqueidentifier
		SELECT @PeriodTypeID = e.PeriodTypeID FROM Employee e
		WHERE e.ID = @EmployeeID and e.company = @company and e.InstanceID = @InstanceId

		--Get PeriodDetailID and PeriodFinalDate from PeriodType where status is Calculating = 1
		Declare @PeriodDetailID uniqueidentifier
		Declare @PeriodFinalDate datetime
		SELECT @PeriodDetailID = pd.ID, @PeriodFinalDate = pd.FinalDate FROM PeriodDetail pd
		INNER JOIN [Period] p
		ON p.ID = pd.PeriodID and p.company = @company and p.InstanceID = @InstanceId  
		WHERE pd.company = @company and pd.InstanceID = @InstanceId  and
		p.PeriodTypeID = @PeriodTypeID and pd.PeriodStatus = 1

		IF (@PeriodDetailID is null)
			RAISERROR ('El Periodo que está asignado el empleado no está habilitado o no se encuentra en estatus de calculando', 
					   16, -- Severity.  
					   1 -- State.  
					   );  

		IF EXISTS (
					SELECT o.ID FROM Overdraft o 
					WHERE o.EmployeeID = @EmployeeID and 
					PeriodDetailID = @PeriodDetailID and
					o.OverdraftStatus != 99
					)
			RAISERROR ('El empleado ya tiene un sobrerecibo para el periodo proporcionado', 
					   16, -- Severity.  
					   1 -- State.  
					   );  

		PRINT N'Crear el sobrerecibo del empleado seleccionado para el periodo calculating'; 
		--10. Crea los siguientes sobrerecibos de todos los empleados del periodo
		DECLARE @OverdraftTable AS TABLE(
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

		INSERT INTO @OverdraftTable
		SELECT ID = newid(),
			   Active = 1,
			   DeleteDate = null,
			   [Timestamp] = getdate(),
			   [Name] = 'S',
			   [Description] = 'S',
			   StatusID = 1,
			   CreationDate = getdate(),
			   [user] = @user,
			   company = @company,
			   InstanceID = @InstanceId,
			   PeriodDetailId = @PeriodDetailID,
			   EmployeeID = e.ID,
			   OverdraftType = 1,
			   WorkingDays = 0,
			   UUID = '00000000-0000-0000-0000-000000000000',
			   OverdraftStatus = 0,
			   HistoricEmployeeID = null,
			   [OverdraftPreviousCancelRelationshipID] = null
		FROM Employee e
		where e.ID = @EmployeeID and 
			  e.InstanceID = @InstanceId and 
			  e.company = @company and 
			  e.EntryDate <= @PeriodFinalDate

		--Insert to Overdarft table
		INSERT INTO Overdraft
		SELECT * FROM @OverdraftTable

		DECLARE @ConceptPayments AS TABLE 
		(ConceptPaymentID UNIQUEIDENTIFIER NOT NULL,
		company UNIQUEIDENTIFIER NOT NULL,
		InstanceID UNIQUEIDENTIFIER NOT NULL 
		 )	 

		 INSERT INTO @ConceptPayments
		 SELECT cp.ID, cp.company, cp.InstanceID FROM ConceptPayment cp 
		 WHERE 
		  cp.InstanceID = @InstanceId and 
		  cp.company = @company and 
		  cp.GlobalAutomatic = 1

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
			[Name] = 'SD',
			[Description] = 'SD',
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
			LEFT JOIN @ConceptPayments cp
				ON cp.InstanceID = @InstanceId and cp.company = @company
			WHERE o.InstanceID = @InstanceId and o.company = @company 
			ORDER BY o.Id, cp.ConceptPaymentID

			COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		
		DECLARE @ErrorMessage NVARCHAR(4000);  
		DECLARE @ErrorSeverity INT;  
		DECLARE @ErrorState INT;  
  
		SELECT   
			@ErrorMessage = ERROR_MESSAGE(),  
			@ErrorSeverity = ERROR_SEVERITY(),  
			@ErrorState = ERROR_STATE();  
  
		
		-- Use RAISERROR inside the CATCH block to return error  
		-- information about the original error that caused  
		-- execution to jump to the CATCH block.  
		RAISERROR (@ErrorMessage, -- Message text.  
				   @ErrorSeverity, -- Severity.  
				   @ErrorState -- State.  
				   );  
		ROLLBACK TRANSACTION;
	END CATCH;
END
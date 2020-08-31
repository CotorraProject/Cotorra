CREATE PROCEDURE [dbo].[CreateCancelationStamp]
@Cancelstampoverdraft dbo.cancelstampoverdrafttabletype READONLY,
@CancelationRequestXMLID uniqueidentifier,
@CancelationResponseXMLID uniqueidentifier,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS
BEGIN
	 
	BEGIN TRY 
		BEGIN TRANSACTION; 
		
		Declare @cancelationFiscalDocumentID uniqueidentifier
		SET @cancelationFiscalDocumentID = newid()

		--Create CancelationFiscal
		INSERT INTO CancelationFiscalDocument
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
		  ,[CancelationRequestXMLID]
		  ,[CancelationResponseXMLID])		
		SELECT [ID] = @cancelationFiscalDocumentID
		  ,[Active] = 1
		  ,[DeleteDate] = null
		  ,[Timestamp] = getdate()
		  ,[Name] = ''
		  ,[Description] = ''
		  ,[StatusID] = 1
		  ,[CreationDate] = getdate()
		  ,[user] = @user
		  ,[company] = @company
		  ,[InstanceID] = @InstanceId
		  ,[CancelationRequestXMLID] = @CancelationRequestXMLID
		  ,[CancelationResponseXMLID] = @CancelationResponseXMLID

		INSERT INTO CancelationFiscalDocumentDetail ([ID]
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
		  ,[CancelationFiscalDocumentID]
		  ,[OverdraftID]
		  ,[CancelationFiscalDocumentStatus])
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
		  ,[InstanceID] = @InstanceId
		  ,[CancelationFiscalDocumentID] = @cancelationFiscalDocumentID
		  ,[OverdraftID] = cso.[OverdraftID]
		  ,[CancelationFiscalDocumentStatus] = cso.[CancelationFiscalDocumentStatus]
		   FROM @Cancelstampoverdraft cso

		

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
			   [Name] = 'S_ByCancelation',
			   [Description] = 'S_ByCancelation',
			   StatusID = 1,
			   CreationDate = getdate(),
			   [user] = @user,
			   company = @company,
			   InstanceID = @InstanceId,
			   PeriodDetailId = o.PeriodDetailID,
			   EmployeeID = o.EmployeeID,
			   OverdraftType = o.OverdraftType,
			   WorkingDays = o.WorkingDays,
			   UUID = '00000000-0000-0000-0000-000000000000',
			   OverdraftStatus = 1, --Authorized
			   HistoricEmployeeID = o.HistoricEmployeeID,
			   [OverdraftPreviousCancelRelationshipID] = o.ID
		FROM Overdraft o
		INNER JOIN @Cancelstampoverdraft cso
		ON cso.[OverdraftID] = o.ID
		where 
			  o.InstanceID = @InstanceId and 
			  o.company = @company

		--Insert to Overdarft table
		INSERT INTO Overdraft
		SELECT * FROM @OverdraftTable

	
		--Insert details of the overdraft
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
			ConceptPaymentId = od.ConceptPaymentID,
			[Value] = od.[Value],
			[Amount] = od.Amount,
			Label1 = od.Label1,
			Label2 = od.Label2,
			Label3 = od.Label3,
			Label4 = od.Label4,
			Taxed = od.Taxed,
			Exempt = od.Exempt,
			IMSSTaxed = od.IMSSTaxed,
			IMSSExempt = od.IMSSExempt,
			IsGeneratedByPermanentMovement = od.IsGeneratedByPermanentMovement,
			IsValueCapturedByUser = od.IsValueCapturedByUser,
			IsTotalCBU = od.IsTotalAmountCapturedByUser,
			IsAmount1CBU = od.IsAmount1CapturedByUser,
			IsAmount2CBU = od.IsAmount2CapturedByUser,
			IsAmount3CBU = od.IsAmount3CapturedByUser,
			IsAmount4CBU = od.IsAmount4CapturedByUser
			FROM OverdraftDetail od					
			INNER JOIN Overdraft o
				ON o.OverdraftPreviousCancelRelationshipID = od.[OverdraftID]
			INNER JOIN @Cancelstampoverdraft cso
				ON cso.[OverdraftID] = o.OverdraftPreviousCancelRelationshipID
			WHERE od.InstanceID = @InstanceId and od.company = @company 
			ORDER BY od.OverdraftID, od.ConceptPaymentID

			--UPDATE PREVIOUS OVERDRAFT TO Cancelled (99)
			UPDATE o
				SET o.OverdraftStatus = 99 --Cancelled
			FROM Overdraft o
				INNER JOIN @Cancelstampoverdraft cso
				ON cso.[OverdraftID] = o.ID

			--UPDATE ALL PERIOD DETAILS TO AUTHORIZED
			UPDATE pd
				SET pd.PeriodStatus = 2 --Authorized
			FROM PeriodDetail pd
				INNER JOIN Overdraft o
				ON o.PeriodDetailID = pd.ID
				INNER JOIN @Cancelstampoverdraft cso
				ON cso.[OverdraftID] = o.ID

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
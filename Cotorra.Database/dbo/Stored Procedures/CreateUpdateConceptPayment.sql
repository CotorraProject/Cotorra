
CREATE PROCEDURE [dbo].[CreateUpdateConceptPayment]
@OverdraftId uniqueidentifier,
@ConceptPaymentId uniqueidentifier,
@Amount decimal(18,6),
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS
BEGIN TRAN T_CreateUpdateConceptPayment

DECLARE @OverdraftDetailTable TABLE(
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
	[OverdraftID] [uniqueidentifier] NOT NULL,
	[ConceptPaymentID] [uniqueidentifier] NOT NULL,
	[Value] [decimal](18, 6) NOT NULL,
	[Amount] [decimal](18, 6) NOT NULL,
	[Label1] [nvarchar](500) NOT NULL,
	[Label2] [nvarchar](500) NOT NULL,
	[Label3] [nvarchar](500) NOT NULL,
	[Label4] [nvarchar](500) NOT NULL,
	[Taxed] [decimal](18, 6) NOT NULL,
	[Exempt] [decimal](18, 6) NOT NULL,
	[IMSSTaxed] [decimal](18, 6) NOT NULL,
	[IMSSExempt] [decimal](18, 6) NOT NULL,
	[IsGeneratedByPermanentMovement] [bit] NOT NULL,
	[IsValueCapturedByUser] [bit] NOT NULL,
	[IsTotalAmountCapturedByUser] [bit] NOT NULL,
	[IsAmount1CapturedByUser] [bit] NOT NULL,
	[IsAmount2CapturedByUser] [bit] NOT NULL,
	[IsAmount3CapturedByUser] [bit] NOT NULL,
	[IsAmount4CapturedByUser] [bit] NOT NULL)

INSERT INTO @OverdraftDetailTable 
	  SELECT 
	   [ID] = newid()
      ,[Active] = 1
      ,[DeleteDate] = null
      ,[Timestamp] = getdate()
      ,[Name] = 'Ajuste al neto (autogenerado)'
      ,[Description] = 'Ajuste al neto (autogenerado)'
      ,[StatusID]= 1
      ,[CreationDate] = getdate()
      ,[user] = @user
      ,[company] = @company
      ,[InstanceID] = @InstanceId
      ,[OverdraftID] = @OverdraftId
      ,[ConceptPaymentID] = @ConceptPaymentId
      ,[Value] = 0
      ,[Amount] = @Amount
      ,[Label1] = 'Gravado'
      ,[Label2] = 'Exento'
      ,[Label3] = 'IMSS Gravado'
      ,[Label4] = 'IMSS Exento'
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

--MERGING DATA
MERGE OverdraftDetail od 
	USING @OverdraftDetailTable odt
	ON (odt.[ConceptPaymentID] = od.[ConceptPaymentID] and odt.[OverdraftID] = od.[OverdraftID]) 
WHEN MATCHED
	THEN UPDATE SET 
        od.[Amount] = odt.[Amount]
WHEN NOT MATCHED BY TARGET 
	THEN INSERT ([ID]
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
      VALUES(odt.[ID]
      ,odt.[Active]
      ,odt.[DeleteDate]
      ,odt.[Timestamp]
      ,odt.[Name]
      ,odt.[Description]
      ,odt.[StatusID]
      ,odt.[CreationDate]
      ,odt.[user]
      ,odt.[company]
      ,odt.[InstanceID]
      ,odt.[OverdraftID]
      ,odt.[ConceptPaymentID]
      ,odt.[Value]
      ,odt.[Amount]
      ,odt.[Label1]
      ,odt.[Label2]
      ,odt.[Label3]
      ,odt.[Label4]
      ,odt.[Taxed]
      ,odt.[Exempt]
      ,odt.[IMSSTaxed]
      ,odt.[IMSSExempt]
      ,odt.[IsGeneratedByPermanentMovement]
      ,odt.[IsValueCapturedByUser]
      ,odt.[IsTotalAmountCapturedByUser]
      ,odt.[IsAmount1CapturedByUser]
      ,odt.[IsAmount2CapturedByUser]
      ,odt.[IsAmount3CapturedByUser]
      ,odt.[IsAmount4CapturedByUser]);

SELECT od.ID FROM OverdraftDetail od
WHERE od.InstanceID = @InstanceId and od.company = @company 
and ConceptPaymentID = @ConceptPaymentId
and od.OverdraftID = @OverdraftId

COMMIT TRAN T_CreateUpdateConceptPayment
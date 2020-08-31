

CREATE   VIEW [dbo].[vwFonacotMovement]
AS
select [ID]
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
      ,[EmployeeID]
      ,[ConceptPaymentID]
      ,[CreditNumber]
      ,[Month]
      ,[Year]
      ,[RetentionType]
      ,[FonacotMovementStatus]
      ,[Observations]
      ,[EmployeeConceptsRelationID]
      from FonacotMovement fm
     
﻿CREATE VIEW [dbo].[vwEmployeeConceptsRelation]
	AS 
SELECT [ID]
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
      ,[CreditAmount]
      ,[OverdraftDetailValue]
      ,[OverdraftDetailAmount]
      ,[PaymentsMadeByOtherMethod]
      ,[ConceptPaymentStatus]
      ,[BalanceCalculated]
      ,[AccumulatedAmountWithHeldCalculated]
      ,[InitialCreditDate] FROM
    [EmployeeConceptsRelation]

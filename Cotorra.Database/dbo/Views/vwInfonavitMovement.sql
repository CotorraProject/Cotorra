

CREATE   VIEW [dbo].[vwInfonavitMovement]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[EmployeeID]
      ,[CreditNumber]
      ,[InfonavitCreditType]
      ,[InitialApplicationDate]
      ,[RegisterDate]
      ,[MonthlyFactor]
      ,[IncludeInsurancePayment_D14]
      ,[AccumulatedAmount]
      ,[AppliedTimes]
      ,[InfonavitStatus] from InfonavitMovement
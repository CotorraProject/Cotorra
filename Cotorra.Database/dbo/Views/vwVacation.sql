
CREATE   VIEW [dbo].[vwVacation]
AS
select [ID]
      ,[Active]    
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[EmployeeID]
      ,[VacationsCaptureType]
      ,[InitialDate]
      ,[FinalDate]
      ,[PaymentDate]
      ,[Break_Seventh_Days]
      ,[VacationsBonusDays]
      ,[VacationsDays] from Vacation
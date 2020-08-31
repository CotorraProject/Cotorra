

CREATE   VIEW [dbo].[vwVacationDaysOff]
AS
select [ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[VacationID]
      ,[Date] from VacationDaysOff
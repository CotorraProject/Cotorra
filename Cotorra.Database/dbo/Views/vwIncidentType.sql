

CREATE   VIEW [dbo].[vwIncidentType]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[Code]
      ,[TypeOfIncident]
      ,[UnitValue]
      ,[Percentage]
      ,[ItConsiders]
      ,[SalaryRight]
      ,[DecreasesSeventhDay] from IncidentType
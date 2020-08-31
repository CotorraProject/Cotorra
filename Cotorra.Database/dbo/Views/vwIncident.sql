

CREATE   VIEW [dbo].[vwIncident]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[IncidentTypeID]
      ,[PeriodDetailID]
      ,[EmployeeID]
      ,[Date]
      ,[Value] from Incident
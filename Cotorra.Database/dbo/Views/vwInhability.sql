

CREATE   VIEW [dbo].[vwInhability]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[Folio]
      ,[IncidentTypeID]
      ,[AuthorizedDays]
      ,[InitialDate]
      ,[CategoryInsurance]
      ,[RiskType]
      ,[Percentage]
      ,[Consequence]
      ,[InhabilityControl]
      ,[EmployeeID] from Inhability
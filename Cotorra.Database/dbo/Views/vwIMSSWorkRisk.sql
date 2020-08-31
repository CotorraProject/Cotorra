

CREATE   VIEW [dbo].[vwIMSSWorkRisk]
AS
select [ID]
      ,[Active]      
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[ValidityDate]
      ,[Value] from IMSSWorkRisk
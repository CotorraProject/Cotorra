

CREATE   VIEW [dbo].[vwMinimunSalary]
AS
select [ID]
      ,[Active]      
      ,[user]
      ,[company]
      ,[ExpirationDate]
      ,[InstanceID]
      ,[ZoneA]
      ,[ZoneB]
      ,[ZoneC] from MinimunSalary
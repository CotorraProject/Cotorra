

CREATE   VIEW [dbo].[vwSettlementCatalog]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[ValidityDate]
      ,[Code]
      ,[Number]
      ,[CASUSMO]
      ,[CASISR86]
      ,[CalDirecPerc]
      ,[Indem90]
      ,[Indem20]
      ,[PrimaAntig] from SettlementCatalog
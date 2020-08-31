


CREATE   VIEW [dbo].[vwAnualEmploymentSubsidy]
AS
select [ID]    
      ,[Active]
      ,[user]
      ,[company]
      ,[LowerLimit]
      ,[AnualSubsidy]
      ,[InstanceID]
      ,[ValidityDate] from AnualEmploymentSubsidy
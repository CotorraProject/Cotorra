

CREATE   VIEW [dbo].[vwMonthlyEmploymentSubsidy]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[LowerLimit]
      ,[MonthlySubsidy]
      ,[InstanceID]
      ,[ValidityDate] from MonthlyEmploymentSubsidy
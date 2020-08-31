

CREATE   VIEW [dbo].[vwMonthlyIncomeTax]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[LowerLimit]
      ,[FixedFee]
      ,[Rate]
      ,[InstanceID]
      ,[ValidityDate] from MonthlyIncomeTax


CREATE   VIEW [dbo].[vwAnualIncomeTax]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[LowerLimit]
      ,[FixedFee]
      ,[Rate]
      ,[ValidityDate] from AnualIncomeTax
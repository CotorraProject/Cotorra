

CREATE   VIEW [dbo].[vwInfonavitInsurance]
AS
SELECT [ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[CreationDate]
      ,[Value]
      ,[ValidityDate]
  FROM [dbo].[InfonavitInsurance]
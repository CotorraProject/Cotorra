

CREATE   VIEW [dbo].[vwUMI]
AS
SELECT [ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[CreationDate]
      ,[Value]
      ,[ValidityDate]
  FROM [dbo].[UMI]
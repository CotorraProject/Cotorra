



CREATE   VIEW [dbo].[vwConceptPayment]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[Code]
      ,[ConceptType]
      ,[GlobalAutomatic]
      ,[AutomaticDismissal]
      ,[Kind]
      ,[Print]
      ,[SATGroupCode]     
      ,[Formula]
      ,[Formula1]
      ,[Formula2]
      ,[Formula3]
      ,[Formula4] from ConceptPayment
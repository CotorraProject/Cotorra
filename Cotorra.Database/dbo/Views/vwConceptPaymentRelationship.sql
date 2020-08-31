


CREATE   VIEW [dbo].[vwConceptPaymentRelationship]
AS
select [ID]
      ,[Active]      
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[ConceptPaymentID]
      ,[AccumulatedTypeID]
      ,[ConceptPaymentRelationshipType]
      ,[ConceptPaymentType] from ConceptPaymentRelationship
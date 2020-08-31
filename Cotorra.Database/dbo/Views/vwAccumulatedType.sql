


CREATE   VIEW [dbo].[vwAccumulatedType]
AS
select ID, 
       Active, 	   
	   [user], 
	   company, 
	   InstanceID,
	   TypeOfAccumulated, 
	   Code
 from AccumulatedType
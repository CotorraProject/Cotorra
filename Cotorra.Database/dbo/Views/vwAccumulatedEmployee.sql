



CREATE   VIEW [dbo].[vwAccumulatedEmployee]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]   
      ,[EmployeeID]
      ,[AccumulatedTypeID]
      ,[ExerciseFiscalYear]
      ,[InitialExerciseAmount]
      ,[PreviousExerciseAccumulated]
      ,[January]
      ,[February]
      ,[March]
      ,[April]
      ,[May]
      ,[June]
      ,[July]
      ,[August]
      ,[September]
      ,[October]
      ,[November]
      ,[December] from HistoricAccumulatedEmployee
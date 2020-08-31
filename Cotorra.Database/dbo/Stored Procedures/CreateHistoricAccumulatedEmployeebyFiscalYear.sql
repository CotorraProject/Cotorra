CREATE PROCEDURE [dbo].[CreateHistoricAccumulatedEmployeebyFiscalYear]
@FiscalYear int,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS
BEGIN TRAN THistoricAccumulatedEmployee;  

 DECLARE @HistoricAccumulatedEmployeeTable AS TABLE 
(
	[ID] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[DeleteDate] [datetime2](7) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[company] [uniqueidentifier] NOT NULL,
	[InstanceID] [uniqueidentifier] NOT NULL,
	[StatusID] [int] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[EmployeeID] [uniqueidentifier] NOT NULL,
	[AccumulatedTypeID] [uniqueidentifier] NOT NULL,
	[ExerciseFiscalYear] [int] NOT NULL,
	[InitialExerciseAmount] [decimal](18, 6) NOT NULL,
	[PreviousExerciseAccumulated] [decimal](18, 6) NOT NULL,
	[January] [decimal](18, 6) NOT NULL,
	[February] [decimal](18, 6) NOT NULL,
	[March] [decimal](18, 6) NOT NULL,
	[April] [decimal](18, 6) NOT NULL,
	[May] [decimal](18, 6) NOT NULL,
	[June] [decimal](18, 6) NOT NULL,
	[July] [decimal](18, 6) NOT NULL,
	[August] [decimal](18, 6) NOT NULL,
	[September] [decimal](18, 6) NOT NULL,
	[October] [decimal](18, 6) NOT NULL,
	[November] [decimal](18, 6) NOT NULL,
	[December] [decimal](18, 6) NOT NULL)	 

--Insertar en la tabla temporal los historic acumulados que debería tener el empleado
INSERT INTO @HistoricAccumulatedEmployeeTable([ID]
           ,[Active]
           ,[DeleteDate]
           ,[Timestamp]
           ,[user]
           ,[company]
           ,[InstanceID]
           ,[StatusID]
           ,[Name]
           ,[Description]
           ,[CreationDate]
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
           ,[December])
SELECT 
	ID = newid(),
	Active = 1,
	DeleteDate = null,
	[Timestamp] = getdate(),
	[user] = @user,
	company = @company,
	InstanceID = @InstanceId,
	StatusID = 1,
	[Name] = 'ae',
	[Description] = 'ae',
	CreationDate = getdate(),
	EmployeeID = e.ID, 
	AccumulatedTypeID = at.ID,
	ExerciseFiscalYear = @FiscalYear,
	InitialExerciseAmount = 0,
	PreviousExerciseAccumulated = 0,
	January = 0,
	Febrary = 0,
	March = 0,
	April = 0,
	May = 0,
	June = 0,
	July = 0,
	August = 0,
	September = 0,
	October = 0,
	November = 0,
	December = 0
FROM Employee e
LEFT JOIN AccumulatedType at
ON at.company = @company and at.InstanceID = @InstanceId
WHERE e.company = @company and e.InstanceID = @InstanceId

--MERGE AccumulatedType INSERT IF NOT EXISTS
MERGE HistoricAccumulatedEmployee hae 
	USING @HistoricAccumulatedEmployeeTable thae
ON hae.AccumulatedTypeID = thae.AccumulatedTypeID and
   hae.EmployeeID = thae.EmployeeID and
   hae.ExerciseFiscalYear = thae.ExerciseFiscalYear and
   hae.company = thae.company and
   hae.InstanceId = thae.InstanceId
WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([ID] ,[Active] ,[DeleteDate] ,[Timestamp] ,[user] ,[company]
      ,[InstanceID] ,[StatusID] ,[Name] ,[Description]  ,[CreationDate]
      ,[EmployeeID] ,[AccumulatedTypeID] ,[ExerciseFiscalYear]  ,[InitialExerciseAmount]
      ,[PreviousExerciseAccumulated]  ,[January] ,[February]  ,[March]
      ,[April]  ,[May]  ,[June]  ,[July]  ,[August] ,[September]  ,[October]
      ,[November] ,[December])
	  VALUES(thae.[ID] ,thae.[Active] ,thae.[DeleteDate] ,thae.[Timestamp] ,thae.[user] ,thae.[company]
      ,thae.[InstanceID] ,thae.[StatusID] ,thae.[Name] ,thae.[Description]  ,thae.[CreationDate]
      ,thae.[EmployeeID] ,thae.[AccumulatedTypeID] ,thae.[ExerciseFiscalYear]  ,thae.[InitialExerciseAmount]
      ,thae.[PreviousExerciseAccumulated]  ,thae.[January] ,thae.[February]  ,thae.[March]
      ,thae.[April]  ,thae.[May]  ,thae.[June]  ,thae.[July]  ,thae.[August] ,thae.[September]  ,thae.[October]
      ,thae.[November] ,thae.[December])
	  --OUTPUT
		 --  $action,
		 --  inserted.*
      ;

  COMMIT TRAN THistoricAccumulatedEmployee;
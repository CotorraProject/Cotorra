/****** Script for SelectTopNRows command from SSMS  ******/
CREATE PROCEDURE [dbo].[DeleteEmployeeInformation]
@EmployeeIDs dbo.guidlisttabletype READONLY,
@InstanceId uniqueidentifier,
@company uniqueidentifier
AS

BEGIN TRAN T_DELETEEMPLOYEEINFORMATION

 
 delete FROM [dbo].[EmployeeIdentityRegistration] where [EmployeeID] in (SELECT ei.ID from @EmployeeIDs ei)
 delete FROM [dbo].[EmployeeSalaryIncrease] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId
 delete FROM [dbo].[FonacotMovement] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId
 delete FROM [dbo].[Incident] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId
 delete FROM [dbo].[InfonavitMovement] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId
 delete FROM [dbo].[Inhability] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId
 delete FROM [dbo].[Vacation] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId
 delete FROM [dbo].[PermanentMovement] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId 
 delete FROM [dbo].[HistoricEmployeeSalaryAdjustment] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId
 update [dbo].[Employee] set [ImmediateLeaderEmployeeID] = null where [ImmediateLeaderEmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId
 delete from [dbo].[OverdraftDetail] where [OverdraftID] in (select [ID]  from [dbo].[Overdraft] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId)  
 delete from [dbo].[Overdraft] where [EmployeeID]  in ( (SELECT ei.ID from @EmployeeIDs ei)) and [InstanceID] = @InstanceId  

COMMIT TRAN T_DELETEEMPLOYEEINFORMATION
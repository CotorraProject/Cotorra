CREATE PROCEDURE [dbo].[CreateEmployeesNodes]
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@EmployeesIds dbo.guidlisttabletype READONLY
AS
BEGIN TRAN T_CreateEmployeesNodes

INSERT INTO EmployeeNode (ID)
SELECT ei.ID from @EmployeesIds ei

COMMIT TRAN T_CreateEmployeesNodes
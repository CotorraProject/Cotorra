CREATE VIEW [dbo].[vwEmployeeConceptsRelationDetail]
	AS 
SELECT [ID],            
    [Active],           
    [DeleteDate],       
    [Timestamp]  ,      
    [Name],             
    [Description],      
    [StatusID],         
    [CreationDate],     
    [user],             
    [company],           
    [InstanceID],         
    [EmployeeConceptsRelationID],   
    [PaymentDate]
    FROM
    [EmployeeConceptsRelationDetail]

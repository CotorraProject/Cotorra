CREATE TABLE [dbo].[EmployeeSBCAdjustment] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Active]       BIT              NOT NULL,
    [DeleteDate]   DATETIME2 (7)    NULL,
    [Timestamp]    DATETIME2 (7)    NOT NULL,
    [user]         UNIQUEIDENTIFIER NOT NULL,
    [company]      UNIQUEIDENTIFIER NOT NULL,
    [StatusID]         INT              NOT NULL, 
    [Name] NVARCHAR(150) NOT NULL, 
    [Description]      NVARCHAR (250)   NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [InstanceID]   UNIQUEIDENTIFIER NOT NULL,
    [ModificationDate] DATETIME2 (7) NOT NULL,
    [EmployeeID] UNIQUEIDENTIFIER NOT NULL,   
	[SBCFixedPart] DECIMAL(18, 6)  NULL, 
	[SBCVariablePart] DECIMAL(18, 6)   NULL, 
	[SBCMax25UMA] DECIMAL(18, 6)   NULL,    	

    CONSTRAINT [PK_EmployeeSBCAdjustment] PRIMARY KEY CLUSTERED ([ID] ASC), 
    CONSTRAINT [FK_EmployeeSBCAdjustment_Employee] FOREIGN KEY (EmployeeID) REFERENCES Employee([ID])  
);

GO
--GetAll 
CREATE NONCLUSTERED INDEX [IX_EmployeeSBCAdjustment_InstanceID]
    ON [dbo].[EmployeeSBCAdjustment]([InstanceID], [Active]);
GO
   

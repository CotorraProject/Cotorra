CREATE TABLE [dbo].[Incident] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (100)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [company]          UNIQUEIDENTIFIER NOT NULL,    
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,  
	[IncidentTypeID]   UNIQUEIDENTIFIER NOT NULL, 
    [PeriodDetailID]         UNIQUEIDENTIFIER NOT NULL, 
    [EmployeeID]       UNIQUEIDENTIFIER NOT NULL, 
    [Date]             DATETIME2 (7)    NOT NULL,
    [Value] DECIMAL(18, 6) NOT NULL, 

    CONSTRAINT [PK_Incident] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_Incident_IncidentType_IncidentTypeID] FOREIGN KEY ([IncidentTypeID]) REFERENCES [dbo].[IncidentType] ([ID]),
    CONSTRAINT [FK_Incident_PeriodDetail_PeriodDetail_ID] FOREIGN KEY ([PeriodDetailID]) REFERENCES [dbo].[PeriodDetail] ([ID]),
    CONSTRAINT [FK_Incident_Employee_Employee_ID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID]),
);

GO

--All
CREATE NONCLUSTERED INDEX [IX_Incident_InstanceID]
    ON [dbo].[Incident]([InstanceID], [Active]);
GO

--InhabilityValidation - Incident
CREATE NONCLUSTERED INDEX [IX_Inhability_Instance_InstanceID_EmployeeID]
    ON [dbo].[Incident]([InstanceID], [EmployeeID], [Date]);
GO

--For calculation
CREATE NONCLUSTERED INDEX [IX_Incident_Calculation]
    ON [dbo].[Incident]([InstanceID],[company], [EmployeeID], [PeriodDetailID], [Active]);
GO
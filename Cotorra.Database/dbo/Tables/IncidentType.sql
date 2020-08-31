
CREATE TABLE [dbo].[IncidentType] (
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
    Code nvarchar(5) NOT NULL, 
	TypeOfIncident int NOT NULL,
	UnitValue DECIMAL(18, 6) NOT NULL, 
	[Percentage] DECIMAL(18, 6) NOT NULL, 
	ItConsiders int NOT NULL, 
	SalaryRight bit NOT NULL, 
	DecreasesSeventhDay bit NOT NULL,

    CONSTRAINT [PK_IncidentType] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_IncidentType_InstanceID]
    ON [dbo].[IncidentType]([InstanceID], [Active]);
GO

--InhabilityValidation - Incident
CREATE NONCLUSTERED INDEX [IX_Inhability_InstanceType_InstanceID_ItConsiders]
    ON [dbo].[IncidentType](ID, [InstanceID], [ItConsiders]);
GO
CREATE TABLE [dbo].[IncidentTypeRelationship] (
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

	IncidentTypeID UNIQUEIDENTIFIER NOT NULL, 
	AccumulatedTypeID UNIQUEIDENTIFIER NOT NULL, 

    CONSTRAINT [PK_IncidentTypeRelationship] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_IncidentTypeRelationship_IncidentType_IncidentTypeID] FOREIGN KEY ([IncidentTypeID]) REFERENCES [dbo].[IncidentType] ([ID]),
	CONSTRAINT [FK_IncidentTypeRelationship_AccumulatedType_AccumulatedTypeID] FOREIGN KEY ([AccumulatedTypeID]) REFERENCES [dbo].[AccumulatedType] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_IncidentTypeRelationship_InstanceID]
    ON [dbo].[IncidentTypeRelationship]([InstanceID], [Active]);
GO

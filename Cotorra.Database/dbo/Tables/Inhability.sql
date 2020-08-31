CREATE TABLE [dbo].[Inhability] (
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

    [Folio]            VARCHAR(9)       NOT NULL,
    [IncidentTypeID]   UNIQUEIDENTIFIER NOT NULL,
    [AuthorizedDays]   INT          NOT NULL,
    [InitialDate]      DATETIME         NOT NULL,
    [CategoryInsurance]  INT NOT NULL,
    [RiskType]  INT NOT NULL,
    [Percentage]   DECIMAL(18,6) NOT NULL,
    [Consequence]  INT NOT NULL,
    [InhabilityControl]  INT NOT NULL,   
    [EmployeeID] UNIQUEIDENTIFIER NOT NULL,
    
    CONSTRAINT [PK_Inhability] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Inhability_IncidentType_IncidentTypeID] FOREIGN KEY ([IncidentTypeID]) REFERENCES [dbo].[IncidentType] ([ID]),
     CONSTRAINT [FK_Inhability_Employee_EmployeeID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_Inhability_InstanceID]
    ON [dbo].[Inhability]([InstanceID], [Active]);
GO

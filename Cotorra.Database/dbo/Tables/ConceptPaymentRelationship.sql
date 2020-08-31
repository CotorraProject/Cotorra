CREATE TABLE [dbo].[ConceptPaymentRelationship] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (100)   NOT NULL,
    [Description]      NVARCHAR (250)   NOT NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [company]          UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
    [ConceptPaymentID] UNIQUEIDENTIFIER NOT NULL,
    [AccumulatedTypeID] UNIQUEIDENTIFIER NOT NULL,
    [ConceptPaymentRelationshipType] INT NOT NULL,
    
    [ConceptPaymentType] INT NOT NULL, 
    CONSTRAINT [PK_ConceptPaymentRelationship] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ConceptPaymentRelationship_ConceptPayment_ConceptPaymentID] FOREIGN KEY ([ConceptPaymentID]) REFERENCES [dbo].[ConceptPayment] ([ID]),
    CONSTRAINT [FK_ConceptPaymentRelationship_AccumulatedType_AccumulatedTypeID] FOREIGN KEY ([AccumulatedTypeID]) REFERENCES [dbo].[AccumulatedType] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_ConceptPaymentRelationship_InstanceID]
    ON [dbo].[ConceptPaymentRelationship]([InstanceID], [Active]);
GO

--Mejora la consulta  94.8434%
CREATE NONCLUSTERED INDEX [IX_ConceptPaymentRelationship_Active_Company_Instance]
ON [dbo].[ConceptPaymentRelationship] ([Active],[company],[InstanceID])
INCLUDE ([DeleteDate],[Timestamp],[Name],[Description],[StatusID],[CreationDate],[user],[ConceptPaymentID],[AccumulatedTypeID],[ConceptPaymentRelationshipType],[ConceptPaymentType])
GO

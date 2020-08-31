CREATE TABLE [dbo].[EmployerRegistration]
(
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

    [Code] NVARCHAR(100) NULL, 
    [RiskClass] NVARCHAR(100)       NULL, 
	[RiskClassFraction] DECIMAL(18, 6) NULL, 
    [AddressID] UNIQUEIDENTIFIER NULL, 
	
    CONSTRAINT [PK_EmployerRegistration] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_EmployerRegistration_Address_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID]) 
)

GO
--GetAll 
CREATE NONCLUSTERED INDEX [IX_EmployerRegistration_InstanceID]
    ON [dbo].[EmployerRegistration]([InstanceID], [Active]);
GO
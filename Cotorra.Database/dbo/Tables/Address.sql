CREATE TABLE [dbo].[Address]
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

    [ZipCode] NVARCHAR(100) NULL, 
	[FederalEntity] NVARCHAR(100) NULL,
	[Municipality] NVARCHAR(100) NULL,
	[Street] NVARCHAR(100) NULL,
	[ExteriorNumber] NVARCHAR(100) NULL,
	[InteriorNumber] NVARCHAR(100) NULL,
	[Suburb] NVARCHAR(100) NULL,
	[Reference] NVARCHAR(100) NULL,
	
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([ID] ASC)
)

GO
--GetAll 
CREATE NONCLUSTERED INDEX [IX_Address_InstanceID]
    ON [dbo].[Address]([InstanceID], [Active]);
GO

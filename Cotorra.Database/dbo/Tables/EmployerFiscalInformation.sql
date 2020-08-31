CREATE TABLE [dbo].[EmployerFiscalInformation]
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
   
    [RFC]               NVARCHAR(13) NOT NULL DEFAULT(''),
    [CertificateCER]    TEXT NOT NULL,
    [CertificateKEY]    TEXT NOT NULL,
    [CertificatePwd]    TEXT NOT NULL, 
    [StartDate]    DATETIME NOT NULL, 
    [EndDate]    DATETIME NOT NULL, 
    [CertificateNumber] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_EmployerFiscalInformationn] PRIMARY KEY CLUSTERED ([ID] ASC)
)

GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_EmployerFiscalInformation_InstanceID]
    ON [dbo].[EmployerFiscalInformation]([InstanceID], [Active]);
GO
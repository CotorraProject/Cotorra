CREATE TABLE [dbo].[PayrollCompanyConfiguration] (
    [ID]                                   UNIQUEIDENTIFIER NOT NULL,
    [Active]                               BIT              NOT NULL,
    [DeleteDate]                           DATETIME2 (7)    NULL,
    [Timestamp]                            DATETIME2 (7)    NOT NULL,
    [user]                                 UNIQUEIDENTIFIER NOT NULL,
    [company]                              UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (100)   NOT NULL,
    [Description]      NVARCHAR (250)   NOT NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [InstanceID]                           UNIQUEIDENTIFIER NOT NULL,

    [RFC]          NVARCHAR(13)         NOT NULL DEFAULT(''),
    [SocialReason] NVARCHAR(4000)       NOT NULL DEFAULT(''),
    [CURP]         NVARCHAR(18)         NULL,
    [SalaryZone]                           INT NOT NULL DEFAULT 1, 
    [FiscalRegime ]                        INT NOT NULL DEFAULT 601, 
    [NonDeducibleFactor ]                  DECIMAL(18, 6) NOT NULL DEFAULT 0.25, 
    [CurrentExerciseYear]                  INT NOT NULL DEFAULT(0),
    [CurrentPeriod]                        INT NOT NULL DEFAULT(0),
    [StartDate]                            DATETIME2 (7)    NOT NULL,
    [CurrencyID]                           UNIQUEIDENTIFIER NOT NULL,
    [AddressID]                            UNIQUEIDENTIFIER NULL,	
    [CompanyInformation]                   TEXT NULL,
    [ComercialName]                        NVARCHAR(500) NULL,
    [CompanyCreationDate]                  DATETIME2 (7) NULL,
    [CompanyScope]                         INT NOT NULL DEFAULT(0),
    [CompanyContactEmail]                  NVARCHAR(150) NULL,
    [CompanyContactPhone]                  NVARCHAR(15) NULL,
    [CompanyBusinessSector]                INT NOT NULL DEFAULT(0),
    [CompanyWebSite]                       NVARCHAR(200) NULL,
    [Facebook]                             NVARCHAR(150) NULL,
    [Instagram]                            NVARCHAR(150) NULL,
    [Twitter]                              NVARCHAR(150) NULL,
    [Youtube]                              NVARCHAR(150) NULL,
    CONSTRAINT [PK_PayrollCompanyConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PayrollCompanyConfiguration_Address_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID]) 
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_PayrollCompanyConfiguration_InstanceID]
    ON [dbo].[PayrollCompanyConfiguration]([InstanceID], [Active]);
GO
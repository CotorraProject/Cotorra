CREATE TABLE [dbo].[EmployeeIdentityRegistration]
(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,

    [EmployeeID]      UNIQUEIDENTIFIER NOT NULL,
    [IdentityUserID]  UNIQUEIDENTIFIER  NULL,
    [ActivationCode]  NVARCHAR(8) NULL,
    [Email]           NVARCHAR(150) NOT NULL,
    [EmployeeIdentityRegistrationStatus]    INT NOT NULL,

    CONSTRAINT [PK_EmployeeIdentityRegistration] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--Get By EmployeeID 
CREATE NONCLUSTERED INDEX [IX_EmployeeIdentityRegistration_EmployeeID]
    ON [dbo].[EmployeeIdentityRegistration]([EmployeeID]);
GO

--Get By IdentityUserID 
CREATE NONCLUSTERED INDEX [IX_EmployeeIdentityRegistration_IdentityUserID]
    ON [dbo].[EmployeeIdentityRegistration]([IdentityUserID]);
GO

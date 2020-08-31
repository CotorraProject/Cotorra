CREATE TABLE [dbo].[UserCustomSettings] (
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
    [Number]           INT              NOT NULL, 
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
    [Key]              NVARCHAR(250) NOT NULL,
    [Value]            NVARCHAR(250) NOT NULL,
    CONSTRAINT [PK_UserCustomSettings] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll Instance
CREATE NONCLUSTERED INDEX [IX_UserCustomSettings_InstanceID]
    ON [dbo].[UserCustomSettings]([InstanceID], [Active]);
GO

--GetAll User
CREATE NONCLUSTERED INDEX [IX_UserCustomSettings_Instance_User]
    ON [dbo].[UserCustomSettings]([InstanceID], [user], [Active]);
GO

--GetByKeyInstance
CREATE NONCLUSTERED INDEX [IX_UserCustomSettings_Key_InstanceID]
    ON [dbo].[UserCustomSettings]([InstanceID], [Key], [Active]);
GO

--GetByKeyInstance User
CREATE NONCLUSTERED INDEX [IX_UserCustomSettings_Key_User_InstanceID]
    ON [dbo].[UserCustomSettings]([InstanceID],[user], [Key], [Active]);
GO

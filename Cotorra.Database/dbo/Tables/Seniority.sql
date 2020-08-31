CREATE TABLE [dbo].[Seniority] (
    [ID]                      UNIQUEIDENTIFIER NOT NULL,
    [Active]                  BIT              NOT NULL,
    [DeleteDate]              DATETIME2 (7)    NULL,
    [Timestamp]               DATETIME2 (7)    NOT NULL,
    [Name]                    NVARCHAR (100)   NULL,
    [Description]             NVARCHAR (250)   NULL,
    [StatusID]                INT              NOT NULL,
    [CreationDate]            DATETIME2 (7)    NOT NULL,
    [user]                    UNIQUEIDENTIFIER NOT NULL,
    [company]                 UNIQUEIDENTIFIER NOT NULL,
    [Years]                   INT              NOT NULL,
    [InstanceID]              UNIQUEIDENTIFIER NOT NULL,
    [TrustedHolidayDays]      DECIMAL (18, 6)  NULL,
    [TrustedHolidayBonus]     DECIMAL (18, 6)  NULL,
    [TrustedSeniority]        DECIMAL (18, 6)  NULL,
    [TrustedChristmasBonus]   DECIMAL (18, 6)  NULL,
    [UnionizedHolidayDays]    DECIMAL (18, 6)  NULL,
    [UnionizedHolidayBonus]   DECIMAL (18, 6)  NULL,
    [UnionizedSeniority]      DECIMAL (18, 6)  NULL,
    [UnionizedChristmasBonus] DECIMAL (18, 6)  NULL,
    CONSTRAINT [PK_Seniority] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--All
CREATE NONCLUSTERED INDEX [IX_Seniority_InstanceID]
    ON [dbo].[Seniority]([InstanceID] ASC, [Active] ASC);
GO


CREATE TABLE [dbo].[BenefitType](
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
    [Antiquity]        INT NOT NULL,
    [Holidays]         DECIMAL(18,6) NOT NULL,
    [HolidayPremiumPortion]     DECIMAL(18,6) NOT NULL,
    [DaysOfChristmasBonus]      DECIMAL(18,6) NOT NULL,
    [IntegrationFactor]         DECIMAL(18,6) NOT NULL,
    [DaysOfAntiquity]           DECIMAL(18,6) NOT NULL,

    CONSTRAINT [PK_BenefitType] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_BenefitType_InstanceID]
    ON [dbo].[BenefitType]([InstanceID], [Active]);
GO


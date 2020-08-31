CREATE TABLE [dbo].[PeriodType] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [Active]       BIT              NOT NULL,
    [DeleteDate]   DATETIME2 (7)    NULL,
    [Timestamp]    DATETIME2 (7)    NOT NULL,
    [Name]         NVARCHAR (100)   NOT NULL,
    [Description]  NVARCHAR (250)   NOT NULL,
    [ExtraordinaryPeriod]       BIT              NOT NULL,
    [MonthCalendarFixed]       BIT              NOT NULL,
    [PaymentDays]     DECIMAL(18, 6)              NOT NULL,
    [FortnightPaymentDays]     INT              NOT NULL,
    [PaymentDayPosition]     INT              NOT NULL,
    [PaymentPeriodicity]  INT   NOT NULL,
    [PeriodTotalDays]     INT              NOT NULL,
    [StatusID]     INT              NOT NULL,
    [CreationDate] DATETIME2 (7)    NOT NULL,
	[company]      UNIQUEIDENTIFIER NOT NULL,
    [InstanceID]   UNIQUEIDENTIFIER NOT NULL,
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [SeventhDays] INT NOT NULL DEFAULT 0,
    [SeventhDayPosition] NVARCHAR(100) NOT NULL  default('-1'), 
    [HolidayPremiumPaymentType] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_PeriodType] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_PeriodType_InstanceID]
    ON [dbo].[PeriodType]([InstanceID], [Active]);
GO
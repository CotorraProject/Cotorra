CREATE TABLE [dbo].[PeriodDetail] (
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

    Number int NOT NULL,
	InitialDate DATETIME2 NOT NULL, 
    FinalDate DATETIME2 NOT NULL, 
    PeriodID uniqueidentifier NOT NULL, 
    [PeriodMonth] INT NOT NULL,
    [PeriodBimonthlyIMSS] INT NOT NULL,
    [PeriodFiscalYear] INT NOT NULL,
    PaymentDays decimal(18, 6) NOT NULL,
    PeriodStatus INT NOT  NULL DEFAULT 0,
    [SeventhDays] INT NOT NULL DEFAULT 0,
    [SeventhDayPosition] NVARCHAR(100) NOT NULL  default('-1'), 
    CONSTRAINT [PK_PeriodDetail] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PeriodDetail_Period_PeriodID] FOREIGN KEY ([PeriodID]) REFERENCES [dbo].[Period] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_PeriodDetail_InstanceID]
    ON [dbo].[PeriodDetail]([InstanceID], [Active]);
GO

--Validator Delete 
CREATE NONCLUSTERED INDEX [IX_PeriodDetail_InstanceID_Number_Active]
    ON [dbo].[PeriodDetail]([InstanceID], [Number], [Active]);
GO

--Validator Inhability 
CREATE NONCLUSTERED INDEX [IX_PeriodDetail_Inhability_InstanceID_Number_Active]
    ON [dbo].[PeriodDetail]([InstanceID], [PeriodStatus]);
GO


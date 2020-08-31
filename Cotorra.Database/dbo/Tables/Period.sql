CREATE TABLE [dbo].[Period] (
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
    FiscalYear int not null,
	InitialDate DATETIME2 NOT NULL, 
    FinalDate DATETIME2 NOT NULL, 
    PeriodTypeID uniqueidentifier NOT NULL, 

    [IsFiscalYearClosed] BIT NOT NULL, 
    [IsActualFiscalYear] BIT NOT NULL, 
    [PeriodTotalDays] INT NOT NULL, 
    [PaymentDays] DECIMAL(18, 6) NOT NULL, 
    [ExtraordinaryPeriod] BIT NOT NULL, 
    [MonthCalendarFixed] BIT NOT NULL, 
    [FortnightPaymentDays] INT NOT NULL, 
    [PaymentDayPosition] INT NOT NULL, 
    [PaymentPeriodicity] INT NOT NULL, 
    CONSTRAINT [PK_Period] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Period_PeriodType_PeriodTypeID] FOREIGN KEY ([PeriodTypeID]) REFERENCES [dbo].[PeriodType] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_Period_InstanceID]
    ON [dbo].[Period]([InstanceID], [Active]);
GO
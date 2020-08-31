CREATE TABLE [dbo].[MonthlyIncomeTax] (
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
    [LowerLimit]	   DECIMAL(18, 6) NOT NULL, 
    [FixedFee]		   DECIMAL(18, 6) NOT NULL, 
    [Rate]             DECIMAL(18, 6) NOT NULL, 
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
    ValidityDate       DATETIME NOT NULL DEFAULT('2020-01-01'),

    CONSTRAINT [PK_MonthlyIncomeTax] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_MonthlyIncomeTax_InstanceID]
    ON [dbo].[MonthlyIncomeTax]([InstanceID], [Active]);
GO
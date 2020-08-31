CREATE TABLE [dbo].[AnualIncomeTax] (
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
	LowerLimit DECIMAL(18, 6) NOT NULL, 
	FixedFee DECIMAL(18, 6) NOT NULL, 
	Rate DECIMAL(18, 6) NOT NULL, 
    ValidityDate       DATETIME NOT NULL DEFAULT('2020-01-01'),
    CONSTRAINT [PK_AnualIncomeTax] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_AnualIncomeTax_InstanceID]
    ON [dbo].[AnualIncomeTax]([InstanceID], [Active]);
GO
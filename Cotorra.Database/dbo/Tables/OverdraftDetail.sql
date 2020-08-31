CREATE TABLE [dbo].[OverdraftDetail]
(
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
    
    OverdraftID        UNIQUEIDENTIFIER NOT NULL,
    ConceptPaymentID   UNIQUEIDENTIFIER NOT NULL, --COULD BE 3 DIFERENT TABLEs
    [Value]            DECIMAL(18,6) NOT NULL,
    [Amount]           DECIMAL(18, 6) NOT NULL, 
    Label1             NVARCHAR(500) NOT NULL,
    Label2             NVARCHAR(500) NOT NULL,
    Label3             NVARCHAR(500) NOT NULL,
    Label4             NVARCHAR(500) NOT NULL,
    [Taxed]          DECIMAL(18, 6) NOT NULL, 
    [Exempt]          DECIMAL(18, 6) NOT NULL, 
    [IMSSTaxed]          DECIMAL(18, 6) NOT NULL, 
    [IMSSExempt]          DECIMAL(18, 6) NOT NULL, 
    [IsGeneratedByPermanentMovement] BIT NOT NULL,
    [IsValueCapturedByUser] BIT NOT NULL,
    [IsTotalAmountCapturedByUser] BIT NOT NULL,
    [IsAmount1CapturedByUser] BIT NOT NULL,
    [IsAmount2CapturedByUser] BIT NOT NULL,
    [IsAmount3CapturedByUser] BIT NOT NULL,
    [IsAmount4CapturedByUser] BIT NOT NULL,

    CONSTRAINT [PK_OverdraftDetail] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_OverdraftDetail_Overdraft_OverdraftID] FOREIGN KEY ([OverdraftID]) REFERENCES [dbo].[Overdraft] ([ID])
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_OverdraftDetail_InstanceID]
    ON [dbo].[OverdraftDetail]([InstanceID], [Active]);
GO

CREATE NONCLUSTERED INDEX [IX_OverdraftDetail_InstanceID_company]
    ON [dbo].[OverdraftDetail]([InstanceID], [company]);
GO

--GetAll Overdraft 
CREATE NONCLUSTERED INDEX [IX_OverdraftDetail_InstanceID_OverdraftID]
    ON [dbo].[OverdraftDetail]([InstanceID], [OverdraftID]);
GO

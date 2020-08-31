CREATE TABLE [dbo].[MinimunSalary] (
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
    [ExpirationDate]   DateTime         NOT NULL,
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,
    [ZoneA] DECIMAL(18, 6) NOT NULL, 
    [ZoneB] DECIMAL(18, 6) NOT NULL, 
    [ZoneC] DECIMAL(18, 6) NOT NULL, 
    CONSTRAINT [PK_MinimunSalary] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_MinimunSalary_InstanceID]
    ON [dbo].[MinimunSalary]([InstanceID], [Active]);
GO
--Calculation 
CREATE NONCLUSTERED INDEX [IX_MinimunSalary_Calculation_InstanceID]
    ON [dbo].[MinimunSalary]([InstanceID], [company], [Active]);
GO
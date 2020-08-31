
CREATE TABLE [dbo].[IMSSFare] (
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

	[IMMSBranch]	   NVARCHAR(50)     NOT NULL,
	[EmployerShare]	   DECIMAL(18, 6)   NOT NULL, 
	[EmployeeShare]	   DECIMAL(18, 6)   NOT NULL,
	[MaxSMDF]          INT			    NOT NULL

    CONSTRAINT [PK_IMSSFare] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_IMSSFare_InstanceID]
    ON [dbo].[IMSSFare]([InstanceID], [Active]);
GO


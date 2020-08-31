CREATE TABLE [dbo].[Vacation]
(
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

	[EmployeeID]                UNIQUEIDENTIFIER    NOT NULL, 
    [VacationsCaptureType]      INT                 NOT NULL, 
    [InitialDate]               DATETIME2 (7)       NOT NULL,
    [FinalDate]                 DATETIME2 (7)       NOT NULL,
    [PaymentDate]               DATETIME2 (7)       NOT NULL,
    [Break_Seventh_Days]        DECIMAL(18, 6)      NOT NULL, 
    [VacationsBonusDays]        DECIMAL(18, 6)      NOT NULL, 
    [VacationsDays]             DECIMAL(18, 6) NOT NULL DEFAULT 0, 
    [VacationsBonusPercentage]  DECIMAL(18, 6) NOT NULL DEFAULT 0, 

    CONSTRAINT [PK_Vacation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Vacation_Employee_Employee_ID] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([ID]),
)
GO

--All
CREATE NONCLUSTERED INDEX [IX_Vacation_InstanceID]
    ON [dbo].[Vacation]([InstanceID], [Active]);
GO

--For calculation
CREATE NONCLUSTERED INDEX [IX_Vacation_InstanceID_company_EmployeeID_InitialDate_FinalDate_Active]
    ON [dbo].[Vacation]([InstanceID], [company], [EmployeeID], [InitialDate], [FinalDate], [Active]);
GO
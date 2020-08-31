CREATE TABLE [dbo].[VacationDaysOff]
(
	[ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,   
    [user]             UNIQUEIDENTIFIER NOT NULL,
    [company]          UNIQUEIDENTIFIER NOT NULL,    
    [InstanceID]       UNIQUEIDENTIFIER NOT NULL,  
	[VacationID]       UNIQUEIDENTIFIER    NOT NULL,  
    [Date]             DATETIME2 (7)       NOT NULL,    

    CONSTRAINT [PK_VacationDaysOff] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_VacationDaysOff_Vacation_VacationID] FOREIGN KEY ([VacationID]) REFERENCES [dbo].[Vacation] ([ID]),
)
GO

--All
CREATE NONCLUSTERED INDEX [IX_VacationDaysOff_InstanceID]
    ON [dbo].[VacationDaysOff]([InstanceID], [Active]);
GO
 
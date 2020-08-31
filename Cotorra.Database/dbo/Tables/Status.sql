CREATE TABLE [dbo].[Status] (
    [ID]          INT           NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (250) NOT NULL,
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([ID] ASC)
);



CREATE TABLE [dbo].[IMSSEmployerTable] (
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

    [ValidityDate]     DATETIME NOT NULL,
	EG_Especie_GastosMedicos_1	    DECIMAL(18, 6)   NOT NULL, 
	EG_Especie_Fija_2	            DECIMAL(18, 6)   NOT NULL,
    EG_Especie_mas_3SMDF_3	    DECIMAL(18, 6)   NOT NULL, 
    EG_Prestaciones_en_Dinero_4	    DECIMAL(18, 6)   NOT NULL, 
    Invalidez_y_vida_5	    DECIMAL(18, 6)   NOT NULL, 
    Cesantia_y_vejez_6	    DECIMAL(18, 6)   NOT NULL, 
    Guarderias_7	    DECIMAL(18, 6)   NOT NULL, 
    Retiro_8	    DECIMAL(18, 6)   NOT NULL, 

    CONSTRAINT [PK_IMSSEmployerTable] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetAll 
CREATE NONCLUSTERED INDEX [IX_IMSSEmployerTable_InstanceID]
    ON [dbo].[IMSSEmployerTable]([InstanceID], [Active]);
GO


CREATE TABLE [dbo].[catCFDI_CodigoPostal] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,

    [c_CodigoPostal]   NVARCHAR(5) NOT NULL,
    [c_Estado]         NVARCHAR(100) NOT NULL,
    [c_Municipio]      NVARCHAR(100) NOT NULL,
    [c_Localidad]      NVARCHAR(100) NOT NULL,
    [Estimulo_Franja_Fronteriza]      NVARCHAR(100) NOT NULL,
    [FechaInicioVigencia]   NVARCHAR(150) NOT NULL,
    [FechaFinVigencia]      NVARCHAR(150) NOT NULL,
    [DescripcionHusoHorario]      NVARCHAR(200) NOT NULL,
    [Mes_Inicio_Horario_Verano]   NVARCHAR(100) NOT NULL,
    [Dia_Inicio_Horario_Verano]   NVARCHAR(100) NOT NULL,
    [Hora_Horario_Verano]        NVARCHAR(100) NOT NULL,
    [Diferencia_Horaria_Verano]  NVARCHAR(4) NOT NULL,
    [Mes_Inicio_Horario_Invierno] NVARCHAR(100) NOT NULL,
    [Dia_Inicio_Horario_Invierno] NVARCHAR(100) NOT NULL,
    [Hora_Inicio_Horario_Invierno] NVARCHAR(100) NOT NULL,
    [Diferencia_Horaria_Invierno] NVARCHAR(4) NOT NULL,
    CONSTRAINT [PK_catCFDI_CodigoPostal] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

--GetByCode 
CREATE NONCLUSTERED INDEX [IX_catCFDI_CodigoPostal_Code]
    ON [dbo].[catCFDI_CodigoPostal]([c_CodigoPostal]);
GO

/*
Plantilla de script posterior a la implementación							
--------------------------------------------------------------------------------------
 Este archivo contiene instrucciones de SQL que se anexarán al script de compilación.		
 Use la sintaxis de SQLCMD para incluir un archivo en el script posterior a la implementación.			
 Ejemplo:      :r .\miArchivo.sql								
 Use la sintaxis de SQLCMD para hacer referencia a una variable en el script posterior a la implementación.		
 Ejemplo:      :setvar TableName miTabla							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

DECLARE @VersionTable TABLE(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Version] NVARCHAR(200) NOT NULL,
	[DateTime] DATETIME
  )  

INSERT @VersionTable ([ID], [Version], [DateTime])
 VALUES 
  ('19763B1E-88BA-480B-B509-3919AFA09F38', 'Version 1', getdate())  
   
BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando version'
 MERGE [Version] AS TARGET
 USING @VersionTable as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT(ID, [Version], [DateTime])
  VALUES(SOURCE.ID, SOURCE.[Version], SOURCE.[DateTime])
 WHEN MATCHED THEN
  UPDATE SET TARGET.[Version] = SOURCE.[Version],
   TARGET.[DateTime] = SOURCE.[DateTime];

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH
------------------------------------

DECLARE @Status_Table TABLE(
  ID int, 
  Name NVARCHAR (100),
  [Description] NVARCHAR (250)
  )  

INSERT @Status_Table ([ID], [Name], [Description])
 VALUES 
  (1, 'Habilitado', 'Este status se establece por default para especificar que se pueden utilizar.'),
  (2, 'Deshabilitado', 'Este status es para especificar que no se pueden utilizar.'),
  (3, 'Timbrado', 'Este status es para especificar que un documento esta timbrado.'),
  (4, 'Cancelado', 'Este status es para especificar que un documento esta cancelado.'),
  (5, 'Sin Timbrar', 'Este status es para especificar que un documento esta sin timbrar.'),
  (6, 'Timbrado Cancelado', 'Este status es para especificar que un documento esta cancelado pero estuvo timbrado.'),
  (7, 'Terminado', 'Este status es para especificar si el proceso ya termino.'),
  (8, 'Terminado con Errores', 'Este status es para especificar si el proceso termino pero tiene errores.'),
  (9, 'En Proceso', 'Este status es para especificar si todavia no se termina el proceso.'),
  (10, 'Bloqueado', 'Este status es para especificar que un documento se encuentra bloqueado por algún proceso en ejecución.')
   
BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando status'
 MERGE [Status] AS TARGET
 USING @Status_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT(ID, NAME,DESCRIPTION)
  VALUES(SOURCE.ID, SOURCE.NAME, SOURCE.DESCRIPTION)
 WHEN MATCHED THEN
  UPDATE SET TARGET.NAME = SOURCE.NAME,
   TARGET.DESCRIPTION = SOURCE.DESCRIPTION;

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH

/*Banks*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
DECLARE @Bank_Table TABLE(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [Code]           INT              NOT NULL
  )  

INSERT @Bank_Table ([ID], Active, DeleteDate, [Timestamp], [Name], [Description], StatusID, CreationDate, Code)
 VALUES 
('6E8C8100-214C-4A92-8CAE-10D229F296F4',1,null,getdate(), 'BANAMEX', 'Banco Nacional de México, S.A., Institución de Banca Múltiple, Grupo Financiero Banamex', 1, getdate(), 2 ),
('EA83D416-62EF-45FB-AD31-C4639098AA26',1,null,getdate(), 'BANCOMEXT', 'Banco Nacional de Comercio Exterior, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo', 1, getdate(), 6 ),
('8A19189B-6C5E-4C3F-8D1C-FC98AD6C7665',1,null,getdate(), 'BANOBRAS', 'Banco Nacional de Obras y Servicios Públicos, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo', 1, getdate(), 9 ),
('7B2E933B-8160-474F-A5F0-0D712332A776',1,null,getdate(), 'BBVA BANCOMER', 'BBVA Bancomer, S.A., Institución de Banca Múltiple, Grupo Financiero BBVA Bancomer', 1, getdate(), 12 ),
('E8CC4C40-F7A9-4BAE-BE72-D9AAFF4185D4',1,null,getdate(), 'SANTANDER', 'Banco Santander (México), S.A., Institución de Banca Múltiple, Grupo Financiero Santander', 1, getdate(), 14 ),
('412E51A9-9501-45DE-A50E-C773B55668B3',1,null,getdate(), 'BANJERCITO', 'Banco Nacional del Ejército, Fuerza Aérea y Armada, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo', 1, getdate(), 19 ),
('1DD4F754-9DDB-4C29-AE32-940B0324EB33',1,null,getdate(), 'HSBC', 'HSBC México, S.A., institución De Banca Múltiple, Grupo Financiero HSBC', 1, getdate(), 21 ),
('1C64F577-BCCF-466C-8045-47BFE717D022',1,null,getdate(), 'BAJIO', 'Banco del Bajío, S.A., Institución de Banca Múltiple', 1, getdate(), 30 ),
('998195AF-FF2C-48B1-A91B-03BBBCFF3381',1,null,getdate(), 'IXE', 'IXE Banco, S.A., Institución de Banca Múltiple, IXE Grupo Financiero', 1, getdate(), 32 ),
('6BB2F3BA-BCD5-4DE4-B1FA-FB40C82804AD',1,null,getdate(), 'INBURSA', 'Banco Inbursa, S.A., Institución de Banca Múltiple, Grupo Financiero Inbursa', 1, getdate(), 36 ),
('ACA9E939-487C-415E-825C-98C7D8A37681',1,null,getdate(), 'INTERACCIONES', 'Banco Interacciones, S.A., Institución de Banca Múltiple', 1, getdate(), 37 ),
('07BCBCED-3D9E-47B7-AFF6-FBABC4C5FA67',1,null,getdate(), 'MIFEL', 'Banca Mifel, S.A., Institución de Banca Múltiple, Grupo Financiero Mifel', 1, getdate(), 42 ),
('6E9C56DA-1CB8-48D6-BF23-9849CCCA7211',1,null,getdate(), 'SCOTIABANK', 'Scotiabank Inverlat, S.A.', 1, getdate(), 44 ),
('938AEAB2-D60C-4DA0-AEDA-FCAF7B17AB3D',1,null,getdate(), 'BANREGIO', 'Banco Regional de Monterrey, S.A., Institución de Banca Múltiple, Banregio Grupo Financiero', 1, getdate(), 58 ),
('D9209103-0383-4C40-A1C9-BAE9063C9C1E',1,null,getdate(), 'INVEX', 'Banco Invex, S.A., Institución de Banca Múltiple, Invex Grupo Financiero', 1, getdate(), 59 ),
('5A728F80-02D4-4E89-AC9F-90A4C2275013',1,null,getdate(), 'BANSI', 'Bansi, S.A., Institución de Banca Múltiple', 1, getdate(), 60 ),
('3421F967-F355-4E8A-B5A2-8CF18B4CA3BD',1,null,getdate(), 'AFIRME', 'Banca Afirme, S.A., Institución de Banca Múltiple', 1, getdate(), 62 ),
('A32D0046-3E0D-4200-A6CD-BC45A0C3D484',1,null,getdate(), 'BANORTE', 'Banco Mercantil del Norte, S.A., Institución de Banca Múltiple, Grupo Financiero Banorte', 1, getdate(), 72 ),
('16B0C845-92B8-4B99-B13B-9AA9FFEDAC33',1,null,getdate(), 'THE ROYAL BANK', 'The Royal Bank of Scotland México, S.A., Institución de Banca Múltiple', 1, getdate(), 102 ),
('7A77FB09-3A28-45A6-A943-8846BC0C604F',1,null,getdate(), 'AMERICAN EXPRESS', 'American Express Bank (México), S.A., Institución de Banca Múltiple', 1, getdate(), 103 ),
('0CDB0B68-1072-46E9-BCBF-A850333636BE',1,null,getdate(), 'BAMSA', 'Bank of America México, S.A., Institución de Banca Múltiple, Grupo Financiero Bank of America', 1, getdate(), 106 ),
('0099CC36-5684-4366-B370-D835683CD5DE',1,null,getdate(), 'TOKYO', 'Bank of Tokyo-Mitsubishi UFJ (México), S.A.', 1, getdate(), 108 ),
('D3F29C24-99C6-4751-98DF-2CF950E3F990',1,null,getdate(), 'JP MORGAN', 'Banco J.P. Morgan, S.A., Institución de Banca Múltiple, J.P. Morgan Grupo Financiero', 1, getdate(), 110 ),
('44332A00-FDCB-40F1-A7E6-6E685E92D051',1,null,getdate(), 'BMONEX', 'Banco Monex, S.A., Institución de Banca Múltiple', 1, getdate(), 112 ),
('B9FF37D4-E206-4802-ACD5-9F63C1EED3B2',1,null,getdate(), 'VE POR MAS', 'Banco Ve Por Mas, S.A. Institución de Banca Múltiple', 1, getdate(), 113 ),
('92A44086-636F-4C4C-AB8B-79633A9E81E9',1,null,getdate(), 'ING', 'ING Bank (México), S.A., Institución de Banca Múltiple, ING Grupo Financiero', 1, getdate(), 116 ),
('AFC69F02-99FE-4E65-AC01-A93E554DB8C0',1,null,getdate(), 'DEUTSCHE', 'Deutsche Bank México, S.A., Institución de Banca Múltiple', 1, getdate(), 124 ),
('D6C4EE75-7D43-40D4-9F67-9D08AAC0B537',1,null,getdate(), 'CREDIT SUISSE', 'Banco Credit Suisse (México), S.A. Institución de Banca Múltiple, Grupo Financiero Credit Suisse (México)', 1, getdate(), 126 ),
('68CF2FC1-B918-4601-8B15-8FFBBF421771',1,null,getdate(), 'AZTECA', 'Banco Azteca, S.A. Institución de Banca Múltiple.', 1, getdate(), 127 ),
('72EDD7A6-569D-4A08-8296-6CA83D5937D2',1,null,getdate(), 'AUTOFIN', 'Banco Autofin México, S.A. Institución de Banca Múltiple', 1, getdate(), 128 ),
('762677A6-9F1F-4015-ACEF-5D4140ECBA1E',1,null,getdate(), 'BARCLAYS', 'Barclays Bank México, S.A., Institución de Banca Múltiple, Grupo Financiero Barclays México', 1, getdate(), 129 ),
('796C759D-4CF8-49FA-9B98-122FBC01B70B',1,null,getdate(), 'COMPARTAMOS', 'Banco Compartamos, S.A., Institución de Banca Múltiple', 1, getdate(), 130 ),
('0D1B55F9-0BCF-42C3-B676-215B31CAC464',1,null,getdate(), 'BANCO FAMSA', 'Banco Ahorro Famsa, S.A., Institución de Banca Múltiple', 1, getdate(), 131 ),
('3E0384D1-D2AA-495E-A8BF-22901EC445A3',1,null,getdate(), 'BMULTIVA', 'Banco Multiva, S.A., Institución de Banca Múltiple, Multivalores Grupo Financiero', 1, getdate(), 132 ),
('3A44DD9A-3627-44C9-BAD4-7E5FF956A441',1,null,getdate(), 'ACTINVER', 'Banco Actinver, S.A. Institución de Banca Múltiple, Grupo Financiero Actinver', 1, getdate(), 133 ),
('43AD5AB6-BF82-43BB-BC9B-89771E99A786',1,null,getdate(), 'WAL-MART', 'Banco Wal-Mart de México Adelante, S.A., Institución de Banca Múltiple', 1, getdate(), 134 ),
('4BC615B9-BAA3-49CD-B212-BB405C3E3CC9',1,null,getdate(), 'NAFIN', 'Nacional Financiera, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo', 1, getdate(), 135 ),
('868C9DEA-C30A-4132-B0C6-185878DC4314',1,null,getdate(), 'INTERBANCO', 'Inter Banco, S.A. Institución de Banca Múltiple', 1, getdate(), 136 ),
('58D7D72F-909B-4668-8FA8-56D927FC6B98',1,null,getdate(), 'BANCOPPEL', 'BanCoppel, S.A., Institución de Banca Múltiple', 1, getdate(), 137 ),
('B1B4DCB3-EB11-406F-A5A9-6282C0190318',1,null,getdate(), 'ABC CAPITAL', 'ABC Capital, S.A., Institución de Banca Múltiple', 1, getdate(), 138 ),
('389BB4BC-60A3-4342-8915-C0C8443AA30D',1,null,getdate(), 'UBS BANK', 'UBS Bank México, S.A., Institución de Banca Múltiple, UBS Grupo Financiero', 1, getdate(), 139 ),
('809EACAA-254D-461F-BB7E-B0044CD363DF',1,null,getdate(), 'CONSUBANCO', 'Consubanco, S.A. Institución de Banca Múltiple', 1, getdate(), 140 ),
('95163775-3E27-4248-A998-CD92112D0660',1,null,getdate(), 'VOLKSWAGEN', 'Volkswagen Bank, S.A., Institución de Banca Múltiple', 1, getdate(), 141 ),
('5EB50EE8-18C5-41C6-AFCD-98545E8F75C6',1,null,getdate(), 'CIBANCO', 'CIBanco, S.A.', 1, getdate(), 143 ),
('E9C16C65-D9B9-4CB9-A2F7-C20AC0856039',1,null,getdate(), 'BBASE', 'Banco Base, S.A., Institución de Banca Múltiple', 1, getdate(), 145 ),
('F3F523BD-3DE2-4EEF-98E7-DC129823612F',1,null,getdate(), 'BANSEFI', 'Banco del Ahorro Nacional y Servicios Financieros, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo', 1, getdate(), 166 ),
('59D735D2-DB97-449E-AC7B-F86D1BB85FB0',1,null,getdate(), 'HIPOTECARIA FEDERAL', 'Sociedad Hipotecaria Federal, Sociedad Nacional de Crédito, Institución de Banca de Desarrollo', 1, getdate(), 168 ),
('345F5AB3-2571-4F12-BCB3-885D2BB5F416',1,null,getdate(), 'MONEXCB', 'Monex Casa de Bolsa, S.A. de C.V. Monex Grupo Financiero', 1, getdate(), 600 ),
('B600C9E7-BF9B-40BA-9522-48AAF8A58365',1,null,getdate(), 'GBM', 'GBM Grupo Bursátil Mexicano, S.A. de C.V. Casa de Bolsa', 1, getdate(), 601 ),
('0B620611-6EE4-4961-895C-24B5B4B4E9A2',1,null,getdate(), 'MASARI', 'Masari Casa de Bolsa, S.A.', 1, getdate(), 602 ),
('453E6CA3-A157-4ECE-BCD1-52A1E7B150E6',1,null,getdate(), 'VALUE', 'Value, S.A. de C.V. Casa de Bolsa', 1, getdate(), 605 ),
('91B9F081-6997-4DF3-B4E6-A2FC4A506A78',1,null,getdate(), 'ESTRUCTURADORES', 'Estructuradores del Mercado de Valores Casa de Bolsa, S.A. de C.V.', 1, getdate(), 606 ),
('6BF6309F-76CE-460B-98C5-B347D5C6A129',1,null,getdate(), 'TIBER', 'Casa de Cambio Tiber, S.A. de C.V.', 1, getdate(), 607 ),
('FB91AACE-4F53-44C3-8F93-1F0588E7593A',1,null,getdate(), 'VECTOR', 'Vector Casa de Bolsa, S.A. de C.V.', 1, getdate(), 608 ),
('6ED8489E-CC4B-404E-A29C-9E07FD702957',1,null,getdate(), 'B&B', 'B y B, Casa de Cambio, S.A. de C.V.', 1, getdate(), 610 ),
('F8F5F688-E838-485D-85A3-AEC99BAC406A',1,null,getdate(), 'ACCIVAL', 'Acciones y Valores Banamex, S.A. de C.V., Casa de Bolsa', 1, getdate(), 614 ),
('F1D482E0-2EED-435B-BB82-F6DB4DB97710',1,null,getdate(), 'MERRILL LYNCH', 'Merrill Lynch México, S.A. de C.V. Casa de Bolsa', 1, getdate(), 615 ),
('22EB14F8-6E46-424F-B70C-BB17109B40A9',1,null,getdate(), 'FINAMEX', 'Casa de Bolsa Finamex, S.A. de C.V.', 1, getdate(), 616 ),
('9CFFCD18-B507-420E-AB34-8BF344E726D6',1,null,getdate(), 'VALMEX', 'Valores Mexicanos Casa de Bolsa, S.A. de C.V.', 1, getdate(), 617 ),
('3D3DFF21-C994-444E-8A50-8E16B99EE582',1,null,getdate(), 'UNICA', 'Unica Casa de Cambio, S.A. de C.V.', 1, getdate(), 618 ),
('C82D15AE-5942-44D7-A125-660FF5426E6B',1,null,getdate(), 'MAPFRE', 'MAPFRE Tepeyac, S.A.', 1, getdate(), 619 ),
('CCEAEE37-AFDF-49A5-A2BC-60D3F53F5445',1,null,getdate(), 'PROFUTURO', 'Profuturo G.N.P., S.A. de C.V., Afore', 1, getdate(), 620 ),
('93212DCC-44C6-4498-9490-2391C87D7232',1,null,getdate(), 'CB ACTINVER', 'Actinver Casa de Bolsa, S.A. de C.V.', 1, getdate(), 621 ),
('B2240534-3720-4C11-8E91-E49002A2EC19',1,null,getdate(), 'OACTIN', 'OPERADORA ACTINVER, S.A. DE C.V.', 1, getdate(), 622 ),
('81DBEADD-A908-48A0-847F-4535C20FBD6E',1,null,getdate(), 'SKANDIA', 'Skandia Vida, S.A. de C.V.', 1, getdate(), 623 ),
('BE079985-1D12-4EE2-ADB2-219A4A612C49',1,null,getdate(), 'CBDEUTSCHE', 'Deutsche Securities, S.A. de C.V. CASA DE BOLSA', 1, getdate(), 626 ),
('8B883487-702C-437C-8E2A-5D883890878D',1,null,getdate(), 'ZURICH', 'Zurich Compañía de Seguros, S.A.', 1, getdate(), 627 ),
('28D96ADA-3578-4B35-BDAA-C6FAB7488917',1,null,getdate(), 'ZURICHVI', 'Zurich Vida, Compañía de Seguros, S.A.', 1, getdate(), 628 ),
('649DCF61-941B-4C53-BDA1-14F0A5D906E4',1,null,getdate(), 'SU CASITA', 'Hipotecaria Su Casita, S.A. de C.V. SOFOM ENR', 1, getdate(), 629 ),
('EB11EF4B-E0A7-4FE5-B786-DB32C57C96F3',1,null,getdate(), 'CB INTERCAM', 'Intercam Casa de Bolsa, S.A. de C.V.', 1, getdate(), 630 ),
('678E3A4F-AE2A-49D9-92DE-B474BF54A923',1,null,getdate(), 'CI BOLSA', 'CI Casa de Bolsa, S.A. de C.V.', 1, getdate(), 631 ),
('660AF908-10C7-4ECC-AB48-14DFCA4FCCF5',1,null,getdate(), 'BULLTICK CB', 'Bulltick Casa de Bolsa, S.A., de C.V.', 1, getdate(), 632 ),
('2EA05EA4-8FCB-487B-8756-CB27981B80D0',1,null,getdate(), 'STERLING', 'Sterling Casa de Cambio, S.A. de C.V.', 1, getdate(), 633 ),
('B4D4A5C7-8E21-48DB-8F3B-5AB373CE3086',1,null,getdate(), 'FINCOMUN', 'Fincomún, Servicios Financieros Comunitarios, S.A. de C.V.', 1, getdate(), 634 ),
('2C692ABF-042F-4F40-8762-97DAAA5D4A35',1,null,getdate(), 'HDI SEGUROS', 'HDI Seguros, S.A. de C.V.', 1, getdate(), 636 ),
('55B18FC3-30D7-4666-89E5-DD3F21953F78',1,null,getdate(), 'ORDER', 'Order Express Casa de Cambio, S.A. de C.V', 1, getdate(), 637 ),
('D57168C9-9125-4771-B8B3-363937F24AE3',1,null,getdate(), 'AKALA', 'Akala, S.A. de C.V., Sociedad Financiera Popular', 1, getdate(), 638 ),
('A7869E08-7317-438B-A37E-FD0B116F240C',1,null,getdate(), 'CB JPMORGAN', 'J.P. Morgan Casa de Bolsa, S.A. de C.V. J.P. Morgan Grupo Financiero', 1, getdate(), 640 ),
('D3EAB6AE-609B-4564-8E24-26F6A6AE7C8D',1,null,getdate(), 'REFORMA', 'Operadora de Recursos Reforma, S.A. de C.V., S.F.P.', 1, getdate(), 642 ),
('7F34E8FB-A005-45B6-B815-DDD21FFAE2DF',1,null,getdate(), 'STP', 'Sistema de Transferencias y Pagos STP, S.A. de C.V.SOFOM ENR', 1, getdate(), 646 ),
('BE8D5BFA-D3A7-415B-9DA8-21C029DC2B12',1,null,getdate(), 'TELECOMM', 'Telecomunicaciones de México', 1, getdate(), 647 ),
('AF252E68-CCEE-4DD7-8168-4AC82B919677',1,null,getdate(), 'EVERCORE', 'Evercore Casa de Bolsa, S.A. de C.V.', 1, getdate(), 648 ),
('94100A73-D078-441F-A7FC-3722574D141B',1,null,getdate(), 'SKANDIA', 'Skandia Operadora de Fondos, S.A. de C.V.', 1, getdate(), 649 ),
('E21E847C-B47B-4D97-BF78-43F986E09159',1,null,getdate(), 'SEGMTY', 'Seguros Monterrey New York Life, S.A de C.V', 1, getdate(), 651 ),
('60137155-D12B-42CF-B9B4-2FCE74245822',1,null,getdate(), 'ASEA', 'Solución Asea, S.A. de C.V., Sociedad Financiera Popular', 1, getdate(), 652 ),
('ACF454FD-FDF9-4737-A074-1F6689F5CA2A',1,null,getdate(), 'KUSPIT', 'Kuspit Casa de Bolsa, S.A. de C.V.', 1, getdate(), 653 ),
('4B951174-6AF4-4D46-96FD-D4A0764FD49D',1,null,getdate(), 'SOFIEXPRESS', 'J.P. SOFIEXPRESS, S.A. de C.V., S.F.P.', 1, getdate(), 655 ),
('BE5E4D5E-5E6D-4F6E-9075-DF58AB8374EC',1,null,getdate(), 'UNAGRA', 'UNAGRA, S.A. de C.V., S.F.P.', 1, getdate(), 656 ),
('81EA94FB-50F9-45D9-9C13-255AC90EF1AC',1,null,getdate(), 'OPCIONES EMPRESARIALES DEL NOROESTE', 'OPCIONES EMPRESARIALES DEL NORESTE, S.A. DE C.V., S.F.P.', 1, getdate(), 659 ),
('0AB830FC-D190-4F7C-9488-DCCEADFA508E',1,null,getdate(), 'CLS', 'Cls Bank International', 1, getdate(), 901 ),
('8D2B1558-A098-4488-8FAD-D4033129A891',1,null,getdate(), 'INDEVAL', 'SD. Indeval, S.A. de C.V.', 1, getdate(), 902 ),
('26AABCAE-AE6D-423C-A72F-90AAAB64E551',1,null,getdate(), 'LIBERTAD', 'Libertad Servicios Financieros, S.A. De C.V.', 1, getdate(), 670 ),
('D9009E64-1107-4AF5-B26C-1C5C227F7392',1,null,getdate(), 'N/A', '', 1, getdate(), 999 )  
   
BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando Banks'
 MERGE [Bank] AS TARGET
 USING @Bank_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[Name]
      ,[Description]
      ,[StatusID]
      ,[CreationDate]
      ,[Code])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.[Name]
      ,SOURCE.[Description]
      ,SOURCE.[StatusID]
      ,SOURCE.[CreationDate]
      ,SOURCE.[Code])
 WHEN MATCHED THEN
  UPDATE SET 
  TARGET.NAME = SOURCE.NAME,
   TARGET.Code = SOURCE.Code,
   TARGET.[DESCRIPTION] = SOURCE.[DESCRIPTION];

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH


/*NOM Structure - GUIDE*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
DECLARE @NOMEvaluationGuide_Table TABLE(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [Number]           INT              NOT NULL
  )  

INSERT @NOMEvaluationGuide_Table ([ID], Active, DeleteDate, [Timestamp], [Name], [Description], StatusID, CreationDate, Number)
 VALUES 
  ('782D481C-2781-48D9-A76C-8CC449AE7654', 1, null, getdate(),   'ATS', 'Acontecimientos traumáticos severos', 1, getdate(), 1),
  ('917A40C1-1AEC-4C7B-BD63-3F1841868FFD', 1, null, getdate(),   'RP',  'Factores de riesgo psicosocial', 1, getdate(), 2),
  ('CAF01975-813B-4762-A79E-2185ADD7FCB7', 1, null, getdate(),   'EEO', 'Entorno organizacional en los centros de trabajo.', 1, getdate(), 3)
   
BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando NOMEvaluationGuide'
 MERGE [NOMEvaluationGuide] AS TARGET
 USING @NOMEvaluationGuide_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[Name]
      ,[Description]
      ,[StatusID]
      ,[CreationDate]
      ,[Number])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.[Name]
      ,SOURCE.[Description]
      ,SOURCE.[StatusID]
      ,SOURCE.[CreationDate]
      ,SOURCE.[Number])
 WHEN MATCHED THEN
  UPDATE SET TARGET.NAME = SOURCE.NAME,
   TARGET.Number = SOURCE.Number,
   TARGET.DESCRIPTION = SOURCE.DESCRIPTION;

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH

/*SURVEY*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
DECLARE @NOMEvaluationSurvey_Table TABLE(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [NOMEvaluationGuideID] UNIQUEIDENTIFIER NOT NULL,
    [Number]           INT              NOT NULL
  )  

INSERT @NOMEvaluationSurvey_Table ([ID], Active, DeleteDate, [Timestamp], [Name], [Description], StatusID, CreationDate, NOMEvaluationGuideID, Number)
 VALUES 
  ('6EC14905-F634-418F-B0D2-EF2F315835E8', 1, null, getdate(),   'Cuestionario de situaciones traumáticas', 'Identificar y apoyar a quienes vivieron situaciones fuertes en el trabajo.', 1, getdate(), '782D481C-2781-48D9-A76C-8CC449AE7654', 1),
  ('612A7938-1D11-4400-8BB7-AD29191AC33C', 1, null, getdate(),   'Evaluación de factores de riesgo psicosocial', 'Identificar los riesgos emocionales y sociales que se viven en el trabajo.', 1, getdate(), '917A40C1-1AEC-4C7B-BD63-3F1841868FFD', 1),
  ('DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 1, null, getdate(),   'Encuesta de entorno organizacional', 'Identificar factores de riesgo psicosocial y el entorno en la organización.', 1, getdate(), 'CAF01975-813B-4762-A79E-2185ADD7FCB7', 1)
BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando NOMEvaluationSurvey'
 MERGE NOMEvaluationSurvey AS TARGET
 USING @NOMEvaluationSurvey_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[Name]
      ,[Description]
      ,[StatusID]
      ,[CreationDate]
      ,[NOMEvaluationGuideID]
      ,[Number])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.[Name]
      ,SOURCE.[Description]
      ,SOURCE.[StatusID]
      ,SOURCE.[CreationDate]
      ,SOURCE.[NOMEvaluationGuideID]
      ,SOURCE.[Number])
 WHEN MATCHED THEN
  UPDATE SET TARGET.NAME = SOURCE.NAME,
  TARGET.DESCRIPTION = SOURCE.DESCRIPTION,
   TARGET.Number = SOURCE.Number,
   TARGET.NOMEvaluationGuideID = SOURCE.NOMEvaluationGuideID;

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH

/*PHASE*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
DECLARE @NOMEvaluationPhase_Table TABLE(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [NOMEvaluationSurveyID] UNIQUEIDENTIFIER NOT NULL,
    [Number]           INT              NOT NULL
  )  

INSERT @NOMEvaluationPhase_Table ([ID], Active, DeleteDate, [Timestamp], [Name], [Description], StatusID, CreationDate, NOMEvaluationSurveyID, [Number])
 VALUES 
 --ACONTECIMIENTOS TRAUMATICOS SEVEROS
  ('BAFC91F4-608C-4032-B39C-C591AEFF1CCA', 1, null, getdate(),   'I.- Acontecimiento traumático severo', '', 1, getdate(), '6EC14905-F634-418F-B0D2-EF2F315835E8', 1),
  ('BBE318C1-FC4F-4AB0-83D9-092F7F8EED56', 1, null, getdate(),   'II.- Recuerdos persistentes sobre el acontecimiento (durante el último mes).', '', 1, getdate(), '6EC14905-F634-418F-B0D2-EF2F315835E8', 2),
  ('A40C2435-1E1D-4277-91A0-30C85997040B', 1, null, getdate(),   'III.- Esfuerzo por evitar circunstancias parecidas o asociadas al acontecimiento (durante el último mes).', '', 1, getdate(), '6EC14905-F634-418F-B0D2-EF2F315835E8', 3),
  ('E734C8D4-4848-4DE7-B306-4B22D96B440B', 1, null, getdate(),   'IV.- Afectación (durante el último mes).', '', 1, getdate(), '6EC14905-F634-418F-B0D2-EF2F315835E8', 4),

  --FACTORES DE RIESGO PSICOSOCIAL
  ('C2970694-2727-4EFF-9E04-8394351B02EE', 1, null, getdate(),   'I. Las condiciones de su centro de trabajo, así como la cantidad y ritmo de trabajo.', '', 1, getdate(), '612A7938-1D11-4400-8BB7-AD29191AC33C', 1),
  ('5B3E2990-053B-4FA9-9137-668C9FFAA967', 1, null, getdate(),   'II. Las actividades que realiza en su trabajo y las responsabilidades que tiene.', '', 1, getdate(), '612A7938-1D11-4400-8BB7-AD29191AC33C', 2),
  ('68D20327-DE7B-47F8-BE95-D0056C9B9BEA', 1, null, getdate(),   'III. El tiempo destinado a su trabajo y sus responsabilidades familiares.', '', 1, getdate(), '612A7938-1D11-4400-8BB7-AD29191AC33C', 3),
  ('47AF2E20-D04C-40CE-8372-CCF4490A8F6E', 1, null, getdate(),   'IV. Las decisiones que puede tomar en su trabajo.', '', 1, getdate(), '612A7938-1D11-4400-8BB7-AD29191AC33C', 4),
  ('7F037662-F67A-43C8-B42B-D18AB90EE224', 1, null, getdate(),   'V. La capacitación e información que recibe sobre su trabajo.', '', 1, getdate(), '612A7938-1D11-4400-8BB7-AD29191AC33C', 5),
  ('A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF', 1, null, getdate(),   'VI. Las relaciones con sus compañeros de trabajo y su jefe.', '', 1, getdate(), '612A7938-1D11-4400-8BB7-AD29191AC33C', 6),
  ('C5CDF655-9749-4958-812A-99D9818F6071', 1, null, getdate(),   'VII. En mi trabajo debo brindar servicio a clientes o usuarios.', '', 1, getdate(), '612A7938-1D11-4400-8BB7-AD29191AC33C', 7),
  ('96BCA739-87CE-4B0E-9FF3-B68EDD82A5C8', 1, null, getdate(),   'VIII. Soy jefe de otros trabajadores.', '', 1, getdate(), '612A7938-1D11-4400-8BB7-AD29191AC33C', 8),
 
  --EVALUAR EL ENTORNO ORGANIZACIONAL EN LOS CENTROS DE TRABAJO
  ('8FBA07E8-080D-4E07-9D4F-D949B2DD0CAA', 1, null, getdate(),   'I. Condiciones ambientales de su centro de trabajo.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 1),
  ('2954C496-1744-4DB8-8567-9FC288E961D7', 1, null, getdate(),   'II. La cantidad y ritmo de trabajo que tiene.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 2),
  ('8979F115-8AB3-415B-B352-27CAF9FF81D4', 1, null, getdate(),   'III. El esfuerzo mental que le exige su trabajo.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 3),
  ('3A7DB61C-1CAE-4E25-8BF5-2FD9EB078CB7', 1, null, getdate(),   'IV. Trabajo y las responsabilidades que tiene.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 4),
  ('462BD7C4-0E0D-44CA-9EE5-000C8136319D', 1, null, getdate(),   'V. Jornada de trabajo.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 5),
  ('6A37A438-2C4F-45DB-A135-8AA41AF49B9D', 1, null, getdate(),   'VI. Decisiones que puede tomar en su trabajo.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 6),
  ('D3CF05CD-35FF-4F72-9F13-0DCC697F471D', 1, null, getdate(),   'VII. Cualquier tipo de cambio que ocurra en su trabajo (considere los últimos cambios realizados).', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 7),
  ('54F15956-252A-4305-BE72-7874B9D66BF7', 1, null, getdate(),   'VIII. capacitación e información que se le proporciona sobre su trabajo.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 8),
  ('94517067-8882-402D-8956-E15216638F92', 1, null, getdate(),   'IX. Jefes con quien tiene contacto.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 9),
  ('65338468-6CE3-4A03-BE2A-D4605B688590', 1, null, getdate(),   'X. Relaciones con sus compañeros.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 10),
  ('54AD20E8-A3EA-4CC7-803B-70DE0E69E186', 1, null, getdate(),   'XI. Información que recibe sobre su rendimiento en el trabajo, el reconocimiento, el sentido de pertenencia y la estabilidad que le ofrece su trabajo.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 11),
  ('7C88F094-E770-4E48-9D41-467871420D67', 1, null, getdate(),   'XII. Actos de violencia laboral (malos tratos, acoso, hostigamiento, acoso psicológico).', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 12),
  ('04090E9C-CDA5-45D5-A7ED-4AECC6C66BC6', 1, null, getdate(),   'XIII. Atención a clientes y usuarios.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 13),
  ('6BAE794D-BEC4-4504-B1D6-07238798B4E9', 1, null, getdate(),   'XIV. Las actitudes de las personas que supervisa.', '', 1, getdate(), 'DBED094B-F14F-450A-8EB6-B0F6CA03B9F4', 14)

BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando NOMEvaluationPhase'
 MERGE NOMEvaluationPhase AS TARGET
 USING @NOMEvaluationPhase_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[Name]
      ,[Description]
      ,[StatusID]
      ,[CreationDate]
      ,[NOMEvaluationSurveyID]
      ,[Number])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.[Name]
      ,SOURCE.[Description]
      ,SOURCE.[StatusID]
      ,SOURCE.[CreationDate]
      ,SOURCE.[NOMEvaluationSurveyID]
      ,SOURCE.[Number])
 WHEN MATCHED THEN
  UPDATE SET TARGET.NAME = SOURCE.NAME,
   TARGET.DESCRIPTION = SOURCE.DESCRIPTION,
    TARGET.Number = SOURCE.Number,
   TARGET.NOMEvaluationSurveyID = SOURCE.NOMEvaluationSurveyID;

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH

/*CATEGORY*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
DECLARE @NOMEvaluationCategory_Table TABLE(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [Number]           INT              NOT NULL
  )  

INSERT @NOMEvaluationCategory_Table ([ID], Active, DeleteDate, [Timestamp], [Name], [Description], StatusID, CreationDate, [Number])
 VALUES 
 --CATEGORIES
  ('8175C12C-0241-4CB7-86E5-FDD72EAA0251', 1, null, getdate(), 'Ambiente de trabajo', '', 1, getdate(), 1),
  ('FC2B2A50-A740-4ABE-BCE3-E9CF01010D43', 1, null, getdate(), 'Factores propios de la actividad', '', 1, getdate(), 2),
  ('020371F0-CE49-425D-A9DC-CC1D61243B1C', 1, null, getdate(), 'Organización del tiempo de trabajo', '', 1, getdate(), 3),
  ('7C950A92-9720-40A8-8A20-DFAC2C957BAC', 1, null, getdate(), 'Liderazgo y relaciones en el trabajo', '', 1, getdate(), 4)

BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando NOMEvaluationCategory'
 MERGE NOMEvaluationCategory AS TARGET
 USING @NOMEvaluationCategory_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[Name]
      ,[Description]
      ,[StatusID]
      ,[CreationDate]
      ,[Number])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.[Name]
      ,SOURCE.[Description]
      ,SOURCE.[StatusID]
      ,SOURCE.[CreationDate]
      ,SOURCE.[Number])
 WHEN MATCHED THEN
  UPDATE SET TARGET.NAME = SOURCE.NAME,
   TARGET.DESCRIPTION = SOURCE.DESCRIPTION,
    TARGET.Number = SOURCE.Number;

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH

/*DOMAIN*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
DECLARE @NOMEvaluationDomain_Table TABLE(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [Number]           INT              NOT NULL
  )  

INSERT @NOMEvaluationDomain_Table ([ID], Active, DeleteDate, [Timestamp], [Name], [Description], StatusID, CreationDate, [Number])
 VALUES 
 --DOMAIN
  ('203ED412-15DA-4458-90E3-5B21AD7E264D', 1, null, getdate(), 'Condiciones en el ambiente de trabajo', '', 1, getdate(), 1),
  ('A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 1, null, getdate(), 'Carga de trabajo', '', 1, getdate(), 2),
  ('D166E330-3CED-41C9-883C-E95000DC2317', 1, null, getdate(), 'Falta de control sobre el trabajo', '', 1, getdate(), 3),
  ('20A2A5E5-B383-47AB-ADBC-B06D035E9909', 1, null, getdate(), 'Jornada de trabajo', '', 1, getdate(), 4),
  ('9E4F23AB-34B7-41E0-BA90-8D76D06BEDAB', 1, null, getdate(), 'Interferencia en la relación trabajo-familia', '', 1, getdate(), 5),
  ('F89F3D8A-11F9-4FCD-81D2-62F99184B245', 1, null, getdate(), 'Liderazgo y relaciones en el trabajo', '', 1, getdate(), 6),
  ('33010112-4F95-4FDD-B9B2-5F4C5B734B3A', 1, null, getdate(), 'Relaciones en el trabajo', '', 1, getdate(), 7),
  ('D0BB7621-2E56-4B3A-8A46-B805C154C903', 1, null, getdate(), 'Violencia', '', 1, getdate(), 8)

BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando NOMEvaluationDomain'
 MERGE NOMEvaluationDomain AS TARGET
 USING @NOMEvaluationDomain_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[Name]
      ,[Description]
      ,[StatusID]
      ,[CreationDate]
      ,[Number])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.[Name]
      ,SOURCE.[Description]
      ,SOURCE.[StatusID]
      ,SOURCE.[CreationDate]
      ,SOURCE.[Number])
 WHEN MATCHED THEN
  UPDATE SET TARGET.NAME = SOURCE.NAME,
   TARGET.DESCRIPTION = SOURCE.DESCRIPTION,
    TARGET.Number = SOURCE.Number;

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH

/*QUESTION*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
DECLARE @NOMEvaluationQuestion_Table TABLE(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [Name]             NVARCHAR (1000)   NULL,
    [Description]      NVARCHAR (250)   NULL,
    [StatusID]         INT              NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [NOMEvaluationPhaseID] UNIQUEIDENTIFIER NOT NULL,	
    [NOMEvaluationCategoryID] UNIQUEIDENTIFIER NULL,	
    [NOMEvaluationDomainID] UNIQUEIDENTIFIER NULL,	
    [Number]			INT              NOT NULL	 
  )  

INSERT @NOMEvaluationQuestion_Table ([ID], Active, DeleteDate, [Timestamp], [Name], [Description], StatusID, CreationDate, NOMEvaluationPhaseID,  [NOMEvaluationCategoryID], [NOMEvaluationDomainID], Number)
 VALUES 
  --ACONTECIMIENTOS TRAUMATICOS SEVEROS - I.- Acontecimiento traumático severo
  ('BB06A330-EE7B-453F-B9C1-2A17E7FDEC3D', 1, null, getdate(), '¿Ha presenciado o sufrido alguna vez, durante o con motivo del trabajo un acontecimiento como los siguientes: Accidente que tenga como consecuencia la muerte, la pérdida de un miembro o una lesión grave? Asaltos? Actos violentos que derivaron en lesiones graves? Secuestro? Amenazas?, o Cualquier otro que ponga en riesgo su vida o salud, y/o la de otras personas?', '', 1, getdate(), 'BAFC91F4-608C-4032-B39C-C591AEFF1CCA', NULL, NULL, 1),
  
  --ACONTECIMIENTOS TRAUMATICOS SEVEROS - II.- Recuerdos persistentes sobre el acontecimiento (durante el último mes)
  ('09EEF251-8E94-41BD-8075-0A0849C83CDA', 1, null, getdate(), '¿Ha tenido recuerdos recurrentes sobre el acontecimiento que le provocan malestares?', '', 1, getdate(), 'BBE318C1-FC4F-4AB0-83D9-092F7F8EED56',NULL, NULL, 1),
  ('E8446625-924A-4978-86A6-153941AEC13C', 1, null, getdate(), '¿Ha tenido sueños de carácter recurrente sobre el acontecimiento, que le producen malestar?', '', 1, getdate(), 'BBE318C1-FC4F-4AB0-83D9-092F7F8EED56',NULL, NULL, 2),

  --ACONTECIMIENTOS TRAUMATICOS SEVEROS - III.- Esfuerzo por evitar circunstancias parecidas o asociadas al acontecimiento (durante el último mes)
  ('ED4DE4DE-5BA2-4DA9-A415-2DB88095E1E6', 1, null, getdate(), '¿Se ha esforzado por evitar todo tipo de sentimientos, conversaciones o situaciones que le puedan recordar el acontecimiento?', '', 1, getdate(), 'A40C2435-1E1D-4277-91A0-30C85997040B',NULL, NULL, 1),
  ('7A367CE8-283F-4259-8530-F84312B7B96F', 1, null, getdate(), '¿Se ha esforzado por evitar todo tipo de actividades, lugares o personas que motivan recuerdos del acontecimiento?', '', 1, getdate(), 'A40C2435-1E1D-4277-91A0-30C85997040B',NULL, NULL, 2),
  ('2E4ED644-5D0D-47D1-8B0F-5B7A8B3FBF9F', 1, null, getdate(), '¿Ha tenido dificultad para recordar alguna parte importante del evento?', '', 1, getdate(), 'A40C2435-1E1D-4277-91A0-30C85997040B',NULL, NULL, 3),
  ('A52F3303-E155-4BAA-ACF1-3F50C946DF07', 1, null, getdate(), '¿Ha disminuido su interés en sus actividades cotidianas?', '', 1, getdate(), 'A40C2435-1E1D-4277-91A0-30C85997040B',NULL, NULL, 4),
  ('A34EF1F3-F09E-448E-9BAC-0C4644AC49F2', 1, null, getdate(), '¿Se ha sentido usted alejado o distante de los demás?', '', 1, getdate(), 'A40C2435-1E1D-4277-91A0-30C85997040B',NULL, NULL, 5),
  ('A8014D7C-8974-47F6-BD54-0430BB7FB41F', 1, null, getdate(), '¿Ha notado que tiene dificultad para expresar sus sentimientos?', '', 1, getdate(), 'A40C2435-1E1D-4277-91A0-30C85997040B',NULL, NULL, 6),
  ('A1C53C2C-0E14-4266-97C7-2C196D74BC5D', 1, null, getdate(), '¿Ha tenido la impresión de que su vida se va a acortar, que va a morir antes que otras personas o que tiene un futuro limitado?', '', 1, getdate(), 'A40C2435-1E1D-4277-91A0-30C85997040B',NULL, NULL, 7),

  --ACONTECIMIENTOS TRAUMATICOS SEVEROS - IV.- Afectación (durante el último mes)
  ('038E4393-1FCB-4E10-BCC7-EE1B8FA195D7', 1, null, getdate(), '¿Ha tenido usted dificultades para dormir?', '', 1, getdate(), 'E734C8D4-4848-4DE7-B306-4B22D96B440B',NULL, NULL, 1),
  ('76DE30CC-E462-4E45-9500-E3E7E385BAE0', 1, null, getdate(), '¿Ha estado particularmente irritable o le han dado arranques de coraje?', '', 1, getdate(), 'E734C8D4-4848-4DE7-B306-4B22D96B440B',NULL, NULL, 2),
  ('86BCB86D-B66E-44CC-8F0E-137DFC119D19', 1, null, getdate(), '¿Ha tenido dificultad para concentrarse?', '', 1, getdate(), 'E734C8D4-4848-4DE7-B306-4B22D96B440B',NULL, NULL, 3),
  ('5F56F646-62A3-4B71-86C0-8DD66AD0E730', 1, null, getdate(), '¿Ha estado nervioso o constantemente en alerta?', '', 1, getdate(), 'E734C8D4-4848-4DE7-B306-4B22D96B440B',NULL, NULL, 4),
  ('06B1599B-D9C6-4999-9F07-44C0C1AF0369', 1, null, getdate(), '¿Se ha sobresaltado fácilmente por cualquier cosa?', '', 1, getdate(), 'E734C8D4-4848-4DE7-B306-4B22D96B440B',NULL, NULL, 5),
  
  --RP - I. Las condiciones de su centro de trabajo, así como la cantidad y ritmo de trabajo.
  ('60D48AB2-F8D9-4C83-BB08-D4FBFED8C0DC', 1, null, getdate(), 'Mi trabajo me exige hacer mucho esfuerzo físico.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE','8175C12C-0241-4CB7-86E5-FDD72EAA0251','203ED412-15DA-4458-90E3-5B21AD7E264D', 1),
  ('6E6840B6-6FB1-40B2-9C93-FA048AEE792A', 1, null, getdate(), 'Me preocupa sufrir un accidente en mi trabajo.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE','8175C12C-0241-4CB7-86E5-FDD72EAA0251','203ED412-15DA-4458-90E3-5B21AD7E264D', 2),
  ('5424AABC-4FEB-4DF0-B567-AC04FDC33DDB', 1, null, getdate(), 'Considero que las actividades que realizo son peligrosas.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE','8175C12C-0241-4CB7-86E5-FDD72EAA0251','203ED412-15DA-4458-90E3-5B21AD7E264D', 3),
  ('5E41A1BF-76EB-4C52-A24E-7FD45271D2D4', 1, null, getdate(), 'Por la cantidad de trabajo que tengo debo quedarme tiempo adicional a mi turno.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 4),
  ('693F64CA-4BFD-48C6-8806-4ADF946DF927', 1, null, getdate(), 'Por la cantidad de trabajo que tengo debo trabajar sin parar.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE', 'FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 5),
  ('24ACBD6E-399B-48A4-A113-7C822899D23B', 1, null, getdate(), 'Considero que es necesario mantener un ritmo de trabajo acelerado.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE', 'FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 6),
  ('E0977A6F-12BF-4095-B269-0125371E2804', 1, null, getdate(), 'Mi trabajo exige que esté muy concentrado.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 7),
  ('8A8C46F5-D4E4-4D7C-ABCE-3A0CC4CD818F', 1, null, getdate(), 'Mi trabajo requiere que memorice mucha información.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 8),
  ('B3DDC8C0-E98D-47D6-9C5C-59529AC2F943', 1, null, getdate(), 'Mi trabajo exige que atienda varios asuntos al mismo tiempo.', '', 1, getdate(), 'C2970694-2727-4EFF-9E04-8394351B02EE','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 9),

  --RP - II. Las actividades que realiza en su trabajo y las responsabilidades que tiene.
  ('F7C3232C-D529-4D80-8E47-07B06AD93DA8', 1, null, getdate(), 'En mi trabajo soy responsable de cosas de mucho valor.', '', 1, getdate(), '5B3E2990-053B-4FA9-9137-668C9FFAA967','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 10),
  ('86324B28-9964-429A-BCC4-C13607A99189', 1, null, getdate(), 'Respondo ante mi jefe por los resultados de toda mi área de trabajo.', '', 1, getdate(), '5B3E2990-053B-4FA9-9137-668C9FFAA967','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 11),
  ('A7635767-3451-4B31-9338-5DF133C6B682', 1, null, getdate(), 'En mi trabajo me dan órdenes contradictorias.', '', 1, getdate(), '5B3E2990-053B-4FA9-9137-668C9FFAA967','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 12),
  ('96BAFC6F-8985-439F-8CAC-99F905668E91', 1, null, getdate(), 'Considero que en mi trabajo me piden hacer cosas innecesarias.', '', 1, getdate(), '5B3E2990-053B-4FA9-9137-668C9FFAA967','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 13),
  
  --RP - III. El tiempo destinado a su trabajo y sus responsabilidades familiares.
  ('B854B373-D450-4D41-9A0C-0AD38AE337B8', 1, null, getdate(), 'Trabajo horas extras más de tres veces a la semana.', '', 1, getdate(), '68D20327-DE7B-47F8-BE95-D0056C9B9BEA', '020371F0-CE49-425D-A9DC-CC1D61243B1C','20A2A5E5-B383-47AB-ADBC-B06D035E9909', 14),
  ('92144242-1590-4168-B3CE-460489910BF6', 1, null, getdate(), 'Mi trabajo me exige laborar en días de descanso, festivos o fines de semana.', '', 1, getdate(), '68D20327-DE7B-47F8-BE95-D0056C9B9BEA', '020371F0-CE49-425D-A9DC-CC1D61243B1C','20A2A5E5-B383-47AB-ADBC-B06D035E9909', 15),
  ('7CDCAE1B-56ED-4ECB-8766-44BDDCCE0ADF', 1, null, getdate(), 'Considero que el tiempo en el trabajo es mucho y perjudica mis actividades familiares o personales.', '', 1, getdate(), '68D20327-DE7B-47F8-BE95-D0056C9B9BEA', '020371F0-CE49-425D-A9DC-CC1D61243B1C', '9E4F23AB-34B7-41E0-BA90-8D76D06BEDAB', 16),
  ('4244AD34-1F66-44D4-9543-7C171276C7BD', 1, null, getdate(), 'Pienso en las actividades familiares o personales cuando estoy en mi trabajo.', '', 1, getdate(), '68D20327-DE7B-47F8-BE95-D0056C9B9BEA',  '020371F0-CE49-425D-A9DC-CC1D61243B1C', '9E4F23AB-34B7-41E0-BA90-8D76D06BEDAB', 17),

  --RP - IV. Las decisiones que puede tomar en su trabajo.
  ('B50006F7-0BF8-4DD3-BCC1-C9C57AC92E56', 1, null, getdate(), 'Mi trabajo permite que desarrolle nuevas habilidades.', '', 1, getdate(), '47AF2E20-D04C-40CE-8372-CCF4490A8F6E', 'FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','D166E330-3CED-41C9-883C-E95000DC2317', 18),
  ('ECA98BFF-EAEB-4838-83CF-8E40BBEC102B', 1, null, getdate(), 'En mi trabajo puedo aspirar a un mejor puesto.', '', 1, getdate(), '47AF2E20-D04C-40CE-8372-CCF4490A8F6E', 'FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','D166E330-3CED-41C9-883C-E95000DC2317',19),
  ('6504790C-6E50-4C79-9242-1064266B10F1', 1, null, getdate(), 'Durante mi jornada de trabajo puedo tomar pausas cuando las necesito.', '', 1, getdate(), '47AF2E20-D04C-40CE-8372-CCF4490A8F6E','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','D166E330-3CED-41C9-883C-E95000DC2317', 20),
  ('C9453123-619F-47BD-B7FB-D0464F8EA64A', 1, null, getdate(), 'Puedo decidir la velocidad a la que realizo mis actividades en mi trabajo.', '', 1, getdate(), '47AF2E20-D04C-40CE-8372-CCF4490A8F6E','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','D166E330-3CED-41C9-883C-E95000DC2317', 21),
  ('AAF4B9C5-075E-42DB-8DEE-11CB05ECB2FF', 1, null, getdate(), 'Puedo cambiar el orden de las actividades que realizo en mi trabajo.', '', 1, getdate(), '47AF2E20-D04C-40CE-8372-CCF4490A8F6E','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','D166E330-3CED-41C9-883C-E95000DC2317', 22),
  
  --RP - V. La capacitación e información que recibe sobre su trabajo.
  ('94FDA6DD-455B-4C70-BDE8-99CE2AFA4566', 1, null, getdate(), 'Me informan con claridad cuáles son mis funciones.', '', 1, getdate(), '7F037662-F67A-43C8-B42B-D18AB90EE224','7C950A92-9720-40A8-8A20-DFAC2C957BAC','F89F3D8A-11F9-4FCD-81D2-62F99184B245', 23),
  ('7F037662-F67A-43C8-B42B-D18AB90EE224', 1, null, getdate(), 'Me explican claramente los resultados que debo obtener en mi trabajo.', '', 1, getdate(), '7F037662-F67A-43C8-B42B-D18AB90EE224','7C950A92-9720-40A8-8A20-DFAC2C957BAC','F89F3D8A-11F9-4FCD-81D2-62F99184B245', 24),
  ('AD69943F-986E-447B-B467-8EAEC31E5EC9', 1, null, getdate(), 'Me informan con quién puedo resolver problemas o asuntos de trabajo.', '', 1, getdate(), '7F037662-F67A-43C8-B42B-D18AB90EE224','7C950A92-9720-40A8-8A20-DFAC2C957BAC','F89F3D8A-11F9-4FCD-81D2-62F99184B245', 25),
  ('78B07275-E350-4E49-B2D4-80DAFEF37DAA', 1, null, getdate(), 'Me permiten asistir a capacitaciones relacionadas con mi trabajo.', '', 1, getdate(), '7F037662-F67A-43C8-B42B-D18AB90EE224','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','D166E330-3CED-41C9-883C-E95000DC2317', 26),
  ('E09B4D6D-165C-474B-B15B-79D69C0CBF0D', 1, null, getdate(), 'Recibo capacitación útil para hacer mi trabajo.', '', 1, getdate(), '7F037662-F67A-43C8-B42B-D18AB90EE224','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','D166E330-3CED-41C9-883C-E95000DC2317', 27),
  
  --RP - VI. Las relaciones con sus compañeros de trabajo y su jefe.
  ('301F7649-47AA-4C03-A5A5-D0AAFD34D1C7', 1, null, getdate(), 'Mi jefe tiene en cuenta mis puntos de vista y opiniones.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC','F89F3D8A-11F9-4FCD-81D2-62F99184B245', 28),
  ('7CEEC221-E3B3-4B0D-8418-B469CF14EE3A', 1, null, getdate(), 'Mi jefe ayuda a solucionar los problemas que se presentan en el trabajo.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF', '7C950A92-9720-40A8-8A20-DFAC2C957BAC','F89F3D8A-11F9-4FCD-81D2-62F99184B245', 29),
  ('7F01309C-4971-423D-AA29-320D4D50E09A', 1, null, getdate(), 'Puedo confiar en mis compañeros de trabajo.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC','33010112-4F95-4FDD-B9B2-5F4C5B734B3A', 30),
  ('5BEF19ED-3373-46AD-A88B-C41629D246C3', 1, null, getdate(), 'Cuando tenemos que realizar trabajo de equipo los compañeros colaboran.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC','33010112-4F95-4FDD-B9B2-5F4C5B734B3A', 31),
  ('009514A8-6DD2-4B39-B80E-C5F34E4F479E', 1, null, getdate(), 'Mis compañeros de trabajo me ayudan cuando tengo dificultades.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC','33010112-4F95-4FDD-B9B2-5F4C5B734B3A', 32),
  ('1C080B53-A1C4-48A8-ABE5-74FBCC054133', 1, null, getdate(), 'En mi trabajo puedo expresarme libremente sin interrupciones.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC', 'D0BB7621-2E56-4B3A-8A46-B805C154C903', 33),
  ('0A72EBF5-5436-4A87-9460-AF6709A6B6F3', 1, null, getdate(), 'Recibo críticas constantes a mi persona y/o trabajo.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC', 'D0BB7621-2E56-4B3A-8A46-B805C154C903',  34),
  ('84365117-E0C5-42BD-8A90-DC20CE18C3A9', 1, null, getdate(), 'Recibo burlas, calumnias, difamaciones, humillaciones o ridiculizaciones.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC', 'D0BB7621-2E56-4B3A-8A46-B805C154C903',  35),
  ('AA2F7B32-A6CB-4513-9652-2E78DC5E561D', 1, null, getdate(), 'Se ignora mi presencia o se me excluye de las reuniones de trabajo y en la toma de decisiones.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC', 'D0BB7621-2E56-4B3A-8A46-B805C154C903',  36),
  ('9A80394A-4982-4D3E-A984-1E7A3E3E636B', 1, null, getdate(), 'Se manipulan las situaciones de trabajo para hacerme parecer un mal trabajador.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF', '7C950A92-9720-40A8-8A20-DFAC2C957BAC', 'D0BB7621-2E56-4B3A-8A46-B805C154C903', 37),
  ('D7B4287F-F17D-4677-BDF8-F071C8F21279', 1, null, getdate(), 'Se ignoran mis éxitos laborales y se atribuyen a otros trabajadores.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF', '7C950A92-9720-40A8-8A20-DFAC2C957BAC', 'D0BB7621-2E56-4B3A-8A46-B805C154C903', 38),
  ('DF968460-4FB9-4AE3-95E5-C624CD0D879A', 1, null, getdate(), 'Me bloquean o impiden las oportunidades que tengo para obtener ascenso o mejora en mi trabajo.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF','7C950A92-9720-40A8-8A20-DFAC2C957BAC', 'D0BB7621-2E56-4B3A-8A46-B805C154C903',  39),
  ('401D4712-22ED-4B32-AF40-C26BE6FB6E6E', 1, null, getdate(), 'He presenciado actos de violencia en mi centro de trabajo.', '', 1, getdate(), 'A1EBC87C-6902-4F5C-9C27-DC159AAA7EBF', '7C950A92-9720-40A8-8A20-DFAC2C957BAC', 'D0BB7621-2E56-4B3A-8A46-B805C154C903', 40),

  --RP - VII. En mi trabajo debo brindar servicio a clientes o usuarios.
  ('70A85006-DC1C-4D71-B4A8-F8D003173500', 1, null, getdate(), 'Atiendo clientes o usuarios muy enojados.', '', 1, getdate(), 'C5CDF655-9749-4958-812A-99D9818F6071','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E', 41),
  ('D188B220-60A4-4CC0-BE2C-CC005EDAB918', 1, null, getdate(), 'Mi trabajo me exige atender personas muy necesitadas de ayuda o enfermas.', '', 1, getdate(), 'C5CDF655-9749-4958-812A-99D9818F6071','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E',  42),
  ('46E9E96F-4E8E-4500-8E45-76A336464A7B', 1, null, getdate(), 'Para hacer mi trabajo debo demostrar sentimientos distintos a los míos.', '', 1, getdate(), 'C5CDF655-9749-4958-812A-99D9818F6071','FC2B2A50-A740-4ABE-BCE3-E9CF01010D43','A975DC07-95E3-4220-AFD1-91E0CEE2C69E',  43),
  
  --RP - VIII. Soy jefe de otros trabajadores.
  ('317BDA14-EDBC-48DD-9F4A-52852B48F7BF', 1, null, getdate(), 'Comunican tarde los asuntos de trabajo.', '', 1, getdate(), '96BCA739-87CE-4B0E-9FF3-B68EDD82A5C8','7C950A92-9720-40A8-8A20-DFAC2C957BAC','33010112-4F95-4FDD-B9B2-5F4C5B734B3A', 44),
  ('6D492677-E071-46AA-9FDE-E7D4AA44B911', 1, null, getdate(), 'Dificultan el logro de los resultados del trabajo.', '', 1, getdate(), '96BCA739-87CE-4B0E-9FF3-B68EDD82A5C8','7C950A92-9720-40A8-8A20-DFAC2C957BAC','33010112-4F95-4FDD-B9B2-5F4C5B734B3A', 45),
  ('9235BED5-C168-4599-B47B-C9AF070B8834', 1, null, getdate(), 'Ignoran las sugerencias para mejorar su trabajo.', '', 1, getdate(), '96BCA739-87CE-4B0E-9FF3-B68EDD82A5C8','7C950A92-9720-40A8-8A20-DFAC2C957BAC','33010112-4F95-4FDD-B9B2-5F4C5B734B3A', 46),

  --EEO - I. Condiciones ambientales de su centro de trabajo.
  ('3EBCF53A-1BBA-4CB8-B55E-E70A44B7161D', 1, null, getdate(), 'El espacio donde trabajo me permite realizar mis actividades de manera segura e higiénica.', '', 1, getdate(), '8FBA07E8-080D-4E07-9D4F-D949B2DD0CAA',NULL, NULL, 1),
  ('0D1130A1-57EE-4FC4-9387-2DFB1B34ED75', 1, null, getdate(), 'Mi trabajo me exige hacer mucho esfuerzo físico.', '', 1, getdate(), '8FBA07E8-080D-4E07-9D4F-D949B2DD0CAA', NULL, NULL,2),
  ('794EDFE6-6E2D-46D1-9486-35074C8980A3', 1, null, getdate(), 'Me preocupa sufrir un accidente en mi trabajo.', '', 1, getdate(), '8FBA07E8-080D-4E07-9D4F-D949B2DD0CAA', NULL, NULL,3),
  ('3707E77B-E722-40BD-9E27-CA09E3C0E1A2', 1, null, getdate(), 'Considero que en mi trabajo se aplican las normas de seguridad y salud en el trabajo.', '', 1, getdate(), '8FBA07E8-080D-4E07-9D4F-D949B2DD0CAA',NULL, NULL, 4),
  ('AE48905C-D98B-4EB8-8927-EAA88D4A5AC3', 1, null, getdate(), 'Considero que las actividades que realizo son peligrosas.', '', 1, getdate(), '8FBA07E8-080D-4E07-9D4F-D949B2DD0CAA',NULL, NULL, 5),

  --EEO - II. La cantidad y ritmo de trabajo que tiene.
  ('2BCD14D9-462F-4643-9D27-D89E47169FD6', 1, null, getdate(), 'Por la cantidad de trabajo que tengo debo quedarme tiempo adicional a mi turno.', '', 1, getdate(), '2954C496-1744-4DB8-8567-9FC288E961D7',NULL, NULL, 1),
  ('A1A8986A-D0A0-4592-91FC-53FDF5099057', 1, null, getdate(), 'Por la cantidad de trabajo que tengo debo trabajar sin parar.', '', 1, getdate(), '2954C496-1744-4DB8-8567-9FC288E961D7',NULL, NULL, 2),
  ('F9AF2815-6F9F-4F0E-A1F8-6BB5E5FC4F93', 1, null, getdate(), 'Considero que es necesario mantener un ritmo de trabajo acelerado.', '', 1, getdate(), '2954C496-1744-4DB8-8567-9FC288E961D7', NULL, NULL,3),
  
  --EEO - III. El esfuerzo mental que le exige su trabajo.
  ('6B1E0774-B681-4295-98C2-878273F1502C', 1, null, getdate(), 'Mi trabajo exige que esté muy concentrado.', '', 1, getdate(), '8979F115-8AB3-415B-B352-27CAF9FF81D4',NULL, NULL, 1),
  ('78462A24-CD17-4CDA-9D10-24EA61742FAF', 1, null, getdate(), 'Mi trabajo requiere que memorice mucha información.', '', 1, getdate(), '8979F115-8AB3-415B-B352-27CAF9FF81D4',NULL, NULL, 2),
  ('60C3EFBB-D882-4F67-AAEB-D678DF6039FB', 1, null, getdate(), 'En mi trabajo tengo que tomar decisiones difíciles muy rápido.', '', 1, getdate(), '8979F115-8AB3-415B-B352-27CAF9FF81D4',NULL, NULL, 3),
  ('82F45920-CC06-41D3-9E7A-25C8046AD545', 1, null, getdate(), 'Mi trabajo exige que atienda varios asuntos al mismo tiempo.', '', 1, getdate(), '8979F115-8AB3-415B-B352-27CAF9FF81D4', NULL, NULL,4),
  
  --EEO - IV. Trabajo y las responsabilidades que tiene.
  ('CE5DDD82-1992-4DC8-A692-4C0AA1F63AE9', 1, null, getdate(), 'En mi trabajo soy responsable de cosas de mucho valor.', '', 1, getdate(), '3A7DB61C-1CAE-4E25-8BF5-2FD9EB078CB7',NULL, NULL, 1),
  ('69F64BBA-5EFB-4B81-9624-12149715FA9B', 1, null, getdate(), 'Respondo ante mi jefe por los resultados de toda mi área de trabajo.', '', 1, getdate(), '3A7DB61C-1CAE-4E25-8BF5-2FD9EB078CB7',NULL, NULL, 2),
  ('ABC1198E-3104-4080-A56A-20666E089FC5', 1, null, getdate(), 'En el trabajo me dan órdenes contradictorias.', '', 1, getdate(), '3A7DB61C-1CAE-4E25-8BF5-2FD9EB078CB7', NULL, NULL,3),
  ('DDC1E4FA-66C4-4CF6-A979-AE617FB7C03C', 1, null, getdate(), 'Considero que en mi trabajo me piden hacer cosas innecesarias.', '', 1, getdate(), '3A7DB61C-1CAE-4E25-8BF5-2FD9EB078CB7', NULL, NULL,4),

  --EEO - V. Jornada de trabajo.
  ('98DD7268-9066-4675-A770-29269077940E', 1, null, getdate(), 'Trabajo horas extras más de tres veces a la semana.', '', 1, getdate(), '462BD7C4-0E0D-44CA-9EE5-000C8136319D',NULL, NULL, 1),
  ('9670ACF7-8EA7-4371-9618-0303CEC09BF9', 1, null, getdate(), 'Mi trabajo me exige laborar en días de descanso, festivos o fines de semana.', '', 1, getdate(), '462BD7C4-0E0D-44CA-9EE5-000C8136319D', NULL, NULL,2),
  ('C1E7CE27-B19B-44B8-B7CC-31C145F2E0A4', 1, null, getdate(), 'Considero que el tiempo en el trabajo es mucho y perjudica mis actividades familiares o personales.', '', 1, getdate(), '462BD7C4-0E0D-44CA-9EE5-000C8136319D',NULL, NULL, 3),
  ('D23EDA7F-09B2-4747-85FC-381E3E28A16A', 1, null, getdate(), 'Debo atender asuntos de trabajo cuando estoy en casa.', '', 1, getdate(), '462BD7C4-0E0D-44CA-9EE5-000C8136319D',NULL, NULL, 4),
  ('B0C328E9-1988-4850-B89F-BCE0B0C9D5E6', 1, null, getdate(), 'Pienso en las actividades familiares o personales cuando estoy en mi trabajo.', '', 1, getdate(), '462BD7C4-0E0D-44CA-9EE5-000C8136319D',NULL, NULL, 5),
  ('45D91013-1CAA-466E-B27A-840049586E87', 1, null, getdate(), 'Pienso que mis responsabilidades familiares afectan mi trabajo.', '', 1, getdate(), '462BD7C4-0E0D-44CA-9EE5-000C8136319D',NULL, NULL, 6),

  --EEO - VI. Decisiones que puede tomar en su trabajo.
  ('6DCEE5A9-A6DE-4D24-AB4B-B2DD546E95E1', 1, null, getdate(), 'Mi trabajo permite que desarrolle nuevas habilidades.', '', 1, getdate(), '6A37A438-2C4F-45DB-A135-8AA41AF49B9D', NULL, NULL,1),
  ('D0053B02-6564-4B66-8DE1-FB2FAFF68B19', 1, null, getdate(), 'En mi trabajo puedo aspirar a un mejor puesto.', '', 1, getdate(), '6A37A438-2C4F-45DB-A135-8AA41AF49B9D',NULL, NULL, 2),
  ('EB1ED9D9-735F-408D-935E-FEE2DEDD241E', 1, null, getdate(), 'Durante mi jornada de trabajo puedo tomar pausas cuando las necesito.', '', 1, getdate(), '6A37A438-2C4F-45DB-A135-8AA41AF49B9D',NULL, NULL, 3),
  ('C11B9F8D-30C0-489D-8E5A-4734C5C26E55', 1, null, getdate(), 'Puedo decidir cuánto trabajo realizo durante la jornada laboral.', '', 1, getdate(), '6A37A438-2C4F-45DB-A135-8AA41AF49B9D', NULL, NULL,4),
  ('591341C5-959A-4AA5-8C85-009069AB220B', 1, null, getdate(), 'Puedo decidir la velocidad a la que realizo mis actividades en mi trabajo.', '', 1, getdate(), '6A37A438-2C4F-45DB-A135-8AA41AF49B9D', NULL, NULL,5),
  ('1FB9F44D-BEB8-4770-A571-5B27C1D691C1', 1, null, getdate(), 'Puedo cambiar el orden de las actividades que realizo en mi trabajo.', '', 1, getdate(), '6A37A438-2C4F-45DB-A135-8AA41AF49B9D',NULL, NULL, 6),

  --EEO - VII. Cualquier tipo de cambio que ocurra en su trabajo (considere los últimos cambios realizados).
  ('73138B46-7869-4476-BCDA-FABCD17947BC', 1, null, getdate(), 'Los cambios que se presentan en mi trabajo dificultan mi labor.', '', 1, getdate(), 'D3CF05CD-35FF-4F72-9F13-0DCC697F471D',NULL, NULL, 1),
  ('9E2EE0C7-4F0B-48CD-801C-530155B0A549', 1, null, getdate(), 'Cuando se presentan cambios en mi trabajo se tienen en cuenta mis ideas o aportaciones.', '', 1, getdate(), 'D3CF05CD-35FF-4F72-9F13-0DCC697F471D',NULL, NULL, 2),
  
  --EEO - VIII. capacitación e información que se le proporciona sobre su trabajo.
  ('500DAA70-53CC-4788-A3EC-BFCFFC72D6EB', 1, null, getdate(), 'Me informan con claridad cuáles son mis funciones.', '', 1, getdate(), '54F15956-252A-4305-BE72-7874B9D66BF7', NULL, NULL,1),
  ('FFCEC1B0-7D75-443E-9F16-22AC9B956EA0', 1, null, getdate(), 'Me explican claramente los resultados que debo obtener en mi trabajo.', '', 1, getdate(), '54F15956-252A-4305-BE72-7874B9D66BF7',NULL, NULL, 2),
  ('2302A7E4-F0C7-4537-A474-2C71A0866B15', 1, null, getdate(), 'Me explican claramente los objetivos de mi trabajo.', '', 1, getdate(), '54F15956-252A-4305-BE72-7874B9D66BF7', NULL, NULL,3),
  ('E624E446-5472-4C95-8C8E-E6575F267325', 1, null, getdate(), 'Me informan con quién puedo resolver problemas o asuntos de trabajo.', '', 1, getdate(), '54F15956-252A-4305-BE72-7874B9D66BF7',NULL, NULL, 4),
  ('45F0C8A8-9C9F-4C2E-86E0-DF3B56752471', 1, null, getdate(), 'Me permiten asistir a capacitaciones relacionadas con mi trabajo.', '', 1, getdate(), '54F15956-252A-4305-BE72-7874B9D66BF7',NULL, NULL, 5),
  ('0D9FF05D-7200-4A6D-A8D6-A5CE1E13FD0D', 1, null, getdate(), 'Recibo capacitación útil para hacer mi trabajo.', '', 1, getdate(), '54F15956-252A-4305-BE72-7874B9D66BF7', NULL, NULL,6),
  
  --EEO - IX. Jefes con quien tiene contacto.
  ('4EC22284-C518-4ECE-B8D8-4B778A27DD5D', 1, null, getdate(), 'Mi jefe ayuda a organizar mejor el trabajo.', '', 1, getdate(), '94517067-8882-402D-8956-E15216638F92',NULL, NULL, 1),
  ('A57D8D34-B535-400B-952D-1A277E667440', 1, null, getdate(), 'Mi jefe tiene en cuenta mis puntos de vista y opiniones.', '', 1, getdate(), '94517067-8882-402D-8956-E15216638F92',NULL, NULL, 2),
  ('9914C569-4657-4F7F-A3DD-D4E0DB4FAFAA', 1, null, getdate(), 'Mi jefe me comunica a tiempo la información relacionada con el trabajo.', '', 1, getdate(), '94517067-8882-402D-8956-E15216638F92',NULL, NULL, 3),
  ('0E7BE521-182C-48A4-88A4-94C4E44E5EAB', 1, null, getdate(), 'La orientación que me da mi jefe me ayuda a realizar mejor mi trabajo.', '', 1, getdate(), '94517067-8882-402D-8956-E15216638F92',NULL, NULL, 4),
  ('F4B603C3-BDFA-4D85-ABC9-45751420A67E', 1, null, getdate(), 'Mi jefe ayuda a solucionar los problemas que se presentan en el trabajo.', '', 1, getdate(), '94517067-8882-402D-8956-E15216638F92', NULL, NULL,5),
  
  --EEO - X. Relaciones con sus compañeros.
  ('F490F6A8-F5E5-48EE-BC4F-BB0240DF8963', 1, null, getdate(), 'Puedo confiar en mis compañeros de trabajo.', '', 1, getdate(), '65338468-6CE3-4A03-BE2A-D4605B688590',NULL, NULL, 1),
  ('7561D7D4-E905-43C0-92C7-E560EC9450C1', 1, null, getdate(), 'Entre compañeros solucionamos los problemas de trabajo de forma respetuosa.', '', 1, getdate(), '65338468-6CE3-4A03-BE2A-D4605B688590',NULL, NULL, 2),
  ('A20B2BFA-EB0F-4526-8A42-49DC3BC38389', 1, null, getdate(), 'En mi trabajo me hacen sentir parte del grupo.', '', 1, getdate(), '65338468-6CE3-4A03-BE2A-D4605B688590',NULL, NULL, 3),
  ('3CE9FF6E-98B8-4268-9846-2D964170464B', 1, null, getdate(), 'Cuando tenemos que realizar trabajo de equipo los compañeros colaboran.', '', 1, getdate(), '65338468-6CE3-4A03-BE2A-D4605B688590',NULL, NULL, 4),
  ('B605F994-3563-46FD-B82A-96C94112FA87', 1, null, getdate(), 'Mis compañeros de trabajo me ayudan cuando tengo dificultades.', '', 1, getdate(), '65338468-6CE3-4A03-BE2A-D4605B688590', NULL, NULL,5),

  --EEO - XI. Información que recibe sobre su rendimiento en el trabajo, el reconocimiento, el sentido de pertenencia y la estabilidad que le ofrece su trabajo.
  ('94B2C38C-1CE9-4E4D-9B75-14AAD2AC2169', 1, null, getdate(), 'Me informan sobre lo que hago bien en mi trabajo.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186', NULL, NULL,1),
  ('B93CE7C0-D2CC-4134-82B3-23F60E34905A', 1, null, getdate(), 'La forma como evalúan mi trabajo en mi centro de trabajo me ayuda a mejorar mi desempeño.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186', NULL, NULL,2),
  ('A9F974AE-D57E-4DD9-A0EB-C99469F0B611', 1, null, getdate(), 'En mi centro de trabajo me pagan a tiempo mi salario.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186', NULL, NULL,3),
  ('DD361B81-FBFA-4600-83A3-A9F9608B32F3', 1, null, getdate(), 'El pago que recibo es el que merezco por el trabajo que realizo.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186', NULL, NULL,4),
  ('0E39CC00-CE80-4CB4-AB17-EC1E221C11E3', 1, null, getdate(), 'Si obtengo los resultados esperados en mi trabajo me recompensan o reconocen.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186',NULL, NULL, 5),
  ('395EC084-074D-4C09-BCA5-3DD0E05D2656', 1, null, getdate(), 'Las personas que hacen bien el trabajo pueden crecer laboralmente.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186',NULL, NULL, 6),
  ('4D79EA8E-EC64-45B5-943B-EE2BCBF00329', 1, null, getdate(), 'Considero que mi trabajo es estable.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186', NULL, NULL,7),
  ('8A004D41-91CC-4D67-B823-002542CF9181', 1, null, getdate(), 'En mi trabajo existe continua rotación de personal.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186',NULL, NULL, 8),
  ('BD916779-3763-4FAF-BBF3-E4970834FD28', 1, null, getdate(), 'Siento orgullo de laborar en este centro de trabajo.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186', NULL, NULL,9),
  ('679E73B2-8DBC-489F-9422-9CC0F003DD7A', 1, null, getdate(), 'Me siento comprometido con mi trabajo.', '', 1, getdate(), '54AD20E8-A3EA-4CC7-803B-70DE0E69E186', NULL, NULL,10),

  --EEO - XII. Actos de violencia laboral (malos tratos, acoso, hostigamiento, acoso psicológico).
  ('51E90F84-2F1F-43A6-9467-801AFF4F9170', 1, null, getdate(), 'En mi trabajo puedo expresarme libremente sin interrupciones.', '', 1, getdate(), '7C88F094-E770-4E48-9D41-467871420D67', NULL, NULL,1),
  ('3A1CE515-7080-488D-B3D9-91DF361092C3', 1, null, getdate(), 'Recibo críticas constantes a mi persona y/o trabajo.', '', 1, getdate(), '7C88F094-E770-4E48-9D41-467871420D67',NULL, NULL, 2),
  ('93DC54A9-3191-49B0-AFF1-612993AE19AA', 1, null, getdate(), 'Recibo burlas, calumnias, difamaciones, humillaciones o ridiculizaciones.', '', 1, getdate(), '7C88F094-E770-4E48-9D41-467871420D67', NULL, NULL,3),
  ('271BA6B1-E9BC-4062-830C-FB1921BF2F7F', 1, null, getdate(), 'Se ignora mi presencia o se me excluye de las reuniones de trabajo y en la toma de decisiones.', '', 1, getdate(), '7C88F094-E770-4E48-9D41-467871420D67', NULL, NULL,4),
  ('13993225-6CE2-4B45-99A6-A4FA55564D27', 1, null, getdate(), 'Se manipulan las situaciones de trabajo para hacerme parecer un mal trabajador.', '', 1, getdate(), '7C88F094-E770-4E48-9D41-467871420D67', NULL, NULL,5),
  ('68A1A191-E812-44A1-907C-36A103388EA4', 1, null, getdate(), 'Se ignoran mis éxitos laborales y se atribuyen a otros trabajadores.', '', 1, getdate(), '7C88F094-E770-4E48-9D41-467871420D67', NULL, NULL,6),
  ('CE80AA54-5E32-4812-A7DE-9D6AA90FBC45', 1, null, getdate(), 'Me bloquean o impiden las oportunidades que tengo para obtener ascenso o mejora en mi trabajo.', '', 1, getdate(), '7C88F094-E770-4E48-9D41-467871420D67', NULL, NULL,7),
  ('CC0FB843-1959-44B8-8A77-D5DDE51E5C08', 1, null, getdate(), 'He presenciado actos de violencia en mi centro de trabajo.', '', 1, getdate(), '7C88F094-E770-4E48-9D41-467871420D67', NULL, NULL,8),

  --EEO - XIII. Atención a clientes y usuarios.
  ('7E27C04C-2E60-4B9C-90CC-6D7A0588E1CD', 1, null, getdate(), 'Atiendo clientes o usuarios muy enojados.', '', 1, getdate(), '04090E9C-CDA5-45D5-A7ED-4AECC6C66BC6', NULL, NULL,1),
  ('69D668D7-44C9-41D3-92AC-951DF1A3CD46', 1, null, getdate(), 'Mi trabajo me exige atender personas muy necesitadas de ayuda o enfermas.', '', 1, getdate(), '04090E9C-CDA5-45D5-A7ED-4AECC6C66BC6',NULL, NULL, 2),
  ('41057E48-0992-4B6B-AAD5-1A739B67747F', 1, null, getdate(), 'Para hacer mi trabajo debo demostrar sentimientos distintos a los míos.', '', 1, getdate(), '04090E9C-CDA5-45D5-A7ED-4AECC6C66BC6',NULL, NULL, 3),
  ('BCDE4F3B-1855-4367-B817-3CEE5DED76C6', 1, null, getdate(), 'Mi trabajo me exige atender situaciones de violencia.', '', 1, getdate(), '04090E9C-CDA5-45D5-A7ED-4AECC6C66BC6', NULL, NULL,4),

  --EEO - XIV. Las actitudes de las personas que supervisa.
  ('ACF55547-09F9-4404-A28F-7D3900E875A3', 1, null, getdate(), 'Comunican tarde los asuntos de trabajo.', '', 1, getdate(), '6BAE794D-BEC4-4504-B1D6-07238798B4E9', NULL, NULL,1),
  ('259BBCDE-8551-455B-8E09-D38F035308FF', 1, null, getdate(), 'Dificultan el logro de los resultados del trabajo.', '', 1, getdate(), '6BAE794D-BEC4-4504-B1D6-07238798B4E9',NULL, NULL,2),
  ('0741BE9D-A357-420C-8E0C-274FA7151FB4', 1, null, getdate(), 'Cooperan poco cuando se necesita.', '', 1, getdate(), '6BAE794D-BEC4-4504-B1D6-07238798B4E9', NULL, NULL,3),
  ('FEF9D7EB-C081-4C3D-91E5-9582CACB2EE4', 1, null, getdate(), 'Ignoran las sugerencias para mejorar su trabajo.', '', 1, getdate(), '6BAE794D-BEC4-4504-B1D6-07238798B4E9',NULL, NULL, 4)

BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando NOMEvaluationQuestion'
 MERGE NOMEvaluationQuestion AS TARGET
 USING @NOMEvaluationQuestion_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID]
      ,[Active]
      ,[DeleteDate]
      ,[Timestamp]
      ,[Name]
      ,[Description]
      ,[StatusID]
      ,[CreationDate]
      ,[NOMEvaluationPhaseID]
      ,[NOMEvaluationCategoryID]
      ,[NOMEvaluationDomainID]
      ,[Number])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.[Name]
      ,SOURCE.[Description]
      ,SOURCE.[StatusID]
      ,SOURCE.[CreationDate]
      ,SOURCE.[NOMEvaluationPhaseID]
      ,SOURCE.[NOMEvaluationCategoryID]
      ,SOURCE.[NOMEvaluationDomainID]
      ,SOURCE.[Number])
 WHEN MATCHED THEN
  UPDATE SET TARGET.NAME = SOURCE.NAME,
   TARGET.Description = SOURCE.Description,
   TARGET.Number = SOURCE.Number,
   TARGET.NOMEvaluationPhaseID = SOURCE.NOMEvaluationPhaseID,
   TARGET.NOMEvaluationCategoryID = SOURCE.NOMEvaluationCategoryID,
   TARGET.NOMEvaluationDomainID = SOURCE.NOMEvaluationDomainID;

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH








-----------------------------------------------------
-----------------------------------------------------
-- GLOBALS CATALOGS UPDATES
-----------------------------------------------------
--  MINIMUN SALARY
-----------------------------------------------------

BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando Nuevos salarios minimos 2020, a las empresas que aún no lo tienen dado de alta'

INSERT INTO [dbo].[MinimunSalary]
		(
		[InstanceID],
		[ID], 
		[Active], 
		[DeleteDate], 
		[Timestamp], 
		[Name],
		[Description],
		[StatusID],
		[CreationDate],
		[user],
		[company],
		[ExpirationDate],
		[ZoneA],       
		[ZoneB],
		[ZoneC]
		)	

SELECT distinct instanceId, 
	 newid() as id,  
	 1 as Active,
	 null as [DeleteDate],
	 getdate() as [Timestamp],
	 '' as [Name],
	 '' as [Description],
	 1 as [StatusID],
	 getdate() as [CreationDate],
	 '00000000-0000-0000-0000-000000000000' as [user],
	 company as [company],
	 '2020-01-01' as [ExpirationDate],
	 123.22 as [ZoneA],
	 123.22 as [ZoneB],
	 185.56 as [ZoneC]
  FROM [dbo].[MinimunSalary]
  GROUP BY instanceId, company
  HAVING year(max(ExpirationDate)) <> '2020'

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH



/*UMI*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
 Declare @UMI_Table table(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [Value]			   DECIMAL(18, 6) NOT NULL, 
	[ValidityDate]	   DATETIME2 (7)    NOT NULL
)

INSERT @UMI_Table ([ID], Active, DeleteDate, [Timestamp], CreationDate, [Value], [ValidityDate])
 VALUES 
 
  ('3bcb2a5d-9e8e-4274-af1b-0630fd93f0b2', 1, null, getdate(),  getdate(), 75.49, '01/01/2017'),
  ('d3eed992-7762-48b3-8b30-7f26bff9baf3', 1, null, getdate(),  getdate(), 78.43, '01/01/2018'),
  ('470ce473-b26c-4db9-bb3e-bf1f9344dfdd', 1, null, getdate(),  getdate(), 82.22, '01/01/2019'),
  ('8acb4512-6a87-4f9c-b5c7-62af4598da42', 1, null, getdate(),  getdate(), 84.55, '01/01/2020')
 

BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando  UMI_Table'
 MERGE UMI AS TARGET
 USING @UMI_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID], Active, DeleteDate, [Timestamp], CreationDate, [Value],[ValidityDate])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.CreationDate
      ,SOURCE.[Value]
      ,SOURCE.[ValidityDate])
 WHEN MATCHED THEN
  UPDATE SET
   TARGET.[Value] = SOURCE.[Value],
    TARGET.ValidityDate = SOURCE.[ValidityDate];

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH

/*InfonavitInsurance*/
/*-----------------------------------------------------------------------------------------------------------------------------*/
 Declare @InfonavitInsurance_Table table(
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Active]           BIT              NOT NULL,
    [DeleteDate]       DATETIME2 (7)    NULL,
    [Timestamp]        DATETIME2 (7)    NOT NULL,
    [CreationDate]     DATETIME2 (7)    NOT NULL,
    [Value]			   DECIMAL(18, 6) NOT NULL, 
	[ValidityDate]	   DATETIME2 (7)    NOT NULL
)

INSERT @InfonavitInsurance_Table ([ID], Active, DeleteDate, [Timestamp], CreationDate, [Value], [ValidityDate])
 VALUES 
 
  ('6ca2960b-0106-4786-8fbb-ebd90f3cd18e', 1, null, getdate(),  getdate(), 13.00, '01/01/2009'),
  ('41f18484-eb4f-4629-81cb-a24b5f9c73de', 1, null, getdate(),  getdate(), 15.00, '01/01/2010')
 

BEGIN TRANSACTION;

BEGIN TRY

PRINT 'Insertando  InfonavitInsurance_Table'
 MERGE InfonavitInsurance AS TARGET
 USING @InfonavitInsurance_Table as SOURCE
 ON (TARGET.ID = SOURCE.ID)
 WHEN NOT MATCHED BY TARGET THEN
  INSERT([ID], Active, DeleteDate, [Timestamp], CreationDate, [Value],[ValidityDate])
  VALUES(SOURCE.ID, 
      SOURCE.[Active]
      ,SOURCE.[DeleteDate]
      ,SOURCE.[Timestamp]
      ,SOURCE.CreationDate
      ,SOURCE.[Value]
      ,SOURCE.[ValidityDate])
 WHEN MATCHED THEN
  UPDATE SET
   TARGET.[Value] = SOURCE.[Value],
    TARGET.ValidityDate = SOURCE.[ValidityDate];

COMMIT TRANSACTION
END TRY
BEGIN CATCH
 SELECT  ERROR_NUMBER() AS ErrorNumber
 ,ERROR_MESSAGE() AS ErrorMessage;
 Print ERROR_MESSAGE()
      ROLLBACK TRANSACTION
END CATCH
GO

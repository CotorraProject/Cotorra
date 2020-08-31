

CREATE   VIEW [dbo].[vwIMSSEmployerTable]
AS
select [ID]
      ,[Active]     
      ,[user]
      ,[company]
      ,[InstanceID]
      ,[ValidityDate]
      ,[EG_Especie_GastosMedicos_1]
      ,[EG_Especie_Fija_2]
      ,[EG_Especie_mas_3SMDF_3]
      ,[EG_Prestaciones_en_Dinero_4]
      ,[Invalidez_y_vida_5]
      ,[Cesantia_y_vejez_6]
      ,[Guarderias_7]
      ,[Retiro_8] from IMSSEmployerTable
CREATE FUNCTION dbo.getDescriptionMonth(@month int, @language int = 1)
RETURNS nvarchar(100)
AS
BEGIN
    declare @returnValue nvarchar(100)
	set @returnValue = 'Not defined'
	IF (@language = 1)
	BEGIN
		SELECT @returnValue = 
		CASE 
					WHEN @month = 1 THEN 'Enero' 
					WHEN @month = 2 THEN 'Febrero' 
					WHEN @month = 3 THEN 'Marzo' 
					WHEN @month = 4 THEN 'Abril' 
					WHEN @month = 5 THEN 'Mayo' 
					WHEN @month = 6 THEN 'Junio' 
					WHEN @month = 7 THEN 'Julio' 
					WHEN @month = 8 THEN 'Agosto' 
					WHEN @month = 9 THEN 'Septiembre' 
					WHEN @month = 10 THEN 'Octubre' 
					WHEN @month = 11 THEN 'Noviembre' 
					WHEN @month = 12 THEN 'Diciembre' 
		END 
	END
    RETURN @returnValue
END;
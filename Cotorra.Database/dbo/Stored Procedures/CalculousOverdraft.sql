
CREATE PROCEDURE [dbo].[CalculousOverdraft]
@OverdraftDetails  dbo.stampoverdraftdetailtabletype READONLY,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS

BEGIN TRAN TCalculousOverdraft;  

--Change PeriodDetail Status 3 Stamped
IF EXISTS (SELECT ID FROM @OverdraftDetails)
BEGIN
    update od 
    set od.Amount =  odi.Amount,
		od.Value =  odi.Value,
		od.Taxed =  odi.Taxed,
		od.Exempt =  odi.Exempt,
		od.IMSSTaxed =  odi.IMSSTaxed,
		od.IMSSExempt =  odi.IMSSExempt,
		od.IsGeneratedByPermanentMovement = odi.IsGeneratedByPermanentMovement
    FROM OverdraftDetail as od 
    INNER JOIN @OverdraftDetails odi 
    on od.ID = odi.ID
END


COMMIT TRAN TCalculousOverdraft;  
 --ROLLBACK TRAN TCalculousOverdraft;
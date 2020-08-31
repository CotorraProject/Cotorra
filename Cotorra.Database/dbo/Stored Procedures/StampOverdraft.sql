
CREATE PROCEDURE [dbo].[StampOverdraft]
@PeriodDetailIds  dbo.guidlisttabletype READONLY,
@OverdraftIds  dbo.stampoverdrafttabletype READONLY,
@InstanceId uniqueidentifier,
@company uniqueidentifier,
@user uniqueidentifier
AS

BEGIN TRAN TStampOverdraft;  

--Change Overdraft Status 2 Stamped
IF EXISTS (SELECT ID FROM @OverdraftIds)
BEGIN
    update o
    set o.UUID = oi.UUID,
        o.OverdraftStatus = 2
    FROM Overdraft o
    INNER JOIN @OverdraftIds oi
    ON o.ID = oi.ID
END

--Change PeriodDetail Status 3 Stamped
IF EXISTS (SELECT ID FROM @PeriodDetailIds) AND NOT EXISTS(SELECT o.ID FROM Overdraft o
INNER JOIN PeriodDetail pd
	ON pd.ID = o.PeriodDetailID
INNER JOIN @PeriodDetailIds pdi 
    ON pd.ID = pdi.ID
WHERE o.OverdraftStatus = 0 or o.OverdraftStatus = 1)
BEGIN
    update pd 
    set pd.PeriodStatus = 3 
    FROM PeriodDetail as pd 
    INNER JOIN @PeriodDetailIds pdi 
    on pd.ID = pdi.ID
END

COMMIT TRAN TStampOverdraft;  
 --ROLLBACK TRAN TStampOverdraft;
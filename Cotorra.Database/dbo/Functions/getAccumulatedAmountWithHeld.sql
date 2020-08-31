CREATE FUNCTION dbo.getAccumulatedAmountWithHeld(@EmployeeConceptsRelationID uniqueidentifier)
RETURNS decimal(18,6)
AS
BEGIN
    declare @bav decimal(18,6)
    SELECT @bav = isnull(SUM(ecrd.AmountApplied), 0) FROM EmployeeConceptsRelationDetail ecrd
	WHERE ecrd.EmployeeConceptsRelationID = @EmployeeConceptsRelationID and ecrd.ConceptsRelationPaymentStatus = 1
    RETURN @bav
END;
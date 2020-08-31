
CREATE PROCEDURE [dbo].[GetRequiredOverdraftDICalculationData]
(
	@CompanyID uniqueidentifier,
	@InstanceID uniqueidentifier
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
  
	select * from vwMinimunSalary where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwUMA where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwSGDFLimits where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwIMSSEmployeeTable where InstanceID = @InstanceID and company = @CompanyID and active = 1
	select * from vwIMSSEmployerTable where InstanceID = @InstanceID and company = @CompanyID and active = 1
	select * from vwIMSSWorkRisk where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwSettlementCatalog where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwMonthlyIncomeTax where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwAnualIncomeTax where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwMonthlyEmploymentSubsidy where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwAnualEmploymentSubsidy where InstanceID = @InstanceID  and company = @CompanyID and active = 1	

	select * from vwAccumulatedType where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwConceptPaymentRelationship where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwConceptPayment where InstanceID = @InstanceID  and company = @CompanyID and active = 1	

END
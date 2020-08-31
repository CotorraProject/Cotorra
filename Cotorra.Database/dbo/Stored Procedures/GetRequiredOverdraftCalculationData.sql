
CREATE PROCEDURE [dbo].[GetRequiredOverdraftCalculationData]
(
	@CompanyID uniqueidentifier,
	@InstanceID uniqueidentifier,
	@EmployeeID uniqueidentifier,
	@PeriodDetailID uniqueidentifier,
	@PeriodDetailInitialDate datetime,
	@PeriodDetailFinalDate datetime
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    SELECT * from vwVacation where InstanceID = @InstanceID and company = @CompanyID and EmployeeID = @EmployeeID and 
		((InitialDate >= @PeriodDetailInitialDate and InitialDate <= @PeriodDetailFinalDate) or
         (FinalDate >= @PeriodDetailInitialDate and FinalDate <= @PeriodDetailFinalDate)) and active = 1

	select * from vwIncident where InstanceID = @InstanceID and company = @CompanyID and EmployeeID = @EmployeeID and PeriodDetailID = @PeriodDetailID
		and active = 1

	select * from vwInhability where InstanceID = @InstanceID and company = @CompanyID and EmployeeID = @EmployeeID and active = 1 and
		(
			(InitialDate BETWEEN @PeriodDetailInitialDate AND @PeriodDetailFinalDate) OR 
			(DATEADD(DD, AuthorizedDays, InitialDate) BETWEEN @PeriodDetailInitialDate AND @PeriodDetailFinalDate) OR 
			(InitialDate <= @PeriodDetailInitialDate AND DATEADD(DD, AuthorizedDays, InitialDate) >= @PeriodDetailFinalDate)
		)
	
	select * from vwMinimunSalary where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwUMA where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwInfonavitMovement where InstanceID = @InstanceID and company = @CompanyID and EmployeeID = @EmployeeID 
		and InfonavitStatus = 1 and InitialApplicationDate <= @PeriodDetailFinalDate and active = 1

	select * from vwSGDFLimits where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwIMSSEmployeeTable where InstanceID = @InstanceID and company = @CompanyID and active = 1
	select * from vwIMSSEmployerTable where InstanceID = @InstanceID and company = @CompanyID and active = 1
	select * from vwIMSSWorkRisk where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwAccumulatedEmployee where InstanceID = @InstanceID and company = @CompanyID and EmployeeID = @EmployeeID 
		and ExerciseFiscalYear = year(@PeriodDetailFinalDate) and active = 1

	select * from vwSettlementCatalog where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwMonthlyIncomeTax where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwAnualIncomeTax where InstanceID = @InstanceID and company = @CompanyID and active = 1

	select * from vwMonthlyEmploymentSubsidy where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwAnualEmploymentSubsidy where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwFonacotMovement where InstanceID = @InstanceID and company = @CompanyID and EmployeeID = @EmployeeID and
		FonacotMovementStatus = 1 --active

	select * from vwIncidentType where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwAccumulatedType where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwConceptPaymentRelationship where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from vwConceptPayment where InstanceID = @InstanceID  and company = @CompanyID and active = 1

	select * from [vwVacationDaysOff] where InstanceID = @InstanceID and company = @CompanyID and
		[Date] >= @PeriodDetailInitialDate and [Date] <= @PeriodDetailFinalDate and active = 1

	select * from [vwEmployeeConceptsRelation] where InstanceID = @InstanceID and company = @CompanyID and 
		EmployeeID = @EmployeeID and ConceptPaymentStatus = 1 --active	

	select * from [vwInfonavitInsurance] 

	select * from [vwUMI] 

END
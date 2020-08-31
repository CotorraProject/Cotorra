using Cotorra.Schema;
using System;
using System.Collections.Generic;
using Xunit;
using Cotorra.Core.Validator;

namespace Cotorra.UnitTest
{
    public class VacationInhabilityIncidentHelperValidatorUT
    {

        private static Inhability BuildInhability(Guid instanceId, Guid identityWorkId, Guid employeeId, Guid incidentTypeId, DateTime initialDate, int authorizedDays)
        {
            return new Inhability()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                user = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                Name = "_",
                StatusID = 1,
                Description = "desc",
                AuthorizedDays = authorizedDays,
                CategoryInsurance = CategoryInsurance.WorkRisk,
                Consequence = Consequence.Death,
                EmployeeID = employeeId,
                Folio = "A12345678",
                IncidentTypeID = incidentTypeId,
                InhabilityControl = InhabilityControl.DeathST3,
                InitialDate = initialDate,
                Percentage = 25,
                RiskType = RiskType.WorkAccident,

            };
        }

        public static Vacation BuildVacation(Guid identityWorkID, Guid instanceID, Guid employeeID,
             DateTime initialDate, DateTime finalDate)
        {
            var vacation = new Vacation()
            {
                Active = true,
                company = identityWorkID,
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                Description = "Descripcion",
                EmployeeID = employeeID,
                InitialDate = initialDate,
                FinalDate = finalDate,
                ID = Guid.NewGuid(),
                InstanceID = instanceID,
                Name = "NameVacations",
                PaymentDate = DateTime.UtcNow,
                StatusID = 1,
                user = Guid.NewGuid(),
                VacationsBonusDays = 0,
                VacationsCaptureType = VacationsCaptureType.PayVacationsAndBonusInPeriod,
                VacationsDays = Convert.ToDecimal((finalDate - initialDate).TotalDays) + 1
            };
            return vacation;

        }

        public static VacationDaysOff BuildVacationDaysOff(Guid identityWorkID, Guid instanceID,
          DateTime Date)
        {
            var vacationDaysOff = new VacationDaysOff()
            {
                Active = true,
                company = identityWorkID,
                DeleteDate = null,
                ID = Guid.NewGuid(),
                user = Guid.NewGuid(),
                Date = Date,
                InstanceID = instanceID,
            };
            return vacationDaysOff;

        }


        public static Incident BuildIncident(Guid identityWorkId, Guid instanceID, Guid employeeID, Guid incidentTypeID, Guid periodDetailID, DateTime date)
        {
            return new Incident()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                Description = "1FINJ",
                CreationDate = DateTime.Now,
                Name = "Una falta injustificada",
                StatusID = 1,
                IncidentTypeID = incidentTypeID,
                PeriodDetailID = periodDetailID,
                Date = date,
                Value = 2,
                EmployeeID = employeeID
            };
        }



        public class ValidateInDate
        {
            [Fact]
            public void Should_Let_Pass_When_Dates_are_exclusive()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime initialVacationDate = new DateTime(2020, 8, 18);
                DateTime finalVacationDate = new DateTime(2020, 8, 25);
                DateTime vacationDayOff = new DateTime(2020, 8, 23);
                DateTime inhabilityInitialDay = new DateTime(2020, 8, 13);
                DateTime incidentDay = new DateTime(2020, 8, 26);
                int inhabilityAuthDays = 5;

                Vacation vac1 = BuildVacation(identityWorkId, instanceId, employeeId, initialVacationDate, finalVacationDate);
                VacationDaysOff vac1dayoff = BuildVacationDaysOff(identityWorkId, instanceId, vacationDayOff);
                vac1.VacationDaysOff = new List<VacationDaysOff>() { vac1dayoff };

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { vac1 };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };


                //Act
                validator.ValidateInDate(listVacation, listInhability, listIncident);


            }

            [Fact]
            public void Should_Fail_When_Vacation_Dates_are_inclusive_with_inhabilties()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime initialVacationDate = new DateTime(2020, 8, 18);
                DateTime finalVacationDate = new DateTime(2020, 8, 25);
                DateTime vacationDayOff = new DateTime(2020, 8, 23);
                DateTime inhabilityInitialDay = new DateTime(2020, 8, 20);
                DateTime incidentDay = new DateTime(2020, 8, 26);
                int inhabilityAuthDays = 5;

                Vacation vac1 = BuildVacation(identityWorkId, instanceId, employeeId, initialVacationDate, finalVacationDate);
                VacationDaysOff vac1dayoff = BuildVacationDaysOff(identityWorkId, instanceId, vacationDayOff);
                vac1.VacationDaysOff = new List<VacationDaysOff>() { vac1dayoff };

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { vac1 };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };

                try
                {
                    //Act
                    validator.ValidateInDate(listVacation, listInhability, listIncident);
                    Assert.False(true, "No lanzó error");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(1024, ex.ErrorCode);
                }


            }

            [Fact]
            public void Should_Fail_When_Vacation_Dates_are_inclusive_with_incident()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime initialVacationDate = new DateTime(2020, 8, 18);
                DateTime finalVacationDate = new DateTime(2020, 8, 25);
                DateTime vacationDayOff = new DateTime(2020, 8, 23);
                DateTime inhabilityInitialDay = new DateTime(2020, 8, 13);
                DateTime incidentDay = new DateTime(2020, 8, 24);
                int inhabilityAuthDays = 5;

                Vacation vac1 = BuildVacation(identityWorkId, instanceId, employeeId, initialVacationDate, finalVacationDate);
                VacationDaysOff vac1dayoff = BuildVacationDaysOff(identityWorkId, instanceId, vacationDayOff);
                vac1.VacationDaysOff = new List<VacationDaysOff>() { vac1dayoff };

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { vac1 };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };

                try
                {
                    //Act
                    validator.ValidateInDate(listVacation, listInhability, listIncident);
                    Assert.False(true, "No lanzó error");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(1025, ex.ErrorCode);
                }
            }

            [Fact]
            public void Should_Fail_When_Inhabilties_Dates_are_inclusive_with_incident()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime initialVacationDate = new DateTime(2020, 8, 18);
                DateTime finalVacationDate = new DateTime(2020, 8, 25);
                DateTime vacationDayOff = new DateTime(2020, 8, 23);
                DateTime inhabilityInitialDay = new DateTime(2020, 8, 13);
                DateTime incidentDay = new DateTime(2020, 8, 14);
                int inhabilityAuthDays = 5;

                Vacation vac1 = BuildVacation(identityWorkId, instanceId, employeeId, initialVacationDate, finalVacationDate);
                VacationDaysOff vac1dayoff = BuildVacationDaysOff(identityWorkId, instanceId, vacationDayOff);
                vac1.VacationDaysOff = new List<VacationDaysOff>() { vac1dayoff };

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { vac1 };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };

                try
                {
                    //Act
                    validator.ValidateInDate(listVacation, listInhability, listIncident);
                    Assert.False(true, "No lanzó error");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(1026, ex.ErrorCode);
                }
            }

            [Fact]
            public void Should_Let_Pass_When_Inhabilties_are_inclusive_with_vacation_daysOff()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime initialVacationDate = new DateTime(2020, 8, 18);
                DateTime finalVacationDate = new DateTime(2020, 8, 25);
                DateTime vacationDayOff = new DateTime(2020, 8, 23);
                DateTime inhabilityInitialDay = new DateTime(2020, 8, 23);
                DateTime incidentDay = new DateTime(2020, 8, 26);
                int inhabilityAuthDays = 1;

                Vacation vac1 = BuildVacation(identityWorkId, instanceId, employeeId, initialVacationDate, finalVacationDate);
                VacationDaysOff vac1dayoff = BuildVacationDaysOff(identityWorkId, instanceId, vacationDayOff);
                vac1.VacationDaysOff = new List<VacationDaysOff>() { vac1dayoff };

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { vac1 };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };


                //Act
                validator.ValidateInDate(listVacation, listInhability, listIncident);
            }

            [Fact]
            public void Should_fail_When__not_all_Inhabilties_are_inclusive_with_vacation_daysOff()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime initialVacationDate = new DateTime(2020, 8, 18);
                DateTime finalVacationDate = new DateTime(2020, 8, 25);
                DateTime vacationDayOff = new DateTime(2020, 8, 23);
                DateTime inhabilityInitialDay = new DateTime(2020, 8, 23);
                DateTime incidentDay = new DateTime(2020, 8, 26);
                int inhabilityAuthDays = 2;

                Vacation vac1 = BuildVacation(identityWorkId, instanceId, employeeId, initialVacationDate, finalVacationDate);
                VacationDaysOff vac1dayoff = BuildVacationDaysOff(identityWorkId, instanceId, vacationDayOff);
                vac1.VacationDaysOff = new List<VacationDaysOff>() { vac1dayoff };

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { vac1 };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };


                try
                {
                    //Act
                    validator.ValidateInDate(listVacation, listInhability, listIncident);
                    Assert.False(true, "No lanzó error");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(1024, ex.ErrorCode);
                }
            }

            [Fact]
            public void Should_Let_Pass_When_Incidents_are_inclusive_with_vacation_daysOff()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime initialVacationDate = new DateTime(2020, 8, 18);
                DateTime finalVacationDate = new DateTime(2020, 8, 25);
                DateTime vacationDayOff = new DateTime(2020, 8, 23);
                DateTime inhabilityInitialDay = new DateTime(2020, 8, 15);
                DateTime incidentDay = new DateTime(2020, 8, 23);
                int inhabilityAuthDays = 1;

                Vacation vac1 = BuildVacation(identityWorkId, instanceId, employeeId, initialVacationDate, finalVacationDate);
                VacationDaysOff vac1dayoff = BuildVacationDaysOff(identityWorkId, instanceId, vacationDayOff);
                vac1.VacationDaysOff = new List<VacationDaysOff>() { vac1dayoff };

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { vac1 };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };


                //Act
                validator.ValidateInDate(listVacation, listInhability, listIncident);
            }

            [Fact]
            public void Should_fail_When_not_all_Incidents_are_inclusive_with_vacation_daysOff()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime initialVacationDate = new DateTime(2020, 8, 18);
                DateTime finalVacationDate = new DateTime(2020, 8, 25);
                DateTime vacationDayOff = new DateTime(2020, 8, 23);
                DateTime inhabilityInitialDay = new DateTime(2020, 8, 23);
                DateTime incidentDay = new DateTime(2020, 8, 23);
                DateTime incidentDay2 = new DateTime(2020, 8, 24);
                int inhabilityAuthDays = 2;

                Vacation vac1 = BuildVacation(identityWorkId, instanceId, employeeId, initialVacationDate, finalVacationDate);
                VacationDaysOff vac1dayoff = BuildVacationDaysOff(identityWorkId, instanceId, vacationDayOff);
                vac1.VacationDaysOff = new List<VacationDaysOff>() { vac1dayoff };

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                Incident incident2 = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay2);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { vac1 };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident, incident2 };


                try
                {
                    //Act
                    validator.ValidateInDate(listVacation, listInhability, listIncident);
                    Assert.False(true, "No lanzó error");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(1024, ex.ErrorCode);
                }
            }

            [Fact]
            public void Should_fail_When_inhabilities_and_incidents_are_inclusive_with_no_vacation()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid(); 

                DateTime inhabilityInitialDay = new DateTime(2020, 8, 23);
                DateTime incidentDay = new DateTime(2020, 8, 23); 
                int inhabilityAuthDays = 1;
 
                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);                 

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() {   };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };


                try
                {
                    //Act
                    validator.ValidateInDate(listVacation, listInhability, listIncident);
                    Assert.False(true, "No lanzó error");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(1026, ex.ErrorCode);
                }
            }

            [Fact]
            public void Should_fail_When_inhabilities_and_incidents_are_inclusive_with_no_vacation_Bug_617()
            {
                //Arrange
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                Guid employeeId = Guid.NewGuid();
                Guid incidentTypeID = Guid.NewGuid();
                Guid periodDetailID = Guid.NewGuid();

                DateTime inhabilityInitialDay = new DateTime(2020, 2, 3);
                DateTime incidentDay = new DateTime(2020, 2, 4);
                int inhabilityAuthDays = 3;

                Inhability inhability = BuildInhability(instanceId, identityWorkId, employeeId, incidentTypeID, inhabilityInitialDay, inhabilityAuthDays);

                Incident incident = BuildIncident(identityWorkId, instanceId, employeeId, incidentTypeID, periodDetailID, incidentDay);

                VacationInhabilityIncidentHelperValidator validator = new VacationInhabilityIncidentHelperValidator();

                List<Vacation> listVacation = new List<Vacation>() { };
                List<Inhability> listInhability = new List<Inhability>() { inhability };
                List<Incident> listIncident = new List<Incident>() { incident };


                try
                {
                    //Act
                    validator.ValidateInDate(listVacation, listInhability, listIncident);
                    Assert.False(true, "No lanzó error");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(1026, ex.ErrorCode);
                }
            }
        }


    }
}

using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using CotorraNube.CommonApp.RestClient;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Cotorra.Core.Utils;
using Cotorra.Core.Utils.Mail;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.thridparty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Cotorra.Core
{
    public class EmployeeIdentityRegistrationManager
    {
        private readonly string _userHost;

        #region "Constructor"
        public EmployeeIdentityRegistrationManager()
        {
            _userHost = ConfigManager.GetValue("userhost");
        }
        #endregion

        /// <summary>
        /// Create or Get the EmployeeIdentiyRegistration
        /// </summary>
        /// <param name="middlewareManagerRegis"></param>
        /// <param name="employee"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        private async Task<EmployeeIdentityRegistration> createEmployeeIdentityRegistration(MiddlewareManager<EmployeeIdentityRegistration> middlewareManagerRegis,
            Employee employee, Guid company)
        {
            var currentProcess = await middlewareManagerRegis.FindByExpressionAsync(p =>
                            p.EmployeeID == employee.ID && p.Email == employee.Email &&
                            p.EmployeeIdentityRegistrationStatus != EmployeeIdentityRegistrationStatus.Completed, company);

            List<EmployeeIdentityRegistration> lstToCreate = null;

            //No debe de existir un registro pendiente
            //Paso 1. Crear el registro
            if (!currentProcess.Any())
            {
                lstToCreate = new List<EmployeeIdentityRegistration>()
                        {
                            new EmployeeIdentityRegistration()
                            {
                                Active = true,
                                CreationDate = DateTime.UtcNow,
                                DeleteDate = null,
                                Description = "",
                                Email = employee.Email,
                                EmployeeID = employee.ID,
                                ID = Guid.NewGuid(),
                                EmployeeIdentityRegistrationStatus = EmployeeIdentityRegistrationStatus.MailSent,
                                ActivationCode = CodeGenerator.GetRandomCode(8),
                                IdentityUserID = null,
                                Name = "",
                                StatusID = 1,
                                Timestamp = DateTime.UtcNow
                            }
                        };
                await middlewareManagerRegis.CreateAsync(lstToCreate, company);
            }
            else
            {
                //El registro ya se relacionó correctamente
                lstToCreate = currentProcess;
            }

            return lstToCreate.FirstOrDefault();
        }

        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="employeeIdentityRegistration"></param>
        /// <returns></returns>
        private async Task sendMailToEmployee(EmployeeIdentityRegistration employeeIdentityRegistration)
        {
            var bodyContent = "Bienvenido a Cotorria, tu empresa te invita a usar la App Móvil de Cotorria, te mandamos tu código de activación ";
            var mailProvider = FactoryMailProvider.CreateInstance(FactoryMailProvider.GetProviderFromConfig());
            var sendParams = new SendMailParams()
            {
                HTMLContent = $"{bodyContent}: {employeeIdentityRegistration.ActivationCode}",
                PlainContentText = $"{bodyContent}: {employeeIdentityRegistration.ActivationCode}",
                SendMailAddresses = new List<SendMailAddress>() { new SendMailAddress() { Email = employeeIdentityRegistration.Email } },
                Subject = "Invitación a Cotorria"
            };

            await mailProvider.SendMailAsync(sendParams);
        }

        /// <summary>
        /// Create or Verify the Employee Registration (fire and forget)
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task CreateOrVerifyRegistrationAsync(Employee employee)
        {
            Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var company = employee.company;

                //Get identityUserId from email of employee
                if (!String.IsNullOrEmpty(employee.Email))
                {
                    //Obtener el registro del empleado
                    var middlewareManagerRegis = new MiddlewareManager<EmployeeIdentityRegistration>(
                        new BaseRecordManager<EmployeeIdentityRegistration>(),
                        new EmployeeIdentityRegistrationValidator()
                        );

                    //Paso 1. Crear el registro y el activation code
                    var employeeIdentityRegistration = await createEmployeeIdentityRegistration(middlewareManagerRegis, employee, company);

                    //Paso 2. Mandar el mail al empleado, invitación a usar la app móvil
                    await sendMailToEmployee(employeeIdentityRegistration);

                }

                stopwatch.Stop();
                Trace.WriteLine($"Time elapsed in the fire and forget employee registration");

            });

        }

        /// <summary>
        /// Get IdentityUser by Email
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Guid?> GetIdentityUserAsync(string authorizationHeader, string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                var result = await ServiceHelperExtensions.CallRestServiceAsync(
                        Format.JSON,
                        RestMethod.GET,
                        authorizationHeader,
                        new Uri($"{_userHost}api/Users/{email}"),
                        new object[] { }).ContinueWith((i) =>
                        {
                            if (i.Exception != null)
                            {
                                throw i.Exception;
                            }
                            return i.Result;
                        });

                if (!String.IsNullOrEmpty(result))
                {
                    var identityPlatform = JsonConvert.DeserializeObject<IdentityPlatform>(result);
                    return identityPlatform.ID;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

    }
}

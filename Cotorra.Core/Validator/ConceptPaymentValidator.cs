using CotorraNode.Common.Config;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Validator
{
    public class ConceptPaymentValidator : IValidator<ConceptPayment>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<ConceptPayment> MiddlewareManager { get; set; }

        public ConceptPaymentValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 6001));
            createRules.Add(new IntRule("Code", "Número", 0, 9999, 6003));
            createRules.Add(new SimpleStringRule("Description", "Descripción", true, 1, 250, 6004));
            createRules.Add(new DuplicateItemRule<ConceptPayment>(new string[] { "Code", "ConceptType", "InstanceID" }, "No se permiten códigos de conceptos duplicados", this, 6005));
        }

        private void setGlobalAutomatic(List<ConceptPayment> lstObjectsToValidate)
        {
            //Si lo seteo a automático global
            if (lstObjectsToValidate.Any(p => p.GlobalAutomatic))
            {
                var instanceId = lstObjectsToValidate.FirstOrDefault().InstanceID;
                var company = lstObjectsToValidate.FirstOrDefault().company;
                var user = lstObjectsToValidate.FirstOrDefault().user;
                var overdraftsIds = new List<Guid>();

                using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "AddConceptPaymentAutomaticGlobal";
                        var guids = lstObjectsToValidate.Select(p => p.ID).ToList();
                        DataTable dtGuidList = new DataTable();
                        dtGuidList.Columns.Add("ID", typeof(string));
                        guids.ForEach(p =>
                        {
                            dtGuidList.Rows.Add(p);
                        });
                        SqlParameter param = new SqlParameter("@guidConceptPayments", SqlDbType.Structured)
                        {
                            TypeName = "dbo.guidlisttabletype",
                            Value = dtGuidList
                        };
                        command.Parameters.Add(param);
                        command.Parameters.AddWithValue("@InstanceId", instanceId);
                        command.Parameters.AddWithValue("@company", company);
                        command.Parameters.AddWithValue("@user", user);

                        //Execute SP
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var Id = Guid.Parse(reader["ID"].ToString());
                            overdraftsIds.Add(Id);
                        }
                    }
                }

                //Calculate all overdrafts with the new details
                Task.Run(async () => {
                    if (overdraftsIds.Any())
                    {
                        await new OverdraftCalculationManager().CalculationFireAndForgetAsync(overdraftsIds, company,
                            instanceId, user);
                    }
                });

            }
        }

        public void AfterCreate(List<ConceptPayment> lstObjectsToValidate)
        {
            setGlobalAutomatic(lstObjectsToValidate);
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<ConceptPayment> lstObjectsToValidate)
        {
            setGlobalAutomatic(lstObjectsToValidate);
        }

        private void FixTaxedConceptPayments(ref List<ConceptPayment> lstObjectsToValidate)
        {
            //Fix concepts with default 100% Taxed ISR
            lstObjectsToValidate.ForEach(conceptPayment =>
            {
                //Percepciones gravan al 100% si son generadas por el usuario
                if (conceptPayment.ConceptType == ConceptType.SalaryPayment && 
                    conceptPayment.Code >= 501 &&
                    (String.IsNullOrEmpty(conceptPayment.Formula1) || conceptPayment.Formula1 == "0"))
                {
                    conceptPayment.Formula1 = $"Percepcion.Total({conceptPayment.Code})";
                }
            });
        }

        public void BeforeCreate(List<ConceptPayment> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<ConceptPayment>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            //Fix conceptPayments Taxed Issue
            FixTaxedConceptPayments(ref lstObjectsToValidate);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<ConceptPayment> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<ConceptPayment>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            //Fix conceptPayments Taxed Issue
            FixTaxedConceptPayments(ref lstObjectsToValidate);
        }
    }
}

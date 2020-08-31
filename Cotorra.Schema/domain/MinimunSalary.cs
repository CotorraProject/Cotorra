using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    public static class MinimumSalaryExtension
    {
        public static MinimunSalary SetData(this MinimunSalary minimunSalary, DateTime expirationDate, decimal zoneA, decimal zoneB,
            decimal zoneC)
        {
            minimunSalary.ZoneA = zoneA;
            minimunSalary.ZoneB = zoneB;
            minimunSalary.ZoneC = zoneC;
            minimunSalary.ExpirationDate = expirationDate;

            return minimunSalary;
        }

        public static MinimunSalary SetOwnerData(this MinimunSalary minimunSalary, Guid companyId, Guid instanceId, Guid userId)
        {
            minimunSalary.company = companyId;
            minimunSalary.InstanceID = instanceId;
            minimunSalary.user = userId;

            return minimunSalary;
        }
    }

    /// <summary>
    /// Salario Mínimo
    /// </summary>
    [DataContract]
    [Serializable]
    public class MinimunSalary : IdentityCatalogEntityExt
    {
        [DataMember]
        public DateTime ExpirationDate { get; set; }

        [DataMember]
        public decimal ZoneA { get; set; }

        [DataMember]
        public decimal ZoneB { get; set; }

        [DataMember]
        public decimal ZoneC { get; set; }
    }

    [DataContract]
    public enum ZoneType
    {
        A = 1,
        B = 2,
        C = 3,
    }

}

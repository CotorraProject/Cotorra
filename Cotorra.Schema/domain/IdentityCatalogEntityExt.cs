using CotorraNode.Common.Base.Schema;
using CotorraNode.CommonApp.Schema;
using CotorraNube.CommonApp.Schema.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class IdentityCatalogEntityExt : IdentityCatalogEntity, IInstanceData
    {
        public IdentityCatalogEntityExt()
        {
            this.Active = true;
            this.StatusID = 1;
            this.Name = String.Empty;
            this.Description = String.Empty;
            this.Timestamp = DateTime.Now;
            this.CreationDate = DateTime.Now;
        }

        [NotMapped]
        [IgnoreDataMember]
        public virtual Guid IdentityID
        {
            get
            {
                return this.user;
            }

            set
            {
                this.user = value;
            }
        }

        [NotMapped]
        [IgnoreDataMember]
        public virtual Guid CompanyID
        {
            get
            {
                return this.company;
            }

            set
            {
                this.company = value;
            }
        }

        [DataMember]
        public Guid InstanceID { get; set; }
    }
}

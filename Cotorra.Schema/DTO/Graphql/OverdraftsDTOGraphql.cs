using Cotorra.Schema.DTO.Catalogs;
using System.Collections.Generic;

namespace Cotorra.Schema
{

    public class OverdraftsDTOGraphql
    {
        public List<OverdraftDTO> Overdrafts { get; set; }
    }

    public class OverdraftDTOGraphql
    {
        public OverdraftDTO Overdraft { get; set; }
    }
}

using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core
{ 
    public interface IBankPaymentLayoutManager
    {
        string GenerateLayout(List<Overdraft> overdrafts, IBankAdditionalInformation additionalInformation);
    }
}

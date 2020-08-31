using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public enum PaymentBase
    {
        Salary = 1,// SUELDO
        Comission = 2,// COMISION
        Piecework = 3,// DESTAJO
        SalaryComission = 4,// SUELDO – COMISION
        SalaryPieceWork = 5,// SUELDO – DESTAJO
    }
}
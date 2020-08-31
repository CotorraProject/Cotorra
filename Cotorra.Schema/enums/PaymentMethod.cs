using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public enum PaymentMethod
    {
        Cash = 1,// EFECTIVO
        Check = 2,// CHEQUE NOMINATIVO
        ElectronicTransfer = 3,// TRANSFERENCIA ELECTRONICA 
        CreditCard = 4,// TARJETA DE CREDITO 
        ElectronicWallet = 5,//        MONEDERO ELECTRONICO
        ElectronicCash = 6,//    DINERO ELECTRONICO
        GroceryCoupon = 7, //VALES DE DESPENSA
        DebitCard = 8, // TARJETA DE DEBITO
        ServiceCard = 9,// TARJETA DE SERVICIOS
        Undefined = 99, //POR DEFINIR 
    }
}
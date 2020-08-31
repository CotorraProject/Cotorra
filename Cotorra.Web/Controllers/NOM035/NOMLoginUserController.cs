using System;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq.Expressions;
using System.Linq;
using CotorraNode.TelemetryComponent.Attributes;
using Cotorra.Web.Utils;
using Cotorra.Schema.nom035;

namespace Cotorra.Web.Controllers
{
    public class NOMLoginUserController : Controller
    {

        public NOMLoginUserController()
        {
            SessionModel.Initialize();
        }

    }
}

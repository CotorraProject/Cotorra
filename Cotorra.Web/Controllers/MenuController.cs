using System;
using Microsoft.AspNetCore.Mvc;

namespace Cotorra.Web.Controllers
{
    public class MenuController : Controller
    {
        //private readonly string jsonMenu;

        public MenuController()
        {
            //var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "Cotorra.Web.Menu.cotorra.json";

            //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //{
            //    using (StreamReader reader = new StreamReader(stream))
            //    {
            //        jsonMenu = reader.ReadToEnd();
            //    }
            //}
        }

        public String Get()
        {
            var file = System.IO.File.ReadAllText("Menu/cotorra.json");
            return file;
        }
    }
}

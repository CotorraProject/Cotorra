using CotorraNode.Common.Config;
using CotorraNube.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.FiscalStampingProviders.PAC
{
    public enum PACProvider
    { 
        COTORRAi = 1,
        COTORRAiWrapper = 2,
        Dummy = 3
    }

    public static class FactoryPACProvider
    {
        private static readonly string PACProviderConfig = ConfigManager.GetValue("PACProvider");

        public static IPACProvider CreateInstance(PACProvider pACProviders)
        {
            if (pACProviders == PACProvider.COTORRAi)
            {
                return new COTORRAiPACProvider();
            }
            else if (pACProviders == PACProvider.COTORRAiWrapper)
            {
                return new COTORRAiWrapperPACProvider();
            }
            else
            {
                return new DummyPACProvider();
            }
        }

        public static IPACProvider CreateInstanceFromConfig()
        {
            var enumProvider = (PACProvider)(Enum.Parse(typeof(PACProvider), PACProviderConfig));
            if (enumProvider == PACProvider.COTORRAi)
            {
                return new COTORRAiPACProvider();
            }
            else if (enumProvider == PACProvider.COTORRAiWrapper)
            {
                return new COTORRAiWrapperPACProvider();
            }
            else
            {
                return new DummyPACProvider();
            }
        }
    }
}

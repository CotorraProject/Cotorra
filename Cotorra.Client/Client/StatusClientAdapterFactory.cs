﻿using System.Reflection;
using System;
using System.IO;
using Microsoft.Extensions.DependencyModel;
using System.Linq;
using CotorraNode.Common.Config;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel.Resolution;
using System.Collections.Generic;
using System.Runtime.Loader;
using Cotorra.Schema;

namespace Cotorra.Client
{
    //------------------------------------------------------------------------------
    // <auto-generated>
    //    Este código se generó a partir de una plantilla.
    // No se sobreescribirán los cambios manuales si se regenera el código
    // </auto-generated>
    //------------------------------------------------------------------------------


    //Factory for client adapters
    /// <summary>
    /// 
    /// </summary>
    public class StatusClientAdapterFactory
    {

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientType">Type of the client.</param>
        /// <param name="authorizationHeader">The authorization header.</param>
        /// <param name="clientadapter">The clientadapter.</param>
        /// <returns></returns>
        public static IStatusClient<T> GetInstance<T>(string authorizationHeader, IStatusFullValidator<T> validator, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        where T : StatusIdentityCatalogEntityExt
        {
            return GetInstanceAsembly<T>(authorizationHeader, config: null, clientadapter: clientadapter,
                validator: validator);
        }

        public static IStatusClient<T> GetInstance<T>(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        where T : StatusIdentityCatalogEntityExt
        {
            return GetInstanceAsembly<T>(authorizationHeader, config: null, clientadapter: clientadapter);
        }

        public static IStatusClient<T> GetInstance<T>(string authorizationHeader, IConfigProvider configProvider, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        where T : StatusIdentityCatalogEntityExt
        {
            return GetInstanceAsembly<T>(authorizationHeader, config: configProvider, clientadapter: clientadapter);
        }

        /// <summary>
        /// Gets the instance asembly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientType">Type of the client.</param>
        /// <param name="authorizationHeader">The authorization header.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="clientadapter">The clientadapter.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Client adapter not found</exception>

        private static IStatusClient<T> GetInstanceAsembly<T>(string authorizationHeader,
            IStatusFullValidator<T> validator,
            IConfigProvider config = null,
            ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
            where T : StatusIdentityCatalogEntityExt
        {
            Assembly assembly = null;
            var variable = PlatformServices.Default.Application.ApplicationBasePath;
            Type type = null;
            try
            {
                if (clientadapter == ClientConfiguration.ClientAdapter.Proxy)
                {
                    assembly = Assembly.Load(variable + "Cotorra.ClientProxy.dll");
                }
                else if (clientadapter == ClientConfiguration.ClientAdapter.Local)
                {
                    assembly = Assembly.Load(variable + "Cotorra.ClientLocal.dll");
                }
                else if (clientadapter == ClientConfiguration.ClientAdapter.Internal)
                {
                    assembly = Assembly.Load(variable + "Cotorra.ClientLocal.dll");
                }
                if (assembly != null)
                {
                    string clientName = String.Empty;
                    if (clientadapter == ClientConfiguration.ClientAdapter.Local || clientadapter == ClientConfiguration.ClientAdapter.Internal)
                    {
                        clientName = "Cotorra.ClientLocal.StatusClientLocal`1";
                    }
                    else
                    {
                        clientName = "Cotorra.ClientProxy.StatusClientProxy`1";
                    }
                    type = assembly.DefinedTypes.FirstOrDefault(p => p.FullName.Contains(clientName)).UnderlyingSystemType;

                }
                else
                {
                    throw new CotorraException(100, "100", "assembly is null", null);
                }

                if (config != null)
                {
                    var makeme = type.MakeGenericType(typeof(T));
                    return Activator.CreateInstance(makeme, authorizationHeader, validator, config) as IStatusClient<T>;
                }
                else
                {
                    var makeme = type.MakeGenericType(typeof(T));
                    return Activator.CreateInstance(makeme, authorizationHeader, validator) as IStatusClient<T>;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static IStatusClient<T> GetInstanceAsembly<T>(string authorizationHeader,
            IConfigProvider config = null,
            ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
            where T : StatusIdentityCatalogEntityExt
        {
            Assembly assembly = null;
            var variable = String.Empty;

            //#if __ANDROID__
            variable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //#else 
            variable = PlatformServices.Default.Application.ApplicationBasePath;
            //#endif
            Type type = null;
            try
            {
                if (clientadapter == ClientConfiguration.ClientAdapter.Proxy)
                {
                    var file = $"{variable}Cotorra.ClientProxy.dll";
                    if (File.Exists(file))
                    {
                        assembly = Assembly.Load(file);
                    }
                }
                else if (clientadapter == ClientConfiguration.ClientAdapter.Local)
                {
                    assembly = Assembly.Load(variable + "Cotorra.ClientLocal.dll");
                }
                else if (clientadapter == ClientConfiguration.ClientAdapter.Internal)
                {
                    assembly = Assembly.Load(variable + "Cotorra.ClientLocal.dll");
                }
                if (assembly != null)
                {
                    string clientName = String.Empty;
                    if (clientadapter == ClientConfiguration.ClientAdapter.Local || clientadapter == ClientConfiguration.ClientAdapter.Internal)
                    {
                        clientName = "Cotorra.ClientLocal.StatusClientLocal`1";
                    }
                    else
                    {
                        clientName = "Cotorra.ClientProxy.StatusClientProxy`1";
                    }
                    type = assembly.DefinedTypes.FirstOrDefault(p => p.FullName.Contains(clientName)).UnderlyingSystemType;

                }
                else
                {
                    throw new CotorraException(100,"100","assembly is null", null);
                }

                if (config != null)
                {
                    var makeme = type.MakeGenericType(typeof(T));
                    return Activator.CreateInstance(makeme, authorizationHeader, config) as IStatusClient<T>;
                }
                else
                {
                    var makeme = type.MakeGenericType(typeof(T));
                    return Activator.CreateInstance(makeme, authorizationHeader) as IStatusClient<T>;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }      
    }
}
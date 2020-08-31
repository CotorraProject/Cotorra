using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Cotorra.Web.Utils
{
    public static class SessionUtils
    {
        public static dynamic GetSession(String sessionID, String identityID)
        {
            String serializedResult;
            dynamic result;

            if (CotorraTools.MemoryCache == null)
            {
                CotorraTools.MemoryCache = new MemoryCache(new MemoryCacheOptions());
            }

            //Check is session its on cache
            if (CotorraTools.MemoryCache.TryGetValue(sessionID + identityID, out serializedResult))
            {
                //There a session on cache, then...

                //Deserialize session
                result = Newtonsoft.Json.JsonConvert.DeserializeObject(serializedResult);
            }
            else
            {
                //No session, then...

                //Get session from redis
                result = CotorraNode.App.Common.UX.SessionUtils.GetSession(sessionID, identityID);

                //Serialize object to store it
                serializedResult = Newtonsoft.Json.JsonConvert.SerializeObject(result);

                //Set it on memory cache
                //var cacheEntryOptions = new MemoryCacheEntryOptions()
                //    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

                CotorraTools.MemoryCache.Set(sessionID + identityID, serializedResult/*, cacheEntryOptions*/);
            }

            return result;
        }
    }
}
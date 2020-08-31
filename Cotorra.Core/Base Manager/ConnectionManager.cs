using CotorraNode.Common.Config;

namespace Cotorra.Core
{
    /// <summary>
    /// ConnectionManager 
    /// </summary>
    public static class ConnectionManager
    {
        /// <summary>
        /// ConnectionString 
        /// </summary>
        public static readonly string ConfigConnectionString = ConfigManager.GetValue("ConfigConnectionString");
        public static readonly string ConfigConnectionStringGeneral = ConfigManager.GetValue("ConfigConnectionStringGeneral");
    }
}

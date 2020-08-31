
namespace Cotorra.Web
{
    public class VersionClient
    {
        private readonly string _versionBuildNumber;
        private readonly string _instrumentationKey;

        public VersionClient(string versionNumber, string instrumentationKey)
        {
            _versionBuildNumber = versionNumber;
            _instrumentationKey = instrumentationKey;
        }

        public string GetVersionNumber()
        {
            return $"{_versionBuildNumber} {_instrumentationKey}";
        }
    }
}

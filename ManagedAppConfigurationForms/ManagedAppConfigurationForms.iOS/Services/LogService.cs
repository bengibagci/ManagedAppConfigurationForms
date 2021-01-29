using ManagedAppConfigurationForms.Interfaces;
using ManagedAppConfigurationForms.iOS.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogService))]
namespace ManagedAppConfigurationForms.iOS.Services
{
    public class LogService : ILogService
    {
        static string _log = "";
        public LogService()
        {
        }

        public string Log()
        {
            return _log;
        }

        public static void GetLog(string log)
        {
            _log = log;
        }
    }
}

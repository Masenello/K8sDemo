using System;

namespace K8sBackendShared.Utils
{
    public static class UniqueIdentifiers
    {
        public static string GenerateDateTimeUniqueId()
        {
            return $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}{DateTime.Now.Millisecond}";
        }
    }
}
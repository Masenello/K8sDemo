using System;
using System.IO;
using System.Reflection;

namespace K8sBackendShared.Utils
{
    public class AppProperties
    {
        private static AppProperties _instance;
        public static AppProperties Instance => _instance ?? (_instance = new AppProperties());

        public readonly string ApplicationLogFileName = "Log.txt";
        public readonly string LogFilePath;

        public readonly string ManualName = "ApplicationManual.pdf";
        public readonly string ApplicationManualPath;

        public readonly string ReleaseNotesName = "ReleaseNotes.txt";
        public readonly string ReleaseNotesPath;

        private string _applicationName;

        public string ApplicationName
        {
            get => _applicationName;
            set
            {
                _applicationName = value;
            }
        }

        private string _applicationPathFolder;
        public string ApplicationPathFolder
        {
            get => _applicationPathFolder;
            set
            {
                _applicationPathFolder = value;
            }

        }

        private string _applicationPath;
        public string ApplicationPath
        {
            get => _applicationPath;
            set
            {
                _applicationPath = value;
            }

        }

        private string _applicationVersion;
        public string ApplicationVersion
        {
            get => _applicationVersion;
            set
            {
                _applicationVersion = value;
            }

        }


        public string AppIdFile { get; private set; }

        public string ApplicationGuid { get; private set; }

        public AppProperties()
        {


            Assembly assembly = Assembly.GetEntryAssembly();
            assembly = assembly ?? Assembly.GetCallingAssembly();

            ApplicationName = assembly.GetName().Name;
            ApplicationPathFolder = Path.GetDirectoryName(assembly.Location);
            ApplicationPath = assembly.Location;
            ApplicationVersion = assembly.GetName().Version.ToString();

            LogFilePath = Path.Combine(ApplicationPathFolder, "Logs", ApplicationLogFileName);
            ApplicationManualPath = Path.Combine(ApplicationPathFolder, "Manual", ManualName);
            ReleaseNotesPath = Path.Combine(ApplicationPathFolder, ReleaseNotesName);
            AppIdFile = Path.Combine(ApplicationPathFolder, "AppId.txt");

            if (System.IO.File.Exists(AppIdFile))
            {
                ApplicationGuid = System.IO.File.ReadAllText(AppIdFile);
            }
            else
            {
                ApplicationGuid = Guid.NewGuid().ToString();
                System.IO.File.WriteAllText(AppIdFile, ApplicationGuid);
            }

        }

        public override string ToString()
        {
            return $"Application name: {ApplicationName} \n" +
                   $"Application executable: {ApplicationPath} \n" +
                   $"Application version: {ApplicationVersion} \n";
        }
    }
}

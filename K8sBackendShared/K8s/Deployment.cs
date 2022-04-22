namespace K8sBackendShared.K8s
{
    public class Deployment
    {
        private Deployment(string value) { Value = value; }

        public string Value { get; private set; }

        public static Deployment api   { get { return new Deployment("k8sdemo-api"); } }
        public static Deployment app   { get { return new Deployment("k8sdemo-app"); } }
        public static Deployment database    { get { return new Deployment("k8sdemo-database"); } }
        public static Deployment director { get { return new Deployment("k8sdemo-director"); } }
        public static Deployment hub   { get { return new Deployment("k8sdemo-hub-manager"); } }
        public static Deployment worker   { get { return new Deployment("k8sdemo-worker"); } }

        public static Deployment rabbit   { get { return new Deployment("k8sdemo-rabbitmq"); } }
    }
}
namespace K8sBackendShared.K8s
{
    public class K8sNamespace
    {
        private K8sNamespace(string value) { Value = value; }

        public string Value { get; private set; }

        public static K8sNamespace defaultNamespace   { get { return new K8sNamespace("default"); } }
       
    }
}
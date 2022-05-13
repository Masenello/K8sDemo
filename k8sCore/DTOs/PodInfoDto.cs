using K8sCore.Enums;

namespace K8sCore.DTOs
{
    public class PodInfoDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string Node { get; set; }

        
    }
}
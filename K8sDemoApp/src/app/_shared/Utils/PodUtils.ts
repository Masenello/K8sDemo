

export class PodUtils {
    public GetPodIconPath(podImageName: string):string
    {
        if (podImageName.includes("k8sdemo-api"))
        {
            return "./assets/images/icon-api.png"
        }
        else if(podImageName.includes("k8sdemo-database"))
        {
            return "./assets/images/icon-sql-server.png"
        }
        else if(podImageName.includes("k8sdemo-director"))
        {
            return "./assets/images/icon-director.png"
        }
        else if(podImageName.includes("k8sdemo-hub-manager"))
        {
            return "./assets/images/icon-hub.png"
        }
        else if(podImageName.includes("k8sdemo-logmanager"))
        {
            return "./assets/images/icon-log-managr.png"
        }
        else if(podImageName.includes("k8sdemo-mongodatabase"))
        {
            return "./assets/images/icon-mongo.png"
        }
        else if(podImageName.includes("k8sdemo-rabbitmq"))
        {
            return "./assets/images/icon-rabbit.png"
        }
        else if(podImageName.includes("k8sdemo-worker"))
        {
            return "./assets/images/icon-worker.png"
        }
        else if(podImageName.includes("k8sdemo-app"))
        {
            return "./assets/images/icon-app.png"
        }
        return "";
    }
}
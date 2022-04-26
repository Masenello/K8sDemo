#*********************************    CODE      ********************************************************
#Build .Net API image
Set-Location D:\Code\K8sDemo
docker build -t masenellomarco/k8sdemoapi:latest -f k8sdemoapi/k8sdemoapi-dockerfile .

#Build .Net HUB image
Set-Location D:\Code\K8sDemo
docker build -t masenellomarco/k8sdemohubmanager:latest -f k8sdemohubmanager/k8sdemohubmanager-dockerfile .

#Build .Net demo worker image
Set-Location D:\Code\K8sDemo
docker build -t masenellomarco/k8sdemoworker:latest -f k8sdemoworker/k8sdemoworker-dockerfile .

#Build .Net logger image
Set-Location D:\Code\K8sDemo
docker build -t masenellomarco/k8sdemologmanager:latest -f k8sdemologmanager/k8sdemologmanager-dockerfile .

#Build .Net director image
Set-Location D:\Code\K8sDemo
docker build -t masenellomarco/k8sdemodirector:latest -f k8sdemodirector/k8sdemodirector-dockerfile .

#Build Angular APP image
Set-Location D:\Code\K8sDemo\K8sDemoApp
docker build -t masenellomarco/k8sdemoapp:latest -f k8sdemoapp_dockerfile .

#*********************************   AUX      **********************************************************
#Build RabbitMq image
#Set-Location D:\Code\K8sDemo
#docker build -t masenellomarco/k8sdemorabbitmq:latest -f K8sDemoRabbitMq/k8sdemorabbitmq-dockerfile .

#Build SqlServer image
#Set-Location D:\Code\K8sDemo
#docker build -t masenellomarco/k8sdemosqlserver:latest -f K8sDemoSqlServer/k8sdemosqlserver-dockerfile .


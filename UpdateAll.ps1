#Docker Hub login
docker login -u masenellomarco 

#Delete all deployments
kubectl delete -f .k8deploy

#clean up docker
docker system prune -f

# #1) .Net API image
# docker rmi masenellomarco/k8sdemoapi
# Set-Location D:\Code\K8sDemo
# docker build -t masenellomarco/k8sdemoapi:latest -f k8sdemoapi/k8sdemoapi-dockerfile .
# docker push masenellomarco/k8sdemoapi:latest

#2) .Net HUB image
docker rmi masenellomarco/k8sdemohubmanager
Set-Location D:\Code\K8sDemo
docker build -t masenellomarco/k8sdemohubmanager:latest -f k8sdemohubmanager/k8sdemohubmanager-dockerfile .
docker push masenellomarco/k8sdemohubmanager:latest

# #3) .Net demo worker image
# docker rmi masenellomarco/k8sdemoworker
# Set-Location D:\Code\K8sDemo
# docker build -t masenellomarco/k8sdemoworker:latest -f k8sdemoworker/k8sdemoworker-dockerfile .
# docker push masenellomarco/k8sdemoworker:latest

# #4) .Net logger image
# docker rmi masenellomarco/k8sdemologmanager
# Set-Location D:\Code\K8sDemo
# docker build -t masenellomarco/k8sdemologmanager:latest -f k8sdemologmanager/k8sdemologmanager-dockerfile .
# docker push masenellomarco/k8sdemologmanager:latest

# #5) .Net director image
# docker rmi masenellomarco/k8sdemodirector
# Set-Location D:\Code\K8sDemo
# docker build -t masenellomarco/k8sdemodirector:latest -f k8sdemodirector/k8sdemodirector-dockerfile .
# docker push masenellomarco/k8sdemodirector:latest

# #6)Angular APP image
# docker rmi masenellomarco/k8sdemoapp
# Set-Location D:\Code\K8sDemo\K8sDemoApp
# docker build -t masenellomarco/k8sdemoapp:latest -f k8sdemoapp_dockerfile .
# docker push masenellomarco/k8sdemoapp:latest

# #7)RabbitMq image
# docker rmi masenellomarco/k8sdemorabbitmq
# Set-Location D:\Code\K8sDemo
# docker build -t masenellomarco/k8sdemorabbitmq:latest -f K8sDemoRabbitMq/k8sdemorabbitmq-dockerfile .
# docker push masenellomarco/k8sdemorabbitmq:latest

# #8)SqlServer image
# docker rmi masenellomarco/k8sdemosqlserver
# Set-Location D:\Code\K8sDemo
# docker build -t masenellomarco/k8sdemosqlserver:latest -f K8sDemoSqlServer/k8sdemosqlserver-dockerfile .
# docker push masenellomarco/k8sdemosqlserver:latest

#(NOT USED MongoDb image is pulled directly from official mongo image by K8s deploy)
#9)MongoDb image 
# docker rmi masenellomarco/k8sdemomongo
# Set-Location D:\Code\K8sDemo
# docker build -t masenellomarco/k8sdemomongo:latest -f K8sDemoMongodb/k8sdemomongo-dockerfile .
# docker push masenellomarco/k8sdemomongo:latest


#Apply all deployments
Set-Location D:\Code\K8sDemo
kubectl apply -f .k8deploy

#Stop currente docker compose 
#docker-compose stop
#remove existing containers
docker-compose rm -f
#Build .Net API image
Set-Location D:\Code\K8sDemo
docker build -t k8sdemoapi:latest -f k8sdemoapi/Dockerfile .
#Build .Net HUB image
Set-Location D:\Code\K8sDemo
docker build -t k8sdemohubmanager:latest -f k8sdemohubmanager/Dockerfile .
#Build .Net demo worker image
Set-Location D:\Code\K8sDemo
docker build -t k8sdemoworker:latest -f k8sdemoworker/Dockerfile .
#Build .Net logger image
Set-Location D:\Code\K8sDemo
docker build -t k8sdemologmanager:latest -f k8sdemologmanager/Dockerfile .
#Build .Net director image
Set-Location D:\Code\K8sDemo
docker build -t k8sdemodirector:latest -f k8sdemodirector/Dockerfile .
#Build Angular APP image
Set-Location D:\Code\K8sDemo\K8sDemoApp
#docker build --no-cache -t k8sdemoapp:latest .
docker build -t k8sdemoapp:latest .
#Start dockers from docker compose file
Set-Location D:\Code\K8sDemo
docker-compose up --no-build
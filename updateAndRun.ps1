#Stop currente docker compose 
docker-compose stop
#remove existing containers
docker-compose rm -f
#Build .Net API image
Set-Location D:\Code\K8sDemo
docker build -t k8sdemoapi:latest -f k8sdemoapi/Dockerfile .
#Build Angular APP image
Set-Location D:\Code\K8sDemo\K8sDemoApp
docker build -t k8sdemoapp:latest .
#Start dockers from docker compose file
Set-Location D:\Code\K8sDemo
docker-compose up
#Stop currente docker compose 
docker-compose down
#remove existing containers
docker-compose rm -f
#Start dockers from docker compose file
Set-Location D:\Code\K8sDemo
docker-compose up --no-build
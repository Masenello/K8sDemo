docker login -u masenellomarco -p Mangusta1!

docker push masenellomarco/k8sdemoapi:latest
docker push masenellomarco/k8sdemodirector:latest
docker push masenellomarco/k8sdemoworker:latest
docker push masenellomarco/k8sdemohubmanager:latest
docker push masenellomarco/k8sdemologmanager:latest
docker push masenellomarco/k8sdemoapp:latest

#docker push masenellomarco/k8sdemosqlserver:latest
#docker push masenellomarco/k8sdemorabbitmq:latest

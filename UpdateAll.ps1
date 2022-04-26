#Delete all kubeconfig
#kubectl delete -f .k8deploy
#Delete all docker images
#& "$PSScriptRoot\RemoveAllImages.ps1"
#Build all docker images
#&  "$PSScriptRoot\BuildAllImages.ps1"
#Push all docker images
#&  "$PSScriptRoot\PushAllImages.ps1"
#Apply all kubeconfig
Set-Location D:\Code\K8sDemo\K8sDemo
kubectl apply -f .k8deploy

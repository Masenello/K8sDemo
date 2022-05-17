#Script to build Angular application

#$app = name on k8s deployment (ex k8sdemo-hub-manager)

param (
    [Parameter(Mandatory = $true)][string]$app,
    [Parameter(Mandatory = $true)][string]$version,
    [string]$basefolder = "D:\Code\K8sDemo",
    [string]$repo = "masenellomarco"
)
Set-Location $basefolder


#$vscodename = name of visual studio project subfolder (ex k8sdemohubmanager)
$vscodename = $app.replace('-', '')
$appfolder = [IO.Path]::Combine($basefolder, $vscodename)
set-location $appfolder
#tag image with version
$image = $repo + "/" + $vscodename + ":" + $version
$dockerfile = [io.path]::combine($appfolder, $vscodename + "_dockerfile")
write-host "building docker image:" $image -foregroundcolor blue
#docker build -t $image -f $app_dockerfile .
start-process "docker"  -workingdir "$appfolder" -args "build", "-t", "$image", "-f", "$dockerfile", "." -wait -nonewwindow
write-host "pushing docker image:" $image "to remote repo, please log in as user: " $repo -foregroundcolor blue
#docker login -u masenellomarco 
start-process "docker" -workingdir "$appfolder" -args "login", "-u", "$repo" -wait -nonewwindow
#docker push $image
start-process "docker" -workingdir "$appfolder" -args "push", "$image" -wait -nonewwindow
write-host "deploying image in local k8s cluster" $image -foregroundcolor blue
$deploymentname = "deployment.apps/" + $app 
$containername = $vscodename + "container"
#kubectl set image deployment.apps/k8sdemo-app k8sdemoappcontainer=masenellomarco/k8sdemoapp:0.0.4
Start-Process "kubectl" -WorkingDir "$appfolder" -Args "set image", "$deploymentname", "$containername=$image" -Wait -NoNewWindow
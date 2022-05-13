param(
    [Parameter()]
    [String]$Product,
    [String]$Command
)

$ErrorActionPreference = "Stop"

# ==== Variables ====

$ProjectBase = "C:\AlifaxSoftware"
$CommonBase = "$($ProjectBase)\AlifaxCommon"
# TEMP: idone Visual Studio 2022 folder
# $IdOneBase = "$($ProjectBase)\IdOne"
$IdOneBase = "$($ProjectBase)\IdOne_VS22"
$WebManagerBase = "$($ProjectBase)\AlifaxWebManager"
$LisSimulatorBase = "$($ProjectBase)\LisSimulator"

# ===================

# ==== Functions ====

function Start-Console-App ($ExePath, $Parameters) {
    Clear-Host
    $FileName = [System.IO.Path]::GetFileName($ExePath)
    Write-Host ""
    Write-Host "Launched Console App $($FileName)" -ForegroundColor Blue
    Write-Host ""
    Start-Process "$($ExePath)" -Wait -NoNewWindow -Args $Parameters
    Exit
}

function Start-Gui-App ($ExePath, $Parameters) {
    $FileName = [System.IO.Path]::GetFileName($ExePath)
    Write-Host "Launched Gui App $($FileName)" -ForegroundColor Blue
    Start-Process "$($ExePath)" -Args $Parameters
    Exit
}

function Start-PowerShell-Script ($ScriptPath) {
    $FileName = [System.IO.Path]::GetFileName($ExePath)
    Write-Host ""
    Write-Host "Launched PowerShell Script $($FileName)" -ForegroundColor Blue
    Write-Host ""
    Invoke-Expression -Command "$($ScriptPath)"
    Exit
}

function Start-Command ($WorkDir, $Command, $Arguments) {
    Clear-Host
    $LastDir = [System.IO.Path]::GetFileName($WorkDir)
    Write-Host ""
    Write-Host "Launched Command '$($Command) $($Arguments)' from '$($LastDir)'" -Foreground Blue
    Write-Host ""
    Start-Process "$($Command)" -WorkingDirectory "$($WorkDir)" -Args $Arguments -Wait -NoNewWindow
}

function Print-Usage-Template ($Product) {
    Write-Host ""
    if ($Product) {
        Write-Host "Usage:  ali $($Product) COMMAND"
    }
    else {
        Write-Host "Usage:  ali PRODUCT COMMAND"
    }
    Write-Host ""
    Write-Host "PRODUCT...............COMMAND....................DESCRIPTION"
    Write-Host ""
}

function Command-Not-Found ($Product, $Command) {
    Write-Host ""
    Write-Host "Command '$($Command)' for product '$($Product)' not found, launch without parameters to open command list" -ForegroundColor Yellow
    Write-Host ""
}

function Product-Not-Found ($Product) {
    Write-Host ""
    Write-Host "Product '$($Product)' not found, launch without parameters to open complete list" -ForegroundColor Yellow
    Write-Host ""
}

function Print-Usage-All () {
    Print-Usage-Template
    Print-Usage-Common
    Print-Usage-IdOne
    Print-Usage-WebManager
    Print-Usage-LisSimulator
}

function Print-Usage-Common () {
    Write-Host "com | common          ht1 | hubtestapp1          Alifax.Communication.Standard.App1.Test.exe"
    Write-Host "com | common          ht2 | hubtestapp2          Alifax.Communication.Standard.App2.Test.exe"
    Write-Host "com | common          lht | testlocalhub         Alifax.Communication.Standard.Hub.Test.exe"
    Write-Host "com | common          rht | testremotehub        Alifax.Communication.Standard.Hub.Test.Remote.exe"
    Write-Host "com | common          rmn | rolesmanager         RolesConfigurationEditorApp.exe"
    Write-Host ""
}

function Print-Usage-IdOne () {
    Write-Host "ido | idone           sth | studiohub            Alifax.IdOne.Hub.Studio.exe"
    Write-Host "ido | idone           ser | service              Alifax.IdOne.Ivd.Service.exe /C"
    Write-Host "ido | idone           lhb | localhub             Alifax.IdOne.LocalHub.exe"
    Write-Host "ido | idone           rhb | remotehub            Alifax.IdOne.RemoteHub.exe"
    Write-Host "ido | idone           sts | studioservice        Alifax.IdOne.Studio.Service.exe /C"
    Write-Host "ido | idone           tcm | techmonitor          Alifax.IdOne.TechMonitor.exe"
    Write-Host "ido | idone           cli | client               Alifax.IdOne.Ivd.Client.exe"
    Write-Host "ido | idone           stu | studio               Alifax.IdOne.Studio.Client.exe"
    Write-Host "ido | idone           pck | package              create-package.ps1"
    Write-Host ""
}

function Print-Usage-WebManager() {
    Write-Host "awm | webmanager      ser | server               ./WebManager> dotnet run --configuration Debug"
    Write-Host "awm | webmanager      cli | client               ./alifax-web-manager-gui> npm run ng serve --open"
    Write-Host ""
}

function Print-Usage-LisSimulator() {
    Write-Host "als | lissim          ser | server               ./server> npm run debug"
    Write-Host "als | lissim          cli | client               ./client> npm run ng serve --open"
    Write-Host ""
}

# ===================

# ==== Main ====

if ($Product -eq "") {
    Print-Usage-All
    Exit
}

switch ($Product) {
    {$_ -in "com","common"} {
        switch ($Command) {
            "" {
                Print-Usage-Template $Product
                Print-Usage-Common
                Exit
            }
            {$_ -in "ht1","hubtestapp1"} {
                Start-Console-App "$($CommonBase)\Alifax.Communication.Standard.App1.Test\bin\Debug\Alifax.Communication.Standard.App1.Test.exe"
            }
            {$_ -in "ht2","hubtestapp2"} {
                Start-Console-App "$($CommonBase)\Alifax.Communication.Standard.App2.Test\bin\Debug\Alifax.Communication.Standard.App2.Test.exe"
            }
            {$_ -in "lht","testlocalhub"} {
                Start-Console-App "$($CommonBase)\Alifax.Communication.Standard.Hub.Test\bin\Debug\net5.0\Alifax.Communication.Standard.Hub.Test.exe"
            }
            {$_ -in "rht","testremotehub"} {
                Start-Console-App "$($CommonBase)\Alifax.Communication.Standard.Hub.Test.Remote\bin\Debug\net5.0\Alifax.Communication.Standard.Hub.Test.Remote.exe"
            }
            {$_ -in "rmn","rolesmanager"} {
                Start-Gui-App "$($CommonBase)\RolesConfigurationEditorApp\bin\Debug\RolesConfigurationEditorApp.exe"
            }
            default {
                Command-Not-Found $Product $Command
                Exit
            }
        }
    }
    {$_ -in "ido","idone"} {
        switch ($Command) {
            "" {
                Print-Usage-Template $Product
                Print-Usage-IdOne
                Exit
            }
            {$_ -in "sth","studiohub"} {
                Start-Console-App "$($IdOneBase)\Alifax.IdOne.Hub.Studio\bin\Debug\net5.0\Alifax.IdOne.Hub.Studio.exe"
            }
            {$_ -in "ser","service"} {
                Start-Console-App "$($IdOneBase)\Alifax.IdOne.Ivd.Service\bin\Debug\Alifax.IdOne.Ivd.Service.exe" /C
            }
            {$_ -in "lhb","localhub"} {
                Start-Console-App "$($IdOneBase)\Alifax.IdOne.LocalHub\bin\Debug\net5.0\Alifax.IdOne.LocalHub.exe"
            }
            {$_ -in "rhb","remotehub"} {
                Start-Console-App "$($IdOneBase)\Alifax.IdOne.RemoteHub\bin\Debug\net5.0\Alifax.IdOne.RemoteHub.exe"
            }
            {$_ -in "sts","studioservice"} {
                Start-Console-App "$($IdOneBase)\Alifax.IdOne.Studio.Service\bin\Debug\Alifax.IdOne.Studio.Service.exe" /C
            }
            {$_ -in "tcm","techmonitor"} {
                Start-Gui-App "$($IdOneBase)\Alifax.IdOne.TechMonitor\bin\Debug\Alifax.IdOne.TechMonitor.exe"
            }
            {$_ -in "cli","client"} {
                Start-Gui-App "$($IdOneBase)\Alifax.IdOne.Ivd.Client\bin\Debug\Alifax.IdOne.Ivd.Client.exe"
            }
            {$_ -in "stu","studio"} {
                Start-Gui-App "$($IdOneBase)\Alifax.IdOne.Studio.Client\bin\Debug\Alifax.IdOne.Studio.Client.exe"
            }
            {$_ -in "pck","package"} {
                Start-PowerShell-Script "$($IdOneBase)\Alifax.IdOne.Installer\create-package.ps1"
            }
            default {
                Command-Not-Found $Product $Command
                Exit
            }
        }
    }
    {$_ -in "awm","webmanager"} {
        switch ($Command) {
            "" {
                Print-Usage-Template $Product
                Print-Usage-WebManager
                Exit
            }
            {$_ -in "cli","client"} {
                Start-Command "$($WebManagerBase)\alifax-web-manager-gui" "npm" run,ng,serve,--open
            }
            {$_ -in "ser","server"} {
                Start-Command "$($WebManagerBase)\WebManager" "dotnet" run,--configuration,Debug
            }
            default {
                Command-Not-Found $Product $Command
                Exit
            }
        }
    }
    {$_ -in "als","lissim"} {
        switch ($Command) {
            "" {
                Print-Usage-Template $Product
                Print-Usage-LisSimulator
                Exit
            }
            {$_ -in "cli","client"} {
                Start-Command "$($LisSimulatorBase)\client" "npm" run,ng,serve,--open
            }
            {$_ -in "ser","server"} {
                Start-Command "$($LisSimulatorBase)\server" "npm" run,debug
            }
            default {
                Command-Not-Found $Product $Command
                Exit
            }
        }
    }
    default {
        Product-Not-Found $Product
        Exit
    }
}

# ==============
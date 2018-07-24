$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath
Push-Location $folder

Invoke-Command -ComputerName vm-dev-cont1.dev.kontur.ru -ScriptBlock {  Stop-Process -Name "JobExecutor"  }
Copy-Item bin \\vm-dev-cont1\c$\Executor -recurse -force
#.\PsExec64.exe \\vm-dev-cont1.dev.kontur.ru -d "c:\Executor\bin\JobExecutor.exe"


Invoke-Command -ComputerName vm-dev-cont2.dev.kontur.ru -ScriptBlock {  Stop-Process -Name "JobExecutor"  }
Copy-Item bin \\vm-dev-cont2\c$\Executor -recurse -force
#.\PsExec64.exe \\vm-dev-cont2.dev.kontur.ru -d "c:\Executor\bin\JobExecutor.exe"



Invoke-Command -ComputerName vm-dev-cont3.dev.kontur.ru -ScriptBlock {  Stop-Process -Name "JobExecutor"  }
Copy-Item bin \\vm-dev-cont3\c$\Executor -recurse -force
#.\PsExec64.exe \\vm-dev-cont3.dev.kontur.ru -d "c:\Executor\bin\JobExecutor.exe"



Invoke-Command -ComputerName vm-dev-cont4.dev.kontur.ru -ScriptBlock {  Stop-Process -Name "JobExecutor"  }
Copy-Item bin \\vm-dev-cont4\c$\Executor -recurse -force
#.\PsExec64.exe \\vm-dev-cont4.dev.kontur.ru -d "c:\Executor\bin\JobExecutor.exe"

Pop-Location

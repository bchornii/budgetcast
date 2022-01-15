$protocol = 'Tcp'
$instanceName = 'MSSQLSERVER'
$enable = $true

try
{
	Add-Type -AssemblyName "Microsoft.SqlServer.Smo"
	Add-Type -AssemblyName "Microsoft.SqlServer.SqlWmiManagement"
}
catch { throw "Exception occurred when we refer smo and SqlWmiManagement assembly, and its $($_.Exception)" }

try
{
	Write-Host "Initializing WMI object"		
	$wmi = new-object Microsoft.SqlServer.Management.Smo.WMI.ManagedComputer
	
	Write-Host "Generation URI"		
	$uri = "ManagedComputer[@Name='$env:COMPUTERNAME']/ServerInstance[@Name='$instanceName']/ServerProtocol[@Name='$protocol']";
	$uri
	
	Write-Host "Creating protocol settings"
	$prtocolSettings = $wmi.getsmoobject($uri)
	
	Write-Host "Created Protocol smo  object"
	$prtocolSettings.IsEnabled = $enable
	$prtocolSettings.Alter()
    $prtocolSettings
	
	# Restart SQL Server
	try { Get-Service -Name $instanceName -ErrorAction 'Stop' | Restart-Service -Force -ErrorAction 'Stop' }
	catch { throw "Starting Sql server service failed in server $($_.Exception)" }

    # Add firewall rules
    try {
        New-NetFirewallRule -DisplayName "SQLServer default instance" -Direction Inbound -LocalPort 1433 -Protocol TCP -Action Allow
        New-NetFirewallRule -DisplayName "SQLServer Browser service" -Direction Inbound -LocalPort 1434 -Protocol UDP -Action Allow
    }
    catch {
        catch { throw "Added Sql server firewall rules failed $($_.Exception)" }
    }
}

catch
{
	$_
}
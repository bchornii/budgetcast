
# Parse values from .env file and set up appropriate env variables for current process
Get-Content .\.env | ForEach-Object($_) { 
    if($_ -ne '') { 
        $envVarParts = $_.ToString().Split('=')
        [System.Environment]::SetEnvironmentVariable($envVarParts[0], $envVarParts[1], "Process")
    } 
}

$currentLocation = Get-Location

# Navigate to API
Set-Location -Path .\Services\BudgetCast.Identity\src\BudgetCast.Identity.Api

# Set up Google secrets
dotnet user-secrets set "Social:Google:ClientId" (Get-ChildItem Env:GOOGLE_CLIENT_ID).value
dotnet user-secrets set "Social:Google:ClientSecret" (Get-ChildItem Env:GOOGLE_CLIENT_SECRET).value

# Set up Facebook secrets
dotnet user-secrets set "Social:Facebook:ClientId" (Get-ChildItem Env:FACEBOOK_CLIENT_ID).value
dotnet user-secrets set "Social:Facebook:ClientSecret" (Get-ChildItem Env:FACEBOOK_CLIENT_SECRET).value

# Restore location
Set-Location -Path $currentLocation
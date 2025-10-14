
Add-Type -Path "d:\Test\NT106-Assignment\source\repos\Database\bin\Debug\net8.0\BCrypt.Net-Next.dll"

Write-Host "Generating 25 unique BCrypt hashes for password '123456'..." -ForegroundColor Cyan
Write-Host ""

$password = "123456"
$workFactor = 11

for ($i = 1; $i -le 25; $i++) {
    $hash = [BCrypt.Net.BCrypt]::HashPassword($password, $workFactor)
    Write-Host "User $i : $hash"
}

Write-Host "Done" -ForegroundColor Green

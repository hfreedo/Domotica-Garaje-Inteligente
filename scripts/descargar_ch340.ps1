$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$driverFolder = Join-Path $projectRoot 'support/ch340'
$manifest = Get-Content -Raw -LiteralPath (Join-Path $driverFolder 'procedencia.json') | ConvertFrom-Json
$target = Join-Path $driverFolder $manifest.file
$temporary = Join-Path $driverFolder 'CH341SER.download.exe'
try {
    Invoke-WebRequest -UseBasicParsing -Uri $manifest.download_url -OutFile $temporary
    $hash = (Get-FileHash -LiteralPath $temporary -Algorithm SHA256).Hash
    if ($hash -ne $manifest.sha256) { throw 'El archivo de WCH cambio. Revise la descarga y el manifiesto antes de continuar.' }
    $signature = Get-AuthenticodeSignature -LiteralPath $temporary
    if ($signature.Status -ne 'Valid' -or $signature.SignerCertificate.Thumbprint -ne $manifest.certificate_thumbprint) { throw 'No se pudo verificar la firma esperada de WCH.' }
    Move-Item -LiteralPath $temporary -Destination $target -Force
    Write-Host "Driver oficial verificado: $target. No se ha instalado."
} finally {
    if (Test-Path -LiteralPath $temporary) { Remove-Item -LiteralPath $temporary }
}

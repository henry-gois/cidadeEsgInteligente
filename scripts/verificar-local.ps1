$ErrorActionPreference = 'Stop'

function Invoke-CheckedCommand([string]$Command, [string[]]$Arguments) {
    Write-Host "> $Command $($Arguments -join ' ')" -ForegroundColor Cyan
    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Comando falhou com código $LASTEXITCODE : $Command $($Arguments -join ' ')"
    }
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'SDK .NET não encontrado. Instale o SDK .NET 8 e abra um novo terminal.'
}

Invoke-CheckedCommand 'dotnet' @('restore', 'EsgEnergy.sln')
Invoke-CheckedCommand 'dotnet' @('build', 'EsgEnergy.sln', '--configuration', 'Release', '--no-restore')
Invoke-CheckedCommand 'dotnet' @('test', 'EsgEnergy.sln', '--configuration', 'Release', '--no-build')

if (Get-Command docker -ErrorAction SilentlyContinue) {
    Invoke-CheckedCommand 'docker' @('compose', '--env-file', '.env.example', 'config', '--quiet')
} else {
    Write-Warning 'Docker não encontrado; validação do Compose ignorada.'
}

Write-Host 'Validação concluída.' -ForegroundColor Green

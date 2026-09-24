# Instala o PhoneForge no Windows e cria atalhos na Área de Trabalho e no Menu Iniciar.
# Uso: clique com o botão direito neste arquivo > "Executar com o PowerShell"
#      ou, no terminal: powershell -ExecutionPolicy Bypass -File instalar.ps1

$ErrorActionPreference = 'Stop'
$projeto = $PSScriptRoot
$destino = Join-Path $env:LOCALAPPDATA 'Programs\PhoneForge'

# Fecha o programa se estiver aberto, para poder substituir os arquivos
Get-Process PhoneForge -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host 'Gerando o programa...'
dotnet publish "$projeto\PhoneForge.csproj" -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o $destino --nologo -v q
if ($LASTEXITCODE -ne 0) { throw 'Falha ao gerar o programa.' }

$exe = Join-Path $destino 'PhoneForge.exe'
$shell = New-Object -ComObject WScript.Shell
$pastas = @(
    [Environment]::GetFolderPath('Desktop'),
    (Join-Path ([Environment]::GetFolderPath('StartMenu')) 'Programs')
)

foreach ($pasta in $pastas) {
    $atalho = $shell.CreateShortcut((Join-Path $pasta 'PhoneForge.lnk'))
    $atalho.TargetPath = $exe
    $atalho.WorkingDirectory = $destino
    $atalho.IconLocation = "$exe,0"
    $atalho.Description = 'PhoneForge - gerenciador de smartphones'
    $atalho.Save()
}

Write-Host ''
Write-Host "PhoneForge instalado em: $destino"
Write-Host 'Atalhos criados na Área de Trabalho e no Menu Iniciar.'

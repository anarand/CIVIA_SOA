# Verificação de contract drift: compara o contrato escrito antes do código (docs/openapi.yaml)
# com o documento OpenAPI que a API em execução gera (/swagger/v1/swagger.json).
# Compara cada combinação de rota + método + status code documentado.
# Uso (com a API rodando): powershell -ExecutionPolicy Bypass -File scripts\comparar-contrato.ps1
param([string]$BaseUrl = 'http://localhost:5080')

$raiz = Split-Path $PSScriptRoot -Parent
$yaml = Get-Content (Join-Path $raiz 'docs\openapi.yaml') -Encoding UTF8

# Lê o YAML linha a linha: rota (2 espaços), método (4 espaços) e status (8 espaços dentro de responses).
$contrato = @()
$rota = $null; $metodo = $null; $emResponses = $false
foreach ($linha in $yaml) {
    if ($linha -match '^components:') { break }
    if ($linha -match "^  (/[^:]+):\s*$") { $rota = $Matches[1]; $metodo = $null; continue }
    if ($linha -match '^    (get|post|put|patch|delete):\s*$') { $metodo = $Matches[1].ToUpper(); $emResponses = $false; continue }
    if ($metodo -and $linha -match '^      responses:') { $emResponses = $true; continue }
    if ($emResponses -and $linha -match "^        '(\d{3})':") { $contrato += "$metodo $rota $($Matches[1])" }
}

$gerado = @()
$doc = Invoke-RestMethod "$BaseUrl/swagger/v1/swagger.json"
foreach ($p in $doc.paths.PSObject.Properties) {
    $rotaGerada = $p.Name -replace '\{(\w+)\}', '{$1}'
    foreach ($m in $p.Value.PSObject.Properties) {
        foreach ($s in $m.Value.responses.PSObject.Properties) { $gerado += "$($m.Name.ToUpper()) $rotaGerada $($s.Name)" }
    }
}

$soNoContrato = $contrato | Where-Object { $gerado -notcontains $_ }
$soNoCodigo = $gerado | Where-Object { $contrato -notcontains $_ }

Write-Output "Combinações rota + método + status no contrato: $($contrato.Count)"
Write-Output "Combinações rota + método + status no código:   $($gerado.Count)"
Write-Output ''
Write-Output 'Documentadas no contrato e ausentes no código:'
if ($soNoContrato) { $soNoContrato | ForEach-Object { "  $_" } } else { '  nenhuma' }
Write-Output 'Presentes no código e ausentes no contrato:'
if ($soNoCodigo) { $soNoCodigo | ForEach-Object { "  $_" } } else { '  nenhuma' }

# Executa automaticamente o mesmo roteiro da coleção do Insomnia (insomnia/roteiro.tsv)
# e compara o status obtido com o esperado. Serve para conferir a API antes dos testes manuais.
# Requer a API rodando e o banco recém-criado (os ids esperados começam em 1).
# Uso: powershell -ExecutionPolicy Bypass -File scripts\executar-roteiro.ps1 [-Saida evidencias\02-roteiro-automatico.md]
param([string]$BaseUrl = 'http://localhost:5080/api', [string]$Saida)

Add-Type -AssemblyName System.Net.Http
$raiz = Split-Path $PSScriptRoot -Parent
$linhas = Import-Csv (Join-Path $raiz 'insomnia\roteiro.tsv') -Delimiter "`t" -Encoding UTF8
$http = New-Object System.Net.Http.HttpClient

$relatorio = New-Object System.Text.StringBuilder
$falhas = 0
foreach ($l in $linhas) {
    $msg = New-Object System.Net.Http.HttpRequestMessage ([System.Net.Http.HttpMethod]::new($l.metodo)), ($BaseUrl + $l.caminho)
    if ($l.corpo) { $msg.Content = New-Object System.Net.Http.StringContent $l.corpo, ([Text.Encoding]::UTF8), 'application/json' }
    $resp = $http.SendAsync($msg).Result
    $status = [int]$resp.StatusCode
    $corpo = $resp.Content.ReadAsStringAsync().Result
    $ok = $status -eq [int]$l.esperado
    if (-not $ok) { $falhas++ }
    $marca = if ($ok) { 'OK' } else { 'FALHOU' }
    Write-Output ("{0,-6} {1,-45} esperado {2} | obtido {3}" -f $marca, $l.nome, $l.esperado, $status)

    [void]$relatorio.AppendLine("### $($l.nome)")
    [void]$relatorio.AppendLine("``$($l.metodo) $($l.caminho)`` | esperado **$($l.esperado)** | obtido **$status** | $marca")
    if ($l.corpo) { [void]$relatorio.AppendLine("Requisição: ``$($l.corpo)``") }
    if ($corpo) { [void]$relatorio.AppendLine("Resposta: ``$corpo``") }
    [void]$relatorio.AppendLine()
}
Write-Output ''
Write-Output "Resultado: $($linhas.Count - $falhas) de $($linhas.Count) requisições com o status esperado."

if ($Saida) {
    $cabecalho = "# Execução automática do roteiro de testes`n`nData: $((Get-Date).ToString('yyyy-MM-dd HH:mm:ss')) | API: $BaseUrl | Resultado: $($linhas.Count - $falhas) de $($linhas.Count) com o status esperado.`n`n"
    [IO.File]::WriteAllText((Join-Path $raiz $Saida), $cabecalho + $relatorio.ToString(), (New-Object Text.UTF8Encoding($false)))
}

# Gera a coleção do Insomnia (insomnia/civia-insomnia.json, formato de exportação v4)
# a partir do roteiro de testes insomnia/roteiro.tsv.
# Uso: powershell -ExecutionPolicy Bypass -File scripts\gerar-colecao-insomnia.ps1
$raiz = Split-Path $PSScriptRoot -Parent
$linhas = Import-Csv (Join-Path $raiz 'insomnia\roteiro.tsv') -Delimiter "`t" -Encoding UTF8

$recursos = New-Object System.Collections.ArrayList
[void]$recursos.Add([ordered]@{
    _id = 'wrk_civia'; _type = 'workspace'; parentId = $null; name = 'CIVIA API'; scope = 'collection'
    description = 'Testes manuais da API CIVIA. Execute as requisições em ordem, com o banco recém-criado pela migration.'
})
[void]$recursos.Add([ordered]@{
    _id = 'env_civia_base'; _type = 'environment'; parentId = 'wrk_civia'; name = 'Base Environment'
    data = [ordered]@{ base_url = 'http://localhost:5080/api' }
})

$pastas = @{}
$ordem = 0
foreach ($l in $linhas) {
    if (-not $pastas.ContainsKey($l.pasta)) {
        $idPasta = 'fld_civia_' + ($pastas.Count + 1)
        $pastas[$l.pasta] = $idPasta
        [void]$recursos.Add([ordered]@{
            _id = $idPasta; _type = 'request_group'; parentId = 'wrk_civia'; name = $l.pasta
            metaSortKey = $pastas.Count * 100
        })
    }
    $ordem++
    $req = [ordered]@{
        _id = 'req_civia_' + $ordem.ToString('00'); _type = 'request'; parentId = $pastas[$l.pasta]
        name = "$($l.nome) ($($l.esperado))"; method = $l.metodo
        url = '{{ _.base_url }}' + $l.caminho
        description = "Status esperado: $($l.esperado)"
        metaSortKey = $ordem; headers = @(); body = [ordered]@{}
    }
    if ($l.corpo) {
        $req.body = [ordered]@{ mimeType = 'application/json'; text = ($l.corpo | ConvertFrom-Json | ConvertTo-Json -Depth 5) }
        $req.headers = @([ordered]@{ name = 'Content-Type'; value = 'application/json' })
    }
    [void]$recursos.Add($req)
}

$export = [ordered]@{
    _type = 'export'; __export_format = 4; __export_date = (Get-Date).ToUniversalTime().ToString('o')
    __export_source = 'insomnia.desktop.app:v13.4.0'; resources = $recursos
}
$json = $export | ConvertTo-Json -Depth 10
$destino = Join-Path $raiz 'insomnia\civia-insomnia.json'
[IO.File]::WriteAllText($destino, $json, (New-Object Text.UTF8Encoding($false)))
Write-Output "Coleção gerada: $destino ($ordem requisições em $($pastas.Count) pastas)"

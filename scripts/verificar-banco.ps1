# Consulta o catálogo do SQL Server e mostra o que a migration criou no banco CiviaDb:
# migrations aplicadas, tabelas e colunas, chaves estrangeiras (com regra de exclusão) e índices únicos.
# Uso: powershell -ExecutionPolicy Bypass -File scripts\verificar-banco.ps1
param([string]$ConnectionString = 'Server=(localdb)\MSSQLLocalDB;Database=CiviaDb;Integrated Security=True')

$conn = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
$conn.Open()

function Consultar([string]$titulo, [string]$sql) {
    Write-Output ""
    Write-Output "=== $titulo"
    $cmd = $conn.CreateCommand(); $cmd.CommandText = $sql
    $tabela = New-Object System.Data.DataTable
    $tabela.Load($cmd.ExecuteReader())
    $tabela | Format-Table -AutoSize | Out-String -Width 220
}

Consultar 'Migrations aplicadas (__EFMigrationsHistory)' 'SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId'

Consultar 'Tabelas e colunas' @"
SELECT t.name AS Tabela, c.name AS Coluna, ty.name AS Tipo,
       CASE WHEN ty.name LIKE 'n%char' THEN c.max_length / 2 ELSE c.max_length END AS Tamanho,
       CASE c.is_nullable WHEN 1 THEN 'sim' ELSE 'não' END AS AceitaNulo
FROM sys.tables t
JOIN sys.columns c ON c.object_id = t.object_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
WHERE t.name <> '__EFMigrationsHistory'
ORDER BY t.name, c.column_id
"@

Consultar 'Chaves estrangeiras' @"
SELECT fk.name AS ChaveEstrangeira, tp.name AS TabelaFilha, cp.name AS Coluna,
       tr.name AS TabelaReferenciada, fk.delete_referential_action_desc AS AoExcluir
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
JOIN sys.tables tp ON tp.object_id = fk.parent_object_id
JOIN sys.columns cp ON cp.object_id = fkc.parent_object_id AND cp.column_id = fkc.parent_column_id
JOIN sys.tables tr ON tr.object_id = fk.referenced_object_id
ORDER BY tp.name
"@

Consultar 'Índices únicos (além das chaves primárias)' @"
SELECT t.name AS Tabela, i.name AS Indice
FROM sys.indexes i JOIN sys.tables t ON t.object_id = i.object_id
WHERE i.is_unique = 1 AND i.is_primary_key = 0
ORDER BY t.name
"@

Consultar 'Quantidade de registros por tabela' @"
SELECT 'Usuarios' AS Tabela, COUNT(*) AS Registros FROM Usuarios
UNION ALL SELECT 'PreferenciasAcessibilidade', COUNT(*) FROM PreferenciasAcessibilidade
UNION ALL SELECT 'LocaisRotina', COUNT(*) FROM LocaisRotina
UNION ALL SELECT 'ContatosConfianca', COUNT(*) FROM ContatosConfianca
UNION ALL SELECT 'SolicitacoesAjuda', COUNT(*) FROM SolicitacoesAjuda
UNION ALL SELECT 'AlertasUrbanos', COUNT(*) FROM AlertasUrbanos
"@

$conn.Close()

namespace Civia.Api.Models;

/// <summary>Aviso sobre alteração no deslocamento urbano. Não pertence a um usuário específico.</summary>
public class AlertaUrbano
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public TipoAlerta Tipo { get; set; }
    public string Local { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}

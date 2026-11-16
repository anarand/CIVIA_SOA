namespace Civia.Api.Models;

/// <summary>
/// Pedido de ajuda a um contato de confiança (AJUDA) ou acionamento de emergência (SOS).
/// Os dois fluxos compartilham dados e ciclo de vida, por isso são uma única entidade com um tipo.
/// </summary>
public class SolicitacaoAjuda
{
    // Transições de status permitidas. CONCLUIDA e CANCELADA são estados finais.
    private static readonly Dictionary<StatusSolicitacao, StatusSolicitacao[]> Transicoes = new()
    {
        [StatusSolicitacao.ABERTA] = [StatusSolicitacao.EM_ATENDIMENTO, StatusSolicitacao.CANCELADA],
        [StatusSolicitacao.EM_ATENDIMENTO] = [StatusSolicitacao.CONCLUIDA, StatusSolicitacao.CANCELADA],
        [StatusSolicitacao.CONCLUIDA] = [],
        [StatusSolicitacao.CANCELADA] = [],
    };

    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int? ContatoConfiancaId { get; set; }
    public TipoSolicitacao Tipo { get; set; }
    public StatusSolicitacao Status { get; set; } = StatusSolicitacao.ABERTA;
    public string? Mensagem { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Usuario? Usuario { get; set; }
    public ContatoConfianca? ContatoConfianca { get; set; }

    public bool PodeMudarPara(StatusSolicitacao novoStatus) => Transicoes[Status].Contains(novoStatus);

    /// <summary>Aplica a transição. Retorna false, sem alterar nada, se ela não for permitida.</summary>
    public bool AlterarStatus(StatusSolicitacao novoStatus, DateTime agora)
    {
        if (!PodeMudarPara(novoStatus)) return false;
        Status = novoStatus;
        AtualizadoEm = agora;
        return true;
    }
}

namespace Civia.Api.Models;

/// <summary>Cidadão que utiliza o CIVIA. Raiz do domínio: os demais dados pessoais pertencem a ele.</summary>
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoDeficiencia TipoDeficiencia { get; set; }
    public DateTime CriadoEm { get; set; }

    public PreferenciaAcessibilidade? Preferencia { get; set; }
    public List<LocalRotina> Locais { get; set; } = new();
    public List<ContatoConfianca> Contatos { get; set; } = new();
    public List<SolicitacaoAjuda> Solicitacoes { get; set; } = new();
}

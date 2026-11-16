namespace Civia.Api.Models;

/// <summary>Pessoa autorizada pelo usuário a receber pedidos de ajuda.</summary>
public class ContatoConfianca
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Relacao { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }

    public Usuario? Usuario { get; set; }
    public List<SolicitacaoAjuda> Solicitacoes { get; set; } = new();
}

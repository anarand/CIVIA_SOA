namespace Civia.Api.Models;

/// <summary>Como o usuário deseja receber os avisos. Relação 1:1 com Usuario.</summary>
public class PreferenciaAcessibilidade
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public CanalComunicacao CanalPrincipal { get; set; }
    public bool VibracaoAtiva { get; set; }
    public bool AltoContraste { get; set; }
    public bool PriorizarElevador { get; set; }
    public DateTime AtualizadoEm { get; set; }

    public Usuario? Usuario { get; set; }
}

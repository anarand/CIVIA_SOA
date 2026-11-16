namespace Civia.Api.Models;

/// <summary>Local frequente definido pelo próprio usuário (casa, trabalho, estação...).</summary>
public class LocalRotina
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoLocal Tipo { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CriadoEm { get; set; }

    public Usuario? Usuario { get; set; }
}

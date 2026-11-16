using System.ComponentModel.DataAnnotations;
using Civia.Api.Models;

namespace Civia.Api.Dtos;

public record UsuarioRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    string? Nome,

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O email informado não é válido.")]
    [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres.")]
    string? Email,

    [Required(ErrorMessage = "O tipo de deficiência é obrigatório.")]
    TipoDeficiencia? TipoDeficiencia);

public record UsuarioResponse(int Id, string Nome, string Email, TipoDeficiencia TipoDeficiencia, DateTime CriadoEm)
{
    public static UsuarioResponse De(Usuario u) => new(u.Id, u.Nome, u.Email, u.TipoDeficiencia, u.CriadoEm);
}

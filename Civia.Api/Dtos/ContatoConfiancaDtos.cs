using System.ComponentModel.DataAnnotations;
using Civia.Api.Models;

namespace Civia.Api.Dtos;

public record ContatoConfiancaRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    string? Nome,

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "O telefone deve ter de 10 a 15 dígitos, com DDD, e pode começar com +.")]
    string? Telefone,

    [Required(ErrorMessage = "A relação com o usuário é obrigatória.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "A relação deve ter entre 2 e 50 caracteres.")]
    string? Relacao,

    [Required(ErrorMessage = "Informe se o contato está ativo.")]
    bool? Ativo);

public record ContatoConfiancaResponse(
    int Id, int UsuarioId, string Nome, string Telefone, string Relacao, bool Ativo, DateTime CriadoEm)
{
    public static ContatoConfiancaResponse De(ContatoConfianca c) =>
        new(c.Id, c.UsuarioId, c.Nome, c.Telefone, c.Relacao, c.Ativo, c.CriadoEm);
}

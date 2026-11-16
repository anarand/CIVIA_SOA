using System.ComponentModel.DataAnnotations;
using Civia.Api.Models;

namespace Civia.Api.Dtos;

public record LocalRotinaRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    string? Nome,

    [Required(ErrorMessage = "O tipo do local é obrigatório.")]
    TipoLocal? Tipo,

    [Required(ErrorMessage = "O endereço é obrigatório.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "O endereço deve ter entre 5 e 200 caracteres.")]
    string? Endereco,

    [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
    double? Latitude,

    [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
    double? Longitude);

public record LocalRotinaResponse(
    int Id, int UsuarioId, string Nome, TipoLocal Tipo, string Endereco,
    double? Latitude, double? Longitude, DateTime CriadoEm)
{
    public static LocalRotinaResponse De(LocalRotina l) =>
        new(l.Id, l.UsuarioId, l.Nome, l.Tipo, l.Endereco, l.Latitude, l.Longitude, l.CriadoEm);
}

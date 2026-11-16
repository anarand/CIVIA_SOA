using System.ComponentModel.DataAnnotations;
using Civia.Api.Models;

namespace Civia.Api.Dtos;

public record AlertaUrbanoRequest(
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 120 caracteres.")]
    string? Titulo,

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 500 caracteres.")]
    string? Descricao,

    [Required(ErrorMessage = "O tipo do alerta é obrigatório.")]
    TipoAlerta? Tipo,

    [Required(ErrorMessage = "O local é obrigatório.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O local deve ter entre 3 e 200 caracteres.")]
    string? Local,

    [Required(ErrorMessage = "Informe se o alerta está ativo.")]
    bool? Ativo);

public record AlertaUrbanoResponse(
    int Id, string Titulo, string Descricao, TipoAlerta Tipo, string Local, bool Ativo,
    DateTime CriadoEm, DateTime AtualizadoEm)
{
    public static AlertaUrbanoResponse De(AlertaUrbano a) =>
        new(a.Id, a.Titulo, a.Descricao, a.Tipo, a.Local, a.Ativo, a.CriadoEm, a.AtualizadoEm);
}

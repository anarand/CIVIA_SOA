using System.ComponentModel.DataAnnotations;
using Civia.Api.Models;

namespace Civia.Api.Dtos;

public record PreferenciaRequest(
    [Required(ErrorMessage = "O canal principal é obrigatório.")] CanalComunicacao? CanalPrincipal,
    [Required(ErrorMessage = "Informe se a vibração está ativa.")] bool? VibracaoAtiva,
    [Required(ErrorMessage = "Informe se o alto contraste está ativo.")] bool? AltoContraste,
    [Required(ErrorMessage = "Informe se o elevador deve ser priorizado.")] bool? PriorizarElevador);

public record PreferenciaResponse(
    int UsuarioId, CanalComunicacao CanalPrincipal, bool VibracaoAtiva, bool AltoContraste,
    bool PriorizarElevador, DateTime AtualizadoEm)
{
    public static PreferenciaResponse De(PreferenciaAcessibilidade p) =>
        new(p.UsuarioId, p.CanalPrincipal, p.VibracaoAtiva, p.AltoContraste, p.PriorizarElevador, p.AtualizadoEm);
}

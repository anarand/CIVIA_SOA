using Civia.Api.Data;
using Civia.Api.Dtos;
using Civia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Civia.Api.Controllers;

[Route("api/usuarios/{usuarioId:int}/preferencias")]
[Tags("Preferências")]
public class PreferenciasController(CiviaDbContext db) : CiviaControllerBase
{
    [HttpGet]
    [ProducesResponseType<PreferenciaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PreferenciaResponse>> Obter(int usuarioId)
    {
        if (!await db.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        var preferencia = await db.Preferencias.AsNoTracking().FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        return preferencia is null
            ? NaoEncontrado($"O usuário {usuarioId} ainda não configurou suas preferências.")
            : PreferenciaResponse.De(preferencia);
    }

    /// <summary>Cria ou atualiza as preferências (idempotente: repetir a requisição dá o mesmo resultado).</summary>
    [HttpPut]
    [ProducesResponseType<PreferenciaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PreferenciaResponse>> Salvar(int usuarioId, PreferenciaRequest request)
    {
        if (!await db.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        var preferencia = await db.Preferencias.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        if (preferencia is null)
        {
            preferencia = new PreferenciaAcessibilidade { UsuarioId = usuarioId };
            db.Preferencias.Add(preferencia);
        }

        preferencia.CanalPrincipal = request.CanalPrincipal!.Value;
        preferencia.VibracaoAtiva = request.VibracaoAtiva!.Value;
        preferencia.AltoContraste = request.AltoContraste!.Value;
        preferencia.PriorizarElevador = request.PriorizarElevador!.Value;
        preferencia.AtualizadoEm = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return PreferenciaResponse.De(preferencia);
    }
}

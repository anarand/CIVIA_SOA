using Civia.Api.Data;
using Civia.Api.Dtos;
using Civia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Civia.Api.Controllers;

[Route("api/usuarios/{usuarioId:int}/locais")]
[Tags("Locais de rotina")]
public class LocaisRotinaController(CiviaDbContext db) : CiviaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<LocalRotinaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<LocalRotinaResponse>>> Listar(int usuarioId)
    {
        if (!await db.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        var locais = await db.LocaisRotina.AsNoTracking()
            .Where(l => l.UsuarioId == usuarioId).OrderBy(l => l.Nome).ToListAsync();
        return locais.Select(LocalRotinaResponse.De).ToList();
    }

    [HttpGet("{localId:int}")]
    [ProducesResponseType<LocalRotinaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocalRotinaResponse>> Obter(int usuarioId, int localId)
    {
        var local = await db.LocaisRotina.AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == localId && l.UsuarioId == usuarioId);
        return local is null
            ? NaoEncontrado($"Local {localId} não encontrado para o usuário {usuarioId}.")
            : LocalRotinaResponse.De(local);
    }

    [HttpPost]
    [ProducesResponseType<LocalRotinaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocalRotinaResponse>> Criar(int usuarioId, LocalRotinaRequest request)
    {
        if (!await db.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        var local = new LocalRotina { UsuarioId = usuarioId, CriadoEm = DateTime.UtcNow };
        Aplicar(local, request);
        db.LocaisRotina.Add(local);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { usuarioId, localId = local.Id }, LocalRotinaResponse.De(local));
    }

    [HttpPut("{localId:int}")]
    [ProducesResponseType<LocalRotinaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocalRotinaResponse>> Atualizar(int usuarioId, int localId, LocalRotinaRequest request)
    {
        var local = await db.LocaisRotina.FirstOrDefaultAsync(l => l.Id == localId && l.UsuarioId == usuarioId);
        if (local is null) return NaoEncontrado($"Local {localId} não encontrado para o usuário {usuarioId}.");

        Aplicar(local, request);
        await db.SaveChangesAsync();
        return LocalRotinaResponse.De(local);
    }

    [HttpDelete("{localId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int usuarioId, int localId)
    {
        var local = await db.LocaisRotina.FirstOrDefaultAsync(l => l.Id == localId && l.UsuarioId == usuarioId);
        if (local is null) return NaoEncontrado($"Local {localId} não encontrado para o usuário {usuarioId}.");

        db.LocaisRotina.Remove(local);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void Aplicar(LocalRotina local, LocalRotinaRequest request)
    {
        local.Nome = request.Nome!.Trim();
        local.Tipo = request.Tipo!.Value;
        local.Endereco = request.Endereco!.Trim();
        local.Latitude = request.Latitude;
        local.Longitude = request.Longitude;
    }
}

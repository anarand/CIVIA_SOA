using Civia.Api.Data;
using Civia.Api.Dtos;
using Civia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Civia.Api.Controllers;

[Route("api/usuarios")]
[Tags("Usuários")]
public class UsuariosController(CiviaDbContext db) : CiviaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<UsuarioResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UsuarioResponse>>> Listar()
    {
        var usuarios = await db.Usuarios.AsNoTracking().OrderBy(u => u.Nome).ToListAsync();
        return usuarios.Select(UsuarioResponse.De).ToList();
    }

    [HttpGet("{usuarioId:int}")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> Obter(int usuarioId)
    {
        var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId);
        return usuario is null ? NaoEncontrado($"Usuário {usuarioId} não encontrado.") : UsuarioResponse.De(usuario);
    }

    [HttpPost]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Criar(UsuarioRequest request)
    {
        var email = request.Email!.Trim().ToLowerInvariant();
        if (await db.Usuarios.AnyAsync(u => u.Email == email))
            return Conflito($"Já existe um usuário com o email {email}.");

        var usuario = new Usuario
        {
            Nome = request.Nome!.Trim(),
            Email = email,
            TipoDeficiencia = request.TipoDeficiencia!.Value,
            CriadoEm = DateTime.UtcNow,
        };
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { usuarioId = usuario.Id }, UsuarioResponse.De(usuario));
    }

    [HttpPut("{usuarioId:int}")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Atualizar(int usuarioId, UsuarioRequest request)
    {
        var usuario = await db.Usuarios.FindAsync(usuarioId);
        if (usuario is null) return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        var email = request.Email!.Trim().ToLowerInvariant();
        if (await db.Usuarios.AnyAsync(u => u.Email == email && u.Id != usuarioId))
            return Conflito($"Já existe um usuário com o email {email}.");

        usuario.Nome = request.Nome!.Trim();
        usuario.Email = email;
        usuario.TipoDeficiencia = request.TipoDeficiencia!.Value;
        await db.SaveChangesAsync();

        return UsuarioResponse.De(usuario);
    }

    [HttpDelete("{usuarioId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int usuarioId)
    {
        var usuario = await db.Usuarios.FindAsync(usuarioId);
        if (usuario is null) return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        // As preferências, locais, contatos e solicitações são removidos pelo ON DELETE CASCADE do banco.
        db.Usuarios.Remove(usuario);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

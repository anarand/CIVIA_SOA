using Civia.Api.Data;
using Civia.Api.Dtos;
using Civia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Civia.Api.Controllers;

[Route("api/usuarios/{usuarioId:int}/contatos")]
[Tags("Contatos de confiança")]
public class ContatosConfiancaController(CiviaDbContext db) : CiviaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<ContatoConfiancaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ContatoConfiancaResponse>>> Listar(int usuarioId)
    {
        if (!await db.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        var contatos = await db.ContatosConfianca.AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId).OrderBy(c => c.Nome).ToListAsync();
        return contatos.Select(ContatoConfiancaResponse.De).ToList();
    }

    [HttpGet("{contatoId:int}")]
    [ProducesResponseType<ContatoConfiancaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContatoConfiancaResponse>> Obter(int usuarioId, int contatoId)
    {
        var contato = await db.ContatosConfianca.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == contatoId && c.UsuarioId == usuarioId);
        return contato is null
            ? NaoEncontrado($"Contato {contatoId} não encontrado para o usuário {usuarioId}.")
            : ContatoConfiancaResponse.De(contato);
    }

    [HttpPost]
    [ProducesResponseType<ContatoConfiancaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContatoConfiancaResponse>> Criar(int usuarioId, ContatoConfiancaRequest request)
    {
        if (!await db.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        var contato = new ContatoConfianca { UsuarioId = usuarioId, CriadoEm = DateTime.UtcNow };
        Aplicar(contato, request);
        db.ContatosConfianca.Add(contato);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { usuarioId, contatoId = contato.Id },
            ContatoConfiancaResponse.De(contato));
    }

    [HttpPut("{contatoId:int}")]
    [ProducesResponseType<ContatoConfiancaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContatoConfiancaResponse>> Atualizar(
        int usuarioId, int contatoId, ContatoConfiancaRequest request)
    {
        var contato = await db.ContatosConfianca.FirstOrDefaultAsync(c => c.Id == contatoId && c.UsuarioId == usuarioId);
        if (contato is null) return NaoEncontrado($"Contato {contatoId} não encontrado para o usuário {usuarioId}.");

        Aplicar(contato, request);
        await db.SaveChangesAsync();
        return ContatoConfiancaResponse.De(contato);
    }

    [HttpDelete("{contatoId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Excluir(int usuarioId, int contatoId)
    {
        var contato = await db.ContatosConfianca.FirstOrDefaultAsync(c => c.Id == contatoId && c.UsuarioId == usuarioId);
        if (contato is null) return NaoEncontrado($"Contato {contatoId} não encontrado para o usuário {usuarioId}.");

        // Regra de negócio: o histórico de pedidos de ajuda não pode perder a referência ao contato.
        if (await db.Solicitacoes.AnyAsync(s => s.ContatoConfiancaId == contatoId))
            return Conflito($"O contato {contatoId} possui solicitações registradas. Desative-o com ativo = false.");

        db.ContatosConfianca.Remove(contato);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void Aplicar(ContatoConfianca contato, ContatoConfiancaRequest request)
    {
        contato.Nome = request.Nome!.Trim();
        contato.Telefone = request.Telefone!.Trim();
        contato.Relacao = request.Relacao!.Trim();
        contato.Ativo = request.Ativo!.Value;
    }
}

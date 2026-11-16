using Civia.Api.Data;
using Civia.Api.Dtos;
using Civia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Civia.Api.Controllers;

/// <summary>
/// Pedidos de ajuda e SOS. Não há PUT nem DELETE: o histórico é preservado e só o status muda.
/// A API registra o pedido; nenhuma notificação real é enviada nesta versão.
/// </summary>
[Route("api/usuarios/{usuarioId:int}/solicitacoes")]
[Tags("Solicitações de ajuda")]
public class SolicitacoesAjudaController(CiviaDbContext db) : CiviaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<SolicitacaoAjudaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<SolicitacaoAjudaResponse>>> Listar(
        int usuarioId, [FromQuery] StatusSolicitacao? status)
    {
        if (!await db.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        var consulta = db.Solicitacoes.AsNoTracking().Where(s => s.UsuarioId == usuarioId);
        if (status is not null) consulta = consulta.Where(s => s.Status == status);

        var solicitacoes = await consulta.OrderByDescending(s => s.CriadoEm).ToListAsync();
        return solicitacoes.Select(SolicitacaoAjudaResponse.De).ToList();
    }

    [HttpGet("{solicitacaoId:int}")]
    [ProducesResponseType<SolicitacaoAjudaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitacaoAjudaResponse>> Obter(int usuarioId, int solicitacaoId)
    {
        var solicitacao = await db.Solicitacoes.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == solicitacaoId && s.UsuarioId == usuarioId);
        return solicitacao is null
            ? NaoEncontrado($"Solicitação {solicitacaoId} não encontrada para o usuário {usuarioId}.")
            : SolicitacaoAjudaResponse.De(solicitacao);
    }

    [HttpPost]
    [ProducesResponseType<SolicitacaoAjudaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitacaoAjudaResponse>> Criar(int usuarioId, SolicitacaoAjudaRequest request)
    {
        if (!await db.Usuarios.AnyAsync(u => u.Id == usuarioId))
            return NaoEncontrado($"Usuário {usuarioId} não encontrado.");

        // Se um contato foi informado, ele precisa ser do mesmo usuário e estar ativo.
        if (request.ContatoConfiancaId is int contatoId)
        {
            var contato = await db.ContatosConfianca.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == contatoId && c.UsuarioId == usuarioId);
            if (contato is null)
                return ErroDeCampo(nameof(request.ContatoConfiancaId),
                    $"O contato {contatoId} não pertence ao usuário {usuarioId}.");
            if (!contato.Ativo)
                return ErroDeCampo(nameof(request.ContatoConfiancaId),
                    $"O contato {contatoId} está inativo e não pode receber solicitações.");
        }

        var agora = DateTime.UtcNow;
        var solicitacao = new SolicitacaoAjuda
        {
            UsuarioId = usuarioId,
            ContatoConfiancaId = request.ContatoConfiancaId,
            Tipo = request.Tipo!.Value,
            Status = StatusSolicitacao.ABERTA,
            Mensagem = string.IsNullOrWhiteSpace(request.Mensagem) ? null : request.Mensagem.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CriadoEm = agora,
            AtualizadoEm = agora,
        };
        db.Solicitacoes.Add(solicitacao);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { usuarioId, solicitacaoId = solicitacao.Id },
            SolicitacaoAjudaResponse.De(solicitacao));
    }

    [HttpPatch("{solicitacaoId:int}/status")]
    [ProducesResponseType<SolicitacaoAjudaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SolicitacaoAjudaResponse>> AlterarStatus(
        int usuarioId, int solicitacaoId, AlterarStatusRequest request)
    {
        var solicitacao = await db.Solicitacoes
            .FirstOrDefaultAsync(s => s.Id == solicitacaoId && s.UsuarioId == usuarioId);
        if (solicitacao is null)
            return NaoEncontrado($"Solicitação {solicitacaoId} não encontrada para o usuário {usuarioId}.");

        var statusAtual = solicitacao.Status;
        if (!solicitacao.AlterarStatus(request.Status!.Value, DateTime.UtcNow))
            return Conflito($"Não é possível mudar o status de {statusAtual} para {request.Status}.");

        await db.SaveChangesAsync();
        return SolicitacaoAjudaResponse.De(solicitacao);
    }
}

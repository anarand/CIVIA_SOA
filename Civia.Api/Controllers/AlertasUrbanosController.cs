using Civia.Api.Data;
using Civia.Api.Dtos;
using Civia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Civia.Api.Controllers;

/// <summary>Alertas urbanos cadastrados manualmente com dados simulados (sem integração com fontes oficiais).</summary>
[Route("api/alertas")]
[Tags("Alertas urbanos")]
public class AlertasUrbanosController(CiviaDbContext db) : CiviaControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<AlertaUrbanoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<AlertaUrbanoResponse>>> Listar(
        [FromQuery] TipoAlerta? tipo, [FromQuery] bool? ativo)
    {
        var consulta = db.Alertas.AsNoTracking();
        if (tipo is not null) consulta = consulta.Where(a => a.Tipo == tipo);
        if (ativo is not null) consulta = consulta.Where(a => a.Ativo == ativo);

        var alertas = await consulta.OrderByDescending(a => a.CriadoEm).ToListAsync();
        return alertas.Select(AlertaUrbanoResponse.De).ToList();
    }

    [HttpGet("{alertaId:int}")]
    [ProducesResponseType<AlertaUrbanoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaUrbanoResponse>> Obter(int alertaId)
    {
        var alerta = await db.Alertas.AsNoTracking().FirstOrDefaultAsync(a => a.Id == alertaId);
        return alerta is null ? NaoEncontrado($"Alerta {alertaId} não encontrado.") : AlertaUrbanoResponse.De(alerta);
    }

    [HttpPost]
    [ProducesResponseType<AlertaUrbanoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlertaUrbanoResponse>> Criar(AlertaUrbanoRequest request)
    {
        var agora = DateTime.UtcNow;
        var alerta = new AlertaUrbano { CriadoEm = agora };
        Aplicar(alerta, request, agora);
        db.Alertas.Add(alerta);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { alertaId = alerta.Id }, AlertaUrbanoResponse.De(alerta));
    }

    [HttpPut("{alertaId:int}")]
    [ProducesResponseType<AlertaUrbanoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaUrbanoResponse>> Atualizar(int alertaId, AlertaUrbanoRequest request)
    {
        var alerta = await db.Alertas.FindAsync(alertaId);
        if (alerta is null) return NaoEncontrado($"Alerta {alertaId} não encontrado.");

        Aplicar(alerta, request, DateTime.UtcNow);
        await db.SaveChangesAsync();
        return AlertaUrbanoResponse.De(alerta);
    }

    [HttpDelete("{alertaId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int alertaId)
    {
        var alerta = await db.Alertas.FindAsync(alertaId);
        if (alerta is null) return NaoEncontrado($"Alerta {alertaId} não encontrado.");

        db.Alertas.Remove(alerta);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void Aplicar(AlertaUrbano alerta, AlertaUrbanoRequest request, DateTime agora)
    {
        alerta.Titulo = request.Titulo!.Trim();
        alerta.Descricao = request.Descricao!.Trim();
        alerta.Tipo = request.Tipo!.Value;
        alerta.Local = request.Local!.Trim();
        alerta.Ativo = request.Ativo!.Value;
        alerta.AtualizadoEm = agora;
    }
}

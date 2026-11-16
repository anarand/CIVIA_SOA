using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Civia.Api.Controllers;

/// <summary>
/// Base comum dos controllers: padroniza as respostas de erro no formato Problem Details (RFC 7807),
/// o mesmo formato documentado em components/responses do contrato OpenAPI.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class CiviaControllerBase : ControllerBase
{
    protected ObjectResult NaoEncontrado(string detalhe) =>
        Problem(statusCode: StatusCodes.Status404NotFound, title: "Recurso não encontrado", detail: detalhe);

    protected ObjectResult Conflito(string detalhe) =>
        Problem(statusCode: StatusCodes.Status409Conflict, title: "Conflito com o estado atual", detail: detalhe);

    /// <summary>Erro de validação de regra de negócio, no mesmo formato dos erros de validação automáticos.</summary>
    protected ActionResult ErroDeCampo(string campo, string mensagem)
    {
        ModelState.AddModelError(campo, mensagem);
        var opcoes = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value;
        return (ActionResult)opcoes.InvalidModelStateResponseFactory(ControllerContext);
    }
}

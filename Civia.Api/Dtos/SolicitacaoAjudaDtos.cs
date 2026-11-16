using System.ComponentModel.DataAnnotations;
using Civia.Api.Models;

namespace Civia.Api.Dtos;

public record SolicitacaoAjudaRequest(
    [Required(ErrorMessage = "O tipo da solicitação é obrigatório.")]
    TipoSolicitacao? Tipo,

    int? ContatoConfiancaId,

    [StringLength(500, ErrorMessage = "A mensagem deve ter no máximo 500 caracteres.")]
    string? Mensagem,

    [Range(-90, 90, ErrorMessage = "A latitude deve estar entre -90 e 90.")]
    double? Latitude,

    [Range(-180, 180, ErrorMessage = "A longitude deve estar entre -180 e 180.")]
    double? Longitude) : IValidatableObject
{
    // Regras que dependem de mais de um campo.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Tipo == TipoSolicitacao.AJUDA && ContatoConfiancaId is null)
            yield return new ValidationResult(
                "Uma solicitação do tipo AJUDA precisa de um contato de confiança.", [nameof(ContatoConfiancaId)]);

        if (Tipo == TipoSolicitacao.SOS && (Latitude is null || Longitude is null))
            yield return new ValidationResult(
                "Um SOS precisa da última localização conhecida (latitude e longitude).",
                [nameof(Latitude), nameof(Longitude)]);
    }
}

public record AlterarStatusRequest(
    [Required(ErrorMessage = "O novo status é obrigatório.")]
    StatusSolicitacao? Status);

public record SolicitacaoAjudaResponse(
    int Id, int UsuarioId, StatusSolicitacao Status, TipoSolicitacao Tipo, int? ContatoConfiancaId,
    string? Mensagem, double? Latitude, double? Longitude, DateTime CriadoEm, DateTime AtualizadoEm)
{
    public static SolicitacaoAjudaResponse De(SolicitacaoAjuda s) =>
        new(s.Id, s.UsuarioId, s.Status, s.Tipo, s.ContatoConfiancaId, s.Mensagem,
            s.Latitude, s.Longitude, s.CriadoEm, s.AtualizadoEm);
}

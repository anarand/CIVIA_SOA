using System.Text.Json.Serialization;
using Civia.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        // Enums trafegam como texto ("BAIXA_VISAO"), exatamente como no contrato.
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
    })
    .ConfigureApiBehaviorOptions(o =>
    {
        // Erros de validação no formato Problem Details (RFC 7807), com título em português.
        o.InvalidModelStateResponseFactory = context =>
        {
            var erros = context.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .ToDictionary(
                    e => NomeDoCampo(e.Key),
                    e => e.Value!.Errors.Select(x => Traduzir(x.ErrorMessage)).Distinct().ToArray());
            // Quando o JSON tem um campo em formato inválido, o ASP.NET também acusa "request";
            // mantemos só o erro do campo, que é o que ajuda o cliente.
            if (erros.Count > 1) erros.Remove("request");

            var problema = new ValidationProblemDetails(erros)
            {
                Title = "Dados inválidos",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.HttpContext.Request.Path,
            };
            return new BadRequestObjectResult(problema) { ContentTypes = { "application/problem+json" } };
        };
    });

builder.Services.AddProblemDetails();

builder.Services.AddDbContext<CiviaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CiviaDb")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CIVIA API (gerada pelo código)", Version = "1.0.0" });
});

var app = builder.Build();

app.UseExceptionHandler(); // erros inesperados viram 500 em Problem Details, sem expor detalhes internos
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API implementada (gerada pelo código)");
        c.SwaggerEndpoint("/contrato/openapi.yaml", "Contrato (docs/openapi.yaml, escrito antes do código)");
    });

    // Publica o contrato escrito antes da implementação, para comparar com o que o código gera.
    var contrato = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "docs", "openapi.yaml"));
    app.MapGet("/contrato/openapi.yaml", () => Results.File(contrato, "application/yaml"))
        .ExcludeFromDescription();
}

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

app.Run();

// "$.tipoDeficiencia" ou "TipoDeficiencia" -> "tipoDeficiencia", o mesmo nome usado no JSON e no contrato.
static string NomeDoCampo(string chave)
{
    var nome = chave.StartsWith("$.") ? chave[2..] : chave;
    return nome.Length == 0 ? nome : char.ToLowerInvariant(nome[0]) + nome[1..];
}

// Mensagens geradas pelo próprio ASP.NET (formato do JSON e parâmetros de query), em português.
static string Traduzir(string mensagem) => mensagem switch
{
    _ when mensagem.StartsWith("The JSON value could not be converted") => "Valor inválido ou em formato incorreto.",
    _ when mensagem.StartsWith("The request field is required") => "O corpo da requisição é obrigatório.",
    _ when mensagem.StartsWith("The value '") && mensagem.EndsWith("' is not valid.") =>
        $"O valor '{mensagem[11..^15]}' não é válido.",
    _ => mensagem,
};

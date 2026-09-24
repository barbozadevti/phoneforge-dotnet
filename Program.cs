using System.Text.RegularExpressions;
using DesafioPOO.Models;
using DesafioPOO.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SmartphoneRepository>();

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

var api = app.MapGroup("/api/smartphones");

api.MapGet("/", (SmartphoneRepository repo) =>
    repo.Listar().Select(s => s.ObterInformacoes()));

api.MapPost("/", (NovoSmartphone dados, SmartphoneRepository repo) =>
{
    var erros = Validar(dados);
    if (erros.Count > 0)
        return Results.ValidationProblem(erros);

    // Polimorfismo: a variável é do tipo abstrato, o objeto é da marca escolhida
    Smartphone smartphone = dados.Marca.ToLower() switch
    {
        "nokia" => new Nokia(dados.Numero.Trim(), dados.Modelo.Trim(), dados.Imei.Trim(), dados.Memoria),
        _ => new Iphone(dados.Numero.Trim(), dados.Modelo.Trim(), dados.Imei.Trim(), dados.Memoria),
    };

    repo.Adicionar(smartphone);
    return Results.Created($"/api/smartphones/{smartphone.Id}", smartphone.ObterInformacoes());
});

api.MapDelete("/{id:guid}", (Guid id, SmartphoneRepository repo) =>
    repo.Remover(id) ? Results.NoContent() : Results.NotFound());

api.MapPost("/{id:guid}/ligar", (Guid id, SmartphoneRepository repo) =>
    Acao(id, repo, s => s.Ligar()));

api.MapPost("/{id:guid}/receber-ligacao", (Guid id, SmartphoneRepository repo) =>
    Acao(id, repo, s => s.ReceberLigacao()));

api.MapPost("/{id:guid}/aplicativos", (Guid id, NovoAplicativo dados, SmartphoneRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(dados.Nome))
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["nome"] = ["Informe o nome do aplicativo."] });

    return Acao(id, repo, s => s.InstalarAplicativo(dados.Nome.Trim()));
});

app.Run();

static IResult Acao(Guid id, SmartphoneRepository repo, Func<Smartphone, string> acao)
{
    var smartphone = repo.Buscar(id);
    if (smartphone is null)
        return Results.NotFound();

    try
    {
        var mensagem = repo.Executar(smartphone, acao);
        return Results.Ok(new { mensagem, smartphone = smartphone.ObterInformacoes() });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { mensagem = ex.Message });
    }
}

static Dictionary<string, string[]> Validar(NovoSmartphone d)
{
    var erros = new Dictionary<string, string[]>();

    if (d.Marca?.ToLower() is not ("nokia" or "iphone"))
        erros["marca"] = ["A marca deve ser Nokia ou Iphone."];
    if (string.IsNullOrWhiteSpace(d.Numero))
        erros["numero"] = ["Informe o número."];
    if (string.IsNullOrWhiteSpace(d.Modelo))
        erros["modelo"] = ["Informe o modelo."];
    if (d.Imei is null || !Regex.IsMatch(d.Imei.Trim(), @"^\d{15}$"))
        erros["imei"] = ["O IMEI deve ter exatamente 15 dígitos."];
    if (d.Memoria <= 0)
        erros["memoria"] = ["A memória deve ser maior que zero."];

    return erros;
}

record NovoSmartphone(string Marca, string Numero, string Modelo, string Imei, int Memoria);
record NovoAplicativo(string Nome);

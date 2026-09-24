using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using PhoneForge.Models;
using PhoneForge.Services;

const int Porta = 5180;
var endereco = $"http://localhost:{Porta}";

// Garante uma única cópia do programa, mesmo com vários cliques seguidos no atalho
using var instancia = new Mutex(true, "PhoneForge.InstanciaUnica", out var primeiraInstancia);
if (!primeiraInstancia)
{
    await AtenderNovoClique(endereco);
    return;
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Permite abrir pelo atalho da área de trabalho, de qualquer pasta
    ContentRootPath = AppContext.BaseDirectory,
});
builder.WebHost.UseUrls(endereco);
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var arquivoDados = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhoneForge", "dados.json");
builder.Services.AddSingleton(new SmartphoneRepository(arquivoDados));
var abas = new AbasAbertas();

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

// Cada aba aberta mantém esta conexão; quando a aba fecha, a conexão cai
app.MapGet("/api/presenca", async (HttpContext contexto) =>
{
    contexto.Response.ContentType = "text/event-stream";
    abas.Entrou();
    try
    {
        await contexto.Response.WriteAsync(": conectado\n\n");
        await contexto.Response.Body.FlushAsync();
        await Task.Delay(Timeout.Infinite, contexto.RequestAborted);
    }
    catch (OperationCanceledException)
    {
    }
    finally
    {
        abas.Saiu();
    }
});

var navegadorAbertoEm = DateTime.MinValue;

app.MapGet("/api/abas", () => new
{
    abertas = abas.Quantidade,
    // Nos primeiros segundos o programa ainda está abrindo a própria aba
    abrindo = DateTime.UtcNow - navegadorAbertoEm < TimeSpan.FromSeconds(15),
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.Title = "PhoneForge";
    Console.WriteLine($"PhoneForge está aberto no navegador: {endereco}");
    Console.WriteLine($"Os dados ficam salvos em: {arquivoDados}");
    Console.WriteLine();
    Console.WriteLine("Para encerrar o programa, feche esta janela.");
    navegadorAbertoEm = DateTime.UtcNow;
    AbrirNavegador(endereco);
});

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

// Chamado quando o programa já está aberto e a pessoa clica no atalho de novo
static async Task AtenderNovoClique(string endereco)
{
    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };

    // A primeira cópia pode ainda estar carregando; espera ela responder
    RespostaAbas? situacao = null;
    for (var tentativa = 0; tentativa < 30 && situacao is null; tentativa++)
    {
        try
        {
            situacao = await http.GetFromJsonAsync<RespostaAbas>($"{endereco}/api/abas");
        }
        catch
        {
            await Task.Delay(500);
        }
    }

    // O programa acabou de abrir a aba dele: foram só cliques repetidos no atalho
    if (situacao is null || situacao.Abrindo)
        return;

    if (situacao.Abertas == 0 || AvisoWindows.Perguntar("PhoneForge",
            "O PhoneForge já está aberto no seu navegador.\n\nDeseja abrir mais uma aba?"))
    {
        AbrirNavegador(endereco);
    }
}

static void AbrirNavegador(string endereco)
{
    try
    {
        Process.Start(new ProcessStartInfo(endereco) { UseShellExecute = true });
    }
    catch
    {
        Console.WriteLine($"Abra no navegador: {endereco}");
    }
}

record NovoSmartphone(string Marca, string Numero, string Modelo, string Imei, int Memoria);
record NovoAplicativo(string Nome);
record RespostaAbas(int Abertas, bool Abrindo);

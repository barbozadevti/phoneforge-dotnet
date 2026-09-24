using System.Text.Json;
using PhoneForge.Models;

namespace PhoneForge.Services
{
    // Guarda os celulares em memória e salva tudo num arquivo JSON a cada mudança
    public class SmartphoneRepository
    {
        private readonly List<Smartphone> smartphones = new();
        private readonly object trava = new();
        private readonly string arquivo;
        private static readonly JsonSerializerOptions opcoesJson = new() { WriteIndented = true };

        public SmartphoneRepository(string arquivo)
        {
            this.arquivo = arquivo;

            if (File.Exists(arquivo))
            {
                Carregar();
            }
            else
            {
                smartphones.Add(new Nokia("11 91234-5678", "Nokia 3310", "111111111111111", 64));
                smartphones.Add(new Iphone("21 99876-5432", "iPhone 15", "222222222222222", 128));
                Salvar();
            }
        }

        public List<Smartphone> Listar()
        {
            lock (trava) return smartphones.ToList();
        }

        public Smartphone? Buscar(Guid id)
        {
            lock (trava) return smartphones.FirstOrDefault(s => s.Id == id);
        }

        public void Adicionar(Smartphone smartphone)
        {
            lock (trava)
            {
                smartphones.Add(smartphone);
                Salvar();
            }
        }

        public bool Remover(Guid id)
        {
            lock (trava)
            {
                var removeu = smartphones.RemoveAll(s => s.Id == id) > 0;
                if (removeu) Salvar();
                return removeu;
            }
        }

        public string Executar(Smartphone smartphone, Func<Smartphone, string> acao)
        {
            // A lista de apps de cada celular não é thread-safe, então a ação roda sob a trava
            lock (trava)
            {
                var mensagem = acao(smartphone);
                Salvar();
                return mensagem;
            }
        }

        private void Carregar()
        {
            var dados = JsonSerializer.Deserialize<List<SmartphoneInfo>>(File.ReadAllText(arquivo)) ?? new();

            foreach (var d in dados)
            {
                Smartphone smartphone = d.Marca == "Nokia"
                    ? new Nokia(d.Numero, d.Modelo, d.Imei, d.Memoria, d.Id)
                    : new Iphone(d.Numero, d.Modelo, d.Imei, d.Memoria, d.Id);

                foreach (var app in d.Aplicativos)
                    smartphone.InstalarAplicativo(app);

                smartphones.Add(smartphone);
            }
        }

        private void Salvar()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(arquivo)!);
            var dados = smartphones.Select(s => s.ObterInformacoes());

            // Grava num arquivo temporário e troca no fim, para não corromper os dados se o programa fechar no meio
            var temporario = arquivo + ".tmp";
            File.WriteAllText(temporario, JsonSerializer.Serialize(dados, opcoesJson));
            File.Move(temporario, arquivo, overwrite: true);
        }
    }
}

using PhoneForge.Models;

namespace PhoneForge.Services
{
    // Armazena os celulares em memória (os dados somem ao reiniciar o servidor)
    public class SmartphoneRepository
    {
        private readonly List<Smartphone> smartphones = new();
        private readonly object trava = new();

        public SmartphoneRepository()
        {
            Adicionar(new Nokia("11 91234-5678", "Nokia 3310", "111111111111111", 64));
            Adicionar(new Iphone("21 99876-5432", "iPhone 15", "222222222222222", 128));
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
            lock (trava) smartphones.Add(smartphone);
        }

        public bool Remover(Guid id)
        {
            lock (trava) return smartphones.RemoveAll(s => s.Id == id) > 0;
        }

        public string Executar(Smartphone smartphone, Func<Smartphone, string> acao)
        {
            // A lista de apps de cada celular não é thread-safe, então a ação roda sob a trava
            lock (trava) return acao(smartphone);
        }
    }
}

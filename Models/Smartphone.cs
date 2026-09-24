namespace PhoneForge.Models
{
    public abstract class Smartphone
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Numero { get; set; }
        private string Modelo { get; set; }
        private string IMEI { get; set; }
        private int Memoria { get; set; }
        private readonly List<string> aplicativos = new();

        public abstract string Marca { get; }

        public Smartphone(string numero, string modelo, string imei, int memoria)
        {
            Numero = numero;
            Modelo = modelo;
            IMEI = imei;
            Memoria = memoria;
        }

        public string Ligar()
        {
            return $"[{Modelo}] Ligando a partir do número {Numero}...";
        }

        public string ReceberLigacao()
        {
            return $"[{Modelo}] Recebendo ligação no número {Numero}...";
        }

        public abstract string InstalarAplicativo(string nome);

        // Usado pelas classes filhas para guardar o app depois de instalar
        protected void RegistrarAplicativo(string nome)
        {
            if (aplicativos.Contains(nome, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException($"O aplicativo {nome} já está instalado.");

            aplicativos.Add(nome);
        }

        // Expõe os dados privados apenas para leitura, sem quebrar o encapsulamento
        public SmartphoneInfo ObterInformacoes()
        {
            return new SmartphoneInfo(Id, Marca, Numero, Modelo, IMEI, Memoria, aplicativos.ToList());
        }
    }

    public record SmartphoneInfo(
        Guid Id, string Marca, string Numero, string Modelo, string Imei, int Memoria, List<string> Aplicativos);
}

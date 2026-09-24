namespace DesafioPOO.Models
{
    public abstract class Smartphone
    {
        public string Numero { get; set; }
        private string Modelo { get; set; }
        private string IMEI { get; set; }
        private int Memoria { get; set; }

        public Smartphone(string numero, string modelo, string imei, int memoria)
        {
            Numero = numero;
            Modelo = modelo;
            IMEI = imei;
            Memoria = memoria;
        }

        public void Ligar()
        {
            Console.WriteLine($"[{Modelo}] Ligando a partir do número {Numero}...");
        }

        public void ReceberLigacao()
        {
            Console.WriteLine($"[{Modelo}] Recebendo ligação no número {Numero}...");
        }

        public void ExibirInformacoes()
        {
            Console.WriteLine($"Modelo: {Modelo} | IMEI: {IMEI} | Memória: {Memoria} GB | Número: {Numero}");
        }

        public abstract void InstalarAplicativo(string nomeApp);
    }
}

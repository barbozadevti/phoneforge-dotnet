namespace DesafioPOO.Models
{
    public class Iphone : Smartphone
    {
        public override string Marca => "Iphone";

        public Iphone(string numero, string modelo, string imei, int memoria)
            : base(numero, modelo, imei, memoria)
        {
        }

        public override string InstalarAplicativo(string nome)
        {
            RegistrarAplicativo(nome);
            return $"Instalando o aplicativo {nome} no iPhone pela App Store...";
        }
    }
}

namespace PhoneForge.Models
{
    public class Nokia : Smartphone
    {
        public override string Marca => "Nokia";

        public Nokia(string numero, string modelo, string imei, int memoria)
            : base(numero, modelo, imei, memoria)
        {
        }

        public override string InstalarAplicativo(string nome)
        {
            RegistrarAplicativo(nome);
            return $"Instalando o aplicativo {nome} no Nokia pela loja Nokia Store...";
        }
    }
}

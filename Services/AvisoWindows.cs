using System.Runtime.InteropServices;

namespace PhoneForge.Services
{
    // Caixa de diálogo nativa do Windows (Sim / Não)
    public static class AvisoWindows
    {
        private const uint BotoesSimNao = 0x4;
        private const uint IconeInformacao = 0x40;
        private const uint TrazerParaFrente = 0x10000;
        private const uint SempreNoTopo = 0x40000;
        private const int RespostaSim = 6;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBoxW(IntPtr janela, string texto, string titulo, uint tipo);

        public static bool Perguntar(string titulo, string texto)
        {
            if (!OperatingSystem.IsWindows())
                return true;

            var resposta = MessageBoxW(IntPtr.Zero, texto, titulo,
                BotoesSimNao | IconeInformacao | TrazerParaFrente | SempreNoTopo);
            return resposta == RespostaSim;
        }
    }
}

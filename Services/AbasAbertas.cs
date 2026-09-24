namespace PhoneForge.Services
{
    // Conta quantas abas do navegador estão com o PhoneForge aberto
    public class AbasAbertas
    {
        private int quantidade;

        public int Quantidade => Volatile.Read(ref quantidade);

        public void Entrou() => Interlocked.Increment(ref quantidade);

        public void Saiu() => Interlocked.Decrement(ref quantidade);
    }
}

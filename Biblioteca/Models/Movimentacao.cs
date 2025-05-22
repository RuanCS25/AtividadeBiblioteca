namespace Biblioteca.Models
{
    public class Movimentacao
    {
        public int MovimentacaoId { get; set; }
        public DateTime DataRetirada { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int LivroId { get; set; }
        public Livro? Livro { get; set; }
        public bool LivroDevolvido { get; set; }
        public bool LivroAtrasado { get; set; }
    }
}

namespace Biblioteca.Models
{
    public class Reserva
    {
        public int ReservaId { get; set; }
        public DateTime DataReserva { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int LivroId { get; set; }
        public Livro? Livro { get; set; }
        public bool LivroRetirado { get; set; }
        public bool Cancelada { get; set; }
    }
}

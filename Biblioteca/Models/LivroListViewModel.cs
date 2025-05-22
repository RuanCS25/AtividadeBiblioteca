namespace Biblioteca.Models
{
    public class LivroListViewModel
    {
        public IEnumerable<Livro> Livros { get; set; } = Enumerable.Empty<Livro>();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortOrder { get; set; }
        public string? CurrentSort { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

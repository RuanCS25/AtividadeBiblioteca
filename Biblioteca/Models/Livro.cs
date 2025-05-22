using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Biblioteca.Models
{
    public class Livro
    {
        [Display(Name = "Código")]
        public int LivroId { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "O Título é obrigatório.")]
        public string Titulo { get; set; }

        [Display(Name = "Autor(a)")]
        [Required(ErrorMessage = "O Autor(a) é obrigatório.")]
        public string Autor { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Display(Name = "Editora")]
        [Required(ErrorMessage = "A Editora é obrigatória.")]
        public string Editora { get; set; }

        [Display(Name = "Data de Publicação")]
        [Required(ErrorMessage = "A Data de Publicação é obrigatória.")]
        public DateOnly DataPublicacao { get; set; }

        [Display(Name = "Número de Páginas")]
        [Required(ErrorMessage = "O Número de Páginas é obrigatório.")]
        public int NumeroPaginas { get; set; }

        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "A Quantidade é obrigatória.")]
        public int Quantidade { get; set; }

        [Display(Name = "Capa do Livro")]
        public string? UrlCapa { get; set; }

        [Display(Name = "ISBN-10")]
        public string? ISBN10 { get; set; }

        [Display(Name = "ISBN-13")]
        public string? ISBN13 { get; set; }

        [Display(Name = "Gênero Literário")]
        [Required(ErrorMessage = "O Gênero Literário é obrigatório.")]
        public int GeneroId { get; set; }
        public Genero? Genero { get; set; }
    }
}

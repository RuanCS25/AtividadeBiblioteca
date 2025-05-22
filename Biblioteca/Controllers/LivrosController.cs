using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data;
using Biblioteca.Models;

namespace Biblioteca.Controllers
{
    public class LivrosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LivrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Livros
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Livros.Include(l => l.Genero);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        public async Task<IActionResult> Index(
        string? searchTerm,
        string? sortOrder,
        int page = 1)
        {
            int pageSize = 5;

            var livrosQuery = _context.Livros.Include(l => l.Genero).AsQueryable();

            // Busca
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                livrosQuery = livrosQuery.Where(l =>
                    l.Titulo.Contains(searchTerm) ||
                    l.Autor.Contains(searchTerm)
                );
            }

            // Ordenação
            ViewData["TituloSortParm"] = sortOrder == "titulo_asc" ? "titulo_desc" : "titulo_asc";
            ViewData["AutorSortParm"] = sortOrder == "autor_asc" ? "autor_desc" : "autor_asc";
            ViewData["EditoraSortParm"] = sortOrder == "editora_asc" ? "editora_desc" : "editora_asc";
            ViewData["GeneroSortParm"] = sortOrder == "genero_asc" ? "genero_desc" : "genero_asc";

            switch (sortOrder)
            {
                case "titulo_desc":
                    livrosQuery = livrosQuery.OrderByDescending(l => l.Titulo);
                    break;
                case "titulo_asc":
                    livrosQuery = livrosQuery.OrderBy(l => l.Titulo);
                    break;
                case "autor_desc":
                    livrosQuery = livrosQuery.OrderByDescending(l => l.Autor);
                    break;
                case "autor_asc":
                    livrosQuery = livrosQuery.OrderBy(l => l.Autor);
                    break;
                case "editora_desc":
                    livrosQuery = livrosQuery.OrderByDescending(l => l.Editora);
                    break;
                case "editora_asc":
                    livrosQuery = livrosQuery.OrderBy(l => l.Editora);
                    break;
                case "genero_desc":
                    livrosQuery = livrosQuery.OrderByDescending(l => l.Genero.Nome);
                    break;
                case "genero_asc":
                    livrosQuery = livrosQuery.OrderBy(l => l.Genero.Nome);
                    break;
                default:
                    livrosQuery = livrosQuery.OrderBy(l => l.Titulo);
                    break;
            }

            // Paginação
            int totalCount = await livrosQuery.CountAsync();
            var livros = await livrosQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new LivroListViewModel
            {
                Livros = livros,
                PageIndex = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                SearchTerm = searchTerm,
                SortOrder = sortOrder,
                CurrentSort = sortOrder,
                ErrorMessage = ViewData["ErrorMessage"] as string
            };

            return View(viewModel);
        }

        // GET: Livros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livro = await _context.Livros
                .Include(l => l.Genero)
                .FirstOrDefaultAsync(m => m.LivroId == id);
            if (livro == null)
            {
                return NotFound();
            }

            return View(livro);
        }

        // GET: Livros/Create
        public IActionResult Create()
        {
            ViewData["GeneroId"] = new SelectList(_context.Generos.OrderBy(g => g.Nome), "GeneroId", "Nome");
            return View();
        }

        // POST: Livros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LivroId,Titulo,Autor,Descricao,Editora,DataPublicacao,NumeroPaginas,Quantidade,UrlCapa,ISBN10,ISBN13,GeneroId")] Livro livro, IFormFile UrlCapa)
        {
            if (ModelState.IsValid)
            {
                if (UrlCapa != null && UrlCapa.Length > 0)
                {
                    // Definir o caminho para salvar a imagem
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Books");

                    // Extrai a extensão do arquivo enviado (ex: .jpg, .png)
                    var fileExtension = Path.GetExtension(UrlCapa.FileName);

                    // Usa apenas o ID do livro + extensão
                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);


                    // Se já existir, adiciona um GUID ao nome
                    if (System.IO.File.Exists(filePath))
                    {
                        uniqueFileName = $"{Guid.NewGuid()}_{fileExtension}";
                        filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    }

                    // Criar a pasta se não existir
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Salvar o arquivo no diretório
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await UrlCapa.CopyToAsync(fileStream);
                    }

                    // Atualizar o campo UrlCapa com o caminho relativo
                    livro.UrlCapa = Path.Combine("Resources", "Books", uniqueFileName).Replace("\\", "/");
                }

                _context.Add(livro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos.OrderBy(g => g.Nome), "GeneroId", "Nome", livro.GeneroId);
            return View(livro);
        }

        // GET: Livros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
            {
                return NotFound();
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos.OrderBy(g => g.Nome), "GeneroId", "Nome", livro.GeneroId);
            return View(livro);
        }

        // POST: Livros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LivroId,Titulo,Autor,Descricao,Editora,DataPublicacao,NumeroPaginas,Quantidade,UrlCapa,ISBN10,ISBN13,GeneroId")] Livro livro)
        {
            if (id != livro.LivroId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(livro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivroExists(livro.LivroId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos.OrderBy(g => g.Nome), "GeneroId", "Nome", livro.GeneroId);
            return View(livro);
        }

        // GET: Livros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livro = await _context.Livros
                .Include(l => l.Genero)
                .FirstOrDefaultAsync(m => m.LivroId == id);
            if (livro == null)
            {
                return NotFound();
            }

            return View(livro);
        }

        // POST: Livros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livro = await _context.Livros.FindAsync(id);
            if (livro != null)
            {
                _context.Livros.Remove(livro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LivroExists(int id)
        {
            return _context.Livros.Any(e => e.LivroId == id);
        }

        public IActionResult BuscarCapa(int id)
        {
            var livro = _context.Livros.Find(id);
            if (livro == null || string.IsNullOrEmpty(livro.UrlCapa))
                return NotFound();

            // Monta o caminho físico absoluto a partir do diretório base do projeto
            var caminho = Path.Combine(Directory.GetCurrentDirectory(), livro.UrlCapa.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!System.IO.File.Exists(caminho))
                return NotFound();

            var contentType = "image/*"; // Ajuste se necessário para outros formatos
            return PhysicalFile(caminho, contentType);
        }

        // GET: Livros/Search
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var livrosQuery = _context.Livros.Include(l => l.Genero).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                livrosQuery = livrosQuery.Where(l =>
                    l.Titulo.Contains(searchTerm) ||
                    l.Autor.Contains(searchTerm) ||
                    l.Editora.Contains(searchTerm)
                );
            }

            var livros = await livrosQuery.ToListAsync();

            var viewModel = new LivroListViewModel
            {
                Livros = livros,
                PageIndex = 1,
                TotalPages = 1,
                SearchTerm = searchTerm,
                SortOrder = null,
                CurrentSort = null,
                ErrorMessage = null
            };

            return View("Index", viewModel);
        }

        public async Task<IActionResult> UltimosLancamentos()
        {
            var livros = await _context.Livros
                .Include(l => l.Genero)
                .OrderByDescending(l => l.LivroId)
                .Take(4)
                .ToListAsync();

            return PartialView("_UltimosLancamentos", livros);
        }




    }
}
